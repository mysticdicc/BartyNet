function insertAtCurrentPos(inputString, dotnetObj) {
    var textarea = document.getElementById("markdown_editor");
    var curPos = textarea.selectionStart;

    let x = textarea.value;
    str = x.slice(0, curPos) + inputString + x.slice(curPos);

    textarea.value = str;
    dotnetObj.invokeMethodAsync('markupTextAdded', str);
}