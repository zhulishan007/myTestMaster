$(document).ready(function () {
    $("#frmtestsuite").validate({
        rules: {
            testsuitename: {
                required: true
            },
            testsuitedesc: {
                required: true,
            },
            stDrpApplication: {
                required: true
            }
        },
    });
    $('#stDrpApplication').on('changed.bs.stDrpApplication', function () {
        validator.element($(this));
    });
});

function CheckTestSuiteNameExist() {
    var TestSuiteName = $("#testsuitename").val();
    var TestSuiteId = $("#hdnTestSuiteId").val();
    $.ajax({
        url: "/TestSuite/CheckDuplicateTestSuiteNameExist",
        data: '{"TestSuiteName":"' + TestSuiteName + '","TestSuiteId":"' + TestSuiteId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1 && result.data) {
                $("#testsuitename").val("");
                $("#existtestsuitename").css("display", "block");
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
$("#testsuitename").on('keyup', function () {
    $("#existtestsuitename").css("display", "none");
});
$("#testsuitename").on('keyup', function () {
    $("#testsuitevalidate").css("display", "none");
});
$("#btnadd").click(function () {
    var suiteobj = {};
    suiteobj.TestSuiteId = $("#testsuiteid").val();
    suiteobj.TestSuiteName = $("#testsuitename").val();
    suiteobj.TestSuiteDescription = $("#testsuitedesc").val();
    // var applist = new Object();
    //  applist.Entrys = new Array("#kt_select2_3_model");
    suiteobj.Application = $("#kt_select2_3_model").val();
    suiteobj.Application = applist.Entrys;
    // suiteobj.Project = $("").val();
    $.ajax({
        url: "/TestSuite/AddEditTestSuite",
        type: "POST",
        data: JSON.stringify(suiteobj),
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result) {
                window.location.href = "/Accounts/ListOfUsers";
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
});
function AddEditTestSuiteSave() {
    if (!$("#frmtestsuite").valid()) {
        var validationobj = $("#clssuiteApplication").find("#stDrpApplication-error");
        if (validationobj.length > 0) {
            $("#clssuiteApplication").find("#stDrpApplication-error").remove();
            validationHTML = "<div id='stDrpApplication-error' class='error invalid-feedback ' >This field is required.</div>";
            $("#clssuiteApplication").append(validationHTML);
        }
        return false;
    }
    $("#addedittestsuite").attr("disabled", true);
    var lId = $("#hdnTestSuiteId").val();
    if (lId == null && lId == "")
        lId = 0;
    var regex = /^[a-zA-Z0-9-._(&)*  ]*$/;
    var TestSuiteModel = {};
    var testsuitename = $("#testsuitename").val();
    TestSuiteModel.TestSuiteId = lId,
        TestSuiteModel.TestSuiteName = $("#testsuitename").val(),
        TestSuiteModel.TestSuiteDescription = $("#testsuitedesc").val(),
        TestSuiteModel.ApplicationId = $("#stDrpApplication").val().toString(),
        TestSuiteModel.ProjectId = $("#DrpProject").val().toString();
    if (!regex.test(TestSuiteModel.TestSuiteName)) {
        $("#testsuitename").val("");
        $("#testsuitevalidate").css("display", "block");
        $("#addedittestsuite").attr("disabled", false);
        return false;
    }
    // var lModel = JSON.stringify(TestCaseModel);
    $.ajax({
        url: "/TestSuite/AddEditTestSuite",
        data: JSON.stringify(TestSuiteModel),
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
                if (result.data.length > 0) {

                    var resultstring = "";
                    for (i = 0; i < result.data.length; i++) {

                        resultstring = resultstring + result.data[i] + " , ";
                    }
                    swal.fire(
                        '',
                        'Following Storyboards contains this Test Suite. In order to remove this Test Suite please remove it from all storyboards. ' + "<br>" + resultstring,
                        'error'
                    )
                }
                else {
                    $("#AddTestSuite").modal("hide");
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "icon": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    if (typeof TestSuitetable !== 'undefined')
                        TestSuitetable.table().draw();
                    $.ajax({
                        url: "/Login/LeftPanel",
                        // data: { 'lLoginName': pLoginName, 'lLoginId': TESTER_ID },

                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "HTML",
                        success: function (result) {
                            $("#leftProjectList").html("");
                            $("#leftProjectList").html(result);
                        }
                    });
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
            $("#addedittestsuite").attr("disabled", false);
            // window.location.href = "/Home/Index";
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function AddSuite() {
    $("#addedittestsuite").attr("disabled", false);
    $("#TSexampleModalLabel").text('');
    $("#TSexampleModalLabel").text('Add Test Suite');
    $("#AddTestSuite").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    $("#existtestsuitename").css("display", "none");
    $("#hdnTestSuiteId").val("");
    $("#testsuitename").val("");
    $("#testsuitedesc").val("");
    $("#hdnTestCaseId").val("");
    $("#stDrpApplication").val("");
    $("#stDrpApplication").select2();
    $("#DrpProject").html("");
    $("#DrpProject").select2();
    $("#testsuitevalidate").css("display", "none");
    var validator = $("#frmtestsuite").validate();
    validator.resetForm();
}

function ChangeApplication() {
    var ApplicationIds = $("#stDrpApplication").val().toString();
    var ProjectIds = $("#DrpProject").val().toString();;
    $("#DrpProject").html("");
    $("#DrpProject").select2();
    $.ajax({
        url: "/TestSuite/GetProjectByApplicaton",
        data: '{"ApplicationId":"' + ApplicationIds + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            if (result.status == 1) {
                var lTempApplication = "";
                var optionString = "";
                optionString = optionString + " <option value=0></option>";
                for (i = 0; i < result.data.length; i++) {
                    if (result.data[i].ApplicationName != lTempApplication && lTempApplication != "") {
                        optionString = optionString + "</optgroup>";
                    }
                    if (result.data[i].ApplicationName != lTempApplication) {
                        optionString = optionString + "<optgroup label='Projects:'>";
                        lTempApplication = result.data[i].ApplicationName;
                    }
                    var optionValue = result.data[i].ProjectId;
                    var optionText = result.data[i].ProjectName;
                    optionString = optionString + " <option value='" + optionValue + "'>" + optionText + "</option>";
                }

                $("#DrpProject").append($(optionString));
                $("#DrpProject").select2();

                $("#DrpProject").val(ProjectIds.split(","));
                $("#DrpProject").select2();
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