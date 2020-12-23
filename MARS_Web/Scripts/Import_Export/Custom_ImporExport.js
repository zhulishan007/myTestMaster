function ImportVariables(Default) {
    $.ajax({
        url: "/Variable/ImportVariables",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimportvariable") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-tab="ImportVariable" data-name="ImportVariable" data-toggle="tab" href="#" data-target="#tabimportvariable" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/ivr.png"/>Import Variables</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-tab="ImportVariable" data-name="ImportVariable" data-toggle="tab" href="#" data-target="#tabimportvariable" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/ivr.png"/>Import Variables</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimportvariable" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimportvariable") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimportvariable") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
            }
            else {


                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabimportvariable") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimportvariable") {
                        $(value).removeClass("active");
                    }
                });
            }

        },
    });
}
function ExportVariables() {
    startloader();
    $.ajax({
        url: '/Variable/ExportVariable', //call your controller and action
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function (result) {
        stoploader();
        //if (result.status == 1) {
            window.location = "/TestSuite/DownloadExcel?FileName=" + result;
        //} else if (result.status == 0) {
        //    swal.fire({
        //        "title": "",
        //        "text": result.message,
        //        "type": "error",
        //        "onClose": function (e) {
        //            console.log('on close event fired!');
        //        }
        //    });
        //}
    });
}
function ImportObjects(Default) {
    $.ajax({
        url: "/Object/ImportObjects",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimportobject") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-tab="ImportObject" data-name="ImportObject" data-toggle="tab" href="#" data-target="#tabimportobject" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Import Object" class="tab_icons_img" src="/assets/media/icons/IOB.png"/>Import Objects</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-tab="ImportObject" data-name="ImportObject" data-toggle="tab" href="#" data-target="#tabimportobject" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Import Object" class="tab_icons_img" src="/assets/media/icons/IOB.png"/>Import Objects</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimportobject" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimportobject") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimportobject") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
            }
            else {
                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabimportobject") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimportobject") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}
function ExportAllStoryboards() {
    $.ajax({
        url: "/Storyboard/ExportAllStoryboards",
        contentType: 'application/json; charset=utf-8',
        datatype: 'json',
        data: {
            Storyboardid: 0
        },
        type: "GET",
        success: function () {
            window.location = "/Storyboard/ExportAllStoryboards?Storyboardid=1";
        }
    });
}
function ExportAllTestsuites() {
    swal.fire({
        title: 'Are you sure?',
        text: "Exporting Test Suite will take a while.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes,Export!'
    }).then(function (result) {
        if (result.value == true) {
            $.ajax({
                url: "/TestSuite/ExportAllTestSuites",
                contentType: 'application/json; charset=utf-8',
                datatype: 'json',
                data: {
                    Storyboardid: 0
                },
                type: "GET",
                success: function () {
                    window.location = "/TestSuite/ExportAllTestSuites";
                }
            });
        }
    });
}
function ExportStoryboard(objstoryboard) {

    var sid = $(objstoryboard).attr("data-storyboard-id");
    var pid = $(objstoryboard).attr("data-project-id");
    if (sid != null && pid != "") {
        startloader();
        $.ajax({
            url: '/StoryBoard/ExportStoryboard', //call your controller and action
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                Storyboardid: sid,
                Projectid: pid
            },
        }).done(function (result) {
            
            stoploader();
            //if (result.status == 1) {
                window.location = "/TestSuite/DownloadExcel?FileName=" + result;
            //} else if (result.status == 0) {
            //    swal.fire({
            //        "title": "",
            //        "text": result.message,
            //        "type": "error",
            //        "onClose": function (e) {
            //            console.log('on close event fired!');
            //        }
            //    });
            //}
        });
    }
}
function ExportProject(lobjProject) {

    var pid = $(lobjProject).attr("data-project-id");
    if (pid != null && pid != "") {
        startloader();
        $.ajax({
            url: '/StoryBoard/ExportStoryboardsByProject', //call your controller and action
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                projectid: pid
            },
        }).done(function (result) {
            stoploader();
          //  if (result.status == 1) {
                window.location = "/TestSuite/DownloadExcel?FileName=" + result;
          //  } else if (result.status == 0) {
            //    swal.fire({
            //        "title": "",
            //        "text": result.message,
            //        "type": "error",
            //        "onClose": function (e) {
            //            console.log('on close event fired!');
            //        }
            //    });
            //}
        });
    }
}
function ExportTestSuite(objTestSuite) {
    var TestSuiteId = $(objTestSuite).attr("data-testsuite-id");
    if (TestSuiteId != null && TestSuiteId != "") {
        startloader();
        $.ajax({
            url: '/TestSuite/ExportTestSuite', //call your controller and action
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                TestSuiteId: TestSuiteId
            },
        }).done(function (result) {
            stoploader();
            //if (result.status == 1) {
                window.location = "/TestSuite/DownloadExcel?FileName=" + result;
            //} else if (result.status == 0) {
            //    swal.fire({
            //        "title": "",
            //        "text": result.message,
            //        "type": "error",
            //        "onClose": function (e) {
            //            console.log('on close event fired!');
            //        }
            //    });
            //}
        });
    }
}

function ExportTestCase(objTestCase) {
  
    var TestCaseId = $(objTestCase).attr("data-testcase-id");
    var TestSuiteId = $(objTestCase).attr("data-testsuite-id");
    if (TestCaseId != null && TestCaseId != "") {
        startloader();
        $.ajax({
            url: '/TestCase/ExportTestCase', //call your controller and action
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                TestCaseId: TestCaseId,
                TestSuiteId: TestSuiteId
            },
        }).done(function (result) {
            stoploader();
           // if (result.status == 1) {
                window.location = "/TestSuite/DownloadExcel?FileName=" + result;
            //} else if (result.status == 0) {
            //    swal.fire({
            //        "title": "",
            //        "text": result.message,
            //        "type": "error",
            //        "onClose": function (e) {
            //            console.log('on close event fired!');
            //        }
            //    });
            //}
        });
    }
}

function ExportDatasetTag() {
    startloader();
    $.ajax({
        url: '/TestCase/ExportDatasetTag', //call your controller and action
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function (result) {
        stoploader();
        //if (result.status == 1) {
        window.location = "/TestSuite/DownloadExcel?FileName=" + result;
        //} else if (result.status == 0) {
        //    swal.fire({
        //        "title": "",
        //        "text": result.message,
        //        "type": "error",
        //        "onClose": function (e) {
        //            console.log('on close event fired!');
        //        }
        //    });
        //}
    });
}

function ImportDatasetTag(Default) {
    $.ajax({
        url: "/TestCase/ImportDatasetTag",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimportdatasettag") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-tab="ImportDatasetTag" data-name="ImportDatasetTag" data-toggle="tab" href="#" data-target="#tabimportdatasettag" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Import DatasetTag" class="tab_icons_img" src="/assets/media/icons/idt.png"/>Import DatasetTag</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-tab="ImportDatasetTag" data-name="ImportDatasetTag" data-toggle="tab" href="#" data-target="#tabimportdatasettag" onclick="ActiveTab($(this),0,0,0,"")"><img alt="Import DatasetTag" class="tab_icons_img" src="/assets/media/icons/idt.png"/>Import DatasetTag</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimportdatasettag" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimportdatasettag") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimportdatasettag") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
            }
            else {
                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabimportdatasettag") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimportdatasettag") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function ExportReportDatasetTag() {
    startloader();
    $.ajax({
        url: '/TestCase/ExportDatasetTagReport', //call your controller and action
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    }).done(function (result) {
        stoploader();
        window.location = "/TestSuite/DownloadExcel?FileName=" + result;
    });
}