window.FacePunch = (function () {
    const MODELS = '/lib/face-api/models';
    let stream = null, timer = null, modelsReady = false, busy = false;
    let state = null;
    const cfg = { limit: 0.45, gap: 0.06 };
    try { const j = JSON.parse(localStorage.getItem("facePunchCfg") || "null"); if (j) { cfg.limit = +j.limit || cfg.limit; cfg.gap = j.gap >= 0 ? +j.gap : cfg.gap; } } catch (e) { }
    function getStrictness() { return JSON.stringify(cfg); }
    function setStrictness(limit, gap) {
        cfg.limit = limit; cfg.gap = gap;
        try { localStorage.setItem("facePunchCfg", JSON.stringify(cfg)); } catch (e) { }
    }

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
        if (brightness(v, box) < 85) return JSON.stringify({ ok: false, msg: 'Too dark. Move to a brighter place and face the light.' });
        return JSON.stringify({ ok: true, descriptor: Array.from(r.descriptor) });
    }

    // average brightness (0-255) of the face area
    function brightness(v, box) {
        try {
            const c = document.createElement('canvas'); c.width = 32; c.height = 32;
            const g = c.getContext('2d', { willReadFrequently: true });
            const x = Math.max(0, box.x), y = Math.max(0, box.y);
            g.drawImage(v, x, y, Math.min(box.width, v.videoWidth - x), Math.min(box.height, v.videoHeight - y), 0, 0, 32, 32);
            const d = g.getImageData(0, 0, 32, 32).data;
            let sum = 0;
            for (let i = 0; i < d.length; i += 4) sum += 0.299 * d[i] + 0.587 * d[i + 1] + 0.114 * d[i + 2];
            return sum / (d.length / 4);
        } catch (e) { return 255; }
    }

    function dist(a, b) {
        let s = 0;
        for (let i = 0; i < a.length; i++) { const d = a[i] - b[i]; s += d * d; }
        return Math.sqrt(s);
    }

    // templates : [{ id, samples : [ [128 numbers], ... ] }]
    function startMatching(videoId, dotnet, templates, threshold, claimedId, margin) {
        const v = document.getElementById(videoId);
        const list = templates.map(t => ({ id: t.id, samples: t.samples.map(s => Float32Array.from(s)) }));
        state = { id: 0, hits: 0, done: false };
        void threshold; void margin;

        if (timer) clearInterval(timer);
        timer = setInterval(async () => {
            if (busy || !state || state.done || !v.videoWidth) return;
            busy = true;
            try {
                const r = await faceapi.detectSingleFace(v, opts()).withFaceLandmarks(true).withFaceDescriptor();
                if (!r) {
                    if (state.id !== 0) dotnet.invokeMethodAsync('OnStatus', 'Look at the camera.');
                    state.id = 0; state.hits = 0;
                    return;
                }

                // nearest distance of every enrolled person
                const per = new Map();
                for (const t of list) {
                    let m = 9;
                    for (const s of t.samples) { const d = dist(r.descriptor, s); if (d < m) m = d; }
                    per.set(t.id, m);
                }
                let best = 0, bestD = 9, second = 9;
                for (const [id, d] of per) {
                    if (d < bestD) { second = bestD; bestD = d; best = id; }
                    else if (d < second) second = d;
                }
                const ro = document.getElementById('pcReadout');
                if (ro) ro.textContent = 'Distance ' + bestD.toFixed(3) + '  |  gap ' + (second - bestD).toFixed(3) + (claimedId && best !== claimedId ? '  |  other person' : '');
                if (bestD > cfg.limit || (claimedId && best !== claimedId)) {
                    state.id = 0; state.hits = 0;
                    dotnet.invokeMethodAsync('OnStatus', 'Face not recognized.');
                    return;
                }
                if (second - bestD < cfg.gap) {
                    state.id = 0; state.hits = 0;
                    dotnet.invokeMethodAsync('OnStatus', 'Face not clear. Face the light and hold still.');
                    return;
                }

                if (best !== state.id) { state.id = best; state.hits = 1; return; }
                state.hits++;
                if (state.hits < 3) return;

                state.done = true;
                dotnet.invokeMethodAsync('OnRecognized', best);
            } catch (e) { /* the next frame tries again */ }
            finally { busy = false; }
        }, 300);
    }

    function resume() { if (state) { state.id = 0; state.hits = 0; state.done = false; } }

    function fullscreen() {
        if (document.fullscreenElement) document.exitFullscreen();
        else document.documentElement.requestFullscreen();
    }

    function leaveFullscreen() {
        if (document.fullscreenElement) document.exitFullscreen();
    }

    return { fullscreen, leaveFullscreen, init, stop, capture, startMatching, resume, getStrictness, setStrictness };
})();
