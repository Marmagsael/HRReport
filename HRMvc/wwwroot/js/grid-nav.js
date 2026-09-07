// wwwroot/js/grid-nav.js

window.attachGridKeyboardNav = function (gridContainerId) {
    let container = document.getElementById(gridContainerId);
    if (!container || container.dataset.hasListener) return;
    container.dataset.hasListener = "true";

    container.addEventListener('keydown', function (e) {
        let active = document.activeElement;
        if (!active || active.tagName !== 'INPUT') return;

        let allInputs = Array.from(container.querySelectorAll('input'));
        let currentIndex = allInputs.indexOf(active);
        if (currentIndex === -1) return;

        let currentRow = active.closest('tr');
        if (!currentRow) return;

        let rowInputs = Array.from(currentRow.querySelectorAll('input'));
        let colIndexInRow = rowInputs.indexOf(active);

        let allRows = Array.from(container.querySelectorAll('tbody tr'));
        let rowIndex = allRows.indexOf(currentRow);

        // DOWN ARROW or ENTER
        if (e.key === 'ArrowDown' || e.key === 'Enter') {
            e.preventDefault();
            if (rowIndex + 1 < allRows.length) {
                let nextRowInputs = allRows[rowIndex + 1].querySelectorAll('input');
                if (nextRowInputs[colIndexInRow]) {
                    nextRowInputs[colIndexInRow].focus();
                }
            }
        }
        // UP ARROW
        else if (e.key === 'ArrowUp') {
            e.preventDefault();
            if (rowIndex - 1 >= 0) {
                let prevRowInputs = allRows[rowIndex - 1].querySelectorAll('input');
                if (prevRowInputs[colIndexInRow]) {
                    prevRowInputs[colIndexInRow].focus();
                }
            }
        }
        // RIGHT ARROW
        else if (e.key === 'ArrowRight') {
            if (active.selectionEnd === active.value.length) {
                e.preventDefault();
                if (currentIndex + 1 < allInputs.length) {
                    allInputs[currentIndex + 1].focus();
                }
            }
        }
        // LEFT ARROW
        else if (e.key === 'ArrowLeft') {
            if (active.selectionStart === 0) {
                e.preventDefault();
                if (currentIndex - 1 >= 0) {
                    allInputs[currentIndex - 1].focus();
                }
            }
        }
    });
};