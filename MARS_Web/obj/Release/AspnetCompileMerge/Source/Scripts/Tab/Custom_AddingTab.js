var ltempTestCase = "";
var ltmpstoryboard = "";
var testcaseopenflag = false;
var tempflag = false;
var tmpname = "";
Array.prototype.remove = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) this.splice(i, 1);
    }
}; 
function RecusrsiveTab(name) {
    ltempTestCase = name;
    if (name.indexOf(" ") > -1) {
        ltempTestCase = name.replace(" ", "_");
        RecusrsiveTab(ltempTestCase);
    }
    return ltempTestCase;
}
function RecusrsiveTab1(name) {
    ltmpstoryboard = name;
    if (name.indexOf(" ") > -1) {
        ltmpstoryboard = name.replace(" ", "_");
        RecusrsiveTab1(ltmpstoryboard);
    }
    return ltmpstoryboard;
}
function OpenReport() {
    $.ajax({
        url: "/Home/TestProjectList",

        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabtest") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a class="nav-link active context-tab" data-tab="tabtest" data-id="0" data-name="tabtest" data-toggle="tab" href="#" data-target="#tabtest" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/Test-Result.png">Test Results</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabtest" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabtest") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabtest") {
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
                    if ($(value).children().first().attr("data-target") != "#tabtest") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabtest") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function ReportResultGrid() {
    $.ajax({
        url: "/Home/Test",

        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {

            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabtestresult") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a class="nav-link active context-tab" data-tab="tabtestresult" data-id="0" data-name="tabtestresult" data-toggle="tab" href="#" data-target="#tabtestresult" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/Report.png">Report</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabtestresult" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabtestresult") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabtestresult") {
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
                    if ($(value).children().first().attr("data-target") != "#tabtestresult") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabtestresult") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

function removeleftpenalactivetab() {
    var leftpenallist = document.getElementById("leftProjectList").querySelectorAll(".kt-menu__item--open");
    $.each(leftpenallist, function (key, value) {
        var classlst = value.classList;
        if (classlst[2].trim() == "context-menu-TestCase" || classlst[1].trim() == "context-menu-Storyboard") {
            classlst.remove("kt-menu__item--open");
        }
    });
}

function PartialRightStoryboardGrid(ProjectId, StoryBoardid, storyboardname, Activetab, strObj, Default) {
    //debugger;
    tempflag = false;
    startloader();
    removeleftpenalactivetab();

    if (strObj != null) {
        storyboardname = $(strObj).attr("data-name");
    }
    var storyboard = storyboardname.replace('/', '_').replace('(', '-').replace(')', '-').replace('.', '_').replace('*', '_').replace(/&/g, '_');
    if (storyboard.indexOf(" ") > -1) {
        RecusrsiveTab1(storyboard);
        storyboard = ltmpstoryboard;
    }
    if (openedTestCaseList.Storyboard != null) {
        if (!openedTestCaseList.Storyboard.includes(StoryBoardid))
            openedTestCaseList.Storyboard.push(StoryBoardid);
    }
    $.ajax({
        url: "/Home/PartialRightStoryboardGrid",
        data: '{"storyBoardName":"' + storyboardname +  '","Projectid":"' + ProjectId + '","Storyboardid":"' + StoryBoardid + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //debugger;
            var lflag = false;
            var ltabName = "#tabSB" + storyboard;
            var ltabIdName = "tabSB" + storyboard;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == ltabName) {
                    if ($(value).children().first().attr("data-tab") == "Storyboard") {
                        lflag = true;
                        if (Activetab == null) {
                            Activetab = $(value).children().first();
                        }
                    }
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab" ><a data-pin="true" href="' + ltabName + '" class="nav-link active context-tab" data-tab="Storyboard" data-id="' + StoryBoardid + '" data-name="' + storyboard + '" data-storyboardid="' + StoryBoardid + '" data-projectId="' + ProjectId + '" data-storyboardname="' + storyboard + '" data-toggle="tab" data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="Storyboard" class="tab_icons_img" src="/assets/media/icons/storyboard.png">' + storyboard + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab" ><a data-pin="false" href="' + ltabName + '" class="nav-link active context-tab" data-tab="Storyboard" data-id="' + StoryBoardid + '" data-name="' + storyboard + '" data-storyboardid="' + StoryBoardid + '" data-projectId="' + ProjectId + '" data-storyboardname="' + storyboard + '" data-toggle="tab" data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="Storyboard" class="tab_icons_img" src="/assets/media/icons/storyboard.png">' + storyboard + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="' + ltabIdName + '" role="tabpanel">' + result + '</div>';
            if (lflag) {
                if (Activetab != null) {
                    ActiveTab(Activetab);
                    //var lParentli = $(Activetab).parent();
                    //var ltab = '<a href="' + ltabName + '" class="nav-link active context-tab" data-tab="Storyboard" data-id="' + StoryBoardid + '" data-name="' + storyboard + '" data-storyboardid="' + StoryBoardid + '" data-projectId="' + ProjectId + '" data-storyboardname="' + storyboard + '" data-toggle="tab"  data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="Storyboard" class="tab_icons_img" src="/assets/media/icons/storyboard.png">' + storyboard + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i>';
                    ////var ldiv = result;
                    //var lId = "tabSB" + storyboard;
                    //var lGridDiv = $(".divtablist").find("#" + lId);
                    //$(lParentli).html("");
                    //$(lParentli).html(ltab);
                    //$(lGridDiv).html("");
                    //$(lGridDiv).html(ldiv);
                }
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == ltabName) {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == ltabIdName) {
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
                    if ($(value).children().first().attr("data-target") != "#tabSB" + storyboard) {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabSB" + storyboard) {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
    //stoploader();
}

function DisplayStoryboardGrid(StoryBoardid, Pid, Default) {
    $.ajax({
        url: "/Accounts/GetStoryboradNameyId",
        type: "POST",
        data: JSON.stringify({ "StoryBoardid": StoryBoardid }),
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            result = JSON.parse(result);
            if (result.status == 1) {
                var storyboardname = result.data;
                PartialRightStoryboardGrid(Pid, StoryBoardid, storyboardname, null, null, Default);
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
    });
}

function PartialRightGrid(TestcaseId, TestsuiteId, ProjectId, TestCaseName, Activetab, tcobj, Default) {
    // gridname = true;
    startloader();
    removeleftpenalactivetab();
    $("#projectidtc").val(ProjectId);
    $("#testsuiteidtc").val(TestsuiteId);
    datasetobj = tcobj;
    if (tcobj != null) {
        TestCaseName = $(tcobj).attr("data-name");
    }
    tempflag = false;
    var lTestCaseId = TestcaseId;
    var lTestsuiteId = TestsuiteId;
    var lProjectId = ProjectId;
    var lTestCaseName = "";
    if (TestCaseName != undefined) {
        lTestCaseName = TestCaseName.replace('/', '_').replace('(', '-').replace(')', '-').replace('.', '_').replace('*', '_').replace(/&/g, '_');
    }
    if (lTestCaseName.indexOf(" ") > -1) {
        RecusrsiveTab(lTestCaseName);
        lTestCaseName = ltempTestCase;
    }
    if (openedTestCaseList.TestCase != null) {
        if (!openedTestCaseList.TestCase.includes(TestcaseId))
            openedTestCaseList.TestCase.push(TestcaseId);
    }
    $.ajax({
        url: "/Home/RightSideGridView",
        data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // lTestCaseName = RecusrsiveTab(lTestCaseName);
            var lflag = false;
            var ltabName = "#tab" + lTestCaseName;
            var ltabIdName = "tab" + lTestCaseName;
            
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == ltabName) {
                    if ($(value).children().first().attr("data-tab") == "TestCase") {
                        lflag = true;
                        if (Activetab == null) {
                            Activetab = $(value).children().first();
                        }
                    }
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab" ><a data-pin="true" href="' + ltabName + '"  class="nav-link active context-tab" data-tab="TestCase" data-id="' + lTestCaseId + '" data-name="' + lTestCaseName + '" data-testcaseId="' + lTestCaseId + '" data-testsuiteId="' + lTestsuiteId + '" data-projectId="' + lProjectId + '" data-testcasename="' + lTestCaseName + '" data-toggle="tab" data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="TestCase" class="tab_icons_img" src="/assets/media/icons/test_case.png">' + TestCaseName + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab" ><a data-pin="false" href="' + ltabName + '"  class="nav-link active context-tab" data-tab="TestCase" data-id="' + lTestCaseId + '" data-name="' + lTestCaseName + '" data-testcaseId="' + lTestCaseId + '" data-testsuiteId="' + lTestsuiteId + '" data-projectId="' + lProjectId + '" data-testcasename="' + lTestCaseName + '" data-toggle="tab" data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="TestCase" class="tab_icons_img" src="/assets/media/icons/test_case.png">' + TestCaseName + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="' + ltabIdName + '" role="tabpanel">' + result + '</div>';
            if (lflag) {
                if (Activetab != null) {
                    var lParentli = $(Activetab).parent();
                    var ltab = '<a href="' + ltabName + '"  class="nav-link active context-tab" data-tab="TestCase" data-id="' + lTestCaseId + '" data-name="' + lTestCaseName + '" data-testcaseId="' + lTestCaseId + '" data-testsuiteId="' + lTestsuiteId + '" data-projectId="' + lProjectId + '" data-testcasename="' + lTestCaseName + '" data-toggle="tab" data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="TestCase" class="tab_icons_img" src="/assets/media/icons/test_case.png">' + TestCaseName + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i>';
                    var ldiv = result;
                    var lId = "tab" + TestCaseName;
                    var lGridDiv = $(".divtablist").find("#" + lId);
                    $(lParentli).html("");
                    $(lParentli).html(ltab);
                    $(lGridDiv).html("");
                    $(lGridDiv).html(ldiv);
                }
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == ltabName) {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == ltabIdName) {
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
                    if ($(value).children().first().attr("data-target") != ltabName) {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != ltabIdName) {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
            //activate: function (evt, ui) {
            //    ui.newPanel.find(".pq-grid").pqGrid('refresh');
            //}
        },

    });
    //stoploader();
}

function DisplayTestCaseGrid(Tid, Pid, Default) {
    $.ajax({
        url: "/Accounts/GetTestsuiteIdByTeastcaseId",
        type: "POST",
        data: JSON.stringify({ "Tid": Tid }),
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            result = JSON.parse(result);
            if (result.status == 1) {
                var obj = result.data;
                var TestsuiteId = obj.TestSuiteId;
                var TestCaseName = obj.TestCaseName;
                PartialRightGrid(Tid, TestsuiteId, Pid, TestCaseName, null, null, Default);
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
    });
}

function PartialRightGridTestCaseFromStoryboard(TestcaseId, TestsuiteId, ProjectId, Datasetname, TestCaseName, Activetab, storyboradId, lstoryboardname) {
    var lTestCaseId = TestcaseId;
    var lTestsuiteId = TestsuiteId;
    var lProjectId = ProjectId;
    var lTestCaseName = TestCaseName.replace('/', '_').replace('(', '-').replace(')', '-').replace('.', '_').replace('*', '_').replace(/&/g, '_').replace(' ', '_');
    if (lTestCaseName.indexOf(" ") > -1) {
        RecusrsiveTab(lTestCaseName);
        lTestCaseName = ltempTestCase;
    }
    startloader();
    $.ajax({
        url: "/Home/RightSideGridView",
        data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '","VisibleDataset":"' + Datasetname + '", "storyboradId":"' + storyboradId + '" ,"storyboardname":"' + lstoryboardname + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // lTestCaseName = RecusrsiveTab(lTestCaseName);
            var lflag = false;
            var ltabName = "#tab" + lTestCaseName;
            var ltabIdName = "tab" + lTestCaseName;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == ltabName) {
                    lflag = true;
                    if (Activetab == null) {
                        Activetab = $(value).children().first();
                    }
                }
            });

            var ltab = '<li class="nav-item context-menu-tab" ><a href="' + ltabName + '" class="nav-link active context-tab" data-tab="TestCase" data-id="' + lTestCaseId + '" data-name="' + lTestCaseName + '" data-testcaseId="' + lTestCaseId + '" data-testsuiteId="' + lTestsuiteId + '" data-projectId="' + lProjectId + '" data-testcasename="' + lTestCaseName + '" data-toggle="tab"  data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="TestCase" class="tab_icons_img" src="/assets/media/icons/test_case.png">' + TestCaseName + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            var ldiv = '<div class="tab-pane active div" id="' + ltabIdName + '" role="tabpanel">' + result + '</div>';

            if (lflag) {
                if (Activetab != null) {
                    var lParentli = $(Activetab).parent();
                    ltab = '<a  href="' + ltabName + '"  class="nav-link active context-tab" data-tab="TestCase" data-id="' + lTestCaseId + '" data-name="' + lTestCaseName + '" data-testcaseId="' + lTestCaseId + '" data-testsuiteId="' + lTestsuiteId + '" data-projectId="' + lProjectId + '" data-testcasename="' + lTestCaseName + '" data-toggle="tab"  data-target="' + ltabName + '" onclick="ActiveTab($(this))"><img alt="TestCase" class="tab_icons_img" src="/assets/media/icons/test_case.png">' + TestCaseName + '</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i>';
                    ldiv = result;
                    var lId = "tab" + lTestCaseName;
                    var lGridDiv = $(".divtablist").find("#" + lId);
                    $(lParentli).html("");
                    $(lParentli).html(ltab);
                    $(lGridDiv).html("");
                    $(lGridDiv).html(ldiv);
                }
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == ltabName) {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == ltabIdName) {
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
                    if ($(value).children().first().attr("data-target") != ltabName) {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != ltabIdName) {
                        $(value).removeClass("active");
                    }
                });
            }
            //activate: function (evt, ui) {
            //    ui.newPanel.find(".pq-grid").pqGrid('refresh');
            //}
            stoploader();
        },
    });
}

function ActiveTab(Activetab) {
    var ptempflag = false;
    var ptestcaseopenflag = false;
    var lActiveTab = Activetab.attr("data-target");
    var lTestCaseId = Activetab.attr("data-testcaseId");
    var lTestsuiteId = Activetab.attr("data-testsuiteId");
    var lProjectId = Activetab.attr("data-projectId");
    var lTestCaseName = Activetab.attr("data-testcasename");
    var storyboardid = Activetab.attr("data-storyboardid");
    var storyboardname = Activetab.attr("data-storyboardname");
    var TestCaselst = document.getElementById("leftProjectList").querySelectorAll(".context-menu-TestCase");
    var Storyboardlst = document.getElementById("leftProjectList").querySelectorAll(".context-menu-Storyboard");
    var Name = Activetab.attr("data-name");

    var baseid = Activetab.attr("data-baseid");
    var compareid = Activetab.attr("data-compareid");
    var storyboard = Activetab.attr("data-storyboard");
    var runorder = Activetab.attr("data-runorder");
    var tab = Activetab.attr("data-tab");
    //$('.ULtablist li').each(function (index, value) {
    //    $(value).children().first().removeClass("active");
    //});
    var PrevActivetab = null;
    $('.ULtablist li').each(function (index, value) {
        if ($(value).children().first().attr("class").indexOf("active") > -1) {
            $(value).children().first().removeClass("active");
        }
    });
    if (tab == "DataTag" && baseid != undefined && compareid != undefined && storyboard != undefined && runorder != undefined) {
        var lresultset = "ResultView_BHistoryId_" + baseid + "_CHistoryId_" + compareid;
        setTimeout(function () { gridobj[".grid" + lresultset].reset({ filter: true }); }, 500);
    }

    if (tab == "FolderDataSet" && Name != undefined) {
        setTimeout(function () { gridobj[".grid" + Name.replace(/ /g, '_')].reset({ filter: true }); }, 500);
    }

    if (lTestCaseId != null && lTestsuiteId != null && lProjectId != null && lTestCaseName != null
        && lTestCaseId > 0 && lTestsuiteId > 0 && lProjectId > 0) {
        //if (gridobj[".grid" + lTestCaseName] != null && gridobj[".grid" + lTestCaseName] != undefined && gridobj[".grid" + lTestCaseName].hasOwnProperty("reset"))
            setTimeout(function () { gridobj[".grid" + lTestCaseName].reset({ filter: true }); }, 500);

        //   PartialRightGrid(lTestCaseId, lTestsuiteId, lProjectId, lTestCaseName, Activetab, null);
    }
    if (storyboardid != null && lProjectId != null && storyboardname != null && storyboardname != undefined
        && storyboardid > 0 && lProjectId > 0) {
        setTimeout(function () {
            gridobj[".gridSB" + storyboardname.replace(/ /g, '_')].reset({ filter: true });
        }, 500);
        //setTimeout(function () { gridobj[".grid" + storyboardname].reset({ filter: true }); }, 500);
        //  PartialRightStoryboardGrid(lProjectId, storyboardid, storyboardname, Activetab, null);
    }
    $.each(TestCaselst, function (key, value) {
        var obj = value.innerText;
        var classlst = value.classList;
        
        if (classlst.length > 3) {
            if (classlst[3].trim() == "kt-menu__item--open") {
                classlst.remove("kt-menu__item--open");
            }
        }

        if (value.innerHTML.indexOf('data-id="' + lTestCaseId + '"') > 0) {            
            lTestCaseName = lTestCaseName == undefined ? "" : lTestCaseName.replace(/_/g, ' ');
            obj = obj == undefined ? obj : obj.replace(/_/g, ' ').trim();
            if (obj.includes(lTestCaseName.trim())) {
                value.classList.add("kt-menu__item--open");
                if (value.parentNode.parentNode.parentNode != null && value.parentNode.parentNode.parentNode.children.length >= 2) {
                    value.parentNode.parentNode.parentNode.children[1].style.display = "block";
                    value.parentNode.parentNode.parentNode.children[1].style.overflow = "hidden";
                }
                /*if (value.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode != null && value.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.children.length >= 2) {
                    value.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.classList.add("kt-menu__item--open");
                    value.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.children[1].style.display = "block";
                    value.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode.children[1].style.overflow = "hidden";
                }*/
            }
        }
    });
    $.each(Storyboardlst, function (key, value) {
        var obj = value.innerText;
        var classlst = value.classList;
        console.log(value.parentNode.innerHTML);
        if (classlst.length > 2) {
            if (classlst[2].trim() == "kt-menu__item--open") {
                classlst.remove("kt-menu__item--open");
            }
        }
        if (value.innerHTML.indexOf('data-id="' + storyboardid + '"') > 0) {
                   
            storyboardname = storyboardname == undefined ? "" : storyboardname.replace(/_/g, ' ');
            obj = obj == undefined ? obj : obj.replace(/_/g, ' ').trim();
            if (storyboardname != undefined) {
                if (obj.includes(storyboardname.trim())) {
                    value.classList.add("kt-menu__item--open");
                    if (value.parentNode.parentNode.parentNode != null && value.parentNode.parentNode.parentNode.children.length >= 2) {
                        value.parentNode.parentNode.parentNode.children[1].style.display = "block";
                        value.parentNode.parentNode.parentNode.children[1].style.overflow = "hidden";
                    }
                }
            }
        }
    });
}
function partialOpenObjectList(Default) {
    $.ajax({
        url: "/Object/ObjectList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //CheckAtiveTabStatus();
            if (!openedListTabList.includes("objectlist"))
                openedListTabList.push("objectlist");
            OnPartialRightGridShow(0, "objectlist", "1", "objectlist");
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") === "#tabobjectlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="objectlist" data-id="0" data-name="objectlist" data-toggle="tab" href="#" data-target="#tabobjectlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/ol.png">Object List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="objectlist" data-id="0" data-name="objectlist" data-toggle="tab" href="#" data-target="#tabobjectlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/ol.png">Object List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabobjectlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") === "#tabobjectlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") === "tabobjectlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabobjectlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabobjectlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        }
    });
}
function partialRightOpenUserList(Default) {
    $.ajax({
        url: "/Project/UserList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //CheckAtiveTabStatus();
            //$("#rightsideView").html("");
            //$("#rightsideView").html(result);
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") === "#tabuserlistforproject") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserListbyProject" data-id="0" data-name="UserListByProject" data-toggle="tab" href="#" data-target="#tabuserlistforproject" onclick="ActiveTab($(this))"><img alt="User-Project Mappings List" class="tab_icons_img" src="/assets/media/icons/upm.png">List of User-Project Mappings</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserListbyProject" data-id="0" data-name="UserListByProject" data-toggle="tab" href="#" data-target="#tabuserlistforproject" onclick="ActiveTab($(this))"><img alt="User-Project Mappings List" class="tab_icons_img" src="/assets/media/icons/upm.png">List of User-Project Mappings</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuserlistforproject" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") === "#tabuserlistforproject") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") === "tabuserlistforproject") {
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
                    if ($(value).children().first().attr("data-target") != "#tabuserlistforproject") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuserlistforproject") {
                        $(value).removeClass("active");
                    }
                });
            }
            //stoploader();
        }
    });
}
function PartialRightSideTestSuiteGrid(Default) {
    //TestSuiteList
    //startloader();
    $.ajax({
        url: "/TestSuite/TestSuiteList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            if (!openedListTabList.includes("TestSuiteList"))
                openedListTabList.push("TestSuiteList");
            OnPartialRightGridShow(0, "TestSuiteList", "1", "TestSuiteList");
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabtestsuitelist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" href="#tabtestsuitelist"  class="nav-link active context-tab" data-tab="TestSuiteList" data-id="0" data-name="TestSuiteList" data-toggle="tab" href="#" data-target="#tabtestsuitelist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/tsl.png"> Test Suite List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" href="#tabtestsuitelist"  class="nav-link active context-tab" data-tab="TestSuiteList" data-id="0" data-name="TestSuiteList" data-toggle="tab" href="#" data-target="#tabtestsuitelist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/tsl.png"> Test Suite List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabtestsuitelist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabtestsuitelist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabtestsuitelist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabtestsuitelist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabtestsuitelist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function PartialRightSideProjectGrid(Default) {
    $.ajax({
        url: "/Project/ProjectsList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            if (!openedListTabList.includes("ProjectsList"))
                openedListTabList.push("ProjectsList");
            OnPartialRightGridShow(0, "ProjectsList", "1", "ProjectsList");
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabprojectslist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" href="#tabprojectslist"  class="nav-link active context-tab" data-tab="ProjectsList" data-id="0" data-name="ProjectsList" data-toggle="tab" href="#" data-target="#tabprojectslist" onclick="ActiveTab($(this))"><img alt="Project List" class="tab_icons_img" src="/assets/media/icons/project.png"> Project List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" href="#tabprojectslist"  class="nav-link active context-tab" data-tab="ProjectsList" data-id="0" data-name="ProjectsList" data-toggle="tab" href="#" data-target="#tabprojectslist" onclick="ActiveTab($(this))"><img alt="Project List" class="tab_icons_img" src="/assets/media/icons/project.png"> Project List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabprojectslist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabprojectslist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabprojectslist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabprojectslist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabprojectslist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

//Jenkins Integration - start
function PartialRightSideJenkinsIntegrationGrid(Default) {
    $.ajax({
        url: "/Jenkins/JenkinsIntegration",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabJenkinsIntegration") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" href="#tabJenkinsIntegration"  class="nav-link active context-tab" data-tab="JenkinsIntegration" data-id="0" data-name="JenkinsIntegration" data-toggle="tab" href="#" data-target="#tabJenkinsIntegration" onclick="ActiveTab($(this))"><img alt="Jenkins Integration" class="tab_icons_img" src="/assets/media/icons/project.png"> Jenkins Integration</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" href="#tabJenkinsIntegration"  class="nav-link active context-tab" data-tab="JenkinsIntegration" data-id="0" data-name="JenkinsIntegration" data-toggle="tab" href="#" data-target="#tabJenkinsIntegration" onclick="ActiveTab($(this))"><img alt="Jenkins Integration" class="tab_icons_img" src="/assets/media/icons/project.png"> Jenkins Integration</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabJenkinsIntegration" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabJenkinsIntegration") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabJenkinsIntegration") {
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
                    if ($(value).children().first().attr("data-target") != "#tabJenkinsIntegration") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabJenkinsIntegration") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
//Jenkins Integration - end

function PartialRightSideAplicationGrid(Default) {
    $.ajax({
        url: "/Application/ApplicationList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //CheckAtiveTabStatus();
            var lflag = false;

            if (!openedListTabList.includes("ApplicationList"))
                openedListTabList.push("ApplicationList");
            OnPartialRightGridShow(0, "ApplicationList", "1", "ApplicationList");

            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabapplicationlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" href="#tabapplicationlist"  class="nav-link active context-tab" data-tab="ApplicationList" data-id="0" data-name="ApplicationList" data-toggle="tab" href="#" data-target="#tabapplicationlist" onclick="ActiveTab($(this))"><img alt="Application List" class="tab_icons_img" src="/assets/media/icons/application.png"> Application List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" href="#tabapplicationlist"  class="nav-link active context-tab" data-tab="ApplicationList" data-id="0" data-name="ApplicationList" data-toggle="tab" href="#" data-target="#tabapplicationlist" onclick="ActiveTab($(this))"><img alt="Application List" class="tab_icons_img" src="/assets/media/icons/application.png"> Application List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabapplicationlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabapplicationlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabapplicationlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabapplicationlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabapplicationlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            //stoploader();
        },
    });
}
function PartialRightSideKeywordGrid(Default) {
    $.ajax({
        url: "/Keyword/KeywordList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            if (!openedListTabList.includes("KeywordList"))
                openedListTabList.push("KeywordList");
            OnPartialRightGridShow(0, "KeywordList", "1", "KeywordList");
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabkeywordlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" href="#tabkeywordlist"  class="nav-link active context-tab" data-tab="KeywordList" data-id="0" data-name="KeywordList" data-toggle="tab" href="#" data-target="#tabkeywordlist" onclick="ActiveTab($(this))"><img alt="Keyword List" class="tab_icons_img" src="/assets/media/icons/keyword.png"> Keyword List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" href="#tabkeywordlist"  class="nav-link active context-tab" data-tab="KeywordList" data-id="0" data-name="KeywordList" data-toggle="tab" href="#" data-target="#tabkeywordlist" onclick="ActiveTab($(this))"><img alt="Keyword List" class="tab_icons_img" src="/assets/media/icons/keyword.png"> Keyword List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabkeywordlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabkeywordlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabkeywordlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabkeywordlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabkeywordlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

function PartialConnectionList() {
    $.ajax({
        url: "/DBConnection/ConnectionList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        beforeSend: function () {
            startloader();
        },
        complete: function () {
            stoploader();
        },
        success: function (result) {
            // CheckAtiveTabStatus();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabconnectionlist") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a  href="#tabconnectionlist"  class="nav-link active context-tab" data-tab="ConnectionList" data-id="0" data-name="ConnectionList" data-toggle="tab" href="#" data-target="#tabconnectionlist" onclick="ActiveTab($(this))"><img alt="Connection List" class="tab_icons_img" src="/assets/media/icons/keyword.png"> Connection List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabconnectionlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabconnectionlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabconnectionlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabconnectionlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabconnectionlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

function PartialRightSideStoryboardList() {
    // startloader();
    $.ajax({
        url: "/Storyboard/StoryboardList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //CheckAtiveTabStatus();
            //$("#rightsideView").html("");
            //$("#rightsideView").html(result);
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabstoryboardlist") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a class="nav-link active context-tab" data-tab="StoryboardList" data-id="0" data-name="StoryboardList" data-toggle="tab" href="#" data-target="#tabstoryboardlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/sbl.png"> Storyboard List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabstoryboardlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabstoryboardlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabstoryboardlist") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
                // table.table().draw();
            }
            else {
                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabstoryboardlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabstoryboardlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function PartialRightSideVariablesGrid(Default) {
    //TestSuiteList
    //startloader();
    $.ajax({
        url: "/Variable/VariableList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            //$("#rightsideView").html("");
            //$("#rightsideView").html(result);
            if (!openedListTabList.includes("VariableList"))
                openedListTabList.push("VariableList");
            OnPartialRightGridShow(0, "VariableList", "1", "VariableList");

            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabvariablelist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="VariableList" data-id="0" data-name="VariableList" data-toggle="tab" href="#" data-target="#tabvariablelist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/vrl.png"> Variable List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="VariableList" data-id="0" data-name="VariableList" data-toggle="tab" href="#" data-target="#tabvariablelist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/vrl.png"> Variable List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabvariablelist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabvariablelist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabvariablelist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabvariablelist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabvariablelist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function PartialRightSideImportTestSuite(Default) {
    if (Default == "0")
        startloader();
    $.ajax({
        url: "/TestSuite/ImportTestSuite",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //  CheckAtiveTabStatus();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimporttestsuite") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-tab="ImportTestSuite" data-name="ImportTestSuite" data-toggle="tab" href="#" data-target="#tabimporttestsuite" onclick="ActiveTab($(this))"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/its.png"/>Import Test Suite</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-tab="ImportTestSuite" data-name="ImportTestSuite" data-toggle="tab" href="#" data-target="#tabimporttestsuite" onclick="ActiveTab($(this))"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/its.png"/>Import Test Suite</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimporttestsuite" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimporttestsuite") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimporttestsuite") {
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
                    if ($(value).children().first().attr("data-target") != "#tabimporttestsuite") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimporttestsuite") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function PartialRightImportStoryboard(Default) {
    if (Default == "0")
        startloader();
    $.ajax({
        url: "/Storyboard/ImportStoryboard",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimportstoryboard") {
                    lflag = true;
                }
            });

            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-toggle="tab" href="#" data-tab="ImportStoryboard" data-name="ImportStoryboard" data-target="#tabimportstoryboard" onclick="ActiveTab($(this))"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/isb.png"/>Import Storyboard</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-toggle="tab" href="#" data-tab="ImportStoryboard" data-name="ImportStoryboard" data-target="#tabimportstoryboard" onclick="ActiveTab($(this))"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/isb.png"/>Import Storyboard</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimportstoryboard" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimportstoryboard") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimportstoryboard") {
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
                    if ($(value).children().first().attr("data-target") != "#tabimportstoryboard") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimportstoryboard") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

function RightSideUserActivePageList(Default) {
    $.ajax({
        url: "/Accounts/ListOfUsersActivePage",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabuseractivelist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserActiveList" data-id="0" data-name="UserActiveList" data-toggle="tab" href="#" data-target="#tabuseractivelist" onclick="ActiveTab($(this))"><img alt="User Active List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User Active List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserActiveList" data-id="0" data-name="UserActiveList" data-toggle="tab" href="#" data-target="#tabuseractivelist" onclick="ActiveTab($(this))"><img alt="User Active List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User Active List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuseractivelist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabuseractivelist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabuseractivelist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabuseractivelist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuseractivelist") {
                        $(value).removeClass("active");
                    }
                });
            }
            //stoploader();
        },
    });
}

function RightSideGridList(Default) {
    $.ajax({
        url: "/ConfigurationGrid/GridList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabgridlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="GirdList" data-id="0" data-name="GirdList" data-toggle="tab" href="#" data-target="#tabgridlist" onclick="ActiveTab($(this))"><img alt="Grid List" class="tab_icons_img" src="/assets/media/icons/user.png"/>Grid List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="GirdList" data-id="0" data-name="GirdList" data-toggle="tab" href="#" data-target="#tabgridlist" onclick="ActiveTab($(this))"><img alt="Grid List" class="tab_icons_img" src="/assets/media/icons/user.png"/>Grid List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabgridlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabgridlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabgridlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabgridlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabgridlist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSidePrivilegeRoleMapping(Default) {
    $.ajax({
        url: "/Entitlement/PrivilegeRoleMapping",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabroleprivilegeslist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="RolePrivileges" data-id="0" data-name="RolePrivileges" data-toggle="tab" href="#" data-target="#tabroleprivilegeslist" onclick="ActiveTab($(this))"><img alt="Role Privileges" class="tab_icons_img" src="/assets/media/icons/PRM.png"/>Role Privileges</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="RolePrivileges" data-id="0" data-name="RolePrivileges" data-toggle="tab" href="#" data-target="#tabroleprivilegeslist" onclick="ActiveTab($(this))"><img alt="Role Privileges" class="tab_icons_img" src="/assets/media/icons/PRM.png"/>Role Privileges</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabroleprivilegeslist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabroleprivilegeslist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabroleprivilegeslist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabroleprivilegeslist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabroleprivilegeslist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}
//Privilege Role mapping

function PartialRightSideTestCaseGrid(Default) {
    // startloader();
    $.ajax({
        url: "/TestCase/TestCaseList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            if (!openedListTabList.includes("TestCaseList"))
                openedListTabList.push("TestCaseList");
            OnPartialRightGridShow(0, "TestCaseList", "1", "TestCaseList"); 
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabtestcaselist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="TestCaseList" data-id="0" data-name="TestCaseList" data-toggle="tab" href="#" data-target="#tabtestcaselist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/tcl.png">Test Case List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="TestCaseList" data-id="0" data-name="TestCaseList" data-toggle="tab" href="#" data-target="#tabtestcaselist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/tcl.png">Test Case List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabtestcaselist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabtestcaselist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabtestcaselist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabtestcaselist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabtestcaselist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function UserexecutionEnginePathList(Default) {
    if (Default == "0")
        startloader();
    $.ajax({
        url: "/Accounts/UserexecutionEnginePathList",
        // data: '{"TestcaseId":"' + TestcaseId + '","TestsuiteId":"' + TestsuiteId + '","ProjectId":"' + ProjectId + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            // CheckAtiveTabStatus();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") === "#tabuserExelist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserExePathList" data-id="0" data-name="UserExePathList" data-toggle="tab" href="#" data-target="#tabuserExelist" onclick="ActiveTab($(this))"><img alt="User List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User List with Exepath</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserExePathList" data-id="0" data-name="UserExePathList" data-toggle="tab" href="#" data-target="#tabuserExelist" onclick="ActiveTab($(this))"><img alt="User List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User List with Exepath</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuserExelist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabuserExelist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabuserExelist") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
                Usertable.table().draw();
            }
            else {


                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabuserExelist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuserExelist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        }
    });
}
function RightSideUserGrid(Default) {
    $.ajax({
        url: "/Accounts/ListOfUsers",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        beforeSend: function () {
            startloader();
        },
        complete: function () {
            stoploader();
        },
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabuserlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserList" data-id="0" data-name="UserList" data-toggle="tab" href="#" data-target="#tabuserlist" onclick="ActiveTab($(this))"><img alt="User List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserList" data-id="0" data-name="UserList" data-toggle="tab" href="#" data-target="#tabuserlist" onclick="ActiveTab($(this))"><img alt="User List" class="tab_icons_img" src="/assets/media/icons/user.png"/>User List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuserlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabuserlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabuserlist") {
                        $(value).addClass("active");
                    } else {
                        $(value).removeClass("active");
                    }
                });
                Usertable.table().draw();
            }
            else {


                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabuserlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuserlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            //stoploader();
        },
    });
}
function partialRightOpenProjectList(Default) {
    $.ajax({
        url: "/Project/ProjectListByUserId",
        data: '{"userid":"' + 0 + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        beforeSend: function () {
            startloader();
        },
        complete: function () {
            stoploader();
        },
        success: function (result) {
            // CheckAtiveTabStatus();
            //$("#rightsideView").html("");
            //$("#rightsideView").html(result);
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") === "#tabprojectlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="SelectProject" data-id="0" data-name="SelectProject" data-toggle="tab" href="#" data-target="#tabprojectlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/sp.png">Select Project</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="SelectProject" data-id="0" data-name="SelectProject" data-toggle="tab" href="#" data-target="#tabprojectlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/sp.png">Select Project</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabprojectlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") === "#tabprojectlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") === "tabprojectlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabprojectlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabprojectlist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}
function partialRightOpenProjectListFromUser(id) {
    $.ajax({
        url: "/Project/ProjectListByUserId",
        data: '{"userid":"' + id + '"}',
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            //$("#rightsideView").html("");
            //$("#rightsideView").html(result);
            UpTable.table().draw();
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") === "#tabprojectlist") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a class="nav-link active context-tab" data-tab="ProjectList" data-id="0" data-name="ProjectList" data-toggle="tab" href="#" data-target="#tabprojectlist" onclick="ActiveTab($(this))"><img alt="Test Suite List" class="tab_icons_img" src="/assets/media/icons/vrl.png">Select Project</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabprojectlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") === "#tabprojectlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") === "tabprojectlist") {
                        $(value).addClass("active");
                        $(value).html("");
                        $(value).append(ldiv);
                    } else {
                        $(value).removeClass("active");
                    }
                });

            }
            else {


                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabprojectlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabprojectlist") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        }
    });
}
function AddEditUser(lid) {

    startloader();
    $.ajax({
        url: "/Accounts/AddEditUser",
        data: { "lid": lid },
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {


            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabAddEditUser") {
                    lflag = true;
                }
            });
            var ltab = '<li class="nav-item context-menu-tab"><a class="nav-link active context-tab" data-id="0" data-tab="AddEditUser" data-name="AddEditUser" data-toggle="tab" href="#" data-target="#tabAddEditUser" onclick="ActiveTab($(this))"><img alt="Add/Edit User" class="tab_icons_img" src="/assets/media/icons/add-edit-user.png"/>Add/Edit User</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            var ldiv = '<div class="tab-pane active div" id="tabAddEditUser" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabAddEditUser") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabAddEditUser") {
                        $(value).addClass("active");
                        //$(value).children().first().html("");
                        $(value).html("");
                        $(value).append(ldiv);
                        //$(".divtablist").html("");
                        //$(".divtablist").append(ldiv);
                    } else {
                        $(value).removeClass("active");
                    }
                });
            }
            else {

                $(".ULtablist").append(ltab);
                $(".divtablist").append(ldiv);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") != "#tabAddEditUser") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabAddEditUser") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}
function checkTCSBChangesOnClose(value) {
    var lParent = $(value).parent();
    var lFindTab = $(lParent).children().first().attr("data-tab");
    var lfindname = $(lParent).children().first().attr("data-name");

    if (lFindTab == "TestCase" || lFindTab == "Storyboard" || lFindTab == "DataTag") {
        var checkarray = lFindTab == "Storyboard" ? gridobj[".gridSB" + lfindname].getChanges({ format: "byVal" }) : gridobj[".grid" + lfindname].getChanges({ format: "byVal" });
        if (checkarray.addList.length > 0 || checkarray.deleteList.length > 0 || checkarray.updateList.length > 0 || checkarray.oldList.length > 0) {
            // testcaseopenflag = true;
            return "unsaved";
        }
    } else {
        /*if (openedListTabList != null && openedListTabList.includes(lFindTab)) {
            openedListTabList.remove(lFindTab);
            OnPartialRightGridShow(0, lFindTab, "2", lFindTab);
        }*/
    }
}
var tcsbobj = "";
function alertShow(testcasegrid, lname, tcsbid, suiteid, projectid, tabname, tabcloseObj) {
    tcsbobj = tabcloseObj;
    var lParent = $(tcsbobj).parent();
    var baseid = $(lParent).children().first().attr("data-baseid");
    var compareid = $(lParent).children().first().attr("data-compareid");
    var testcaseid = $(lParent).children().first().attr("data-testcaseid");
    var storyboarddetailid = $(lParent).children().first().attr("data-storyboarddetailid");

    swal.fire({
        html:
            "<button type='button' role='button' tabindex='0' class='swal2-styled swal2-confirm cancelSwalBtn' data-testcaseid='" + testcaseid + "' data-storyboarddetailid='" + storyboarddetailid + "' data-base='" + baseid + "' data-compare='" + compareid + "' data-tabsave='" + testcasegrid + "' data-tcsbid='" + tcsbid + "' data-suiteid='" + suiteid + "' data-projectid='" + projectid + "' data-tabname='" + tabname + "' onclick=swal.closeModal();saveTCSBGrid($(this))>Save</button>" +
            '<button type="button" role="button" tabindex="0" class="swal2-styled swal2-confirm cancelSwalBtn" onclick="swal.closeModal(); return false;" >' + 'Cancel' + '</button>' +
            "<button type='button' role='button' tabindex='0' class='swal2-styled swal2-confirm cancelSwalBtn' data-testcaseid='" + testcaseid + "' data-obj='" + tcsbid + "' onclick=swal.closeModal();cancelTCSBGrid()>Close</button>",
        //html: buttons,
        title: 'Save changes to [' + lname + '] ?',
        text: "You have some unsaved changes left!",
        icon: 'warning',
        showCancelButton: false,
        showConfirmButton: false,
        closeOnConfirm: true
    });
}
function cancelTCSBGrid() {
    var tabobjClose = tcsbobj;
    var lParent = $(tabobjClose).parent();
    var lBeforeTab = $(lParent).prev();
    var testCaseIds = "";
    var lFindTab = $(lParent).children().first().attr("data-target");
    var targetname = $(lParent).children().first().attr("data-tab");
    var lfindname = $(lParent).children().first().attr("data-name");
    var lfindTestcaseid = $(lParent).children().first().attr("data-testcaseid"); 
    if (lfindTestcaseid != null && lfindTestcaseid != undefined && lfindTestcaseid != 0) {
        var lfindidLong = parseInt(lfindTestcaseid);
        if (openedTestCaseList[targetname] != null && openedTestCaseList[targetname].includes(lfindidLong)) {
            openedTestCaseList[targetname].remove(lfindidLong);
            OnPartialRightGridShow(lfindidLong, lfindname, "2", targetname);
        }
    }

    //if (openedListTabList != null && openedListTabList.includes(targetname)) {
    //    openedListTabList.remove(targetname);
    //    OnPartialRightGridShow(0, targetname, "2", targetname);
    //}

    if ($(lParent).children().first().attr("data-tab") == "TestCase") {
        var tcidclose = $(lParent).children().first().attr("data-id");
        if (ExistDataSetRenameList.length > 0) {
            ExistDataSetRenameList = jQuery.grep(ExistDataSetRenameList, function (value) {
                if (value != undefined) {
                    return value["TestCaseId"] != tcidclose;
                }

            });
        }
        if (DeleteColumnsList.length > 0) {
            DeleteColumnsList = jQuery.grep(DeleteColumnsList, function (value) {

                if (value != undefined) {
                    return value["TestCaseId"] != tcidclose;
                }

            });
        }
        testCaseIds = testCaseIds + "," + $(lParent).children().first().attr("data-id");
    }
    var baseid = $(lParent).children().first().attr("data-baseid");
    var compareid = $(lParent).children().first().attr("data-compareid");
    var storyboard = $(lParent).children().first().attr("data-storyboard");
    var runorder = $(lParent).children().first().attr("data-runorder");
    var tab = $(lParent).children().first().attr("data-tab");

    var lTabNameId = lFindTab.replace("#", "").replace('(', '_').replace(')', '_');
    var lDirGrid = $(".divtablist #" + lTabNameId);
    $(lParent).remove();
    $(lDirGrid).remove();
    var lflag = true;
    $('.ULtablist li').each(function (index, value) {
        var lFindclass = $(value).children().first().attr("class");
        if (lFindclass.indexOf("active") > -1) {
            lflag = false;
        }
    });
    if (lflag) {
        var lPrevTab = $(lBeforeTab).children().first().attr("data-target");
        var lPrevTabNameId = lPrevTab.replace("#", "").replace('(', '_').replace(')', '_');
        var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
        $(lBeforeTab).children().first().addClass("active");
        $(lPrevDirGrid).addClass("active");
        var Activetab = $(lBeforeTab).children().first();
        var lTestCaseId = Activetab.attr("data-testcaseId");
        var lTestsuiteId = Activetab.attr("data-testsuiteId");
        var lProjectId = Activetab.attr("data-projectId");
        var lTestCaseName = Activetab.attr("data-testcasename");
        var storyboardid = Activetab.attr("data-storyboardid");
        var storyboardname = Activetab.attr("data-storyboardname");

        if (tab == "DataTag" && baseid != undefined && compareid != undefined && storyboard != undefined && runorder != undefined) {
            var lresultset = "ResultView_BHistoryId_" + baseid + "_CHistoryId_" + compareid;
            setTimeout(function () { gridobj[".grid" + lresultset].reset({ filter: true }); }, 500);
        }

        if (lTestCaseId != null && lTestsuiteId != null && lProjectId != null && lTestCaseName != null
            && lTestCaseId > 0 && lTestsuiteId > 0 && lProjectId > 0) {
            setTimeout(function () { gridobj[".grid" + lTestCaseName].reset({ filter: true }); }, 500);
            //  PartialRightGrid(lTestCaseId, lTestsuiteId, lProjectId, lTestCaseName, Activetab, null);
        }
        if (storyboardid != null && lProjectId != null && storyboardname != null
            && storyboardid > 0 && lProjectId > 0) {
            setTimeout(function () { gridobj[".grid" + storyboardname.replace(/ /g, '_')].reset({ filter: true }); }, 500);
            //   PartialRightStoryboardGrid(lProjectId, storyboardid, storyboardname, Activetab, null);
        }

    }
    $($.fn.dataTable.tables(true)).DataTable()
        .columns.adjust();

    UpdateIsAvailableTestCase(testCaseIds);
}
function saveTCSBGrid(obj) {
    var testcasegrid = $(obj).attr("data-tabsave");
    var tcsbid = $(obj).attr("data-tcsbid");
    var suiteid = $(obj).attr("data-suiteid");
    var projectid = $(obj).attr("data-projectid");
    var tabname = $(obj).attr("data-tabname");

    var baseid = $(obj).attr("data-base");
    var compareid = $(obj).attr("data-compare");
    var testcaseid = $(obj).attr("data-testcaseid");
    var storyboarddetailid = $(obj).attr("data-storyboarddetailid");


    if (tabname == "TestCase") {
        $.when($.ajax(saveChangesTC(testcasegrid, tcsbid, suiteid))).then(function () {
            if (validflag == true || msgflag == true) {
                return false;
            }
            else {
                cancelTCSBGrid();
            }
        });
    }
    else if (tabname == "DataTag" && baseid != undefined && compareid != undefined) {
        var lresultset = ".grid" + "ResultView_BHistoryId_" + baseid + "_CHistoryId_" + compareid;
        $.when($.ajax(SaveSBResultData(lresultset, baseid, compareid, testcaseid, storyboarddetailid))).then(function () {
            if (validflag == true || msgflag == true) {
                return false;
            }
            else {
                cancelTCSBGrid();
            }
        });
    }
    else {
        $.when($.ajax(saveChanges(testcasegrid, projectid, tcsbid))).then(function () {
            if (sbvalidmsg == true)
                return false;
            else
                cancelTCSBGrid();

        });
    }
}
function closetab(tabcloseObj) {
    var lParent = $(tabcloseObj).parent();
    var lBeforeTab = $(lParent).prev();
    var testCaseIds = "";
    var lFindTab = $(lParent).children().first().attr("data-target");
    var targetname = $(lParent).children().first().attr("data-tab");
    var lfindname = $(lParent).children().first().attr("data-name");
    var lfindid = $(lParent).children().first().attr("data-id");
    var ltestcasegridname = targetname == "Storyboard" ? ".gridSB" + lfindname : ".grid" + lfindname;
    var suiteidsave = $("#hdnTestsuiteId").val();
    var projectidsave = $("#hdnSProjectId").val();
    projectidsave = projectidsave === undefined ? 0 : projectidsave;
    suiteidsave = suiteidsave === undefined ? 0 : suiteidsave;

    var baseid = $(lParent).children().first().attr("data-baseid");
    var compareid = $(lParent).children().first().attr("data-compareid");
    var storyboard = $(lParent).children().first().attr("data-storyboard");
    var runorder = $(lParent).children().first().attr("data-runorder");
    var tab = $(lParent).children().first().attr("data-tab");

    if (targetname.trim() == "Storyboard" || targetname.trim() == "TestCase") {
        var leftpenallist = document.getElementById("leftProjectList").querySelectorAll(".kt-menu__item--open");
        $.each(leftpenallist, function (key, value) {
            var obj = value.innerText;
            var classlst = value.classList;
            if (classlst[2].trim() == "context-menu-TestCase" || classlst[1].trim() == "context-menu-Storyboard") {
                lfindname = lfindname.replace(/_/g, ' ');
                obj = obj.replace(/_/g, ' ').trim();
                if (obj.includes(lfindname.trim())) {
                    value.classList.remove("kt-menu__item--open");
                }
            }
        });
    }

    var a = checkTCSBChangesOnClose(tabcloseObj);
    if (a == "unsaved") {
        //check save method work or not 
        return alertShow(ltestcasegridname, lfindname, lfindid, suiteidsave, projectidsave, targetname, tabcloseObj);
    }

    if ($(lParent).children().first().attr("data-tab") == "TestCase") {
        testCaseIds = testCaseIds + "," + $(lParent).children().first().attr("data-id");
    }
    var lTabNameId = lFindTab.replace("#", "").replace('(', '_').replace(')', '_');
    var lDirGrid = $(".divtablist #" + lTabNameId);
    if (ExistDataSetRenameList.length > 0) {
        ExistDataSetRenameList = jQuery.grep(ExistDataSetRenameList, function (value) {
            if (value != undefined) {
                return value["TestCaseId"] != lfindid;
            }
        });
    }
    if (DeleteColumnsList.length > 0) {
        DeleteColumnsList = jQuery.grep(DeleteColumnsList, function (value) {

            if (value != undefined) {
                return value["TestCaseId"] != lfindid;
            }

        });
    }
    $(lParent).remove();
    $(lDirGrid).remove();
    var lflag = true;
    $('.ULtablist li').each(function (index, value) {
        var lFindclass = $(value).children().first().attr("class");
        if (lFindclass.indexOf("active") > -1) {
            lflag = false;
        }
    });
    if (lflag) {
        var lPrevTab = $(lBeforeTab).children().first().attr("data-target");
        var lPrevTabNameId = lPrevTab.replace("#", "").replace('(', '_').replace(')', '_');
        var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
        $(lBeforeTab).children().first().addClass("active");
        $(lPrevDirGrid).addClass("active");
        var Activetab = $(lBeforeTab).children().first();
        var lTestCaseId = Activetab.attr("data-testcaseId");
        var lTestsuiteId = Activetab.attr("data-testsuiteId");
        var lProjectId = Activetab.attr("data-projectId");
        var lTestCaseName = Activetab.attr("data-testcasename");
        var storyboardid = Activetab.attr("data-storyboardid");
        var storyboardname = Activetab.attr("data-storyboardname");

        if (lTestCaseName != undefined) {
            var TestCaselst = document.getElementById("leftProjectList").querySelectorAll(".context-menu-TestCase");
            $.each(TestCaselst, function (key, value) {
                var obj = value.innerText;
                var classlst = value.classList;
                lTestCaseName = lTestCaseName.replace(/_/g, ' ');
                obj = obj.replace(/_/g, ' ').trim();
                if (obj.includes(lTestCaseName.trim())) {
                    value.classList.add("kt-menu__item--open");
                }
            });
        }
        if (storyboardname != undefined) {
            var Storyboardlst = document.getElementById("leftProjectList").querySelectorAll(".context-menu-Storyboard");
            $.each(Storyboardlst, function (key, value) {
                var obj = value.innerText;
                var classlst = value.classList;
                storyboardname = storyboardname.replace(/_/g, ' ');
                obj = obj.replace(/_/g, ' ').trim();
                if (obj.includes(storyboardname.trim())) {
                    value.classList.add("kt-menu__item--open");
                }
            });
        }

        if (tab == "DataTag" && baseid != undefined && compareid != undefined && storyboard != undefined && runorder != undefined) {
            var lresultset = "ResultView_BHistoryId_" + baseid + "_CHistoryId_" + compareid;
            setTimeout(function () {
                if (gridobj[".grid" + lresultset] != null)
                    gridobj[".grid" + lresultset].reset({ filter: true });
            }, 500);
        }
        if (lTestCaseId != null && lTestsuiteId != null && lProjectId != null && lTestCaseName != null
            && lTestCaseId > 0 && lTestsuiteId > 0 && lProjectId > 0) {
            setTimeout(function () {
                if (gridobj[".grid" + lTestCaseName]!=null)
                    gridobj[".grid" + lTestCaseName].reset({ filter: true });
            }, 500);
            //  PartialRightGrid(lTestCaseId, lTestsuiteId, lProjectId, lTestCaseName, Activetab, null);
        }
        if (storyboardid != null && lProjectId != null && storyboardname != null && storyboardname != undefined
            && storyboardid > 0 && lProjectId > 0) {
            setTimeout(function () {
                if (gridobj[".gridSB" + storyboardname.replace(/ /g, '_')]!=null)
                    gridobj[".gridSB" + storyboardname.replace(/ /g, '_')].reset({ filter: true });
            }, 500);
            //setTimeout(function () { gridobj[".grid" + storyboardname].reset({ filter: true }); }, 500);
            //   PartialRightStoryboardGrid(lProjectId, storyboardid, storyboardname, Activetab, null);
        }
    }
    $($.fn.dataTable.tables(true)).DataTable()
        .columns.adjust();
    //Activate previous tab
    SetCloseTabLog("1", lfindname);
    UpdateIsAvailableTestCase(testCaseIds);
}

function closetabforresultset(tabcloseObj) {
    var lParent = $(tabcloseObj).parent();

    var lFindTab = tabcloseObj.attr("data-target");
    var targetname = tabcloseObj.attr("data-tab");
    var lfindname = tabcloseObj.attr("data-name");
    var lfindid = tabcloseObj.attr("data-id");
    var ltestcasegridname = ".grid" + lfindname;

    var baseid = tabcloseObj.attr("data-baseid");
    var compareid = tabcloseObj.attr("data-compareid");
    var storyboard = tabcloseObj.attr("data-storyboard");
    var runorder = tabcloseObj.attr("data-runorder");
    var tab = tabcloseObj.attr("data-tab");

    var lTabNameId = lFindTab.replace("#", "").replace('(', '_').replace(')', '_');
    var lDirGrid = $(".divtablist #" + lTabNameId);

    $(lParent).remove();
    $(lDirGrid).remove();
}

function Closeallbutthistab(tabObject) {
    var ltabId = $(tabObject).attr("data-tab-id");
    var ltabName = $(tabObject).attr("data-tab-name");
    var lActiveTabObj;
    var lflag = false;
    var testCaseIds = "";

    $('.ULtablist li').each(function (index, value) {
        var lFindTab = $(value).children().first().attr("data-target");

        if (lFindTab != undefined) {
            var lFindTabId = $(value).children().first().attr("data-id");
            var lFindTabName = $(value).children().first().attr("data-name");
            if (lFindTabId == ltabId && lFindTabName == ltabName) {
                lActiveTabObj = value;
                lflag = true;
                var lPrevTab = $(value).children().first().attr("data-target");
                var lPrevTabNameId = lPrevTab.replace("#", "");
                var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
                $(value).children().first().addClass("active");
                $(lPrevDirGrid).addClass("active");

            } else {
                if ($(value).children().first().attr("data-tab") == "TestCase") {
                    tcidvar = $(value).children().first().attr("data-id");
                    testCaseIds = testCaseIds + "," + $(value).children().first().attr("data-id");
                    if (ExistDataSetRenameList.length > 0) {
                        ExistDataSetRenameList = jQuery.grep(ExistDataSetRenameList, function (value) {
                            if (value != undefined) {
                                return value["TestCaseId"] != tcidvar;
                            }

                        });
                    }
                    if (DeleteColumnsList.length > 0) {
                        DeleteColumnsList = jQuery.grep(DeleteColumnsList, function (value) {

                            if (value != undefined) {
                                return value["TestCaseId"] != tcidvar;
                            }

                        });
                    }
                }

                var lTabNameId = lFindTab.replace("#", "");
                var lDirGrid = $(".divtablist #" + lTabNameId);
                // $(value).html("");
                $(value).remove();
                // $(lDirGrid).html("");
                $(lDirGrid).remove();
            }
        }
    });

    if (lflag) {
        var lPrevTab = $(lActiveTabObj).children().first().attr("data-target");
        var lPrevTabNameId = lPrevTab.replace("#", "");
        var lPrevDirGrid = $(".divtablist #" + lPrevTabNameId);
        $(lActiveTabObj).children().first().addClass("active");
        $(lPrevDirGrid).addClass("active");
        var Activetab = $(lActiveTabObj).children().first();
        var lTestCaseId = Activetab.attr("data-testcaseId");
        var lTestsuiteId = Activetab.attr("data-testsuiteId");
        var lProjectId = Activetab.attr("data-projectId");
        var lTestCaseName = Activetab.attr("data-testcasename");
        var storyboardid = Activetab.attr("data-storyboardid");
        var storyboardname = Activetab.attr("data-storyboardname");

        if (lTestCaseId != null && lTestsuiteId != null && lProjectId != null && lTestCaseName != null
            && lTestCaseId > 0 && lTestsuiteId > 0 && lProjectId > 0) {
            setTimeout(function () { gridobj[".grid" + lTestCaseName].reset({ filter: true }); }, 500);
            //    PartialRightGrid(lTestCaseId, lTestsuiteId, lProjectId, lTestCaseName, Activetab, null);
        }
        if (storyboardid != null && lProjectId != null && storyboardname != null
            && storyboardid > 0 && lProjectId > 0) {
            setTimeout(function () { gridobj[".grid" + storyboardname].reset({ filter: true }); }, 500);
            //   PartialRightStoryboardGrid(lProjectId, storyboardid, storyboardname, Activetab, null);
        }
    }
    SetCloseTabLog("2", ltabName);
    UpdateIsAvailableTestCase(testCaseIds);
}

function CloseAll(tabObject) {
    var testCaseIds = "";
    ExistDataSetRenameList = [];
    DeleteColumnsList = [];
    $('.ULtablist li').each(function (index, value) {
        var lFindTab = $(value).children().first().attr("data-target");
        if (lFindTab != undefined) {
            if ($(value).children().first().attr("data-tab") == "TestCase") {
                testCaseIds = testCaseIds + "," + $(value).children().first().attr("data-id");
            }
            var lTabNameId = lFindTab.replace("#", "");
            var lDirGrid = $(".divtablist #" + lTabNameId);
            $(value).remove();
            $(lDirGrid).remove();
        }
    });
    SetCloseTabLog("3", "");
    UpdateIsAvailableTestCase(testCaseIds);
}

function closeExistOldtab(tabcloseObj) {
    var testCaseIds = "";
    var lFindTab = $(tabcloseObj).children().first().attr("data-target");
    var suiteidsave = $("#hdnTestsuiteId").val();
    var projectidsave = $("#hdnSProjectId").val();
    projectidsave = projectidsave === undefined ? 0 : projectidsave;
    suiteidsave = suiteidsave === undefined ? 0 : suiteidsave;

    if ($(tabcloseObj).children().first().attr("data-tab") == "TestCase") {
        testCaseIds = testCaseIds + "," + $(lParent).children().first().attr("data-id");
    }
    var lTabNameId = lFindTab.replace("#", "").replace('(', '_').replace(')', '_');
    var lDirGrid = $(".divtablist #" + lTabNameId);

    $(tabcloseObj).remove();
    $(lDirGrid).remove();

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

function PinTab(tabObject) {
    startloader();
    var linkText = tabObject[0].innerText.trim();
    var ltabId = $(tabObject).attr("data-tab-id");
    var ltabName = $(tabObject).attr("data-tab-name");
    var lActiveTabObj;
    $('.ULtablist li').each(function (index, value) {
        var lFindTab = $(value).children().first().attr("data-target");
        if (lFindTab != undefined) {
            var lFindTabId = $(value).children().first().attr("data-id");
            var lFindTabName = $(value).children().first().attr("data-name");
            if (lFindTabId == ltabId && lFindTabName == ltabName) {
                lActiveTabObj = value;
            }
        }
    });

    var Activetab = $(lActiveTabObj).children().first();
    var datatab = Activetab.attr("data-tab");
    var dataid = Activetab.attr("data-id");
    var dataname = Activetab.attr("data-name");
    var ProjectId = Activetab.attr("data-projectId") == undefined ? "0" : Activetab.attr("data-projectId");

    $.ajax({
        url: "/Accounts/UserPinUnPinTab",
        type: "POST",
        data: JSON.stringify({ "datatab": datatab, "dataid": dataid, "dataname": dataname, "linkText": linkText, "ProjectId": ProjectId }),
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            stoploader();
            if (result.status == 1) {
                if (result.data.includes("Successfully AddPin")) {
                    //tabObject[0].innerHTML = '<span class="kt-menu__link-icon"><i class="fa fa-map-pin"></i></span><span class="kt-menu__link-text"> UnPin Tab</span>';
                    //$('.appactivepintab').show();
                }
                else if (result.data.includes("Successfully Remove Pin")) {
                    //tabObject[0].innerHTML = '<span class="kt-menu__link-icon"><i class="fa fa-map-pin"></i></span><span class="kt-menu__link-text"> Pin Tab</span>';
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
        },
    });
}
function MARSEnginePathMapping() {
    alert($("#hdnUserId").val());
    return false;
    //@SessionManager.TESTER_LOGIN_NAM

}


function UpdateIsAvailableTestCase(TestCases) {
    if (TestCases !== "") {
        $.ajax({
            url: "/TestCase/UpdateIsAvailable",
            data: '{"TestCaseIds":"' + TestCases + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "JSON",
            success: function (result) {
                if (result.status == 0) {
                    swal.fire({
                        "title": "",
                        "text": result.message,
                        "icon": "error",
                        "onClose": function (e) {
                            console.log('on close event fired!');
                        }
                    });
                }
            }
        });
    }
}

function SetCloseTabLog(flag, tabname) {
    if (flag !== "") {
        $.ajax({
            url: "/TestCase/SetCloseTabLog",
            data: '{"flag":"' + flag + '", "tabname":"' + tabname + '"}',
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "JSON",
            success: function (result) {
            }
        });
    }
}

function PartialRightImportResultSet(Default, obj) {
    var projectId = 0;
    if (Default == "0")
        startloader();
    if (obj != null)
        projectId = obj[0].dataset.projectId;

    $.ajax({
        url: "/Storyboard/ImportResultSet",
        type: "POST",
        data: '{"projectId":"' + projectId + '"}',
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabimportresultset") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-id="0" data-toggle="tab" href="#" data-tab="ImportResultSet" data-name="ImportResultSet" data-target="#tabimportresultset" onclick="ActiveTab($(this))"><img alt="ResultSet Import" class="tab_icons_img" src="/assets/media/icons/iRS.png"/>Import ResultSet</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-id="0" data-toggle="tab" href="#" data-tab="ImportResultSet" data-name="ImportResultSet" data-target="#tabimportresultset" onclick="ActiveTab($(this))"><img alt="ResultSet Import" class="tab_icons_img" src="/assets/media/icons/iRS.png"/>Import ResultSet</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))"></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabimportresultset" role="tabpanel">' + result + '</div>';

            if (lflag) {
                if (projectId == 0)
                    $('#DrpIProject').val('');
                else
                    $('#DrpIProject').val(projectId.toString());

                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabimportresultset") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabimportresultset") {
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
                    if ($(value).children().first().attr("data-target") != "#tabimportresultset") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabimportresultset") {
                        $(value).removeClass("active");
                    }
                });
            }
            stoploader();
        },
    });
}

function RightSideUserRoleMappingList(Default) {
    $.ajax({
        url: "/Entitlement/UserRoleMappingList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabuserrolemappinglist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserRoleMappingList" data-id="0" data-name="UserRoleMappingList" data-toggle="tab" href="#" data-target="#tabuserrolemappinglist" onclick="ActiveTab($(this))"><img alt="User Role Mapping List" class="tab_icons_img" src="/assets/media/icons/URM.png"/>User Role Mapping List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserRoleMappingList" data-id="0" data-name="UserRoleMappingList" data-toggle="tab" href="#" data-target="#tabuserrolemappinglist" onclick="ActiveTab($(this))"><img alt="User Role Mapping List" class="tab_icons_img" src="/assets/media/icons/URM.png"/>User Role Mapping List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuserrolemappinglist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabuserrolemappinglist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabuserrolemappinglist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabuserrolemappinglist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuserrolemappinglist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSideGroupList(Default) {
    $.ajax({
        url: "/TestCase/GroupList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabgrouplist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="Group" data-id="0" data-name="Group" data-toggle="tab" href="#" data-target="#tabgrouplist" onclick="ActiveTab($(this))"><img alt="Group" class="tab_icons_img" src="/assets/media/icons/GL.png"/>Group List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="Group" data-id="0" data-name="Group" data-toggle="tab" href="#" data-target="#tabgrouplist" onclick="ActiveTab($(this))"><img alt="Group" class="tab_icons_img" src="/assets/media/icons/GL.png"/>Group List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabgrouplist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabgrouplist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabgrouplist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabgrouplist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabgrouplist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSideSetList(Default) {
    $.ajax({
        url: "/TestCase/SetList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabsetlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="Set" data-id="0" data-name="Set" data-toggle="tab" href="#" data-target="#tabsetlist" onclick="ActiveTab($(this))"><img alt="Set" class="tab_icons_img" src="/assets/media/icons/SL.png"/>Set List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="Set" data-id="0" data-name="Set" data-toggle="tab" href="#" data-target="#tabsetlist" onclick="ActiveTab($(this))"><img alt="Set" class="tab_icons_img" src="/assets/media/icons/SL.png"/>Set List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabsetlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabsetlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabsetlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabsetlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabsetlist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSideFolderList(Default) {
    $.ajax({
        url: "/TestCase/FolderList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabfolderlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="Folder" data-id="0" data-name="Folder" data-toggle="tab" href="#" data-target="#tabfolderlist" onclick="ActiveTab($(this))"><img alt="Folder" class="tab_icons_img" src="/assets/media/icons/FL.png"/>Folder List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="Folder" data-id="0" data-name="Folder" data-toggle="tab" href="#" data-target="#tabfolderlist" onclick="ActiveTab($(this))"><img alt="Folder" class="tab_icons_img" src="/assets/media/icons/FL.png"/>Folder List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabfolderlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabfolderlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabfolderlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabfolderlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabfolderlist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSideUserConfigList(Default) {
    $.ajax({
        url: "/Accounts/UserConfigList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabuserconfiglist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="UserConfiguration" data-id="0" data-name="UserConfiguration" data-toggle="tab" href="#" data-target="#tabuserconfiglist" onclick="ActiveTab($(this))"><img alt="User Configuration" class="tab_icons_img" src="/assets/media/icons/UCL.png"/>User Configuration List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="UserConfiguration" data-id="0" data-name="UserConfiguration" data-toggle="tab" href="#" data-target="#tabuserconfiglist" onclick="ActiveTab($(this))"><img alt="User Configuration" class="tab_icons_img" src="/assets/media/icons/UCL.png"/>User Configuration List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabuserconfiglist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabuserconfiglist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabuserconfiglist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabuserconfiglist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabuserconfiglist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}

function RightSideFilterList(Default) {
    $.ajax({
        url: "/TestCase/FilterList",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            var lflag = false;
            $('.ULtablist li').each(function (index, value) {
                if ($(value).children().first().attr("data-target") == "#tabfilterlist") {
                    lflag = true;
                }
            });
            var ltab = "";
            if (Default == "1") {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="true" class="nav-link active context-tab" data-tab="Filter" data-id="0" data-name="Filter" data-toggle="tab" href="#" data-target="#tabfilterlist" onclick="ActiveTab($(this))"><img alt="Filter" class="tab_icons_img" src="/assets/media/icons/FL.png"/>Filter List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            } else {
                ltab = '<li class="nav-item context-menu-tab"><a data-pin="false" class="nav-link active context-tab" data-tab="Filter" data-id="0" data-name="Filter" data-toggle="tab" href="#" data-target="#tabfilterlist" onclick="ActiveTab($(this))"><img alt="Filter" class="tab_icons_img" src="/assets/media/icons/FL.png"/>Filter List</a><i class="fa fa-times-circle tab_close" style="cursor:pointer" onclick="closetab($(this))" ></i></li>';
            }
            var ldiv = '<div class="tab-pane active div" id="tabfilterlist" role="tabpanel">' + result + '</div>';

            if (lflag) {
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("data-target") == "#tabfilterlist") {
                        $(value).children().first().addClass("active");
                    } else {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") == "tabfilterlist") {
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
                    if ($(value).children().first().attr("data-target") != "#tabfilterlist") {
                        $(value).children().first().removeClass("active");
                    }
                });
                $('.divtablist div').each(function (index, value) {
                    if ($(value).first().attr("id") != "tabfilterlist") {
                        $(value).removeClass("active");
                    }
                });
            }
        }
    });
}
