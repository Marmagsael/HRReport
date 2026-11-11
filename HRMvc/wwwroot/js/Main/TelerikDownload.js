window.downloadFile = (fileName, base64) => {
    const link = document.createElement("a");
    link.href =
        "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," +
        base64;
    link.download = fileName;
    link.click();
};

window.downloadFileAs = (fileName, base64, mime) => {
    const link = document.createElement("a");
    link.href = "data:" + mime + ";base64," + base64;
    link.download = fileName;
    link.click();
};
