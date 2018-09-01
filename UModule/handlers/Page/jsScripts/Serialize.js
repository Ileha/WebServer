function Serialize() {
    var ids = ;
    var s = new XMLSerializer();
    var str = "<res>";
    for (i = 0; i < ids.length; i++) {
        str += s.serializeToString(document.getElementById(ids[i]));
    }
    str += "</res>";
    return str;
}