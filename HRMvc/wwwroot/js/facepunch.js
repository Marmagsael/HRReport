window.FacePunch = (function () {
    const BASE = '/lib/human/';
    let stream = null, timer = null, human = null, busy = false;
    let state = null, gtimer = null, gbusy = false, capturing = false;
    const cfg = { limit: 0.40, gap: 0.06 };
    try { const j = JSON.parse(localStorage.getItem("facePunchCfgH") || "null"); if (j) { cfg.limit = +j.limit || cfg.limit; cfg.gap = j.gap >= 0 ? +j.gap : cfg.gap; } } catch (e) { }
    function getStrictness() { return JSON.stringify(cfg); }
    function setStrictness(limit, gap) {
        cfg.limit = limit; cfg.gap = gap;
        try { localStorage.setItem("facePunchCfgH", JSON.stringify(cfg)); } catch (e) { }
    }

    function loadScript(src) {
        return new Promise((ok, fail) => {
            const el = document.createElement('script');
            el.src = src; el.onload = ok; el.onerror = () => fail(new Error('Unable to load ' + src));
            document.head.appendChild(el);
        });
    }

    async function loadModels() {
        if (human) return;
        if (!window.Human) await loadScript(BASE + 'human.js');
        const h = new Human.Human({
            modelBasePath: BASE + 'models/',
            backend: 'webgl',
            cacheSensitivity: 0,
            filter: { enabled: false },
            face: {
                enabled: true,
                detector: { rotation: false, maxDetected: 1, minConfidence: 0.5 },
                mesh: { enabled: true },
                iris: { enabled: false },
                description: { enabled: true },
                antispoof: { enabled: true },
                liveness: { enabled: true },
                emotion: { enabled: false }
            },
            body: { enabled: false }, hand: { enabled: false }, object: { enabled: false },
            gesture: { enabled: false }, segmentation: { enabled: false }
        });
        await h.load();
        await h.warmup();
        human = h;
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
        if (gtimer) { clearInterval(gtimer); gtimer = null; }
        state = null;
        if (stream) { stream.getTracks().forEach(t => t.stop()); stream = null; }
    }

    // the one face of the camera frame (or null)
    async function look(v) {
        const r = await human.detect(v);
        return r && r.face && r.face.length ? r.face[0] : null;
    }

    const boxOf = f => ({ x: f.box[0], y: f.box[1], width: f.box[2], height: f.box[3] });

    // how well the face sits in the guide circle
    function fit(v, box, guideId) {
        const g = document.getElementById(guideId);
        if (!g || !v.videoWidth) return { ok: true };
        const vr = v.getBoundingClientRect(), gr = g.getBoundingClientRect();
        const sc = Math.max(vr.width / v.videoWidth, vr.height / v.videoHeight);
        const offX = (vr.width - v.videoWidth * sc) / 2, offY = (vr.height - v.videoHeight * sc) / 2;
        const cxMirror = vr.width - (box.x + box.width / 2) * sc - offX;
        const cx = vr.left + cxMirror, cy = vr.top + (box.y + box.height / 2) * sc + offY;
        const a = gr.width / 2, b = gr.height / 2;
        const dx = (cx - (gr.left + a)) / a, dy = (cy - (gr.top + b)) / b;
        const off = Math.sqrt(dx * dx + dy * dy);
        const w = box.width * sc / gr.width;
        if (w < 0.6) return { ok: false, msg: 'Move closer so your face fills the circle.' };
        if (w > 1.2) return { ok: false, msg: 'Move back a little so your face fits the circle.' };
        if (off > 0.3) return { ok: false, msg: 'Center your face inside the circle.' };
        return { ok: true };
    }

    // a real person in front of the camera, not a photo or a screen
    function isLive(f) {
        const real = f.real === undefined ? 1 : f.real, live = f.live === undefined ? 1 : f.live;
        if (real < 0.5 || live < 0.5) return { ok: false, msg: 'A live face is needed, not a photo or a screen.' };
        return { ok: true };
    }

    // turns the guide circle green while the face fits it
    function guide(videoId, guideId) {
        const v = document.getElementById(videoId), g = document.getElementById(guideId);
        if (gtimer) clearInterval(gtimer);
        gtimer = setInterval(async () => {
            if (gbusy || capturing || !g || !v.videoWidth || !human) return;
            gbusy = true;
            try {
                const f = await look(v);
                g.classList.toggle('ok', !!f && fit(v, boxOf(f), guideId).ok);
            } catch (e) { }
            finally { gbusy = false; }
        }, 400);
    }

    // one face of the camera -> its face print (numbers) as JSON
    async function capture(videoId, guideId) {
        capturing = true;
        try {
            const v = document.getElementById(videoId);
            const f = await look(v);
            if (!f) return JSON.stringify({ ok: false, msg: 'No face found. Face the camera.' });
            if ((f.faceScore || f.boxScore || 0) < 0.6) return JSON.stringify({ ok: false, msg: 'The face is not clear. Improve the light.' });
            const box = boxOf(f);
            if (guideId) {
                const r = fit(v, box, guideId);
                if (!r.ok) return JSON.stringify({ ok: false, msg: r.msg });
            }
            if (box.width < v.videoWidth * 0.2) return JSON.stringify({ ok: false, msg: 'Move closer to the camera.' });
            if (brightness(v, box) < 85) return JSON.stringify({ ok: false, msg: 'Too dark. Move to a brighter place and face the light.' });
            const lv = isLive(f);
            if (!lv.ok) return JSON.stringify({ ok: false, msg: lv.msg });
            if (!f.embedding || !f.embedding.length) return JSON.stringify({ ok: false, msg: 'The face print could not be made. Try again.' });
            return JSON.stringify({ ok: true, descriptor: Array.from(f.embedding) });
        } finally { capturing = false; }
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

    // templates : [{ id, samples : [ [numbers], ... ] }]  ; distance = 1 - similarity (lower = closer)
    function startMatching(videoId, dotnet, templates, threshold, claimedId, margin) {
        const v = document.getElementById(videoId);
        const list = templates.map(t => ({ id: t.id, samples: t.samples }));
        state = { id: 0, hits: 0, done: false };
        void threshold; void margin;

        if (timer) clearInterval(timer);
        timer = setInterval(async () => {
            if (busy || !state || state.done || !v.videoWidth || !human) return;
            busy = true;
            try {
                const f = await look(v);
                if (!f || !f.embedding || !f.embedding.length) {
                    if (state.id !== 0) dotnet.invokeMethodAsync('OnStatus', 'Look at the camera.');
                    state.id = 0; state.hits = 0;
                    return;
                }

                const lv = isLive(f);
                if (!lv.ok) {
                    state.id = 0; state.hits = 0;
                    dotnet.invokeMethodAsync('OnStatus', lv.msg);
                    return;
                }

                const per = new Map();
                for (const t of list) {
                    let m = 1;
                    for (const s of t.samples) { const d = 1 - human.match.similarity(f.embedding, s); if (d < m) m = d; }
                    per.set(t.id, m);
                }
                let best = 0, bestD = 1, second = 1;
                for (const [id, d] of per) {
                    if (d < bestD) { second = bestD; bestD = d; best = id; }
                    else if (d < second) second = d;
                }
                const ro = document.getElementById('pcReadout');
                if (ro) ro.textContent = 'Distance ' + bestD.toFixed(3) + '  |  gap ' + (second - bestD).toFixed(3) + '  |  real ' + (f.real ?? 0).toFixed(2) + '  live ' + (f.live ?? 0).toFixed(2) + (claimedId && best !== claimedId ? '  |  other person' : '');
                if (bestD > cfg.limit || (claimedId && best !== claimedId)) {
                    state.id = 0; state.hits = 0;
                    dotnet.invokeMethodAsync('OnStatus', 'Face not recognized.');
                    return;
                }
                if (per.size > 1 && second - bestD < cfg.gap) {
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
        }, 350);
    }

    function resume() { if (state) { state.id = 0; state.hits = 0; state.done = false; } }

    function fullscreen() {
        if (document.fullscreenElement) document.exitFullscreen();
        else document.documentElement.requestFullscreen();
    }

    function leaveFullscreen() {
        if (document.fullscreenElement) document.exitFullscreen();
    }

    return { guide, fullscreen, leaveFullscreen, init, stop, capture, startMatching, resume, getStrictness, setStrictness };
})();
