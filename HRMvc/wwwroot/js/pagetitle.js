(function () {
    var fallback = { '/12/102': 'Dashboard', '/12/110': 'Quick Links', '/15/502': 'Payroll Entry' };
    var timer = null;

    function pick() {
        var h = document.querySelector('#mb-body .card-header h5') || document.querySelector('.card-header h5');
        var t = h ? (h.textContent || '').trim() : '';
        if (!t || t === '-') t = fallback[location.pathname.replace(/\/+$/, '')] || '';
        return t;
    }

    function apply() {
        var t = pick();
        if (t && document.title !== t) document.title = t;
    }

    function schedule() {
        if (timer) clearTimeout(timer);
        timer = setTimeout(apply, 150);
    }

    function start() {
        apply();
        new MutationObserver(schedule).observe(document.body, { childList: true, subtree: true, characterData: true });
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', start); else start();
})();
