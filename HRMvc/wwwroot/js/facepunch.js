window.FacePunch = (function () {
    const MODELS = '/lib/face-api/models';
    let stream = null, timer = null, modelsReady = false, busy = false;
    let state = null;

    const opts = () => new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.5 });

    function loadScript(src) {
        return new Promise((ok, fail) => {
            const el = document.createElement('script');
            el.src = src; el.onload = ok; el.onerror = () => fail(new Error('Unable to load ' + src));
            document.head.appendChild(el);
        });
    }

    async function loadModels() {
        if (modelsReady) return;
        if (!window.faceapi) await loadScript('/lib/face-api/face-api.js');
        await faceapi.nets.tinyFaceDetector.loadFromUri(MODELS);
        await faceapi.nets.faceLandmark68TinyNet.loadFromUri(MODELS);
        await faceapi.nets.faceRecognitionNet.loadFromUri(MODELS);
        modelsReady = true;
    }

    async function init(videoId) {
        try {
            if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia)
                return 'The camera needs a secure (https) page.';
            await loadModels();
            stop();
            stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user', width: { ideal: 640 }, height: { ideal: 480 } }, audio: false });
            const v = document.getElementById(videoId);
            v.srcObject = stream;
            await v.play();
            return '';
        } catch (e) {
            return 'Unable to start the camera: ' + (e && e.message ? e.message : e);
        }
    }

    function stop() {
        if (timer) { clearInterval(timer); timer = null; }
        state = null;
        if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
    }

    // one face of the camera -> its 128 numbers (the face print) as JSON
    async function capture(videoId) {
        const v = document.getElementById(videoId);
        const r = await faceapi.detectSingleFace(v, opts()).withFaceLandmarks(true).withFaceDescriptor();
        if (!r) return JSON.stringify({ ok: false, msg: 'No face found. Face the camera.' });
        if (r.detection.score < 0.6) return JSON.stringify({ ok: false, msg: 'The face is not clear. Improve the light.' });
        const box = r.detection.box;
        if (box.width < v.videoWidth * 0.2) return JSON.stringify({ ok: false, msg: 'Move closer to the camera.' });
        return JSON.stringify({ ok: true, descriptor: Array.from(r.descriptor) });
    }

    function dist(a, b) {
        let s = 0;
        for (let i = 0; i < a.length; i++) { const d = a[i] - b[i]; s += d * d; }
        return Math.sqrt(s);
    }

    function ear(p, i) {
        const d = (a, b) => Math.hypot(p[a].x - p[b].x, p[a].y - p[b].y);
        return (d(i + 1, i + 5) + d(i + 2, i + 4)) / (2 * d(i, i + 3));
    }

    // templates : [{ id, samples : [ [128 numbers], ... ] }]
    function startMatching(videoId, dotnet, templates, threshold) {
        const v = document.getElementById(videoId);
        const list = templates.map(t => ({ id: t.id, samples: t.samples.map(s => Float32Array.from(s)) }));
        state = { id: 0, hits: 0, since: 0, closed: false, done: false };
        const limit = threshold || 0.5;

        if (timer) clearInterval(timer);
        timer = setInterval(async () => {
            if (busy || !state || state.done || !v.videoWidth) return;
            busy = true;
            try {
                const r = await faceapi.detectSingleFace(v, opts()).withFaceLandmarks(true).withFaceDescriptor();
                if (!r) {
                    if (state.id !== 0) dotnet.invokeMethodAsync('OnStatus', 'Look at the camera.');
                    state.id = 0; state.hits = 0; state.closed = false;
                    return;
                }

                let best = 0, bestD = 9;
                for (const t of list) for (const s of t.samples) {
                    const d = dist(r.descriptor, s);
                    if (d < bestD) { bestD = d; best = t.id; }
                }
                if (bestD > limit) {
                    state.id = 0; state.hits = 0; state.closed = false;
                    dotnet.invokeMethodAsync('OnStatus', 'Face not recognized.');
                    return;
                }

                if (best !== state.id) { state.id = best; state.hits = 1; state.since = Date.now(); state.closed = false; return; }
                state.hits++;
                if (state.hits < 3) return;

                // the live person has to blink, a photo does not
                const pts = r.landmarks.positions;
                const e = (ear(pts, 36) + ear(pts, 42)) / 2;
                if (e < 0.21) state.closed = true;
                else if (state.closed && e > 0.26) {
                    state.done = true;
                    dotnet.invokeMethodAsync('OnRecognized', best);
                    return;
                }
                dotnet.invokeMethodAsync('OnStatus', 'Recognized. Please blink to confirm.');
                if (Date.now() - state.since > 10000) { state.id = 0; state.hits = 0; }
            } catch (e) { /* the next frame tries again */ }
            finally { busy = false; }
        }, 350);
    }

    function resume() { if (state) { state.id = 0; state.hits = 0; state.closed = false; state.done = false; } }

    return { init, stop, capture, startMatching, resume };
})();
