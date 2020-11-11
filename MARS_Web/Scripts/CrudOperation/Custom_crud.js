function showProjectRenamePopup() {
    $("#RenameProjecterror").css("display", "none");
    $("#ProjectPopup").modal("show");
}
function Showrenamepopup() {
    $("#Renamestoryboarderror").css("display", "none");
    $("#StoryboardPopup").modal("show");
}
function showrenameTestcasePopup() {
    $("#TestCasePopup").modal("show");
    $("#RenameTestCaseerror").css("display", "none");
}
function showrenameTestSuitePopup() {
    $("#TestSuitePopup").modal("show");
    $("#RenameTestSuiteerror").css("display", "none");
}
function CheckduplicateStoryboardExists() {
    var storyboardid = $("#hdnRenameStoryboardId").val();
    var storyboardname = $("#RenameStoryboardname").val();

    if (storyboardname != "" && storyboardname != null) {
        $.ajax({
            url: "/Storyboard/CheckDuplicateStoryboardName",
            data: '{"storyboardname":"' + storyboardname + '","storyboardid":"' + storyboardid + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result == true) {
                    $("#RenameStoryboardname").val("");
                    $("#Renamestoryboarderror").css("display", "block");
                }
            },
        });
    }
}


$("#RenameStoryboardname").on('keyup', function () {
    $("#Renamestoryboarderror").css("display", "none");
});
function RenameStoryboard() {
    var storyboardid = $("#hdnRenameStoryboardId").val();
    var storyboardname = $("#RenameStoryboardname").val();
    var regex = /^[a-zA-Z0-9-._(&)* ]*$/;
    if (!regex.test(storyboardname)) {
        swal.fire({
            "title": "",
            "text": "Storyboard name must contain only letters, numbers, spaces and underscore characters",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
        return false;
    }
    var storyboarddesc = $("#RenamestoryboardDesc").val();
    var projectid = $("#hdnRenameProjectId").val();
    if (storyboardname !== "" && storyboardname !== null) {
        $.ajax({
            url: "/Storyboard/ChangeStoryboardName",
            data: '{"storyboardname":"' + storyboardname + '","storyboarddesc":"' + storyboarddesc + '","storyboardid":"' + storyboardid + '","projectid":"' + projectid + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status === 1) {
                    $("#RenameStoryboardname").val("");
                    $("#hdnRenameStoryboardId").val("");
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });

                    $("#StoryboardPopup").modal("toggle");
                    $(lobjstoryboard).attr("data-name", storyboardname);
                    $(lobjstoryboard).attr("title", storyboarddesc);
                    $(lobjstoryboard).children(".kt-menu__link-text")[0].innerHTML = storyboardname;

                    $('.ULtablist li').each(function (index, value) {
                        var id = $(value).children().first().attr("data-id");
                        if (id == storyboardid) {
                            closeExistOldtab(value);
                        }
                    });
                } else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                }
            },
        });
    }
    else {
        swal.fire({
            "title": "",
            "text": "Storyboard name can not be blank",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
    }
}
function RenameTestSuite() {
    var lTestSuiteName = $("#RenameTestSuiteName").val();
    var lTestSuiteId = $("#hdnRenameTestSuiteId").val();
    var ltestsuitedesc = $("#RenameTestSuiteDesc").val();
    var regex = /^[a-zA-Z0-9-._(&)* ]*$/;
    if (!regex.test(lTestSuiteName)) {
        swal.fire({
            "title": "",
            "text": "TestSuite name must contain only letters, numbers, spaces and underscore characters",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
        return false;
    }
    if (lTestSuiteName != "" && lTestSuiteName != null) {
        $.ajax({
            url: "/TestSuite/ChangeTestSuiteName",
            data: '{"TestSuiteName":"' + lTestSuiteName + '","Testsuitedesc":"' + ltestsuitedesc + '","TestSuiteId":"' + lTestSuiteId + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1) {
                    if (result.data == "success") {
                        $("#RenameTestSuiteName").val("");
                        $("#hdnRenameTestSuiteId").val("");

                        swal.fire({
                            "title": "",
                            "text": result.message,
                            "type": "success",
                            "onClose": function (e) {
                                console.log('on close event fired!');
                            }
                        });
                        $("#TestSuitePopup").modal("toggle");
                        $(lobjTestSuite).attr("data-name", lTestSuiteName);
                        $(lobjTestSuite).attr("title", ltestsuitedesc);
                        var lcountTS = $(lobjTestSuite).children(".kt-menu__link-text").children(".tscount")[0].innerHTML;
                        $(lobjTestSuite).children(".kt-menu__link-text")[0].innerHTML = lTestSuiteName + "<span class='tscount'>" + lcountTS + "</span>";

                    } else if (result.data == true) {
                        $("#RenameTestSuiteName").val("");
                        $("#RenameTestSuiteerror").css("display", "block");
                    }
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                }
            },
        });
    } else {
        swal.fire({
            "title": "",
            "text": "Test Suite name can not be blank",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
    }
}

function CheckDuplicateRenameTestSuiteExist() {
    var lTestSuiteName = $("#RenameTestSuiteName").val();
    var lTestSuiteId = $("#hdnRenameTestSuiteId").val();

    $.ajax({
        url: "/TestSuite/CheckDuplicateTestSuiteNameExist",
        data: '{"TestSuiteName":"' + lTestSuiteName + '","TestSuiteId":"' + lTestSuiteId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1 && result.data == true) {
                $("#RenameTestSuiteName").val("");
                $("#RenameTestSuiteerror").css("display", "block");
            } else if (result.status == 0) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "type": "error",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
            }
        },
    });
}

$("#RenameTestSuiteName").on('keyup', function () {
    $("#RenameTestSuiteerror").css("display", "none");
});

function RenameTestCase() {
    var lTestCaseName = $("#RenameTestCaseName").val();
    var lTestCaseId = $("#hdnRenameTestCaseId").val();
    var objTestCaseId = $("#hdnRenameTestCaseId").val();
    var lTestCaseDesc = $("#RenameTestCaseDesc").val();
    var regex = /^[a-zA-Z0-9-._(&)* ]*$/;
    if (!regex.test(lTestCaseName)) {
        swal.fire({
            "title": "",
            "text": "Test Case name must contain only letters, numbers, spaces and underscore characters",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
        return false;
    }
    if (lTestCaseName != "" && lTestCaseName != null) {
        startloader();
        $("#TestCasePopup").modal("toggle");
        $.ajax({
            url: "/TestCase/ChangeTestCaseName",
            data: '{"TestCaseName":"' + lTestCaseName + '","TestCaseId":"' + lTestCaseId + '","TestCaseDes":"' + lTestCaseDesc + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1) {
                    if (result.data == "success") {
                        $("#RenameTestCaseName").val("");
                        $("#hdnRenameTestCaseId").val("");
                        $("#RenameTestCaseDesc").text("");
                        $("#RenameTestCaseDesc").val("");
                        swal.fire({
                            "title": "",
                            "text": result.message,
                            "type": "success",
                            "onClose": function (e) {
                                console.log('on close event fired!');
                            }
                        });
                        var lTestCaseId = $("#hdnRenameTestSuiteCaseId").val();
                        var lTestProjectId = $("#hdnRenameTestProjectCaseId").val();
                        $(lobjTestCase).attr("data-name", lTestCaseName);
                        var lcountTC = $(lobjTestCase).children(".kt-menu__link-text").children(".tccount")[0].innerHTML;
                        $(lobjTestCase).children(".kt-menu__link-text")[0].innerHTML = lTestCaseName + "<span class='tccount'>" + lcountTC + "</span>";
                        $(lobjTestCase).attr("title", lTestCaseDesc);

                        $('.ULtablist li').each(function (index, value) {
                            var id = $(value).children().first().attr("data-id");
                            if (id == objTestCaseId) {
                                closeExistOldtab(value);
                            }
                        });
                    } else if (result.data == true) {
                        $("#TestCasePopup").modal("toggle");
                        $("#RenameTestCaseerror").css("display", "block");
                        $("#RenameTestCaseName").val("");
                    }
                    stoploader();
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                }
            },
        });
    }
    else {
        swal.fire({
            "title": "",
            "text": "Test Case name can not be blank",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
    }
}

function CheckDuplicateRenameTestCaseExist() {
    var lTestCaseName = $("#RenameTestCaseName").val();
    var lTestCaseId = $("#hdnRenameTestCaseId").val();

    $.ajax({
        url: "/TestCase/CheckDuplicateTestCaseNameExist",
        data: '{"TestCaseName":"' + lTestCaseName + '","TestCaseId":"' + lTestCaseId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 0 && result.data == true) {
                $("#RenameTestCaseName").val("");
                $("#RenameTestCaseerror").css("display", "block");
            }
            else if (result.status == 0) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "type": "error",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
            }
        }
    });
}

$("#RenameTestCaseName").on('keyup', function () {
    $("#RenameTestCaseerror").css("display", "none");
});

function DeleteTestCase(objTestCase) {
    var TestCaseId = $(objTestCase).attr("data-testcase-id");
    if (TestCaseId !== null && TestCaseId !== "") {
        startloader();
        $.ajax({
            url: "/TestCase/DeleteTestCase",
            data: '{"TestCaseId":"' + TestCaseId + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1) {
                    if (result.data === true) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                    $.ajax({
                        url: "/Login/LeftPanel",
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "HTML",
                        success: function (result) {
                            $("#leftProjectList").html("");
                            $("#leftProjectList").html(result);

                            $('.ULtablist li').each(function (index, value) {
                                var lFindTab = $(value).children().first().attr("data-target");

                                if (lFindTab != undefined) {
                                    var lFindTabId = $(value).children().first().attr("data-id");
                                    var lFindTabName = $(value).children().first().attr("data-tab");
                                    if (lFindTabId == TestCaseId && lFindTabName == "TestCase") {
                                        var lPrevTab = $(value).children().first().attr("data-target");
                                        var lPrevTabNameId = lPrevTab.replace("#", "");
                                        var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
                                        $(value).children().first().addClass("active");
                                        $(lPrevDirGrid).addClass("active");
                                        var lTabNameId = lFindTab.replace("#", "");
                                        var lDirGrid = $(".divtablist #" + lTabNameId);
                                        $(value).remove();
                                        $(lDirGrid).remove();
                                    }
                                }
                            });
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
                            'Following Storyboards contains this Test Case.Please remove this Test Case from storyboards ' + "<br>" + resultstring,
                            'error'
                        );
                        stoploader();
                    }
                }
            }
                else if(result.status == 0) {
            swal.fire({
                "title": "",
                "text": result.message,
                "type": "error",
                "onClose": function (e) {
                    console.log('on close event fired!');
                }
            });
        }
            }
        });
    }
}
function DeleteProject(objproject) {
    var projectid = $(objproject).attr("data-project-id");
    if (projectid != null && projectid != "") {
        startloader();
        $.ajax({
            url: "/Project/DeleteProject",
            data: '{"projectid":"' + projectid + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                    window.location.href = "/Home/Index";
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                }
            },
        });
    }
}

function DeleteStoryboard(objStoryboard) {
    var storyboardid = $(objStoryboard).attr("data-storyboard-id");
    if (storyboardid != null && storyboardid != "") {
        startloader();
        $.ajax({
            url: "/Storyboard/DeleteStoryboard",
            data: '{"sid":"' + storyboardid + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1 && result.data == true) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                    $.ajax({
                        url: "/Login/LeftPanel",
                        type: "POST",
                        contentType: "application/json;charset=utf-8",
                        dataType: "HTML",
                        success: function (result) {
                            $("#leftProjectList").html("");
                            $("#leftProjectList").html(result);

                            $('.ULtablist li').each(function (index, value) {
                                var lFindTab = $(value).children().first().attr("data-target");

                                if (lFindTab != undefined) {
                                    var lFindTabId = $(value).children().first().attr("data-id");
                                    var lFindTabName = $(value).children().first().attr("data-tab");
                                    if (lFindTabId == storyboardid && lFindTabName == "Storyboard") {
                                        var lPrevTab = $(value).children().first().attr("data-target");
                                        var lPrevTabNameId = lPrevTab.replace("#", "");
                                        var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
                                        $(value).children().first().addClass("active");
                                        $(lPrevDirGrid).addClass("active");
                                        var lTabNameId = lFindTab.replace("#", "");
                                        var lDirGrid = $(".divtablist #" + lTabNameId);
                                        $(value).remove();
                                        $(lDirGrid).remove();
                                    }
                                }
                            });
                        }
                    });
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                }
            }
        });
    }
}

function DeleteTestSuite(objTestSuite) {
    var TestSuiteId = $(objTestSuite).attr("data-testsuite-id");
    if (TestSuiteId != null && TestSuiteId != "") {
        startloader();
        $.ajax({
            url: "/TestSuite/DeleteTestSuite",
            data: '{"TestSuiteId":"' + TestSuiteId + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1) {
                    if (result.data == true) {
                        swal.fire({
                            "title": "",
                            "text": result.message,
                            "type": "success",
                            "onClose": function (e) {
                                console.log('on close event fired!');
                            }
                        });
                        $.ajax({
                            url: "/Login/LeftPanel",
                            type: "POST",
                            contentType: "application/json;charset=utf-8",
                            dataType: "HTML",
                            success: function (result) {
                                $("#leftProjectList").html("");
                                $("#leftProjectList").html(result);

                                $('.ULtablist li').each(function (index, value) {
                                    var lFindTab = $(value).children().first().attr("data-target");

                                    if (lFindTab != undefined) {
                                        var lFindTabId = $(value).children().first().attr("data-id");
                                        var lFindTabName = $(value).children().first().attr("data-tab");
                                        if (lFindTabId == TestSuiteId && lFindTabName == "TestSuite") {
                                            var lPrevTab = $(value).children().first().attr("data-target");
                                            var lPrevTabNameId = lPrevTab.replace("#", "");
                                            var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
                                            $(value).children().first().addClass("active");
                                            $(lPrevDirGrid).addClass("active");
                                            var lTabNameId = lFindTab.replace("#", "");
                                            var lDirGrid = $(".divtablist #" + lTabNameId);
                                            $(value).remove();
                                            $(lDirGrid).remove();
                                        }
                                    }
                                });
                            }
                        });
                        stoploader();
                    }
                    else {
                        if (result.data.length > 0) {
                            var resultstring = "";
                            for (i = 0; i < result.data.length; i++) {

                                resultstring = resultstring + result.data[i] + " , ";
                            }
                            stoploader();
                            swal.fire(
                                '',
                                'Following Storyboards contain this Test Suite. Please remove this Test Suite from Storyboards. ' + "<br>" + resultstring,
                                'error'
                            );
                        }
                    }
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    stoploader();
                }
            }
        });
    }
}

$("#RenameProjectName").on('keyup', function () {
    $("#RenameProjecterror").css("display", "none");
});
function RenameProject() {
    var lProjectName = $("#RenameProjectName").val();
    var lProjectId = $("#hdnRenameProjectId").val();
    var lProjectdesc = $("#RenameProjectDesc").val();

    if (lProjectName != "" && lProjectName != null) {
        $.ajax({
            url: "/Project/ChangeProjectName",
            data: '{"ProjectName":"' + lProjectName + '","Projectdesc":"' + lProjectdesc + '","ProjectId":"' + lProjectId + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result.status == 1 && result.data) {
                    $("#RenameProjectName").val("");
                    $("#hdnRenameProjectId").val("");
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    $("#ProjectPopup").modal("toggle");
                    $(lobjProject).attr("data-name", lProjectName);
                    $(lobjProject).attr("title", lProjectdesc);
                    $(lobjProject).children(".kt-menu__link-text")[0].innerHTML = lProjectName;
                }
                else if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                }
            },
        });
    } else {
        swal.fire({
            "title": "",
            "text": "Project name cannot be blank",
            "type": "error",
            "onClose": function (e) {
                console.log('on close event fired!');
            }
        });
    }
}

function CheckDuplicateRenameProjectExist() {
    var lProjectName = $("#RenameProjectName").val();
    var lProjectId = $("#hdnRenameProjectId").val();

    $.ajax({
        url: "/Project/CheckDuplicateProjectNameExist",
        data: '{"ProjectName":"' + lProjectName + '","ProjectId":"' + lProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.data) {
                $("#RenameProjectName").val("");
                $("#RenameProjecterror").css("display", "block");
            }
            //else {
            //  RenameProject();
            //}
        },
    });
}

function AddStoryboardContext() {
    $("#AddStoryboardContext").modal("show");
    $("#contextstoryboarderror").css("display", "none");
    var validator = $("#addstoryboardcontext").validate();
    validator.resetForm();
}
$(document).ready(function () {

    $("#addstoryboardcontext").validate({
        rules: {
            contextStoryboardname: {
                required: true
            },

            contextStoryboardDesc: {
                required: true
            }

        }
    });
});
function StoryboardSaveContext() {
    if (!$("#addstoryboardcontext").valid()) {
        return false;
    }
    $("#savestoryboardcontext").prop("disabled", true);
    var Id = 0;
    var Storyboardmodel = {};
    Storyboardmodel.Storyboardid = Id;
    Storyboardmodel.Storyboardname = $("#contextStoryboardname").val();
    Storyboardmodel.StoryboardDescription = $("#contextStoryboardDesc").val();
    Storyboardmodel.ProjectId = $(".ExportProject").attr("data-project-id");
    $.ajax({
        url: "/Storyboard/AddEditStoryboard",
        data: JSON.stringify(Storyboardmodel),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result.status == 1) {
                if (result.data == true) {
                    $("#contextstoryboarderror").css("display", "block");
                    $("#savestoryboardcontext").prop("disabled", false);
                    return false;
                }
                else if (result.data == "success") {
                    $("#AddStoryboardContext").modal("hide");
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "type": "success",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                    $("#savestoryboardcontext").prop("disabled", false);

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
            }
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });

}
function checkstoryboardnameContext() {
    var sname = $("#contextStoryboardname").val();
    $.ajax({
        url: "/Storyboard/CheckDuplicateStoryboardName",
        // data: { 'lLoginName': pLoginName, 'lLoginId': TESTER_ID },
        data: '{"storyboardname":"' + sname + '","storyboardid":"' + null + '"}',
        type: "POST",
        //async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",

        success: function (result) {

            if (result.data == true) {
                $("#contextStoryboardname").val("");
                $("#contextstoryboarderror").css("display", "block");
                // return result;
            }
        },
    });
}
$("#contextStoryboardname").on('keyup', function () {
    $("#contextstoryboarderror").css("display", "none");
});

function ExportResultSetProject(objproject) {
    var projectid = $(objproject).attr("data-project-id");
    $('#hdnExpProjectId').val("");
    $('#hdnExpProjectId').val(projectid);
    $('#ResultSetProjectExport').modal("show");
}

function ExportPResultSet() {
    var projectid = $('#hdnExpProjectId').val();
    if (projectid != null && projectid != "") {
        var mode = $('#drpPRMode').val();
        $('#ResultSetProjectExport').modal("hide");
        startloader();
        $.ajax({
            url: '/StoryBoard/ExportProjectResultSet', //call your controller and action
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                Projectid: projectid,
                Mode: mode
            },
        }).done(function (result) {
            stoploader();
            window.location = "/TestSuite/DownloadExcel?FileName=" + result;
        });
    }
}