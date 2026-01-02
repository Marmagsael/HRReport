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