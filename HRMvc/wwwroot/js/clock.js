// -- 12/102 Employee Dashboard clock (was inline in _12_102_Dashboard.cshtml) --
window.startClock = function (elementId) {
    setInterval(function () {
        const now       = new Date();
        const options   = { weekday: 'long', year: 'numeric', month: 'long', day: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' };

        const el = document.getElementById(elementId);
        if (el) { el.innerText = now.toLocaleString('en-US', options); }

    }, 1000);

}

window.startTextboxTime = function (elementId) {
    setInterval(function ()
    {
        const now = new Date();
        const options = { hour: '2-digit', minute: '2-digit', second: '2-digit' };
        const el = document.getElementById(elementId);

        if (el) { el.value = now.toLocaleTimeString('en-US', options); }

    }, 1000);

}
