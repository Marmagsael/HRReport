window.ConfirmSwal = function (title, text,icon='warning') {
    return Swal.fire({
        title: title,
        html: text,
        icon: icon,  //--"success", "error", "warning", "info", "question"
        showCancelButton: true,
        confirmButtonText: 'Yes',
        cancelButtonText: 'No',
        customClass: {
            popup: 'swal-top-z'
        }
    }).then(result => result.isConfirmed);
};

//--- Client terminal ( used by ClientTerminalService ) : browser time zone + short browser info
window.GetClientTerminal = function () {
    const ua = navigator.userAgent;
    const browser =
        /Edg\//.test(ua)      ? 'Edge'    :
        /OPR\//.test(ua)      ? 'Opera'   :
        /Chrome\//.test(ua)   ? 'Chrome'  :
        /Firefox\//.test(ua)  ? 'Firefox' :
        /Safari\//.test(ua)   ? 'Safari'  : 'Browser';
    const os =
        /Windows/.test(ua)          ? 'Win'     :
        /Android/.test(ua)          ? 'Android' :
        /iPhone|iPad/.test(ua)      ? 'iOS'     :
        /Mac OS X/.test(ua)         ? 'Mac'     :
        /Linux/.test(ua)            ? 'Linux'   : 'OS';
    return {
        timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone || '',
        browser : (browser + '/' + os).substring(0, 20)        // Attpunches1.MacAddress is Char(20)
    };
};

window.AlertSwal = function (title, text, icon='info') {
    Swal.fire({
        title: title,
        html: text,
        icon: icon //--"success", "error", "warning", "info", "question"
    });
};

window.SwalToast = function (message, icon = 'success', position = 'top-end') {
    Swal.fire({
        toast: true,
        position: position,
        icon: icon,
        title: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
};

function scrollToTarget() {
    const targetSection = document.getElementById('target-section');
    if (targetSection) {
        targetSection.scrollIntoView({
            behavior: 'smooth'
        });
    }
}

window.scrollToClass = (className) => {
    const el = document.querySelector('.' + className);
    if (el) {
        // el.scrollIntoView({ behavior: 'smooth', block: 'center' });
        el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

window.Blazor.start({
    reconnectionOptions: {
        maxRetries: 1,
        retryIntervalMilliseconds: 10000
    }
});
