ace.define("ace/theme/rule", ["require", "exports", "module", "ace/lib/dom"], function (require, exports, module) {
    "use strict";

    exports.isDark = false;
    exports.cssText = ".ace-rule .ace_gutter {\
background: #ebebeb;\
border-right: 1px solid rgb(159, 159, 159);\
color: rgb(136, 136, 136);\
}\
.ace-rule .ace_print-margin {\
width: 1px;\
background: #ebebeb;\
}\
.ace-rule {\
background-color: #FFFFFF;\
color: black;\
}\
.ace-rule .ace_fold {\
background-color: rgb(60, 76, 114);\
}\
.ace-rule .ace_cursor {\
color: black;\
}\
.ace-rule .ace_storage,\
.ace-rule .ace_keyword,\
.ace-rule .ace_variable {\
color: blue;\
}\
.ace-rule .ace_constant.ace_buildin {\
color: rgb(88, 72, 246);\
}\
.ace-rule .ace_constant.ace_library {\
color: rgb(6, 150, 14);\
}\
.ace-rule .ace_function {\
color: rgb(60, 76, 114);\
}\
.ace-rule .ace_string {\
color: rgb(42, 0, 255);\
}\
.ace-rule .ace_comment {\
color: rgb(0, 128, 0);\
}\
.ace-rule .ace_comment.ace_doc {\
color: rgb(63, 95, 191);\
}\
.ace-rule .ace_comment.ace_doc.ace_tag {\
color: rgb(127, 159, 191);\
}\
.ace-rule .ace_constant.ace_numeric {\
color: darkblue;\
}\
.ace-rule .ace_tag {\
color: rgb(25, 118, 116);\
}\
.ace-rule .ace_type {\
color: rgb(127, 0, 127);\
}\
.ace-rule .ace_xml-pe {\
color: rgb(104, 104, 91);\
}\
.ace-rule .ace_marker-layer .ace_selection {\
background: rgb(181, 213, 255);\
}\
.ace-rule .ace_marker-layer .ace_bracket {\
margin: -1px 0 0 -1px;\
border: 1px solid rgb(192, 192, 192);\
}\
.ace-rule .ace_meta.ace_tag {\
color:rgb(25, 118, 116);\
}\
.ace-rule .ace_invisible {\
color: #ddd;\
}\
.ace-rule .ace_entity.ace_other.ace_attribute-name {\
color:rgb(127, 0, 127);\
}\
.ace-rule .ace_marker-layer .ace_step {\
background: rgb(255, 255, 0);\
}\
.ace-rule .ace_active-line {\
/*background: rgb(232, 242, 254);*/\
border: 2px solid rgb(234,234,242);\
}\
.ace-rule .ace_gutter-active-line {\
background-color : #DADADA;\
}\
.ace-rule .ace_marker-layer .ace_selected-word {\
border: 1px solid rgb(173, 214, 255);\
}\
.ace-rule .ace_indent-guide {\
background: url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAACCAYAAACZgbYnAAAAE0lEQVQImWP4////f4bLly//BwAmVgd1/w11/gAAAABJRU5ErkJggg==\") right repeat-y;\
}\
.ace-rule .ace_format {\
color: darkmagenta;\
font-weight:bold;\
background-color:rgba(209,209,209,0.5);\
}\
.ace-rule .ace_model {\
color: darkblue;\
font-weight:bold;\
}\
";

    exports.cssClass = "ace-rule";

    var dom = require("../lib/dom");
    dom.importCssString(exports.cssText, exports.cssClass);
});
