$.ajaxSetup({
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      // alert('Error occurred!!!');
    }
});

//$(document).ready(function () {
//    jQuery.validator.addMethod('selectcheck', function (value) {
//        return (value != '0');
//    });
//});

// global variable declaration
var numberRegex = /^[+-]?\d+(\.\d+)?$/; // /^[+-]?\d+(\.\d+)?([eE][+-]?\d+)?$/
var numberOnlyRegex = /^\d+$/;

//this can be used to apply rich text editor to textarea/textbox
function InitializeRichTextEditor(textAreaID) {
    CKEDITOR.replace(textAreaID, { customConfig: "" });
}

function InitializeEmptyRichTextEditor(textAreaID) {
    CKEDITOR.replace(textAreaID, {
        toolbar: 'Custom', //makes all editors use this toolbar
        toolbar_Custom: [''] //define an empty array or whatever buttons you want.
    });
}

//this can be used to get data from rich text enabled text box
function GetDataFromRichTextEditor(textAreaID) {
    CKEDITOR.instances[textAreaID].getData();
}

function hashUrl(hash) {
    var currentURL = window.location;
    currentURL.hash = hash;
    window.location = currentURL;
}

function CheckMaxLengthOnKeyDown(event, ctl) {
    if (event.type == "keydown") {
        var charCode = null;
        if (event.charCode) {
            charCode = event.charCode;
        }
        else {
            charCode = event.keyCode;
        }
        if (charCode == 8 || charCode == 46 || charCode == 37 || charCode == 38 || charCode == 39 || charCode == 40 || charCode == 27)
        { return true; }
    }
    return (parseInt(ctl.value.length, 10) < parseInt(ctl.getAttribute("maxLength"), 10));
}

function RestrictMaxLengthOnPaste(ctl) {
    if (window.clipboardData) {
        var maxLength = parseInt(ctl.getAttribute("maxLength"), 10);
        var ctlValueLength = parseInt(ctl.value.length, 10);
        var remAllowedLength = maxLength - ctlValueLength;
        var str = window.clipboardData.getData("Text");

        if (parseInt(ctl.value.length, 10) < parseInt(ctl.getAttribute("maxLength"), 10)) {

            if (str != null) {
                if (str.length < remAllowedLength) {
                    return true;
                }
                else {
                    window.clipboardData.setData("Text", str.substring(0, remAllowedLength));
                    return true;
                }
            }
        }
        else {
            return false;
        }
    }
}

/****************** Script used in MarketAttributes ******************/
function AddAttribute() {

    ShowLoadingDiv(true);

    if ($("#frmMarketAttribute").valid()) {
        var listItms = $("#sortableMktAtt li");

        var attId = $('#ddlAttributes').val();
        var canAdd = true;

        listItms.each(function (idx, li) {
            $(li).css("border", "");

            if (attId == $(li).attr("id").replace("liId_", "")) {
                alert("cannot add the same attribute that's already been added before");
                // mark the li by bordering it as red line
                $(li).css("border", "1px solid red");
                canAdd = false;
            }
        });

        if (canAdd) {

            var hdnVar = $('#hdnMktAtts').val();
            hdnVar += '+' + attId + ",";
            $('#hdnMktAtts').val(hdnVar);

            var liTxt = '<li id=\"liId_' + attId + '">';
            //liTxt += '<span class="number">99</span>';
            liTxt += '<span class="value">' + $("#ddlAttributes option[value='" + attId + "']").text() + '</span>';
            liTxt += '<a onclick="javascript:RemoveAttribute(' + attId + ')" class="close"><i class="icon"></i></a>';
            liTxt += '<span class="re-order"><i class="icon"></i></span>';
            liTxt += '</li>';

            $("#sortableMktAtt").append(liTxt);
        }
    }
    ShowLoadingDiv(false);

    hashUrl('addmarketattribute?id=' + attId);
}

function RemoveAttribute(remAttId) {

    ShowLoadingDiv(true);

    $('#liId_' + remAttId).remove();

    // to delete an attribute from market, place negative sign before that attribute Id
    // and handle in code
    var hdnVar = $('#hdnMktAtts').val();

    // when the added attribute needs to be removed then no need to save it in hidden var
    if (hdnVar.indexOf("+" + remAttId + ",") > -1) {
        hdnVar = hdnVar.replace("+" + remAttId + ",", "");
    }
        // when the attribute does not have + then it means the attribute is already there and now need to be removed
    else {
        hdnVar += "-" + remAttId + ",";
    }
    $('#hdnMktAtts').val(hdnVar);

    ShowLoadingDiv(false);
}

function PostMarketAttribute(updateOrderUrl) {

    ShowLoadingDiv(true);

    $("#btnSaveMktAtts").attr("disabled", true);

    var retVal = false;

    if ($('#hdnMktAtts').val() != "") {

        $.ajaxSetup({ async: false });

        var form = $('#frmMarketAttribute');

        $.post(form.attr('action'), form.serialize(),
            function (res) {
                ShowLoadingDiv(false);
                if (res == "success") {
                    retVal = true;
                }
                else {
                    alert('some problem');
                }
            });

    }
    else {
        retVal = true;
    }

    if (retVal) {
        retVal = false;

        // now apply the sorting
        var updatedOrder = $("#sortableMktAtt").sortable("serialize");
        updatedOrder = updatedOrder.replace(/\[\]/gi, "");

        $.ajax({
            url: updateOrderUrl,
            data: { 'attributeIds': updatedOrder, 'mktId': $('#ddlMarket').val() },
            type: 'POST',
            success: function (data) {
                if (data = 'success') {
                    retVal = true;
                    alert('Attibutes saved successfully');
                }
                else {
                    alert('Error occured while updating Store order!!!');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (jqXHR.readyState != 0) {
                }
            },
            async: false
        });

        if (retVal) {
            RefreshMktAttrPg();
        }
    }

    ShowLoadingDiv(false);
    $("#btnSaveMktAtts").attr("disabled", false);
}

function RefreshMktAttrPg() {
    document.location.href = "/Attribute/MarketAttributes";
}

function ShowAttValues(atId, u) {

    ShowLoadingDiv(true);
    var dvIdToShow = '#dvAttValues_' + atId;
    var fspTypeId = $('#ddlFiscalPeriodTypes_' + atId).val();
    var doToggle = false;

    if ($(dvIdToShow).css('display') == 'none' || $('#hdnCurFPType').val() != fspTypeId) {

        if ($('#hdnCurAtt').val() != atId) {
            if ($('#hdnCurAtt').val() != '') {
                CollapseAttValues();
                doToggle = true;
            }
            else {
                doToggle = true;
            }
        }

        $('#hdnCurAtt').val(atId);

        $('#hdnCurFPType').val(fspTypeId);
        u += '&fiscalPeriodTypeId=' + fspTypeId;

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $(dvIdToShow).html(html);
                $('#dvMktAttribute_' + atId).removeClass('sa-row').addClass('sa-row open');
                if (doToggle) {
                    $(dvIdToShow).toggle("slow");
                }
                if ($('#dvAttValRow_1').length > 0) {
                    $('#lstAttVals').show();
                    $('#attValsSaveCancelBtns').show();
                }
            },
            error: function () {
                alert("Showing Attribute Values is failed! Please try again");
            }
            //error: function (jqXHR, textStatus, errorThrown) {
            //    if (jqXHR.readyState != 0) {
            //    }
            //}
        });

    }

    document.location.href = '/Attribute/MarketAttributes' + '#showattvalues?id=' + atId;
    ShowLoadingDiv(false);
}

function CollapseAttValues() {
    //if (true) {
    //    if (!confirm("Are you sure you want to cancel?")) {
    //        return;
    //    }
    //}
    ShowLoadingDiv(true);
    var atId = $('#hdnCurAtt').val();
    var dvIdToShow = '#dvAttValues_' + atId;
    $(dvIdToShow).toggle("slow");
    $('#dvMktAttribute_' + atId).removeClass('sa-row open').addClass('sa-row');
    $(dvIdToShow).html('');
    $('#hdnCurAtt').val('');
    document.location.href = '/Attribute/MarketAttributes' + '#collapseattvalues';
    ShowLoadingDiv(false);
}

function ShowLoadingDiv(show) {
    if (show) {
        //$('#dvLoading').attr('class', 'y-display');
        $('#dvLoading').show();
    }
    else {
        //$('#dvLoading').attr('class', 'n-display');
        $('#dvLoading').hide();
    }
}

function validateControlsForAttValsList() {
    var ctlToChk = $('#txtValue');
    var secondVal = $('#txtValueEn');
    if ($.trim(ctlToChk.val()) == "") {
        alert('value is required');
        ctlToChk.focus();
        return false;
    }
    else if (secondVal.length > 0 && $.trim(secondVal.val()) == "") {
        alert('value en is required');
        secondVal.focus();
        return false;
    }
    else if ($('#txtValueEn').length == 0) {
        // when above is true then check the textbox for numeric value
        if (!numberRegex.test(ctlToChk.val())) {
            alert('value must be a number');
            ctlToChk.focus();
            return false;
        }
    }
    return true;
}

function AddAttValueRow(u) {

    // control validation...
    if (!validateControlsForAttValsList()) {
        return;
    }

    var atId = $('#hdnAttID').val()
        , yearVal = $('#ddlYear').val()
        , perVal = $('#ddlPeriodVals').val();

    // check duplicate Ids for att ids and fiscal period Id //
    var mkIdString = atId + ',' + yearVal + ',' + perVal + '|';

    var fpIdsColl = $('#hdnChkDupFPIds').val();
    if (fpIdsColl.indexOf(mkIdString) > -1) {
        alert('The given values are already defined, cannot add the duplicated values');
        return;
    }
    else {
        fpIdsColl += mkIdString;
        $('#hdnChkDupFPIds').val(fpIdsColl);
    }
    // ---------------------------------------------------- //

    ShowLoadingDiv(true);

    var valEnExist = false;
    var currRow = $('#hdnCurrAddedRowNum').val();

    var d = 'mtkId=' + $('#hdnMktId').val();
    d += '&attId=' + atId;
    d += '&perTyp=' + $('#ddlFiscalPeriodTypes_' + atId).val();
    d += '&yearVal=' + yearVal;
    d += '&perVal=' + perVal;
    d += '&perValText=' + $("#ddlPeriodVals :selected").text();
    d += '&valuear=' + $('#txtValue').val();
    d += '&currRowId=' + currRow;

    if ($('#txtValueEn').length > 0) {
        d += '&valueen=' + $('#txtValueEn').val();
        valEnExist = true;
    }

    $.post(u, d,
        function (res) {
            if (res == "failed") {
                alert('problem in adding attribute value row!');
            }
            else {

                $('#lstAttVals').show();
                $('#attValsSaveCancelBtns').show();

                $('#lstAttVals').append(res);
                $('#hdnCurrAddedRowNum').val(++currRow);

                //reset textboxes
                $('#txtValue').val('');
                if (valEnExist) $('#txtValueEn').val('');
            }
        });

    ShowLoadingDiv(false);

    document.location.href = '/Attribute/MarketAttributes' + '#addattvaluerow?id=' + atId;
}

function UpdateAttValueRow(rId) {
    //$('#dvAttValRow_' + rId)
    $('#ddlYear').val($('#hdnYear_' + rId).val());
    $('#ddlPeriodVals').val($('#hdnPerVals_' + rId).val());
    $('#txtValue').val($('#hdnVal_' + rId).val());
    if ($('#txtValueEn').length > 0) {
        $('#txtValueEn').val($('#hdnValEn_' + rId).val());
    }

    //$('#hdnUpdEAVId').val($('#hdnEAVId_' + rId).val());
    $('#hdnUpdRowId').val(rId);

    $('#btnAddAttValRow').hide();
    $('#btnUpdAttValRow').show();

    var atId = $('#hdnAttID').val();
    document.location.href = '/Attribute/MarketAttributes' + '#update_attvaluerow?id=' + atId;
}

function UpdAttValueRowInList(u) {
    if ($('#hdnUpdRowId').val() != "") {

        if (!validateControlsForAttValsList()) {
            return;
        }

        var rId = $('#hdnUpdRowId').val()
            , atId = $('#hdnAttID').val()
            , yearVal = $('#ddlYear').val()
            , perVal = $('#ddlPeriodVals').val();

        // check duplicate Ids for att ids and fiscal period Id //
        var currIdString = atId + ',' + $('#hdnYear_' + rId).val() + ',' + $('#hdnPerVals_' + rId).val() + '|';
        var mkIdString = atId + ',' + yearVal + ',' + perVal + '|';

        if (currIdString != mkIdString) {
            var fpIdsColl = $('#hdnChkDupFPIds').val();
            if (fpIdsColl.indexOf(mkIdString) > -1) {
                alert('The given values are already defined, cannot add duplicated values');
                return;
            }
            else {
                fpIdsColl = fpIdsColl.replace(currIdString, "");
                fpIdsColl += mkIdString;
                $('#hdnChkDupFPIds').val(fpIdsColl);
            }
        }
        // ---------------------------------------------------- //

        u += '?fpTypeId=' + $('#ddlFiscalPeriodTypes_' + atId).val()
            + '&fpVId=' + perVal
            + '&fpVText=' + $("#ddlPeriodVals :selected").text();

        var yearTxt = yearVal, periodFirstPart = '';
        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (res) {
                periodFirstPart = res + ' ';
            },
            error: function () {
                alert("getting period first value is failed! Please try again");
            }
        });

        $('#dvLstPeriod_' + rId).text(periodFirstPart + yearTxt);

        // update all hidden vars
        $('#hdnVal_' + rId).val($('#txtValue').val());
        if ($('#txtValueEn').length > 0) {
            $('#hdnValEn_' + rId).val($('#txtValueEn').val());
        }
        $('#hdnYear_' + rId).val(yearTxt);
        $('#hdnPerVals_' + rId).val(perVal);

        $('#dvLstVal_' + rId).text($('#txtValue').val());
    }

    $('#txtValue').val('');
    if ($('#txtValue').length > 0) $('#txtValueEn').val('');

    $('#btnUpdAttValRow').hide();
    $('#btnAddAttValRow').show();
}

function RemoveAttValueRow(rId, eavId) {

    if (confirm("Are you sure, you want to delete the record?")) {
        if (eavId > 0) {
            var hdnvDelEAVId = $('#hdnDelEAVId').val();
            hdnvDelEAVId += eavId + ',';
            $('#hdnDelEAVId').val(hdnvDelEAVId);
        }

        var atId = $('#hdnAttID').val();
        var currIdString = atId + ',' + $('#hdnYear_' + rId).val() + ',' + $('#hdnPerVals_' + rId).val() + '|';
        var fpIdsColl = $('#hdnChkDupFPIds').val();
        fpIdsColl = fpIdsColl.replace(currIdString, "");
        $('#hdnChkDupFPIds').val(fpIdsColl);


        $('#dvAttValRow_' + rId).remove();

        if (fpIdsColl == "") {
            $('#lstAttVals').hide();
            $('#attValsSaveCancelBtns').hide();
        }
    }
}

function RemoveAttValueFromList(rId, eavId) {

    //if (confirm("Are you sure, you want to delete the record?"))
    {
        if (eavId > 0) {
            var hdnvDelEAVId = $('#hdnDelEAVId').val();
            hdnvDelEAVId += eavId + ',';
            $('#hdnDelEAVId').val(hdnvDelEAVId);
        }

        var atId = $('#hdnAttID').val();
        var currIdString = atId + ',' + $('#hdnYear_' + rId).val() + ',' + $('#hdnPerVals_' + rId).val() + '|';
        var fpIdsColl = $('#hdnChkDupFPIds').val();
        fpIdsColl = fpIdsColl.replace(currIdString, "");
        $('#hdnChkDupFPIds').val(fpIdsColl);

        $('#hdnRowId_' + rId).remove();
        //$('#dvAttValRow_' + rId).remove();

        //if (fpIdsColl == "") {
        //    $('#lstAttVals').hide();
        //    $('#attValsSaveCancelBtns').hide();
        //}
    }
}

function SaveAttValues() {

    if ($('#hdnDelEAVId').val() != "" && !confirm("Are you sure, you want to delete the records?")) {
        return;
    }

    ShowLoadingDiv(true);

    var form = $('#frmAttVals');
    var frmData = form.serialize();

    if (frmData != "") {

        $.post(form.attr('action'), frmData,
        function (res) {

            ShowLoadingDiv(false);

            if (res == "success") {
                alert('saving attribute values success');
                document.location.href = '/Attribute/MarketAttributes';
            }
            else {
                alert('some problem');
            }
        });
    }
    ShowLoadingDiv(false);
}

/****************** Scripts used in Section Templates ******************/
function LoadSecTemplates(o) {
    var u = $('#hdnSecTempUrl').val();
    u += '?lang=' + $(o).val();
    document.location.href = u;
}
function PostSectionTemplate() {

    ShowLoadingDiv(true);
    hashUrl("savesectiontemplate");

    if ($("#frmSectionTemplate").valid()) {


        if ($('#hdnSecTempId').val() == "") {
            $('#hdnSecTempLangId').val(2);
        }

        var form = $('#frmSectionTemplate');
        var frmData = form.serialize();

        if (frmData != "") {

            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: frmData,
                cache: false,
                success: function (res) {

                    ShowLoadingDiv(false);

                    if (res == "success") {
                        alert('section template saved successfully');
                        document.location.href = $('#hdnSecTempUrl').val() + '?lang=' + $('#ddlSrchLanguage').val();
                    }
                    else {
                        alert('problem in saving ');
                    }
                },
                async: false
            });

        }

    }

    ShowLoadingDiv(false);
    return false;
}
function UpdSectionTemp(stId) {
    ShowLoadingDiv(true);
    hashUrl("update_id=" + stId);
    $.ajax({
        url: '/Section/GetSectionTemplateForUpdate?id=' + stId,
        cache: false,
        async: false,

        success: function (data) {
            $('#hdnSecTempId').val(data.SectionTemplateID);
            if (data.LanguageID == 1)
                $('#ddlLanguage').val("ar");
            else
                $('#ddlLanguage').val("en");

            $('#txtTempName').val(data.SectionTemplateName);
            $('#txtTemplate').val(data.TemplateCode);

            $('#txtTempName').focus();
        },
        error: function () {
            alert("Some error has occurred while getting section template record!");
        }
    });
    ShowLoadingDiv(false);

}
function DelSectionTemp(stId) {
    if (stId > 0 && confirm("Are you sure you want to delete this section template?")) {
        ShowLoadingDiv(true);
        var jqxhr = $.ajax({
            type: "get",
            url: "/section/deletesectiontemplate?id=" + stId
        })
        .done(function (data) {
            if (data == "success") {
                document.location.href = $('#hdnSecTempUrl').val() + '?lang=' + $('#ddlSrchLanguage').val();
            }
            else if (data == "failed") {
                alert("Record cannot be deleted as it is been used in sections");
            }
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while deleting a Section Template record.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    }
    ShowLoadingDiv(false);
}
function refreshSectionTemplatePager() {
    $("#secTempPager").paginate({
        count: $("#totalrecords").val() / 6,//20 is pageSize
        start: 1,
        display: 10,
        border: false,
        text_color: '#79B5E3',
        background_color: 'none',
        text_hover_color: '#2573AF',
        background_hover_color: 'none',
        images: false,
        mouse: 'press',
        onChange: function (page) {
            loadSectionTemplatePage(page);
        }
    });
}
function loadSectionTemplatePage(page) {
    hashUrl("page_" + page);
    ShowLoadingDiv(true);
    console.log(page);
    var jqxhr = $.ajax({
        type: "get",
        url: "/section/sectiontemplatelistpage?text=" + $('#srchSTName').val() + "&pageno=" + page + "&lang=" + $('#ddlSrchLanguage').val()
    })
        .done(function (data) {
            $("#dvSTempList").html(data);
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while loading Section Template page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}
function searchSectionTemplates() {
    hashUrl("search");
    ShowLoadingDiv(true);
    var jqxhr = $.ajax({
        type: "POST",
        url: "/section/sectiontemplatesearch",
        data: { lang: $("#ddlSrchLanguage").val(), text: $("#srchSTName").val() },
    })
        .done(function (data) {
            $("#dvSTempList").html(data);
            refreshSectionTemplatePager();
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while searching Section Template.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}

/****************** Scripts used in Section ******************/
function LoadTemplatesByLangId(o) {
    ShowLoadingDiv(true);
    hashUrl("get_templates");
    $.ajax({
        url: '/Section/GetTemplatesOptionsByLangId?lang=' + $(o).val(),
        cache: false,
        async: false,

        success: function (data) {
            $('#ddlSectionTemplates').html(data);
        },
        error: function () {
            alert("Some error has occurred while getting section templates for language!");
        }
    });
    ShowLoadingDiv(false);
}
function LoadSectionsByLang(o) {
    var u = $('#hdnSecUrl').val();
    u += '?lang=' + $(o).val();
    document.location.href = u;
}
function PostSection() {

    ShowLoadingDiv(true);
    hashUrl("savesection");

    if ($("#frmSection").valid()) {

        var form = $('#frmSection');
        var d = form.serializeArray();

        if (d.length > 0) {

            if ($('#hdnSecId').val() != '') {
                d.push({ name: 'ddlLanguage', value: $('#ddlLanguage').val() });
            }
            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: d,
                cache: false,
                success: function (res) {

                    ShowLoadingDiv(false);

                    if (res == "success") {
                        alert('section saved successfully');
                        document.location.href = $('#hdnSecUrl').val() + '?lang=' + $('#ddlSrchLanguage').val();
                    }
                    else {
                        alert('problem in saving section');
                    }
                },
                async: false
            });

        }

    }

    ShowLoadingDiv(false);
    return false;
}
function UpdSection(stId) {
    ShowLoadingDiv(true);
    hashUrl("update_id=" + stId);
    $.ajax({
        url: '/Section/GetSectionForUpdate?id=' + stId,
        cache: false,
        async: false,

        success: function (data) {
            $('#hdnSecId').val(data.SectionID);
            if (data.LanguageID == 1)
                $('#ddlLanguage').val("ar");
            else
                $('#ddlLanguage').val("en");

            $('#ddlLanguage').attr('disabled', true);
            $('#txtName').val(data.SectionName);
            $('#txtDisplayName').val(data.DisplayName);
            $('#ddlPages').val(data.ParentPageID);
            $('#ddlSectionTemplates').val(data.SectionTemplateID);
            $('#ddlRows').val(data.DisplayCount);
            if (data.IsVisible) {
                $('#active').attr('checked', 'checked');
            }
            else {
                $('#active').removeAttr('checked');
            }
            if (data.UsesManualSequencing) {
                $('#UseManualSeq').attr('checked', 'checked');
            }
            else {
                $('#UseManualSeq').removeAttr('checked');
            }

            $('#txtTempName').focus();
        },
        error: function () {
            alert("Some error has occurred while getting section record!");
        }
    });
    ShowLoadingDiv(false);

}
function UpdCancel() {
    $('#hdnSecId').val('');
    $('#active').removeAttr('checked');
    $('#UseManualSeq').removeAttr('checked');
    $('#ddlLanguage').removeAttr('disabled');
    hashUrl("");
}
function refreshSectionPager() {
    $("#sectionPager").paginate({
        count: $("#totalrecords").val() / 6,//pageSize
        start: 1,
        display: 10,
        border: false,
        text_color: '#79B5E3',
        background_color: 'none',
        text_hover_color: '#2573AF',
        background_hover_color: 'none',
        images: false,
        mouse: 'press',
        onChange: function (page) {
            loadSectionPage(page);
        }
    });
}
function loadSectionPage(page) {
    hashUrl("page_" + page);
    ShowLoadingDiv(true);
    //console.log(page);

    var u = "/section/sectionlistpage?pageno=" + page + "&lang=" + $('#ddlLanguage').val();
    u += '&pageID=' + $("#ddlFPages").val() + '&secTempID=' + $("#ddlFSecTemps").val()
        + '&activeID=' + $("#ddlFActive").val();

    var jqxhr = $.ajax({
        type: "get",
        url: u
    })
        .done(function (data) {
            $("#dvSectionList").html(data);
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while loading Section Template page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}
function PreviewSection(link) {
    window.open(link, "Preview", "height=500,width=505");
    return false;
}
function SearchSections() {
    hashUrl("search");
    ShowLoadingDiv(true);
    var jqxhr = $.ajax({
        type: "POST",
        url: "/section/sectionsearch",
        data: {
            pageid: $("#ddlFPages").val(), sectempid: $("#ddlFSecTemps").val()
            , activeId: $("#ddlFActive").val(), lang: $('#ddlSrchLanguage').val()
        },
    })
        .done(function (data) {
            $("#dvSectionList").html(data);
            refreshSectionPager();
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while searching Sections.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}

//Value parameter - required. All other parameters are optional.

function isValidDate(Day, Month, Year) {
    try {
        var OK = true;
        if (OK = ((Year > 1900) && (Year <= new Date().getFullYear()))) {
            if (OK = (Month <= 12 && Month > 0)) {

                var LeapYear = (((Year % 4) == 0) && ((Year % 100) != 0) || ((Year % 400) == 0));

                if (OK = Day > 0) {
                    if (Month == 2) {
                        OK = LeapYear ? Day <= 29 : Day <= 28;
                    }
                    else {
                        if ((Month == 4) || (Month == 6) || (Month == 9) || (Month == 11)) {
                            OK = Day <= 30;
                        }
                        else {
                            OK = Day <= 31;
                        }
                    }
                }
            }
        }
        return OK;
    }
    catch (e) {
        return false;
    }
}
function isValidNumber(numberVal) {
    var retVal = true;
    numberVal = numberVal.trim();
    if (numberVal == '') {
        return true;
    }
    if (!numberRegex.test(numberVal)) {
        retVal = false;
    }
    return retVal;
}


function ScrollTo(scrollToPH) {
    $('html, body').animate({
        scrollTop: $("#" + scrollToPH).offset().top
    }, 400);
}

/****************** used in market sectors screen ******************/
function SelectMarket() {
    var mktId = $('#ddlMarkets').val();
    document.location.href = $('#hdnPageRefUrl').val() + '?mktId=' + mktId;
}
function EditMarketSectorRow(rowId) {

    var hdnVar = $('#hdnSectUpdValues').val();

    if ($('#lnkUpd' + rowId).html() == "<i class=\"icon edit\"></i>") {

        // put the id in hidden var
        hdnVar += rowId + ",";
        $('#hdnSectUpdValues').val(hdnVar);

        $('#spnlDescEnId' + rowId).hide();
        $('#spnDescEnId' + rowId).show();

        $('#spnlDescArId' + rowId).hide();
        $('#spnDescArId' + rowId).show();

        $('#spnlSectEnId' + rowId).hide();
        $('#spnSectEnId' + rowId).show();

        $('#txtSectEnId' + rowId).val($('#spnlSectEnId' + rowId).text());
        $('#txtDescArId' + rowId).val($('#spnlDescArId' + rowId).text());
        $('#txtDescEnId' + rowId).val($('#spnlDescEnId' + rowId).text());

        $('#lnkUpd' + rowId).html("<i class=\"icon cancel\"></i>");

        $("#chkPublished_" + rowId).attr('disabled', 'disabled');
    }
    else {
        // remove the id from hidden var
        hdnVar = hdnVar.replace(rowId + ",", "");
        $('#hdnSectUpdValues').val(hdnVar);

        $('#spnlDescEnId' + rowId).show();
        $('#spnDescEnId' + rowId).hide();

        $('#spnlDescArId' + rowId).show();
        $('#spnDescArId' + rowId).hide();

        $('#spnlSectEnId' + rowId).show();
        $('#spnSectEnId' + rowId).hide();

        $('#lnkUpd' + rowId).html("<i class=\"icon edit\"></i>");

        $("#chkPublished_" + rowId).removeAttr('disabled');
    }

    if (hdnVar.length == 0) {
        $("#sortable").sortable("enable");
    }
    else {
        $("#sortable").sortable("disable");
    }
}
function AddNewSectorRow(u) {

    $('#hdnSectUpdValues').val('');

    if ($.trim($('#lnkCrNewSct').text()) == 'Create New Sector') {
        $('#sortable').each(function () {

            var liItm = $(this).find('li');
            liItm.hide();

        });

        $("#sortable").sortable("disable");
        var html = '<li id="liId_0" class="row">' +
                    '<span class="colum">0</span>' +
                    '<span class="colum"><input id="txtSectArId0" name="txtSectArId0" type="text" maxlength="64" value=""></span>' +
                    '<span class="colum" id="spnSectEnId0"><input id="txtSectEnId0" name="txtSectEnId0" type="text" maxlength="64" value=""></span>' +
                    '<span class="colum" id="spnDescArId0">' +
                        '<textarea id="txtDescArId0" name="txtDescArId0" rows="3" cols="28" maxlength="256" onkeydown="return CheckMaxLengthOnKeyDown(event,this);" onpaste="return RestrictMaxLengthOnPaste(this);"></textarea>' +
                    '</span>' +
                    '<span class="colum" id="spnDescEnId0">' +
                        '<textarea id="txtDescEnId0" name="txtDescEnId0" rows="3" cols="28" maxlength="256" onkeydown="return CheckMaxLengthOnKeyDown(event,this);" onpaste="return RestrictMaxLengthOnPaste(this);"></textarea>' +
                    '</span>' +
                    '<span class="colum edit">&nbsp;</span>' +
                    '<span class="colum published">&nbsp;</span>' +
                    '<span class="colum re-order"></span>' +
                '</li>';

        $("#sortable").append(html);

        $('#lnkCrNewSct').text('Cancel Create Sector');
        $('#lnkCrNewSct').attr('class', 'btn-gray pull-left');
    }
    else {
        $('#sortable').each(function () {

            var liItm = $(this).find('li');
            liItm.show();

        });

        $('#sortable li:last-child').remove()

        $('#lnkCrNewSct').text('Create New Sector');
        $('#lnkCrNewSct').attr('class', 'btn-green pull-left');
    }
}
function CancelAddNewMarketSectorRow(){
}
function PostMarketSector() {

    ShowLoadingDiv(true);
    hashUrl("post_market_sector");
    var submitForm = true;


    if ($('#hdnSectUpdValues').val() == '' && $('#txtSectArId0').length > 0) {
        // check for new sector creation
        if ($.trim($('#txtSectArId0').val()) == "") {
            alert('sector should not be empty');
            $('#txtSectArId0').focus();
            submitForm = false;
        }
    }

    if (submitForm) {

        var form = $('#frm01');
        var frmData = form.serialize();

        if (frmData != "") {

            $.ajax({
                type: "POST",
                url: form.attr('action'),
                data: frmData,
                async: false,
                cache: false,
                success: function (res) {

                    ShowLoadingDiv(false);

                    if (res == "success") {
                        alert('Market sector saved successfully');
                        document.location.href = $('#hdnPageRefUrl').val();
                    }
                    else {
                        alert('problem in saving market sectors');
                    }
                }
            });

        }

    }

    ShowLoadingDiv(false);
    return false;
}
function changePublishMktSectVals(o) {
    var hdn = $('#hdnPublishMktSectVals').val();
    var msId = o.id.replace("chkPublished_", "");
    if ($(o).prop('checked')) {

        if ($.trim($('#spnlDescEnId' + msId).text()) == "" || $.trim($('#spnlSectEnId' + msId).text()) == "") {
            alert('provide english info for current sector');
            $(o).removeAttr('checked');
            return;
        }

        if (hdn.indexOf('-' + msId + ',') > -1) {
            hdn = hdn.replace('-' + msId + ',', '');
        }
        else {
            hdn += msId + ',';
        }
    }
    else {
        if (hdn.indexOf(msId + ',') > -1) {
            hdn = hdn.replace(msId + ',', '');
        }
        else {
            hdn += '-' + msId + ',';
        }
    }
    $('#hdnPublishMktSectVals').val(hdn);
}
function PublishSectorsForEn(u) {
    if ($('#hdnPublishMktSectVals').val() == "") {
        alert('No change found in publishing sectors for english');
    }
    else {
        var d = 'MktSectIds=' + $('#hdnPublishMktSectVals').val();
        $.post(u, d,
            function (res) {
                if (res == "") {
                    alert('Problem in publishing sectors for english!');
                }
                else {
                    alert('Selected sectors are available in english now');
                }
            });
    }
}
function UpdateStoreOrder(updUrl) {
    $("#btnUpdOrder").attr("disabled", true);

    var updatedOrder = $("#sortable").sortable("serialize");
    updatedOrder = updatedOrder.replace(/\[\]/gi, "");
    var updateOrderUrl = updUrl;
    $.ajax({
        url: updateOrderUrl,
        data: { 'marketSectorIds': updatedOrder },
        type: 'POST',
        success: function (data) {
            if (data = 'success') {
                alert('Order Updated');
            }
            else {
                alert('Error occured while updating Store order!!!');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
            }
        },
        async: false
    });

    $("#btnUpdOrder").attr("disabled", false);
    document.location.href = $('#hdnPageRefUrl').val();
}
function UpdateArticleReOrder(updUrl) {
    $("#btnUpdOrder").attr("disabled", true);
    var sectionID = $("#ddlSections").val();
    var updatedOrder = $("#sortable").sortable("serialize");
    updatedOrder = updatedOrder.replace(/\[\]/gi, "");
    var updateOrderUrl = updUrl;
    $.ajax({
        url: updateOrderUrl,
        data: { 'articlespublishedinsectionsid': updatedOrder, 'sectionID': sectionID, 'languagueID': $("#ddlLanguages").val() },
        type: 'POST',
        success: function (data) {
            if (data = 'success') {
                alert('Order Updated');
                var sectionID = $("#ddlSections").val();
                var languageID = $("#ddlLanguages").val();
                var jqxhr = $.ajax({
                    type: "GET",
                    url: "/articles/getarticlesbysection?sectionId=" + sectionID + "&languageID=" + languageID,
                })
                         .done(function (data) {
                             if (data) {
                                 $("#sortable").html(data);
                                 $("#articleReorder").removeClass("n-display");
                                 $("#msg").text("");
                             }
                             else {
                                 $("#articleReorder").addClass("n-display");
                                 $("#msg").text("No article found to re-order");
                             }
                         })
                         .fail(function () { })
                         .always(function () { });
                return false;
            }
            else {
                alert('Error occured while updating article re-order!');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
            }
        },
        async: false
    });

    $("#btnUpdOrder").attr("disabled", false);
}

//This method is dependent on a span with message As ID, put span on suitable place and give id as message and the messaging will work 
showMessage = function (message, isError) {
    $("#message").css("display", "none");
    $("#message").removeClass("msg-red");
    $("#message").removeClass("msg-green");
    $("#message").html(message);
    if (isError) {
        $("#message").addClass("msg-red");
        $("#message").addClass("y-display");
    }
    else {
        $("#message").addClass("msg-green");
        $("#message").addClass("y-display");
    }
    $("#message").slideDown(900, function () {
        $("#message").css("display", "block");
    });
    setTimeout(function () {
        $("#message").slideUp(900, function () {
            $("#message").html("");
            $("#message").removeClass("msg-red");
            $("#message").removeClass("msg-green");
        });


    }, 5000);
};
showMessageCb = function (message, isError, callback) {
    $("#message").css("display", "none");
    $("#message").removeClass("msg-red");
    $("#message").removeClass("msg-green");
    $("#message").html(message);
    if (isError) {
        $("#message").addClass("msg-red");
        $("#message").addClass("y-display");
    }
    else {
        $("#message").addClass("msg-green");
        $("#message").addClass("y-display");
    }
    $("#message").slideDown(900, function () {
        $("#message").css("display", "block");
    });
    setTimeout(function () {
        $("#message").slideUp(900, function () {
            $("#message").html("");
            $("#message").removeClass("msg-red");
            $("#message").removeClass("msg-green");
            callback()
        });


    }, 5000);
};
showMessages = function (message, isError, id, durration) {
    $("#" + id).css("display", "none");
    $("#" + id).removeClass("msg-red");
    $("#" + id).removeClass("msg-green");
    $("#" + id).html(message);
    if (isError) {
        $("#" + id).addClass("msg-red");
    }
    else {
        $("#" + id).addClass("msg-green");
    }
    $("#" + id).slideDown(900, function () {

    });
    setTimeout(function () {
        $("#" + id).slideUp(900, function () {
            $("#" + id).html("");
            $("#" + id).removeClass("msg-red");
            $("#" + id).removeClass("msg-green");
        });


    }, durration);
};
///////End of method showMessage
//checkNumeric = function (o) {
//    var ctl = $('#' + o);
//    if (!numberRegex.test(ctl.val())) {
//        //alert('value must be a number');
//        ctl.focus();
//        return false;
//    }
//    return true;
//}
checkNumericCB = function (o, callback) {
    var ctl = $('#' + o);
    if (!numberRegex.test(ctl.val())) {
        //alert('value must be a number');
        ctl.focus();
        callback();
        return false;
    }
    return true;
}
checkPositiveNumberCB = function (o, callback) {
    var ctl = $('#' + o);
    if (!numberOnlyRegex.test(ctl.val())) {
        //alert('value must be a number');
        ctl.focus();
        callback();
        return false;
    }
    return true;
}

/****************** used in article news screen ******************/
function refreshNewsArticlesPager() {
    $("#artNewsPager").paginate({
        count: $("#totalrecords").val() / 10,//pageSize
        start: 1,
        display: 10,
        border: false,
        text_color: '#79B5E3',
        background_color: 'none',
        text_hover_color: '#2573AF',
        background_hover_color: 'none',
        images: false,
        mouse: 'press',
        onChange: function (page) {
            loadNewsArticlePage(page);
        }
    });
}
function loadNewsArticlePage(page) {
    hashUrl("page_" + page);
    console.log(page);

    if ($('#hdnSearchApplied').val() == "1") {
        DoSearchForArticles(0, ($('#hdnIsMyArticles').length > 0 ? 1 : 0), false, page);
    }
    else {

        var u = "/articles/newsarticlelistpage?pageno=" + page
            + "&lang=" + $('#ddlLanguage').val()
            + '&createdBy=0';
        if ($('#hdnIsMyArticles').length > 0) {
            u = u.replace('createdBy=0', 'createdBy=' + $('#ddlCreatedBy').val());
        }

        var jqxhr = $.ajax({
            type: "get",
            url: u,
            async: false,
            beforeSend: function () {
                ShowLoadingDiv(true);
            }
        })
        .done(function (data) {
            $("#dvNArticleList").html(data);
            ScrollTo('dvNArticleList');
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while loading News Article page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () {
            ShowLoadingDiv(false);
        });
    }
    ShowLoadingDiv(false);
}
function LoadArticleTypesByLangId(o) {
    ShowLoadingDiv(true);
    hashUrl("get_articletypes");
    $.ajax({
        url: '/Articles/GetArticleTypesByLangId?lang=' + $(o).val(),
        cache: false,
        async: false,

        success: function (data) {
            $('#ddlArtType').html(data);
        },
        error: function () {
            alert("Some error has occurred while getting article types for language!");
        }
    });
    ShowLoadingDiv(false);
}
function searchNewsArticles(byId) {

    var artcId = 0;
    if (byId) {
        artcId = $('#txtArtID').val();
        if (!checkPositiveNumberCB('txtArtID', function () { showMessage('value must be a non decimal number', true); })) {
            return;
        }
        else if (artcId > 2147483647) {
            showMessage('Value exceeded. Please give less number value', true);
            return;
        }
    }

    var ismyart = 0;
    if ($('#hdnIsMyArticles').length > 0) {
        ismyart = 1;
    }

    $('#hdnSearchApplied').val(1);

    hashUrl("search");

    DoSearchForArticles(artcId, ismyart, true);
}

function DoSearchForArticles(artcId, ismyart, isSearch) {
    var jqxhr = $.ajax({
        type: "POST",
        url: "/articles/newsarticlesearch",
        async: false,
        data: {
            lang: $("#ddlLanguage").val(), artId: artcId, txtTitle: $("#txtTitle").val()
            , createdByUserId: $("#ddlCreatedBy").val(), artTypeId: $("#ddlArtType").val()
            , createFrom: $("#txtCreateFrom").val(), createTo: $("#txtCreateTo").val()
            , IsMyArticle: ismyart, RelatedCompanyID: $("#ddlRelatedCompanies :selected").val(),
            RelatedMarketSectorID: $("#ddlRelatedMarketSectors :selected").val(),
            RelatedArgaamSectorID: $("#ddlRelatedArgaamSectors :selected").val(),
            Page: (arguments.length == 3 ? 1 : arguments[3])
        },
        beforeSend: function () {
            ShowLoadingDiv(true);
        }
    })
    .done(function (data) {
        $("#dvNArticleList").html(data);
        if (isSearch)
            refreshNewsArticlesPager();
    })
    .fail(function (XMLHttpRequest, textStatus, errorThrown) {
        alert("An error occured while searching Article News.");
        console.log(textStatus);
        console.log(errorThrown);
    })
    .always(function () {
        ShowLoadingDiv(false);
    });

}
/****************** Script used in SectorAttributes ******************/
function GetSectorAttributes() {

    if ($('#ddlSectors').val() == 0) {
        $("#sortableSctAtt").html('');
        //$("#sortableSctAtt").css('display','none');
    }
    else {

        var u = $('#hdnGetSctAttsUrl').val() + '?sctid=' + $('#ddlSectors').val();
        $("#sortableSctAtt").html('');

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#sortableSctAtt").append(html);
            },
            error: function () {
                alert('failed in getting attributes for selected sector');
            }
        });
    }

}

function AddSectorAttribute() {

    ShowLoadingDiv(true);

    if ($("#frmSectorAttribute").valid()) {
        var listItms = $("#sortableSctAtt li");

        var attId = $('#ddlAttributes').val();
        var canAdd = true;

        listItms.each(function (idx, li) {
            $(li).css("border", "");

            if (attId == $(li).attr("id").replace("liId_", "")) {
                alert("cannot add the same attribute that's already been added before");
                // mark the li by bordering it as red line
                $(li).css("border", "1px solid red");
                canAdd = false;
            }
        });

        if (canAdd) {

            var hdnVar = $('#hdnSctAtts').val();
            hdnVar += '+' + attId + ",";
            $('#hdnSctAtts').val(hdnVar);

            var liTxt = '<li id=\"liId_' + attId + '">';
            liTxt += '<span class="value">' + $("#ddlAttributes option[value='" + attId + "']").text() + '</span>';
            liTxt += '<a onclick="javascript:RemoveSctAttribute(' + attId + ')" class="close"><i class="icon"></i></a>';
            liTxt += '<span class="re-order"><i class="icon"></i></span>';
            liTxt += '</li>';

            $("#sortableSctAtt").append(liTxt);
        }
    }
    ShowLoadingDiv(false);

    hashUrl('addsectorattribute?id=' + attId);
}

function RemoveSctAttribute(remAttId) {

    ShowLoadingDiv(true);

    $('#liId_' + remAttId).remove();

    // to delete an attribute from market, place negative sign before that attribute Id
    // and handle in code
    var hdnVar = $('#hdnSctAtts').val();

    // when the added attribute needs to be removed then no need to save it in hidden var
    if (hdnVar.indexOf("+" + remAttId + ",") > -1) {
        hdnVar = hdnVar.replace("+" + remAttId + ",", "");
    }
        // when the attribute does not have + then it means the attribute is already there and now need to be removed
    else {
        hdnVar += "-" + remAttId + ",";
    }
    $('#hdnSctAtts').val(hdnVar);

    ShowLoadingDiv(false);
}

function PostSectorAttribute() {

    if ($('#hdnSctAtts').val().length > 0) {

        if ($('#hdnSctAtts').val().indexOf("-") > -1) {
            if (confirm("You have removed one or more attributes. Are you sure to continue?") == false) {
                return;
            }
        }

        ShowLoadingDiv(true);

        $("#btnSaveSctAtts").attr("disabled", true);

        var retVal = false;

        if ($('#hdnSctAtts').val() != "") {

            $.ajaxSetup({ async: false });

            var form = $('#frmSectorAttribute');

            $.post(form.attr('action'), form.serialize(),
                function (res) {
                    ShowLoadingDiv(false);
                    if (res == "success") {
                        retVal = true;
                    }
                    else {
                        alert('some problem');
                    }
                });
        }
        else {
            retVal = true;
        }

        if (retVal) {
            alert('Attributes saved successful');
            //showMessage('Attributes saved successful', false, 'message', 10000);
            document.location.href = '/Attribute/SectorAttributes';
        }

        ShowLoadingDiv(false);
        $("#btnSaveSctAtts").attr("disabled", false);
    }
    else {
        alert('No changes have been made!');
    }
}

function CancelSectorAttribute() {
    $("#sortableSctAtt").html('');
    $('#hdnSctAtts').val('');
    return true;
}

function CallForUpdateSectorAtt(scId) {

    $('#ddlSectors').val(scId);
    GetSectorAttributes();
}

function RefreshSctAttrPg() {
    $("#secAttPager").paginate({
        count: $("#totalrecords").val() / 10,//pageSize
        start: 1,
        display: 10,
        border: false,
        text_color: '#79B5E3',
        background_color: 'none',
        text_hover_color: '#2573AF',
        background_hover_color: 'none',
        images: false,
        mouse: 'press',
        onChange: function (page) {
            loadSecAttPage(page);
        }
    });
}

function loadSecAttPage(page) {
    hashUrl("page_" + page);
    ShowLoadingDiv(true);
    console.log(page);
    var jqxhr = $.ajax({
        type: "get",
        url: "/attribute/sectorattlistpage?pageno=" + page
    })
        .done(function (data) {
            $("#tbSectorsList").html(data);
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while loading Section Template page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}

function SectorAttSearch() {

    if ($('#srchSectorddl').val() == 0) {
        document.location.href = '/Attribute/SectorAttributes';
        return;
    }

    hashUrl("search");
    ShowLoadingDiv(true);

    var jqxhr = $.ajax({
        type: "post",
        url: "/attribute/SectorAttSearch",
        data: { srchSectorddl: $('#srchSectorddl').val() }
    })
        .done(function (data) {
            $("#tbSectorsList").html(data);
            RefreshSctAttrPg();
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while seaching on sector attributes page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}

function DelSectorAtt(u, sctId, canBeDel) {
    if (canBeDel == 1) {
        if (confirm("Are you sure you want to delete all the attributes assigned to this sector?")) {
            u += '?sctid=' + sctId;

            $.ajax({
                url: u,
                async: false,
                cache: false,
                success: function (html) {
                    if (html == "success") {
                        //alert('Attributes assigned to this sector successfully deleted');
                        document.location.href = '/Attribute/SectorAttributes';
                    }
                },
                error: function () {
                    alert('failed in deleting attributes for selected sector');
                }
            });
        }
    }
    else {
        alert('No attributes are assigned to this sector yet!');
    }
}

/****************** Script used in CompanyAttributes ******************/
function CompanyAutoComplete_CompAtts() {
    $("#txtCompany").autocomplete({
        select: function (a, b) {
            $(this).val(b.item.label);
            $('#hdnCompNam').val(b.item.label);
            $('#ddlComps').val(b.item.value);
            GetCompAttributes();
            $("#txtCompany").attr("disabled", true);
            $("#lnkChngComp").show(1000);
            return false;
        },
        source: getCompaniesJson(),
        minLength: 2
    });
}
function GetCompAttributes() {

    if ($('#ddlComps').val() == 0) {
        $("#sortableCmpAtt").html('');
        //$("#sortableCmpAtt").css('display','none');
    }
    else {

        var u = $('#hdnGetCmpAttsUrl').val() + '?compid=' + $('#ddlComps').val();
        $("#sortableCmpAtt").html('');

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#sortableCmpAtt").append(html);
            },
            error: function () {
                alert('failed in getting attributes for selected company');
            }
        });
    }

}
function AddCompanyAttribute() {

    ShowLoadingDiv(true);

    if ($("#frmCompAttribute").valid()) {
        var listItms = $("#sortableCmpAtt li");

        var attId = $('#ddlAttributes').val();
        var canAdd = true;

        listItms.each(function (idx, li) {
            $(li).css("border", "");

            if (attId == $(li).attr("id").replace("liId_", "")) {
                alert("cannot add the same attribute that's already been added before");
                // mark the li by bordering it as red line
                $(li).css("border", "1px solid red");
                canAdd = false;
            }
        });

        if (canAdd) {

            var hdnVar = $('#hdnCmpAtts').val();
            hdnVar += '+' + attId + ",";
            $('#hdnCmpAtts').val(hdnVar);

            var liTxt = '<li id=\"liId_' + attId + '">';
            liTxt += '<span class="value">' + $("#ddlAttributes option[value='" + attId + "']").text() + '</span>';
            liTxt += '<a onclick="javascript:RemoveCompAttribute(' + attId + ')" class="close"><i class="icon"></i></a>';
            liTxt += '<span class="re-order"><i class="icon"></i></span>';
            liTxt += '</li>';

            $("#sortableCmpAtt").append(liTxt);
        }
    }
    ShowLoadingDiv(false);

    hashUrl('AddCompanyAttribute?id=' + attId);
}
function RemoveCompAttribute(remAttId) {

    ShowLoadingDiv(true);

    $('#liId_' + remAttId).remove();

    // to delete an attribute, place negative sign before that attribute Id
    // and handle in code
    var hdnVar = $('#hdnCmpAtts').val();

    // when the added attribute needs to be removed then no need to save it in hidden var
    if (hdnVar.indexOf("+" + remAttId + ",") > -1) {
        hdnVar = hdnVar.replace("+" + remAttId + ",", "");
    }
        // when the attribute does not have + then it means the attribute is already there and now need to be removed
    else {
        hdnVar += "-" + remAttId + ",";
    }
    $('#hdnCmpAtts').val(hdnVar);

    ShowLoadingDiv(false);
}
function PostCompAttribute() {

    if ($('#hdnCmpAtts').val().length > 0)
    {

        if ($('#hdnCmpAtts').val().indexOf("-") > -1) {
            if (confirm("You have removed one or more attributes. Are you sure to continue?") == false) {
                return;
            }
        }

        ShowLoadingDiv(true);

        $("#btnSaveCmpAtts").attr("disabled", true);

        var retVal = false;

        if ($('#hdnCmpAtts').val() != "") {

            $.ajaxSetup({ async: false });

            var form = $('#frmCompAttribute');

            $.post(form.attr('action'), form.serialize(),
                function (res) {
                    ShowLoadingDiv(false);
                    if (res == "success") {
                        retVal = true;
                    }
                    else {
                        alert('some problem');
                    }
                });
        }
        else {
            retVal = true;
        }

        if (retVal) {
            alert('Attributes saved successful');
            //showMessage('Attributes saved successful', false, 'message', 10000);
            document.location.href = '/Attribute/CompanyAttributes';
        }

        ShowLoadingDiv(false);
        $("#btnSaveCmpAtts").attr("disabled", false);
    }
    else if ($('#ddlComps').val() > 0) {
        alert('No change found.');
        //document.location.href = '/Attribute/CompanyAttributes';
    }
}
function CancelCompanyAttribute() {
    $("#sortableCmpAtt").html('');
    $('#hdnCmpAtts').val('');
    $('#ddlAttributes').val(0);

    $("#txtCompany").attr("disabled", false);
    $("#txtCompany").val('');
    $("#lnkChngComp").hide();
    $('#hdnCompNam').val('');
    $('#ddlComps').val('0');

    return true;
}
function CallForUpdateCompanyAtt(cmpId) {
    $('#ddlComps').val(cmpId);
    GetCompAttributes();
    var _compName = $('#compName_' + cmpId).html();
    $('#hdnCompNam').val(_compName);
    $("#txtCompany").val(_compName);
    $("#txtCompany").attr("disabled", true);
}
function RefreshCompAttrPg() {
    $("#cmpAttPager").paginate({
        count: $("#totalrecords").val() / 10,//pageSize
        start: 1,
        display: 10,
        border: false,
        text_color: '#79B5E3',
        background_color: 'none',
        text_hover_color: '#2573AF',
        background_hover_color: 'none',
        images: false,
        mouse: 'press',
        onChange: function (page) {
            loadCompAttPage(page);
        }
    });
}
function loadCompAttPage(page) {
    hashUrl("page_" + page);
    ShowLoadingDiv(true);
    console.log(page);
    var jqxhr = $.ajax({
        type: "get",
        url: "/attribute/CompanyAttListPage?pageno=" + page
    })
        .done(function (data) {
            $("#tbCompsList").html(data);
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while loading Section Template page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}
function CompAttSearch() {

    if ($('#srchCompddl').val() == 0) {
        document.location.href = '/Attribute/CompanyAttributes';
        return;
    }

    hashUrl("search");
    ShowLoadingDiv(true);

    var jqxhr = $.ajax({
        type: "post",
        url: "/attribute/CompanyAttSearch",
        data: { srchCompddl: $('#srchCompddl').val() }
    })
        .done(function (data) {
            $("#tbCompsList").html(data);
            RefreshCompAttrPg();
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while seaching on sector attributes page.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    ShowLoadingDiv(false);
}

/****************** Script used in both SectorAttributeValues & CompanyAttributeValues ******************/
function SetSectorCompanyAttributeValues(forSector) {

    if (confirm("Are you sure to save the changes?")) {
        var form = $('#frmSctAttVals');

        var jqxhr = $.ajax({
            type: "post",
            url: form.attr('action'),
            data: form.serialize()
        })
        .done(function (data) {
            if (data == 'success') {
                alert("Attributes values saved successfully");
                //if (forSector == -1) {
                //    GetCompaniesForMarket();
                //}
                //else {
                //    GetSectorsForMarket();
                //}
            }
        })
        .fail(function (XMLHttpRequest, textStatus, errorThrown) {
            alert("An error occured while posting sector/company attribute values.");
            console.log(textStatus);
            console.log(errorThrown);
        })
        .always(function () { });
    }
}
function validateTBForAttributeValues(currCtl, id, attDT) {

    var retVal = false;

    if (attDT != 1 && currCtl.value.length > 0) {

        var range = '';
        if (attDT == 2) {
            retVal = checkPositiveNumberCB(currCtl.id, function () { showMessages('value must be non decimal number', true, id, 5000); });
            range = '99,999,999,999,999';
        }
        else if (attDT == 3) {
            retVal = checkNumericCB(currCtl.id, function () { showMessages('value must be numeric', true, id, 5000); });
            range = '99,999,999,999,999.9';
        }

        if (retVal) {
            if (currCtl.value > 99999999999999.9) {
                showMessages('value must be less than ' + range, true, id, 5000);
                retVal = false;
            }
        }
    }
    else {
        retVal = true;
    }

    return retVal;
}

/****************** Script used in SectorAttributeValues ******************/
function GetSectorsForMarket() {
    document.location.href = '/Attribute/SectorAttributeValues?mktId=' + $('#ddlMarkets').val();
}
function GetAttributes4SectAttValues() {
    var firstOpt = '<option value="0">--Select--</option><option value="-1">ALL</option>';
    $("#ddlAttributes").html(firstOpt);

    if ($('#ddlSectors').val() > 0) {

        if ($("input[type='radio']:checked").val() == 'bycompany') {
            var u = $('#hdnGetSctAttsUrl').val() + '?sctid=' + $('#ddlSectors').val();

            $.ajax({
                url: u,
                cache: false,
                async: false,
                success: function (html) {
                    var firstOpt = '<option value="0">--Select--</option>';
                    $("#ddlAttributes").html(firstOpt + html);
                },
                error: function () {
                    alert('failed in getting attributes for selected sector');
                }
            });
        }
    }
}
function FormLayout() {
    if ($("input[type='radio']:checked").val() == 'bycompany') {
        GetAttributes4SectAttValues();
    }
    else {
        var firstOpt = '<option value="-1">ALL</option>';
        $("#ddlAttributes").html(firstOpt);
        $("#tbAttValsList").html('');
    }
}
function SetFormCtlsByFPeriodType() {
    if ($('#ddlFPeriodType option:selected').text().toLowerCase() == "year") {
        $('#dvYear').hide();
    }
    else if ($('#ddlFPeriodType').val() > 0) {
        $('#dvYear').show();
    }
}
function GetSectorAttributeValues() {
    if ($("#frmSectorAttributeVal").valid()) {

        var byComp = ($("input[type='radio']:checked").val() == 'bycompany');
        var entity = byComp ? $('#hdnCompEnitityID').val() : $('#hdnSectEnitityID').val();

        var u = $('#hdnGetSctAttValsUrl').val();
        u += '?mktid=' + $('#ddlMarkets').val() + '&entity=' + entity + '&id=' + $('#ddlSectors').val();
        u += '&valbycompany=' + (byComp ? 1 : 0);
        u += '&attid=' + $("#ddlAttributes").val() + '&fptypeid=' + $('#ddlFPeriodType').val();
        u += '&year=' + $('#ddlYear').val() + '&tblheadtxt=' + (byComp ? $('#ddlAttributes :selected').text() : $('#ddlSectors :selected').text());

        ShowLoadingDiv(true);

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                if ($.trim(html) == '') {
                    $("#tbAttValsList").html('');
                    alert('No Attributes found');
                }
                else {
                    $("#tbAttValsList").html(html);
                }
            },
            error: function () {
                ShowLoadingDiv(false);
                alert('failed in creating table for sector attribtues data');
            }
        });
        ShowLoadingDiv(false);
    }
}

/****************** Script used in CompanyAttributeValues ******************/
function CompanyAutoComplete_CompAttVals() {
    $("#txtCompany").autocomplete({
        select: function (a, b) {
            $(this).val(b.item.label);
            $('#hdnCompNam').val(b.item.label);
            $('#ddlCompany').val(b.item.value);
            GetAttributes4CompAttValues();
            $("#txtCompany").attr("disabled", true);
            $("#lnkChngComp").show(1000);
            return false;
        },
        source: getCompaniesJson(),
        minLength: 2
    });
}
function ChangeCompany() {
    $('#ddlAttributes').html('');
    $('#ddlAttributes').html('<option value="-1">ALL</option>');

    $("#txtCompany").attr("disabled", false);
    $("#txtCompany").val('');
    $("#lnkChngComp").hide();
    $('#hdnCompNam').val('');
    $('#ddlCompany').val('0');
}
function GetCompaniesForMarket() {
    document.location.href = '/Attribute/CompanyAttributeValues?mktId=' + $('#ddlMarkets').val();
}
function GetAttributes4CompAttValues() {
    var firstOpt = '<option value="-1">ALL</option>';
    $("#ddlAttributes").html(firstOpt);

    if ($('#ddlCompany').val() > 0) {

        var u = $('#hdnGetCompAttsUrl').val() + '?cmpid=' + $('#ddlCompany').val();

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#ddlAttributes").html(firstOpt + html);
            },
            error: function () {
                alert('failed in getting attributes for selected company');
            }
        });
    }
}

function UpdateSectionReOrder(updUrl) {
    $("#btnUpdOrder").attr("disabled", true);
    var pageID = $("#ddlPages").val();
    var updatedOrder = $("#sortable").sortable("serialize");
    updatedOrder = updatedOrder.replace(/\[\]/gi, "");
    var updateOrderUrl = updUrl;
    $.ajax({
        url: updateOrderUrl,
        data: { 'sectionsID': updatedOrder },
        type: 'POST',
        success: function (data) {
            if (data = 'success') {
                alert('Order Updated');
                var pageID = $("#ddlPages").val();
                var languageID = $("#ddlLanguages").val();
                var jqxhr = $.ajax({
                    type: "GET",
                    url: "/Section/GetSectionToReorder?pageID=" + pageID + "&languageID=" + languageID,
                })
                     .done(function (data) {
                         if (data) {
                             $("#sortable").html(data);
                             $("#sectionReorder").removeClass("n-display");
                             $("#message").text("");
                         }
                         else {
                             $("#sectionReorder").addClass("n-display");
                             $("#message").text("No section found to re-order");
                         }
                     })
                     .fail(function () { })
                     .always(function () { });
                return false;
            }
            else {
                alert('Error occured while updating section re-order!');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
            }
        },
        async: false
    });

    $("#btnUpdOrder").attr("disabled", false);
}
function GetCompanyAttributeValues() {
    if ($("#frmCompAttributeVal").valid()) {

        if ($('#ddlAttributes option').length <= 1) {
            $("#tbAttValsList").html('');
            alert('No Attributes found');
            return false;
        }

        var entity = $('#hdnCompEnitityID').val();

        var u = $('#hdnGetCompAttValsUrl').val();
        u += '?mktid=' + $('#ddlMarkets').val() + '&id=' + $('#ddlCompany').val();
        u += '&attid=' + $("#ddlAttributes").val() + '&fptypeid=' + $('#ddlFPeriodType').val() + '&year=' + $('#ddlYear').val();

        ShowLoadingDiv(true);

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#tbAttValsList").html(html);
            },
            error: function () {
                ShowLoadingDiv(false);
                alert('failed in creating table for company attribtues data');
            }
        });
        ShowLoadingDiv(false);
    }
}

/****************** Script used in Add/Edit Attribute ******************/
function PostAttribute() {

    var bRetVal = false;

    if ($("#frmAttributes").valid()) {

        $("#dvErrtxtAttFormula").hide();

        if ($('#chkIsComputed').is(":checked") &&
            $.trim(CKEDITOR.instances['txtAttFormula'].getData()) == "") {

            $("#dvErrtxtAttFormula").html('Formula is required');
            $("#dvErrtxtAttFormula").show();
            $("#txtAttFormula").focus();
            return false;
        }
        else {
            $("#dvErrtxtAttFormula").hide();
            var part = '<p>';
            var full = CKEDITOR.instances['txtAttFormula'].getData();
            var first = full.indexOf(part);
            if (first != -1 && first != full.lastIndexOf(part)) {
                $("#dvErrtxtAttFormula").html('Formula is not correct, please remove new lines etc.');
                $("#dvErrtxtAttFormula").show();
                return false;
            }
            else {
                $('#hdnFormulaText').val(CKEDITOR.instances['txtAttFormula'].getData());
                bRetVal = true;
            }
        }

        if (bRetVal) {

            var form = $('#frmAttributes');
            var frmData = form.serialize();

            if (frmData != "") {

                $.post(form.attr('action'), frmData,
                function (res) {

                    ShowLoadingDiv(false);

                    if (res == "success") {
                        alert('Attribute saved successfully');
                        document.location.href = '/Attribute/Attributes';
                    }
                    else {
                        alert('some problem');
                    }
                });
            }
        }
    }
    return false;
}
function ToggleFormulaCtlDisable() {
    if ($('#chkIsComputed').is(":checked")) {

        $('#dvddlFormulaFType').show();
        //$('#dvFnFldType').show();
        $('#dvFldFormula').show();

        $("#ddlFormulaFType").focus();
    }
    else {

        $('#dvddlFormulaFType').hide();
        $('#dvFnFldType').hide();
        $('#dvFldFormula').hide();
    }
}
function RefreshPg() {
    document.location.href = "/Attribute/Attributes";
}
function AttributeFormLayout() {
    if ($("#ddlFormulaFType").val() == 2) {
        $('#dvFnFldType').show();
        $("#lstFields").html('');
    }
    else {//if ($("#ddlFormulaFType").val() == 1) {
        $('#dvFnFldType').hide();
        $("#ddlFinancialFType").val(0);
        GetFieldsForFormulaCreateOrUpdate();
    }
}
function GetFieldsForFormulaCreateOrUpdate() {
    $("#lstFields").html('');
    var formulaTyp = $('#ddlFormulaFType').val();

    if (formulaTyp > 0) {

        var u = $('#hdnGetFieldsByFieldTypeURL').val() + '?ftype=' + formulaTyp;

        if (formulaTyp == 1 || formulaTyp == 3) {
            u += '&fsfcatid=0&fscatcode=0';
        }
        else if (formulaTyp == 2) {
            var finFldType = $("#ddlFinancialFType :selected");
            if (finFldType.val() == 0) {
                return;
            }
            u += '&fsfcatid=' + finFldType.val() + '&fscatcode=' + finFldType.attr('optid');
        }

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#lstFields").html(html);
            },
            error: function () {
                alert('failed in getting field info for attribute forumula creation/updation');
            }
        });
    }
}
function AddToFormula(o) {

    var fieldPrefixVal = '', formulaString = '', formulaExp = '';
    var fieldID;

    var arrID = o.parentElement.id.split('_');
    fieldPrefixVal = arrID[0];
    fieldID = arrID[1];

    formulaString = '[' + fieldPrefixVal + '_' + $(o).text() + ']';
    formulaExp = '[' + fieldPrefixVal + '_' + fieldID + ']';

    //$('#txtAttFormula').text($('#txtAttFormula').text() + formulaString);

    var btn4Editor = '<input id="' + formulaExp + '" type="button" value="' + formulaString + '" />';

    // insertHtml method is not working, the below stmts is the work-around as per stackoverflow
    //CKEDITOR.instances['txtAttFormula'].insertHtml(btn4Editor);

    CKEDITOR.instances['txtAttFormula'].insertElement(
        CKEDITOR.dom.element.createFromHtml(btn4Editor)
    );

}
function AttributeFormLayout4TCAtt() {
    if ($('#txtAttName').val().indexOf("TC_") == 0) {
        $('#chkIsComputed').attr('disabled', 'disabled');
        $('#chkIsComputed').removeAttr('checked');
        $('#dvddlFormulaFType').hide();
        $('#dvFnFldType').hide();
        $('#dvFldFormula').hide();
        //$('#ddlFormulaFType').attr('disabled', 'disabled');
        //CKEDITOR.instances['txtAttFormula'].setData('');
        //CKEDITOR.instances['txtAttFormula'].setReadOnly();
    }
    else {
        $('#chkIsComputed').removeAttr('disabled');
        //$('#ddlFormulaFType').removeAttr('disabled');
        //CKEDITOR.instances['txtAttFormula'].setReadOnly(false);
    }
}

/****************** Script used in Attibutes Display Area Settings ******************/
function RefreshReportSettingPage() {
    document.location.href = "/Attribute/ReportSettings";
}
function ReportFormLayoutByRepoTyp(disableBothControls) {
    var pgSection = $('#pg_section').val();

    $('input[name="comp_sect_values"][value="bysector"]').removeAttr('disabled');

    $('#dvCompSectorValues').show();
    $('#dvChkReportAsChart').hide();

    if (pgSection > 0) {
        if (pgSection == 1 || pgSection == 2) {
            $('#dvChkReportAsChart').show();
        }

        if (pgSection == 1 || pgSection == 2 || pgSection == 3 || pgSection == 7 || pgSection == 5 || pgSection == 9) {
            $('#dvSectors').show(); 
            $('#dvParentReport').show();
            if (pgSection == 9) {
                $('#lblSector').html('Select Product');
                $('#dvCompSectorValues').hide();
            }
            else {
                $('#lblSector').html('Select Sector');
            }

            // change source of auto complete : LATER
            if (arguments.length == 1) {
                if (pgSection == 9) {
                    GetPetroChemProducts('petrochemProducts');
                }
                else {
                    GetPetroChemProducts('sectors');
                }
            }
            $('#dvCompany').hide();
        }
        else if (//pgSection == 4 ||
            pgSection == 7 || pgSection == 8) {// || pgSection == 6) {
            $('#dvSectors').hide();
            $('#dvParentReport').hide();

            $('#dvCompany').show();

            $('input[name="comp_sect_values"][value="bycompany"]').prop('checked', true);
            $('input[name="comp_sect_values"][value="bysector"]').attr('disabled', 'disabled');
        }
        $('#AttFFGroupDiv').html('');
        $('#reportFields').val(0);

    }
    else {
        $('#dvParentReport').hide();
        $('#dvSectors').hide();
        $('#dvCompany').hide();
    }
    if (disableBothControls) {
        $('#sectors').removeAttr('disabled');
        $('#txtCompany').removeAttr('disabled');
        $('#txtCompany').val('');
        //$('#FiscalPeriodTypes').removeAttr('disabled');
    }

    /*if (pgSection == 1) { // Facts and Stats Cement page
        if (arguments.length == 1) {
            //$('#FiscalPeriodTypes').val(2); // set to Month
            $('#FiscalPeriodTypes').html('<option value="0">--Select--</option><option value="2" >Month</option>');
        }
        else if (arguments[1] != 4) {
            //$('#FiscalPeriodTypes').val(2);
            $('#FiscalPeriodTypes').html('<option value="0">--Select--</option><option value="2" >Month</option>');
        }
        else if (arguments[1] == 4) {
            //$('#FiscalPeriodTypes').val(3); // set to Quarter
            $('#FiscalPeriodTypes').html('<option value="0">--Select--</option><option value="3" >Quarter</option>');
        }
        //$('#FiscalPeriodTypes').attr('disabled', 'disabled');
    }
    else if (pgSection == 2) { // Facts and Stats Bank page
        //$('#FiscalPeriodTypes').val(4); // set to Year
        $('#FiscalPeriodTypes').html('<option value="0">--Select--</option><option value="4" >Year</option>');
        //$('#FiscalPeriodTypes').attr('disabled', 'disabled');
    }
    else if (pgSection == 7) { // Home page-Facts& Stats
        //$('#FiscalPeriodTypes').val(2); // set to Month
        $('#FiscalPeriodTypes').html('<option value="0">--Select--</option><option value="2" >Month</option>');
        //$('#FiscalPeriodTypes').attr('disabled', 'disabled');
    }*/

}
function onChangeFiscalPeriodTypes(selVal) {
    var pgSection = $('#pg_section').val();
    var fpType = $('#FiscalPeriodTypes').val();

    if ((pgSection == 8 || pgSection == 9) && (fpType == 2 || fpType == 3 || fpType == 5)) {
        alert('Please select Year');
        $('#FiscalPeriodTypes').val(selVal);
    }
}
function CompanyAutoComplete() {
    $("#txtCompany").autocomplete({
        select: function (a, b) {
            $(this).val(b.item.label);
            $('#hdnCompNam').val(b.item.label);
            $('#hdnCompID').val(b.item.value);
            return false;
        },
        source: getCompaniesJson(),
        minLength: 2
    });
}
function getCompaniesJson() {
    var data1 = '';

    $.ajax({
        url: $('#hdnUrl4CompJson').val(),
        cache: false,
        async: false,
        success: function (data) {
            data1 = data;
        },
        error: function () { alert("Ooops!! Please try again"); }
    });

    return data1;
}
function clearAFGSection() {
    $('#AttFFGroupDiv').html('');
    hashUrl('change report field');
}
function getSectorOrCompanyID() {
    return $('#dvSectors').css('display') == 'none' ?
        ($.trim($('#hdnCompNam').val()) != $.trim($('#txtCompany').val()) ? -1 : $('#hdnCompID').val())
        : $('#sectors').val();
}
function ShowRepoFld() {

    $('#AttFFGroupDiv').html('');

    if ($("#frmRepoSettings").valid()) {

        hashUrl('ShowReportFields');
        $('#AttFFGroupDiv').html('');
        var repoFldId = $('#reportFields').val();

        if (repoFldId > 0) {

            var rowId = getSectorOrCompanyID();

            if (rowId > 0) {

                var entity = ($('#pg_section').val() != 9 ? ($('#dvSectors').css('display') == 'none' ? 'companies' : 'sectors') : 'petrochemProducts');

                var u = $('#hdnGetRepoFldsDataByRepoFldURL').val() + '?repoFld=' + repoFldId
                    + '&entity=' + entity
                    + '&compOrSectorID=' + rowId;

                $.ajax({
                    url: u,
                    cache: false,
                    async: false,
                    success: function (html) {
                        if ($.trim(html) == '') {
                            if (repoFldId == 1 || repoFldId == 3) {
                                alert('No attributes are assigned to selected '
                                    + ($('#dvSectors').css('display') == 'none' ? 'company' : 'sector')
                                );
                            }
                        }
                        else {
                            $('#AttFFGroupDiv').html(html);
                        }
                    },
                    error: function () {
                        alert('failed in getting report fields info by report field Id');
                    }
                });
            }
            else {
                if (rowId == -1) {
                    alert('The company is not a valid company');
                }
            }
        }
    }
}
function GetFieldsForRepoSetting() {
    if ($('#FFCategories').val() > 0) {

        var u = $('#hdnGetFieldsByCatURL').val() + '?fsCatID=' + $('#FFCategories').val();

        $.ajax({
            url: u,
            cache: false,
            async: false,
            success: function (html) {
                $("#FSFields").html('<option value="0">--Select--</option>' + html);
            },
            error: function () {
                alert('failed in getting field info for attribute forumula creation/updation');
            }
        });
    }
}
function GetPetroChemProducts(entity) { // get products drop down values for products report group 
    var u = $('#hdnGetDDLVals_EntityLevel').val() + '?entID=' + (entity == 'sectors' ? 5 : 37/*PetrochemicalProducts entity*/);

    $.ajax({
        url: u,
        cache: false,
        async: false,
        success: function (html) {
            $("#sectors").html('<option value="0">--Select--</option>' + html);
        },
        error: function () {
            alert('failed in getting products info for given report group');
        }
    });
}
function RemoveRow(id) {
    var row_id = $('#' + id);
    if (true/*confirm("Are you sure?")*/) {

        // check if the id is of group and the remaining items are 1
        if (id.indexOf('GRP') > -1) {

            var li_par = row_id.parent()
            
            if (li_par.find('li').length == 1) {

                row_id = li_par.parent();

                var numOfGrps = $('#hdnNumOfGroups').val(); numOfGrps--;
                $('#hdnNumOfGroups').val(numOfGrps);

                $('#hdnAddChildElems').val(0);
            }
        }

        // Change the back color of the Row before deleting
        row_id.css("background-color", "Red");
        // Do some animation effect
        row_id.fadeOut(800, function () {
            //Remove GridView row
            row_id.remove();

            // update number of table rows 
            var numOfTblRows = $('#hftblNumOfRows').val();
            if (--numOfTblRows < 0) {
                numOfTblRows = 0;
            }
            $('#hftblNumOfRows').val(numOfTblRows);

            // when case of update put the removed id to be negative
            if ($('#btnAddReportSettings').css('display') == 'none') {
                
                id = id.replace('liId_', '');

                if (id.indexOf('GRP_') > -1) {
                    id = id.replace(id.substring(0, 6), 'AT_');
                }
                //else if (id.indexOf('FF_') > -1) {
                //    id = id.replace(id.substring(0, 6), 'FD_');
                //}

                var allFlds = $('#hfChkDuplicateFields').val();
                if (allFlds.indexOf(id) > -1) {
                    var tmpId = id;
                    tmpId = tmpId.replace('AT_', 'AE_')
                    tmpId = tmpId.replace('FF_', 'FD_')
                    allFlds = allFlds.replace(id, '-' + tmpId);
                    $('#hfChkDuplicateFields').val(allFlds);
                }
            }

            if (numOfTblRows <= 0) {
                $('#tblData').hide();
                $('#btnsSavUpdCnc').hide();
            }
        });
    }
}
function ListItemText(id, repoFld, txt, parRepoFldId) {

    // check for duplicate fields add-up
    var allFlds = $('#hfChkDuplicateFields').val();
    if (allFlds.indexOf(id + ',') > -1) {
        alert('cannot add duplicate fields');
        return "";
    }

    hashUrl((repoFld == 1 ? 'Sector Attribute' : 'Financial Field') + ' added');

    //isreportAsChart

    var liItm = '<li id="liId_' + id + '" class="row">';
    liItm += '<input id="hdn_' + id + '" name="hdn_' + id + '" type="hidden" value="' + id + '" />';
    if (parRepoFldId > 0) {
        liItm += '<input id="hdn_ParentId_' + id + '" name="hdn_ParentId_' + id + '" type="hidden" value="' + parRepoFldId + '" />';
    }
    liItm += '<span class="colum">' + (repoFld == 1 || repoFld == 3 ? 'Attribute' : 'Financial Field') + '</span>';
    liItm += '<span class="colum">' + txt + '</span>';
    liItm += '<span class="colum delete"><a onclick="javascript:RemoveRow(\'liId_' + id + '\');"><i class="icon delete"></i></a></span>';
    if (arguments.length == 6 && (arguments[5] == 1 || arguments[5] == 2)) { //&& arguments.length > 4) {
        var checked = arguments[4] == 1 ? ' checked="checked"' : '';
        liItm += '<span class="colum"><input id="chk_fld_as_chart' + id + '" type="checkbox" name="chk_fld_as_chart' + id + '"' + checked + '><label>Show Field As Chart Field</label></span>';
    }
    //liItm += '<span class="colum re-order"><i class="icon re-order"></i></span>';
    liItm += '</li>';

    if (id.indexOf('GRP_') > -1) {
        id = id.replace(id.substring(0, 6), 'AT_');
    }
    allFlds += id + ',';
    $('#hfChkDuplicateFields').val(allFlds);

    var numOfTblRows = $('#hftblNumOfRows').val();
    $('#hftblNumOfRows').val(++numOfTblRows);

    if (numOfTblRows == 1) {
        $('#tblData').show();
        $('#btnsSavUpdCnc').show();
    }

    return liItm;
}
function GroupListItemText(id, grpNamAr, grpNamEn, attId, repoFldId, attText, parRepoFldId) {
    
    var liItm = '<li style="background-color: aliceblue;" id="liId_' + id + '" class="row">';//liId_GRP_0
    liItm += '<input id="hdn_main_' + id + '" name="hdn_main_' + id + '" type="hidden" value="' + id + '" />';
    liItm += '<input id="hdn_TxtAr_' + id + '" name="hdn_TxtAr_' + id + '" type="hidden" value="' + grpNamAr + '" />';
    liItm += '<input id="hdn_TxtEn_' + id + '" name="hdn_TxtEn_' + id + '" type="hidden" value="' + grpNamEn + '" />';
    liItm += '<span class="colum">Group</span>';
    liItm += '<span class="colum">' + grpNamEn + '</span>';
    liItm += '<span class="colum delete"></span>';
    //liItm += '<span class="colum re-order"><i class="icon re-order"></i></span>';

    // start group unordered list
    liItm += '<ul id="ul_' + id + '">';

    id += '_' + attId;

    // add first item to group unordered list
    liItm += ListItemText(id, repoFldId, attText, parRepoFldId);

    // end group unordered list
    liItm += '</ul>';

    // end grouped list-item
    liItm += '</li>';

    return liItm;
}
function AddFldRow() {
    // call this method for attribute or financial field
    if ($('#frmFldSelection').valid()) {

        hashUrl('AddReportField');

        // set for group add button to enable group rows addition
        $('#hdnAddChildElems').val(0);


        var attOrffId = ($('#attributes').length > 0 ? $('#attributes').val() : $('#FSFields').val());
        var repoFldId = $('#reportFields').val();
        var id = (repoFldId == 1 ? 'AT_' : 'FF_') + attOrffId;
        var attOrFFText = (repoFldId == 1 ? $('#attributes :selected').text() : $('#FSFields :selected').text());
        var repoGrpId = $('#pg_section').val();

        var liItm = '';
        if (repoGrpId == 1 || repoGrpId == 2) {
            liItm = ListItemText(id, repoFldId, attOrFFText, 0, 0, repoGrpId);
        }
        else {
            liItm = ListItemText(id, repoFldId, attOrFFText, 0);
        }
        $('#repoFields').append(liItm);

    }
}
function AddGroupFldRow() {
    if ($('#frmFldSelection').valid()) {

        hashUrl('AddReportField');

        if ($('#hdnAddChildElems').val() == 0) { // means to add group
            var grpNamAr = $('#txtGroupNameAr').val(), grpNamEn = $('#txtGroupNameEn').val();
            var id = 'GRP_' + $('#hdnNumOfGroups').val();

            // add grouped list-item
            var liItm = GroupListItemText(id, grpNamAr, grpNamEn, $('#attributes').val()
                , $('#reportFields').val(), $('#attributes :selected').text(), 0);

            $('#repoFields').append(liItm);

            // set to 1 means next time no need to add group UL
            $('#hdnAddChildElems').val(1);
            var numOfGrps = $('#hdnNumOfGroups').val(); numOfGrps++;
            $('#hdnNumOfGroups').val(numOfGrps);
        }
        else {
            // add 2nd child

            //first make decrement the group value
            var numOfGrps = $('#hdnNumOfGroups').val(); numOfGrps--;

            var id = 'GRP_' + numOfGrps + '_' + $('#attributes').val();
            var liItm = ListItemText(id, $('#reportFields').val(), $('#attributes :selected').text(), 0);

            $('#ul_GRP_' + numOfGrps).append(liItm);
        }
    }
}
function PostReportSettings() {

    ShowLoadingDiv(true);
    if ($("#frmRepoSettings").valid()) {

        var numOfTblRows = $('#hftblNumOfRows').val();
        if (numOfTblRows <= 0) {
            alert('There are no report fields added for this report. Kindly add report fields!');
            ShowLoadingDiv(false);
            return;
        }

        //var _entity = ($('#dvSectors').css('display') == 'none' ? 'companies' : 'sectors');
        var _entity = ($('#pg_section').val() != 9 ? ($('#dvSectors').css('display') == 'none' ? 'companies' : 'sectors') : 'petrochemProducts');
        var form = $('#frmRepoSettings');
        var d = form.serializeArray();
        d.push({ name: 'entity', value: _entity }, { name: 'rowId', value: getSectorOrCompanyID() });

        $.ajax({
            url: form.attr('action'),
            data: d,
            type: 'POST',
            success: function (data) {

                ShowLoadingDiv(false);

                if (data = 'success') {
                    retVal = true;
                    alert('Report settings saved successfully');
                    RefreshReportSettingPage();
                }
                else {
                    alert('Error occured while saving report settings!!!');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowLoadingDiv(false);
                if (jqXHR.readyState != 0) {
                }
            },
            async: false
        });

    }
}
function GetCompOrSectByRepGrpID() {

    var rptGrpId = $('#reorder_pg_section').val();

    if (rptGrpId > 0) {
        var u = '/Attribute/GetSectorOrCompanyOptionsByReportGrpID?rptGrpID=' + rptGrpId;

        $.ajax({
            url: u,
            cache: false,
            async: false,
            beforeSend: function () {
                //$('#dvLoading').show();
            }
        })
        .done(function (data) {
            $('#reorder_comp_or_sector').html(data);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
                alert("try again later" + '\r' + textStatus + '\r' + errorThrown);
            }
        })
        .always(function () {
            //$('#dvLoading').hide()
        });
    }
    else {
        $('#reorder_comp_or_sector').html('<option value="0">Select</option>');
    }
}
function ShowReportsForReOrder() {

    var u = '/Attribute/ReportSettings?rtid='
        + $('#reorder_pg_section').val()
        + '&rowId=' + $('#reorder_comp_or_sector').val();
    document.location.href = u;
}
function ShowReportFields(toSearchElem, currElem) {

    var li = $(currElem).parent().parent().parent()[0];
    var ul = null;

    if (toSearchElem == 'ul') {
        ul = $(li).find('ul')[0];
    }
    else {
        ul = $(li).find('ul li');
    }

    if (ul != null || ul != undefined) {
        if ($(currElem).text() == "+") {
            $(ul).show();
            $(currElem).html($(currElem).html().replace('+', '-'));
        }
        else {
            $(ul).hide();
            $(currElem).html($(currElem).html().replace('-', '+'));
        }
    }
}
function PostReportSettingsOrder() {

    var retVal = false;
    var i = 0;

    var updatedOrder = null;
    var d = $('#frmReorderRepoSettings').serializeArray();

    var arr = $('#hdnSortIds').val().split(',');
    for (i = 0; i < arr.length - 1; i++) {
        updatedOrder = $("#" + arr[i]).sortable("serialize");
        d.push({ name: arr[i], value: updatedOrder });
    }
    updatedOrder = $("#reportReorderFields").sortable("serialize");
    updatedOrder = updatedOrder.replace(/\[\]/gi, "");
    d.push({ name: 'reportIds', value: updatedOrder });

    // now apply the sorting
    $.ajax({
        url: $("#frmReorderRepoSettings").attr('action'),
        data: d,
        type: 'POST',
        success: function (data) {
            if (data = 'success') {
                retVal = true;
                alert('Attibutes order updated successfully');
            }
            else {
                alert('Error occured while updating attributes order!!!');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
            }
        },
        async: false
    });

    if (retVal) {
        document.location.href = "/Attribute/ReportSettings";
    }

}
function EditReport(rptId) {

    hashUrl('EditReport?id=' + rptId);
    var u = $('#hdnRepoSettingEditUrl').val() + '?rptid=' + rptId;
    $.ajax({
        url: u,
        cache: false,
        async: false,
        success: function (res) {
            $('#frmRepoSettings').html(res);
        },
        error: function () {
            alert("getting report edit record is failed! Please try again");
        }
    });
    ReportFormLayoutByRepoTyp(false, rptId);
    $('#reportFields').val(1);
}
function DelReport(rptId) {
    if (confirm("Are you sure you want to delete the report?")) {

        var u = '/Attribute/DeleteReport?rptid=' + rptId;

        $.ajax({
            url: u,
            cache: false,
            async: false,
            beforeSend: function () {
                $('#dvLoading').show();
            }
        })
        .done(function (data) {
            if (data == "success") {
                $('#roliId_' + rptId).remove();
                //alert('Report deleted successfully.');
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.readyState != 0) {
                alert("try again later" + '\r' + textStatus + '\r' + errorThrown);
            }
        })
        .always(function () {
            $('#dvLoading').hide()
        });

        //ShowReportsForReOrder();
        //document.location.href = '/Attribute/ReportSettings';
        document.location.href = '/Attribute/ReportSettings?rtid=' + $('#reorder_pg_section').val();
    }
    return false;
}
function UpdateReportSettings() {

    if ($("#frmRepoSettings").valid()) {
        ShowLoadingDiv(true);
        var numOfTblRows = $('#hftblNumOfRows').val();
        if (numOfTblRows <= 0) {
            alert('There are no report fields added for this report. Kindly add report fields!');
            ShowLoadingDiv(false);
            return;
        }

        //var _entity = ($('#dvSectors').css('display') == 'none' ? 'companies' : 'sectors');
        var _entity = ($('#pg_section').val() != 9 ? ($('#dvSectors').css('display') == 'none' ? 'companies' : 'sectors') : 'petrochemProducts');
        var form = $('#frmRepoSettings');
        var d = form.serializeArray();
        d.push({ name: 'entity', value: _entity }, { name: 'rowId', value: getSectorOrCompanyID() });

        $.ajax({
            url: $('#hdnUpdateReportURL').val(),
            data: d,
            type: 'POST',
            success: function (data) {

                ShowLoadingDiv(false);

                if (data = 'success') {
                    retVal = true;
                    alert('Report settings saved successfully');
                    RefreshReportSettingPage();
                }
                else {
                    alert('Error occured while saving report settings!!!');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowLoadingDiv(false);
                if (jqXHR.readyState != 0) {
                }
            },
            async: false
        });

    }
}
function CancelCrEd() {
    document.location.reload();

    //$('#repoFields').html('');

    ////reset all hidden variables
    //$('#hdnCompID').val(0);
    //$('#hdnCompNam').val('');
    //$('#hftblNumOfRows').val(0);
    //$('#hfChkDuplicateFields').val('');
    //$('#hdnNumOfGroups').val(0);
    //$('#hdnAddChildElems').val(0);
    //$('#hdnReportID').val(0);

    //// reset all form fields
    //$('#pg_section').val(0);
    //ReportFormLayoutByRepoTyp(true);

    //$('#txtReportNameAr').val('');
    //$('#txtReportNameEn').val('');
    //$('#FiscalPeriodTypes').val(0);

    //$('#btnAddReportSettings').show();
    //$('#btnUpdReportSettings').hide();
    //$('#tblData').hide();
    //$('#btnsSavUpdCnc').hide();
    //hashUrl('Cancel');

    //return false;
}