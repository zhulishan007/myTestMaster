$(document).ready(function () {
    $("#frmAddEditTestCase").validate({
        rules: {
            testcasename: {
                required: true
            },
            testcasedesc: {
                required: true,
            },
            DrpApplication: {
                required: true
            }
        },
    });
});

function AddTestCase() {
    $("#testcasevalidate").css("display", "none");
    $("#existtestcasename").css("display", "none");
    $("#TCexampleModalLabel").text('');
    $("#TCexampleModalLabel").text('Add Test Case');
    $("#AddEditTestCase").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    $("#addedittestcase").attr("disabled", false);
    $("#hdnTestCaseId").val("");
    $("#testcasename").val("");
    $("#testcasedesc").val("");
    $("#hdnTestCaseId").val("");
    loadAppdata("DrpApplication");
    $("#DrpApplication").val("");
    $("#DrpApplication").select2();
    //$("#DrpTestSuite").html("");
    $("#DrpTestSuite").val("");
    $("#DrpTestSuite").select2();
    var validator = $("#frmAddEditTestCase").validate();
    validator.resetForm();
}

function AddEditTestCaseSave() {
    if (!$("#frmAddEditTestCase").valid()) {
        var validationobj = $("#clscaseApplication").find("#DrpApplication-error");
        if (validationobj.length > 0) {
            $("#clscaseApplication").find("#DrpApplication-error").remove();
            validationHTML = "<div id='DrpApplication-error' class='error invalid-feedback ' >This field is required.</div>";
            $("#clscaseApplication").append(validationHTML);
        }
        return false;
    }
    $("#addedittestcase").attr("disabled", true);
    var lId = $("#hdnTestCaseId").val();
    if (lId == null && lId == "")
        lId = 0;
    var regex = /^[a-zA-Z0-9-._(&)*  ]*$/;
    var TestCaseModel = {};
    TestCaseModel.TestCaseId = lId,
        TestCaseModel.TestCaseName = $("#testcasename").val(),
        TestCaseModel.TestCaseDescription = $("#testcasedesc").val(),
        TestCaseModel.ApplicationId = $("#DrpApplication").val().toString(),
        TestCaseModel.TestSuiteId = $("#DrpTestSuite").val();
    if (!regex.test(TestCaseModel.TestCaseName)) {
        $("#testcasename").val("");
        $("#testcasevalidate").css("display", "block");
        $("#addedittestcase").attr("disabled", false);
        return false;
    }
    $.ajax({
        url: "/TestCase/AddEditTestCase",
        data: JSON.stringify(TestCaseModel),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            startloader();
        },
        complete: function () {
            stoploader();
        },
        success: function (result) {
            if (result.status == 1) {
                if (result.data == true) {
                    $("#AddEditTestCase").modal("hide");
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "icon": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    if (typeof tableTestCase !== 'undefined')
                        tableTestCase.table().draw();                    
                    $("#addedittestcase").attr("disabled", false);
                    $.ajax({
                        url: "/Login/LeftPanel",
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "HTML",
                        success: function (result) {
                            $("#leftProjectList").html("");
                            $("#leftProjectList").html(result);
                        }
                    });
                }
                else {
                    if (result.data.length > 0) {
                        var resultstring = "";
                        for (i = 0; i < result.data.length; i++) {
                            resultstring = resultstring + result.data[i] + " , ";
                        }
                        swal.fire(
                            '',
                            'Following Storyboards contains this TestCase. So please remove this TestCase from storyboards. ' + "<br>" + resultstring,
                            'error'
                        )
                        $("#addedittestcase").attr("disabled", false);
                    }
                }
            }
            else if (result.status == 0) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "icon": "error",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
            }
            //window.location.href = "/Home/Index";
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function ChangeApplicationTestsuite() {
    var ApplicationIds = $("#DrpApplication").val().toString();
    var TestSuiteIds = $("#DrpTestSuite").val();
    $("#DrpTestSuite").html("");
    $("#DrpTestSuite").select2();
    $.ajax({
        url: "/TestCase/GetTestSuiteByApplicaton",
        data: '{"ApplicationId":"' + ApplicationIds + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result.status == 1) {
                var optionString = "<optgroup>";
                optionString = optionString + " <option value=0></option>";
                for (i = 0; i < result.data.length; i++) {
                    var optionValue = result.data[i].TestSuiteId;
                    var optionText = result.data[i].TestSuiteName;

                    optionString = optionString + " <option value='" + optionValue + "'>" + optionText + "</option>";
                }
                optionString = optionString + "</optgroup>";
                $("#DrpTestSuite").append($(optionString));
                $("#DrpTestSuite").select2();

                $("#DrpTestSuite").val(TestSuiteIds);
                $("#DrpTestSuite").select2();
            }
            else if (result.status == 0) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "icon": "error",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function CheckTestCaseNameExist() {
    var TestCaseName = $("#testcasename").val();
    var TestCaseId = $("#hdnTestCaseId").val();
    $.ajax({
        url: "/TestCase/CheckDuplicateTestCaseNameExist",
        data: '{"TestCaseName":"' + TestCaseName + '","TestCaseId":"' + TestCaseId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1 && result.data) {
                $("#testcasename").val("");
                $("#existtestcasename").css("display", "block");
            } else if (result.status == 0) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "icon": "error",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
            }
        },
    });
}
$("#testcasename").on('keyup', function () {
    $("#existtestcasename").css("display", "none");
});
$("#testcasename").on('keyup', function () {
    $("#testcasevalidate").css("display", "none");
});