
function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    return pattern.test(trimAll(emailAddress));
}
function trimAll(sString) {
    while (sString.substring(0, 1) == ' ') {
        sString = sString.substring(1, sString.length);
    }
    while (sString.substring(sString.length - 1, sString.length) == ' ') {
        sString = sString.substring(0, sString.length - 1);
    }
    return sString;
}

function isEmptyString(sentence) {
    return (trimAll(sentence) == "");
}

function getText(ctl) {
    if (document.all) {
        return ctl.innerText;
    }
    else {
        return ctl.textContent;
    }
}
function changeText(ctl, text) {
    if (document.all) {
        ctl.innerText = text;
    }
    else {
        ctl.textContent = text;
    }
}
function AttachEventToControl(ctl, eventName, method) {
    if (ctl.addEventListener) { //Mozila, NetScap, FF
        ctl.addEventListener(eventName.replace('on', ''), method, false);
    }
    else { //IE
        ctl.attachEvent(eventName, method);
    }
}
function RemoveEventFromControl(ctl, eventName, method) {

    if (ctl.removeEventListener) { //Mozila, NetScap, FF
        ctl.removeEventListener(eventName.replace('on', ''), method, false);
    }
    else { //IE
        ctl.detachEvent(eventName, method);
    }
}

function SetDefaultValue(ctl, val) {
    if (ctl.value == val) { ctl.value = ""; }
    else if (ctl.value.replace(/^\s+|\s+$/g, "") == "")
    { ctl.value = val; }
}
var EmailPattren = "(?:[aA-zZ0-9!#$%&\'*+/=?^_`{|}~-]+(?:\\.[aA-zZ0-9!#$%&\'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[aA-zZ0-9](?:[aA-zZ0-9-]*[aA-zZ0-9])?\\.)+[aA-zZ0-9](?:[aA-zZ0-9-]*[aA-zZ0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[aA-zZ0-9-]*[aA-zZ0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])";
var PhoneNoPattren = "[+]?^[0-9\\-\\ ]+\\d{4,}$";
var PricePattren = "\\d{1,3}((,\\d{3})|\\d{1,3})*((\\.)?\\d+)?"; //"(\\d)*|(\\d{1,3},(\\d{3},)*\\d{3}(\\.\\d{1,3})?|\\d{1,3}(\\.\\d{3})?)$";  //"^$|^\\d{1,3}((,\\d{3})|\\d{1,3})*((\\.)?\\d+)?";
var PasswordPattren = "^.{4,}$"; //TEAM: This doesn't support Arabic password.
var NamePattren = "^(.{1,21} )*(.{1,21})$";
var NamePattrenWithoutSpace = "^[a-zA-Z-]*$";
var NumberPattren = "^\\d*\\.{0,1}\\d+$";
var AlphabeticPattren = "^([a-zA-Z ])*$"; //
var WebsiteUrlPattern = "(http(s)?://)?([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?";

function GetAssociatedPattern(ptrnName) {
    if (ptrnName == 'EmailPattren')
        return EmailPattren;
    if (ptrnName == 'PhoneNoPattren')
        return PhoneNoPattren;
    if (ptrnName == 'PricePattren')
        return PricePattren;
    if (ptrnName == 'PasswordPattren')
        return PasswordPattren;
    if (ptrnName == 'NamePattren')
        return NamePattren;
    if (ptrnName == "Compare")
        return ptrnName;
    if (ptrnName == "NumberPattren")
        return NumberPattren;
    if (ptrnName == "NamePattrenWithoutSpace")
        return NamePattrenWithoutSpace;
    if (ptrnName == "AlphabeticPattren")
        return AlphabeticPattren;
    if (ptrnName == "WebsiteUrlPattern")
        return WebsiteUrlPattern;

};
function checkRequiredFields() {
    var result = true;
    var emptyValue = '';
    try {
        for (var i = 0; i < requiredFields.length; i++) {
            if (document.getElementById(requiredFields[i]).type == 'select-one') {
                emptyValue = '0';
            }
            else
                emptyValue = '';
            var valueToCheck = document.getElementById(requiredFields[i]).value;
            //valueToCheck = $.trim(valueToCheck);
            if (valueToCheck == emptyValue) {
                document.getElementById(requiredFields[i] + "Required").style.display = 'inline';
                try {
                    AttachEventToControl(document.getElementById(requiredFields[i]), 'onblur', checkRequiredFields);
                }
                catch (e)
                 { }
                result = false;
            }
            else {
                document.getElementById(requiredFields[i] + "Required").style.display = 'none';
                document.getElementById(requiredFields[i]).onblur = null;
            }
        }
    }
    catch (e) {
        alert("An error has occured while verifying data on client side.");
    }
    return result;
};
function inValidateFields() {
    var result = true;
    if (validateFields == undefined) {
        return result;
    }
    try {
        for (var i = 0; i < validateFields.length; i++) {

            var ctlName = validateFields[i].split('|')[0];
            var regPtrn = GetAssociatedPattern(validateFields[i].split('|')[1]);

            if (regPtrn == 'Compare') {
                var ctlCmpr = validateFields[i].split('|')[2];
                if ($.trim(document.getElementById(ctlName).value) != $.trim(document.getElementById(ctlCmpr).value)) {
                    document.getElementById(ctlName + "Compare").style.display = 'inline';
                    if (document.getElementById(ctlName).onblur == null)
                        try {
                        AttachEventToControl(document.getElementById(ctlName), 'onblur', inValidateFields);
                    }
                    catch (e)
                         { }
                    result = false;
                    continue;
                }
                else {
                    document.getElementById(ctlName + "Compare").style.display = 'none';
                    document.getElementById(ctlName).onblur = null;
                    continue;
                }
            }

            var re = new RegExp(regPtrn);
            var m = document.getElementById(ctlName).value.match(re);  
            if ((m != null && m[0] == document.getElementById(ctlName).value) || $.trim(document.getElementById(ctlName).value) == '') {
                if (document.getElementById(ctlName + "Invalid") != null && document.getElementById(ctlName + "Invalid").style.display == 'inline') {
                    document.getElementById(ctlName + "Invalid").style.display = 'none';
                    document.getElementById(ctlName).onblur = null;
                }
                continue;
            }

            if (m == null) { //

                document.getElementById(ctlName + "Invalid").style.display = 'inline';
                if (document.getElementById(ctlName).onblur == null) {
                    try {
                        AttachEventToControl(document.getElementById(ctlName), 'onblur', inValidateFields);
                    }
                    catch (e)
                     { }
                }
                result = false;
            }
            else if (m != null && m[0] != document.getElementById(ctlName).value) {

                document.getElementById(ctlName + "Invalid").style.display = 'inline';
                document.getElementById(ctlName + "Required").style.display = 'none';
                if (document.getElementById(ctlName).onblur == null) {
                    try {
                        AttachEventToControl(document.getElementById(ctlName), 'onblur', inValidateFields);
                    }
                    catch (e)
                         { }
                }
                result = false;
            }
        }
    } catch (e) {
        alert("An error has occured while verifying data on client side.");
    }
    return result;
}
function ValidateControl() {
    $(".ExusersOuter").css("display", "none");
    var reqRes = checkRequiredFields();
    var valRes = inValidateFields();
    if (reqRes == true && valRes == true)
        return true;
    else
        return false;
}



function FireDefaultButton(event, target) {
    if (event.keyCode == 13) {
        var src = event.srcElement || event.target;
        if (!src || (src.tagName.toLowerCase() != "textarea")) {

            var defaultButton;
            defaultButton = document.getElementById(target);
            if (defaultButton == null)
                defaultButton = document.all[target];

            if (defaultButton && typeof (defaultButton.click) != "undefined") {
                defaultButton.click();
                event.cancelBubble = true;
                if (event.stopPropagation) event.stopPropagation();
                return false;
            }
        }
    }
    return true;
}

function SaveClicked_Results(actionURL, matchID, status1ID, status2ID, score1ID, score2ID) {

    var st1ID = $("#" + status1ID).val();
    var st2ID = $("#" + status2ID).val();

    var score1 = $("#" + score1ID).val();
    var score2 = $("#" + score2ID).val();


    var hdnst1ID = $("#" + 'hdn' + status1ID).val();
    var hdnst2ID = $("#" + 'hdn' + status2ID).val();
    var hdnscore1 = $("#" + 'hdn' + score1ID).val();
    var hdnscore2 = $("#" + 'hdn' + score2ID).val();

    if (st1ID == hdnst1ID && st2ID == hdnst2ID && score1 == hdnscore1 && score2 == hdnscore2) {
        return;  //nothing changed
    }

    //validate score for ints
    if (score1 != null && score1 != '') {
        
        score1 = parseInt(score1);

        if (!$.isNumeric(score1) || score1<0)
        {
            alert("Enter Valid Number in score1");
            $("#" + score1ID).focus();
            return;
        }
        
    }

    if (score2 != null && score2 != '') {
        
        score2 = parseInt(score2);
        
        if(!$.isNumeric(score2) || score2 < 0)
        {
            alert("Enter Valid Number in score2");
            $("#" + score2ID).focus();
            return;
        }
        
    }


    $.ajax({
        url: actionURL,
        type: 'POST',
        data: ({ matchID: matchID, status1ID: st1ID, status2ID: st2ID, score1ID: score1, score2ID: score2 }),
        success: function (data) {
            if (data == "Success") {
                

                $("#hdn" + status1ID).val(st1ID);
                $("#hdn" + status2ID).val(st2ID);
                $("#hdn" + score1ID).val(score1);
                $("#hdn" + score2ID).val(score2);

                alert("Success");
            }
            else {

                //restore old vals

                $("#" + status1ID).val(hdnst1ID);
                $("#" + status2ID).val(hdnst2ID);

                $("#" + score1ID).val(hdnscore1);
                $("#" + score2ID).val(hdnscore2);

                alert("Failed to save");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
                alert(textStatus);
            }

        }
    });
}