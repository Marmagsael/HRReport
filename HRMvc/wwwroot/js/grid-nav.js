// wwwroot/js/grid-nav.js

window.attachGridKeyboardNav = function (gridContainerId) {
    let container = document.getElementById(gridContainerId);
    if (!container || container.dataset.hasListener) return;
    container.dataset.hasListener = "true";

    function focusAndSelect(input) {
        if (!input) return;
        input.focus();
        input.select();
        // prevent mouseup from deselecting
        input.addEventListener('mouseup', function preventDeselect(e) {
            e.preventDefault();
            input.removeEventListener('mouseup', preventDeselect);
        });
    }

    container.addEventListener('keydown', function (e) {
        let active = document.activeElement;
        if (!active || active.tagName !== 'INPUT') return;

        // Telerik locked columns live in their own table: navigate inside the table body that holds the active input
        let scope = active.closest('tbody') || container;
        let allInputs = Array.from(scope.querySelectorAll('input'));
        let currentIndex = allInputs.indexOf(active);
        if (currentIndex === -1) return;

        let currentRow = active.closest('tr');
        if (!currentRow) return;

        let rowInputs = Array.from(currentRow.querySelectorAll('input'));
        let colIndexInRow = rowInputs.indexOf(active);

        let allRows = Array.from(scope.querySelectorAll('tr'));
        let rowIndex = allRows.indexOf(currentRow);

        // DOWN ARROW or ENTER
        if (e.key === 'ArrowDown' || e.key === 'Enter') {
            e.preventDefault();
            if (rowIndex + 1 < allRows.length) {
                let nextRowInputs = allRows[rowIndex + 1].querySelectorAll('input');
                focusAndSelect(nextRowInputs[colIndexInRow]);
            }
        }
        // UP ARROW
        else if (e.key === 'ArrowUp') {
            e.preventDefault();
            if (rowIndex - 1 >= 0) {
                let prevRowInputs = allRows[rowIndex - 1].querySelectorAll('input');
                focusAndSelect(prevRowInputs[colIndexInRow]);
            }
        }
        // RIGHT ARROW
        else if (e.key === 'ArrowRight') {
            if (active.selectionEnd === active.value.length) {
                e.preventDefault();
                focusAndSelect(allInputs[currentIndex + 1]);
            }
        }
        // LEFT ARROW
        else if (e.key === 'ArrowLeft') {
            if (active.selectionStart === 0) {
                e.preventDefault();
                focusAndSelect(allInputs[currentIndex - 1]);
            }
        }
    });
};