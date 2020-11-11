var GridTable = function () {
    $.fn.dataTable.Api.register('column().title()', function () {
        return $(this.header()).text().trim();
    });

    var initGridTable = function () {
        GridTable = $('#gridtable').DataTable({
            responsive: false,
            // Pagination settings
            dom: `<'row'<'col-sm-12'tr>>
                                                    <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>`,
            // read more: https://datatables.net/examples/basic_init/dom.html

            lengthMenu: [10, 100, 1000],
            processing: true,
            pageLength: 100,

            language: {
                'lengthMenu': 'Display _MENU_',
                "processing": "<img  src='../assets/media/mars.gif' style='width:80px;'/>"
            },
            "scrollY": $(window).height() - 305,
            searchDelay: 500,
            //  processing: true,
            serverSide: true,
            ajax: {
                url: '/ConfigurationGrid/DataLoadGridList',
                type: 'POST',
                data: {
                    // parameters for custom backend script demo
                    columnsDef: [
                        'GridName', 'Actions',],
                },
            },
            columns: [
                { "data": 'GridName', "name": "Grid Name", width: '60%', "targets": 0, autowidth: false },
                { data: 'Actions', responsivePriority: -1, width: '40%', "targets": 2, autowidth: false }
            ],
            initComplete: function () {
                var thisTable = this;
                var rowFilter = $('<tr class="filter"></tr>').appendTo($(GridTable.table().header()));

                GridTable.columns().every(function () {
                    var column = this;
                    var input;
                    switch (column.title()) {
                        case 'Grid Name':
                            input = $(`<input type="text" class="form-control form-control-sm form-filter kt-input" data-col-index="` + column.index() + `"/>`);
                            $(input).keyup(function () {
                                var params = {};
                                $(rowFilter).find('.kt-input').each(function () {
                                    var i = $(this).data('col-index');
                                    if (params[i]) {
                                        params[i] += '|' + $(this).val();
                                    }
                                    else {
                                        params[i] = $(this).val();
                                    }
                                });
                                $.each(params, function (i, val) {
                                    // apply search params to datatable
                                    GridTable.column(i).search(val ? val : '', false, false);
                                });
                                GridTable.table().draw();
                            });
                            break;
                    }

                    if (column.title() !== 'actions') {
                        $(input).appendTo($('<th>').appendTo(rowFilter));
                    }
                });
            },
            columnDefs: [
                {
                    targets: -1,
                    title: 'Actions',
                    orderable: false,
                    render: function (data, type, full, meta) {
                        var lid = full.GridId;
                        var lgirdname = full.GridName;
                        return " <a href='#' class='btn btn-sm btn-clean btn-icon btn-icon-md' title='View' data-girdname='" + lgirdname + "' data-gridid='" + lid + "'onclick=Editgrid(" + lid + ",$(this)); ><i class='la la-edit'></i></a> ";
                    },
                }
            ],
        });
    };
    return {
        init: function () {
            initGridTable();
        },
    };
}();
$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
    $($.fn.dataTable.tables(true)).DataTable()
        .columns.adjust();
});
jQuery(document).ready(function () {
    GridTable.init();
});

function isFloat(n) {
    return Number(n) === n && n % 1 !== 0;
}

function Editgrid(Id, obj) {
    startloader();
    // application
    $("#appNameError").css("display", "none");
    $("#appDescriptionError").css("display", "none");
    $("#appVersionError").css("display", "none");
    $("#appExtraError").css("display", "none");
    $("#appModeError").css("display", "none");
    $("#appExplorerBitError").css("display", "none");
    $("#appActionsError").css("display", "none");
    $("#apperror").css("display", "none");

    //project
    $("#proNameError").css("display", "none");
    $("#proDescriptionError").css("display", "none");
    $("#proApplicationError").css("display", "none");
    $("#proStatusError").css("display", "none");
    $("#proActionsError").css("display", "none");
    $("#proerror").css("display", "none");

    //keyword
    $("#keyNameError").css("display", "none");
    $("#keyControlTypeError").css("display", "none");
    $("#keyEntryError").css("display", "none");
    $("#keyActionsError").css("display", "none");
    $("#keyerror").css("display", "none");

    //Test Suite
    $("#TSNameError").css("display", "none");
    $("#TSDescriptionError").css("display", "none");
    $("#TSApplicationError").css("display", "none");
    $("#TSProjectError").css("display", "none");
    $("#TSActionsError").css("display", "none");
    $("#TSerror").css("display", "none");

    //Test Case
    $("#TCNameError").css("display", "none");
    $("#TCDescriptionError").css("display", "none");
    $("#TCApplicationError").css("display", "none");
    $("#TCTestSuiteError").css("display", "none");
    $("#TCActionsError").css("display", "none");
    $("#TCerror").css("display", "none");

    // object
    $("#objNameError").css("display", "none");
    $("#objInternalAccessError").css("display", "none");
    $("#objTypeError").css("display", "none");
    $("#objPegwindowError").css("display", "none");
    $("#objActionsError").css("display", "none");
    $("#objSelectError").css("display", "none");
    $("#objerror").css("display", "none");

    //Varible
    $("#VarNameError").css("display", "none");
    $("#VarTypeError").css("display", "none");
    $("#VarValueError").css("display", "none");
    $("#VarStatusError").css("display", "none");
    $("#VarActionsError").css("display", "none");
    $("#Varerror").css("display", "none");

    //Users
    $("#userFNameError").css("display", "none");
    $("#userMNameError").css("display", "none");
    $("#userLNameError").css("display", "none");
    $("#userNameError").css("display", "none");
    $("#userEmailError").css("display", "none");
    $("#userComError").css("display", "none");
    $("#userStatusError").css("display", "none");
    $("#userActionsError").css("display", "none");
    $("#usererror").css("display", "none");

    //Testcase pqgrid
    $("#TCPKeywordError").css("display", "none");
    $("#TCPObjectError").css("display", "none");
    $("#TCPParametersError").css("display", "none");
    $("#TCPCommentError").css("display", "none");
    $("#TCPDataSetsError").css("display", "none");

    //Storyboard pqgrid
    $("#SPActionError").css("display", "none");
    $("#SPStepsError").css("display", "none");
    $("#SPTestSuiteError").css("display", "none");
    $("#SPTestCaseError").css("display", "none");
    $("#SPDatasetError").css("display", "none");
    $("#SPBaseResultError").css("display", "none");
    $("#SPBaseErrorCauseError").css("display", "none");
    $("#SPBaseScriptStartsError").css("display", "none");
    $("#SPBaseScriptDurationError").css("display", "none");
    $("#SPComResultError").css("display", "none");
    $("#SPComErrorCauseError").css("display", "none");
    $("#SPComScriptStartError").css("display", "none");
    $("#SPComScriptDurationError").css("display", "none");
    $("#SPDependencyError").css("display", "none");
    $("#SPDescriptionError").css("display", "none");

    $("#RError").css("display", "none");

    var GridName = obj[0].dataset.girdname;
    $.ajax({
        url: "/ConfigurationGrid/GetGridbyId",
        data: JSON.stringify({ "Id": Id, "gridName": GridName }),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (GridName.toLowerCase().trim() == "application list") {
                var appdata = JSON.parse(result);
                $("#appNamewidth").val(appdata.Name == null ? "20" : appdata.Name);
                $("#appDescriptionwidth").val(appdata.Description == null ? "20" : appdata.Description);
                $("#appVersionwidth").val(appdata.Version == null ? "10" : appdata.Version);
                $("#appExtrawidth").val(appdata.ExtraRequirement == null ? "20" : appdata.ExtraRequirement);
                $("#appModewidth").val(appdata.Mode == null ? "10" : appdata.Mode);
                $("#appExplorerBitwidth").val(appdata.ExplorerBits == null ? "10" : appdata.ExplorerBits);
                $("#appActionswidth").val(appdata.Actions == null ? "10" : appdata.Actions);

                $("#appNameid").val(appdata.NameId);
                $("#appDescriptionid").val(appdata.DescriptionId);
                $("#appVersionid").val(appdata.VersionId);
                $("#appExtraid").val(appdata.ExtraRequirementId);
                $("#appModeid").val(appdata.ModeId);
                $("#appExplorerBitid").val(appdata.ExplorerBitsId);
                $("#appActionsid").val(appdata.ActionsId);
                $("#appGridid").val(appdata.GridId);

                stoploader();
                $('#ResizeApplicationgrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "project list") {
                var prodata = JSON.parse(result);
                $("#proNamewidth").val(prodata.Name == null ? "20" : prodata.Name);
                $("#proDescriptionwidth").val(prodata.Description == null ? "25" : prodata.Description);
                $("#proApplicationwidth").val(prodata.Application == null ? "25" : prodata.Application);
                $("#proStatuswidth").val(prodata.Status == null ? "20" : prodata.Status);
                $("#proActionswidth").val(prodata.Actions == null ? "10" : prodata.Actions);

                $("#proNameid").val(prodata.NameId);
                $("#proDescriptionid").val(prodata.DescriptionId);
                $("#proApplicationid").val(prodata.ApplicationId);
                $("#proStatusid").val(prodata.StatusId);
                $("#proActionsid").val(prodata.ActionsId);
                $("#proGridid").val(prodata.GridId);

                stoploader();
                $('#ResizeProjectgrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "keyword list") {
                var keydata = JSON.parse(result);
                $("#keyNamewidth").val(keydata.Name == null ? "30" : keydata.Name);
                $("#keyControlTypewidth").val(keydata.ControlType == null ? "40" : keydata.ControlType);
                $("#keyEntrywidth").val(keydata.EntryData == null ? "20" : keydata.EntryData);
                $("#keyActionswidth").val(keydata.Actions == null ? "10" : keydata.Actions);

                $("#keyNameid").val(keydata.NameId);
                $("#keyControlTypeid").val(keydata.ControlTypeId);
                $("#keyEntryid").val(keydata.EntryDataId);
                $("#keyActionsid").val(keydata.ActionsId);
                $("#keyGridid").val(keydata.GridId);

                stoploader();
                $('#ResizeKeywordgrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "test suite list") {
                var TSdata = JSON.parse(result);
                $("#TSNamewidth").val(TSdata.Name == null ? "20" : TSdata.Name);
                $("#TSDescriptionwidth").val(TSdata.Description == null ? "20" : TSdata.Description);
                $("#TSApplicationwidth").val(TSdata.Application == null ? "20" : TSdata.Application);
                $("#TSProjectwidth").val(TSdata.Project == null ? "30" : TSdata.Project);
                $("#TSActionswidth").val(TSdata.Actions == null ? "10" : TSdata.Actions);

                $("#TSNameid").val(TSdata.NameId);
                $("#TSDescriptionid").val(TSdata.DescriptionId);
                $("#TSApplicationid").val(TSdata.ApplicationId);
                $("#TSProjectid").val(TSdata.ProjectId);
                $("#TSActionsid").val(TSdata.ActionsId);
                $("#TSGridid").val(TSdata.GridId);

                stoploader();
                $('#ResizeTestSuitegrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "test case list") {
                var TCdata = JSON.parse(result);
                $("#TCNamewidth").val(TCdata.Name == null ? "20" : TCdata.Name);
                $("#TCDescriptionwidth").val(TCdata.Description == null ? "30" : TCdata.Description);
                $("#TCApplicationwidth").val(TCdata.Application == null ? "20" : TCdata.Application);
                $("#TCTestSuitewidth").val(TCdata.TestSuite == null ? "20" : TCdata.TestSuite);
                $("#TCActionswidth").val(TCdata.Actions == null ? "10" : TCdata.Actions);

                $("#TCNameid").val(TCdata.NameId);
                $("#TCDescriptionid").val(TCdata.DescriptionId);
                $("#TCApplicationid").val(TCdata.ApplicationId);
                $("#TCTestSuiteid").val(TCdata.TestSuiteId);
                $("#TCActionsid").val(TCdata.ActionsId);
                $("#TCGridid").val(TCdata.GridId);

                stoploader();
                $('#ResizeTestCasegrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "object list") {
                var Objdata = JSON.parse(result);
                $("#objNamewidth").val(Objdata.Name == null ? "20" : Objdata.Name);
                $("#objInternalAccesswidth").val(Objdata.InternalAccess == null ? "30" : Objdata.InternalAccess);
                $("#objTypewidth").val(Objdata.Type == null ? "10" : Objdata.Type);
                $("#objPegwindowwidth").val(Objdata.Pegwindow == null ? "20" : Objdata.Pegwindow);
                $("#objActionswidth").val(Objdata.Actions == null ? "10" : Objdata.Actions);
                $("#objSelectwidth").val(Objdata.Select == null ? "10" : Objdata.Select);

                $("#objNameid").val(Objdata.NameId);
                $("#objInternalAccessid").val(Objdata.InternalAccessId);
                $("#objTypeid").val(Objdata.TypeId);
                $("#objPegwindowid").val(Objdata.PegwindowId);
                $("#objActionsid").val(Objdata.ActionsId);
                $("#objSelectid").val(Objdata.SelectId);
                $("#objGridid").val(Objdata.GridId);

                stoploader();
                $('#ResizeObjectgrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "variable list") {
                var Vardata = JSON.parse(result);
                $("#VarNamewidth").val(Vardata.Name == null ? "30" : Vardata.Name);
                $("#VarTypewidth").val(Vardata.Type == null ? "20" : Vardata.Type);
                $("#VarValuewidth").val(Vardata.Value == null ? "20" : Vardata.Value);
                $("#VarStatuswidth").val(Vardata.Status == null ? "20" : Vardata.Status);
                $("#VarActionswidth").val(Vardata.Actions == null ? "10" : Vardata.Actions);

                $("#VarNameid").val(Vardata.NameId);
                $("#VarTypeid").val(Vardata.TypeId);
                $("#VarValueid").val(Vardata.ValueId);
                $("#VarStatusid").val(Vardata.StatusId);
                $("#VarActionsid").val(Vardata.ActionsId);
                $("#VarGridid").val(Vardata.GridId);

                stoploader();
                $('#ResizeVariablegrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "user list") {
                var Userdata = JSON.parse(result);
                $("#userFNamewidth").val(Userdata.FName == null ? "10" : Userdata.FName);
                $("#userMNamewidth").val(Userdata.MName == null ? "10" : Userdata.MName);
                $("#userLNamewidth").val(Userdata.LName == null ? "10" : Userdata.LName);
                $("#userNamewidth").val(Userdata.Name == null ? "10" : Userdata.Name);
                $("#userEmailwidth").val(Userdata.Email == null ? "20" : Userdata.Email);
                $("#userComwidth").val(Userdata.Company == null ? "20" : Userdata.Company);
                $("#userStatuswidth").val(Userdata.Status == null ? "10" : Userdata.Status);
                $("#userActionswidth").val(Userdata.Actions == null ? "10" : Userdata.Actions);

                $("#userFNameid").val(Userdata.FNameId);
                $("#userMNameid").val(Userdata.MNameId);
                $("#userLNameid").val(Userdata.LNameId);
                $("#userNameid").val(Userdata.NameId);
                $("#userEmailid").val(Userdata.EmailId);
                $("#userComid").val(Userdata.CompanyId);
                $("#userStatusid").val(Userdata.StatusId);
                $("#userActionsid").val(Userdata.ActionsId);
                $("#userGridid").val(Userdata.GridId);

                stoploader();
                $('#ResizeUsergrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "test case page") {
                var Testcasedata = JSON.parse(result);
                $("#TCPKeywordwidth").val(Testcasedata.Keyword == null ? "200" : Testcasedata.Keyword);
                $("#TCPObjectwidth").val(Testcasedata.Object == null ? "200" : Testcasedata.Object);
                $("#TCPParameterswidth").val(Testcasedata.Parameters == null ? "100" : Testcasedata.Parameters);
                $("#TCPCommentwidth").val(Testcasedata.Comment == null ? "100" : Testcasedata.Comment);

                $("#TCPKeywordid").val(Testcasedata.KeywordId);
                $("#TCPObjectid").val(Testcasedata.ObjectId);
                $("#TCPParametersid").val(Testcasedata.ParametersId);
                $("#TCPCommentid").val(Testcasedata.CommentId);
                $("#TCPGridid").val(Testcasedata.GridId);

                stoploader();
                $('#ResizeTestCasePagegrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "storyboard page") {
                var Sdata = JSON.parse(result);
                $("#SPActionwidth").val(Sdata.Action == null ? "70" : Sdata.Action);
                $("#SPStepswidth").val(Sdata.Steps == null ? "100" : Sdata.Steps);
                $("#SPTestSuitewidth").val(Sdata.TestSuite == null ? "180" : Sdata.TestSuite);
                $("#SPTestCasewidth").val(Sdata.TestCase == null ? "180" : Sdata.TestCase);
                $("#SPDatasetwidth").val(Sdata.Dataset == null ? "180" : Sdata.Dataset);
                $("#SPBaseResultwidth").val(Sdata.BResult == null ? "75" : Sdata.BResult);
                $("#SPBaseErrorCausewidth").val(Sdata.BErrorCause == null ? "75" : Sdata.BErrorCause);
                $("#SPBaseScriptStartswidth").val(Sdata.BScriptStart == null ? "75" : Sdata.BScriptStart);
                $("#SPBaseScriptDurationwidth").val(Sdata.BScriptDuration == null ? "75" : Sdata.BScriptDuration);
                $("#SPComResultwidth").val(Sdata.CResult == null ? "75" : Sdata.CResult);
                $("#SPComErrorCausewidth").val(Sdata.CErrorCause == null ? "75" : Sdata.CErrorCause);
                $("#SPComScriptStartwidth").val(Sdata.CScriptStart == null ? "75" : Sdata.CScriptStart);
                $("#SPComScriptDurationwidth").val(Sdata.CScriptDuration == null ? "75" : Sdata.CScriptDuration);
                $("#SPDependencywidth").val(Sdata.Dependency == null ? "50" : Sdata.Dependency);
                $("#SPDescriptionwidth").val(Sdata.Description == null ? "100" : Sdata.Description);

                $("#SPActionid").val(Sdata.ActionId);
                $("#SPStepsid").val(Sdata.StepsId);
                $("#SPTestSuiteid").val(Sdata.TestSuiteId);
                $("#SPTestCaseid").val(Sdata.TestCaseId);
                $("#SPDatasetid").val(Sdata.DatasetId);
                $("#SPBaseResultid").val(Sdata.BResultId);
                $("#SPBaseErrorCauseid").val(Sdata.BErrorCauseId);
                $("#SPBaseScriptStartsid").val(Sdata.BScriptStartId);
                $("#SPBaseScriptDurationsid").val(Sdata.BScriptDurationId);
                $("#SPComResultid").val(Sdata.CResultId);
                $("#SPComErrorCauseid").val(Sdata.CErrorCauseId);
                $("#SPComScriptStartid").val(Sdata.CScriptStartId);
                $("#SPComScriptDurationid").val(Sdata.CScriptDurationId);
                $("#SPDependencyid").val(Sdata.DependencyId);
                $("#SPDescriptionid").val(Sdata.DescriptionId);
                $("#SPGridid").val(Sdata.GridId);

                stoploader();
                $('#ResizeStoryboardPqgrid').modal('show');
            }
            else if (GridName.toLowerCase().trim() == "resize left panel") {
                var resizeval = JSON.parse(result);
                $("#resizeid").val(resizeval.Resize);

                $("#RLeftGridid").val(resizeval.ResizeId);
                $("#RGridid").val(resizeval.GridId);

                stoploader();
                $('#ResizeLeftPanel').modal('show');
            }
            else
                stoploader();
        }
    });
}

function AppGridWidthSave() {
    var CheckError = false;
    $("#appNameError").css("display", "none");
    $("#appDescriptionError").css("display", "none");
    $("#appVersionError").css("display", "none");
    $("#appExtraError").css("display", "none");
    $("#appModeError").css("display", "none");
    $("#appExplorerBitError").css("display", "none");
    $("#appActionsError").css("display", "none");
    $("#apperror").css("display", "none");

    $("#appGridthsave").prop("disabled", true);


    if (parseFloat($("#appNamewidth").val()) < 10 || $("#appNamewidth").val() == "" || isFloat(parseFloat($("#appNamewidth").val()))) {
        $("#appNameError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appDescriptionwidth").val()) < 10 || $("#appDescriptionwidth").val() == "" || isFloat(parseFloat($("#appDescriptionwidth").val()))) {
        $("#appDescriptionError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appVersionwidth").val()) < 10 || $("#appVersionwidth").val() == "" || isFloat(parseFloat($("#appVersionwidth").val()))) {
        $("#appVersionError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appExtrawidth").val()) < 10 || $("#appExtrawidth").val() == "" || isFloat(parseFloat($("#appExtrawidth").val()))) {
        $("#appExtraError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appModewidth").val()) < 10 || $("#appModewidth").val() == "" || isFloat(parseFloat($("#appModewidth").val()))) {
        $("#appModeError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appExplorerBitwidth").val()) < 10 || $("#appExplorerBitwidth").val() == "" || isFloat(parseFloat($("#appExplorerBitwidth").val()))) {
        $("#appExplorerBitError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#appActionswidth").val()) < 10 || $("#appActionswidth").val() == "" || isFloat(parseFloat($("#appActionswidth").val()))) {
        $("#appActionsError").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#appNamewidth").val())) + Number(parseFloat($("#appDescriptionwidth").val())) +
        Number(parseFloat($("#appVersionwidth").val())) + Number(parseFloat($("#appExtrawidth").val())) +
        Number(parseFloat($("#appModewidth").val())) + Number(parseFloat($("#appExplorerBitwidth").val())) +
        Number(parseFloat($("#appActionswidth").val()));

    if (sum != 100) {
        $("#apperror").css("display", "block");
        $("#appGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var appGridWidth = {};
    appGridWidth.GridId = $("#appGridid").val(),
        appGridWidth.NameId = $("#appNameid").val(),
        appGridWidth.Name = $("#appNamewidth").val(),
        appGridWidth.DescriptionId = $("#appDescriptionid").val(),
        appGridWidth.Description = $("#appDescriptionwidth").val(),
        appGridWidth.VersionId = $("#appVersionid").val(),
        appGridWidth.Version = $("#appVersionwidth").val(),
        appGridWidth.ExtraRequirementId = $("#appExtraid").val(),
        appGridWidth.ExtraRequirement = $("#appExtrawidth").val(),
        appGridWidth.ModeId = $("#appModeid").val(),
        appGridWidth.Mode = $("#appModewidth").val(),
        appGridWidth.ExplorerBitsId = $("#appExplorerBitid").val(),
        appGridWidth.ExplorerBits = $("#appExplorerBitwidth").val(),
        appGridWidth.ActionsId = $("#appActionsid").val(),
        appGridWidth.Actions = $("#appActionswidth").val(),

        $('#ResizeApplicationgrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveAppGridWidth",
        data: JSON.stringify(appGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Application Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#appGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function proGridWidthSave() {
    var CheckError = false;
    $("#proNameError").css("display", "none");
    $("#proDescriptionError").css("display", "none");
    $("#proApplicationError").css("display", "none");
    $("#proStatusError").css("display", "none");
    $("#proActionsError").css("display", "none");
    $("#proerror").css("display", "none");

    $("#proGridthsave").prop("disabled", true);


    if (parseFloat($("#proNamewidth").val()) < 10 || $("#proNamewidth").val() == "" || isFloat(parseFloat($("#proNamewidth").val()))) {
        $("#proNameError").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#proDescriptionwidth").val()) < 10 || $("#proDescriptionwidth").val() == "" || isFloat(parseFloat($("#proDescriptionwidth").val()))) {
        $("#proDescriptionError").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#proApplicationwidth").val()) < 10 || $("#proApplicationwidth").val() == "" || isFloat(parseFloat($("#proApplicationwidth").val()))) {
        $("#proApplicationError").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#proStatuswidth").val()) < 10 || $("#proStatuswidth").val() == "" || isFloat(parseFloat($("#proStatuswidth").val()))) {
        $("#proStatusError").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#proActionswidth").val()) < 10 || $("#proActionswidth").val() == "" || isFloat(parseFloat($("#proActionswidth").val()))) {
        $("#proActionsError").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#proNamewidth").val())) + Number(parseFloat($("#proDescriptionwidth").val())) +
        Number(parseFloat($("#proApplicationwidth").val())) + Number(parseFloat($("#proStatuswidth").val())) +
        Number(parseFloat($("#proActionswidth").val()));

    if (sum != 100) {
        $("#proerror").css("display", "block");
        $("#proGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var proGridWidth = {};
    proGridWidth.GridId = $("#proGridid").val(),
        proGridWidth.NameId = $("#proNameid").val(),
        proGridWidth.Name = $("#proNamewidth").val(),
        proGridWidth.DescriptionId = $("#proDescriptionid").val(),
        proGridWidth.Description = $("#proDescriptionwidth").val(),
        proGridWidth.ApplicationId = $("#proApplicationid").val(),
        proGridWidth.Application = $("#proApplicationwidth").val(),
        proGridWidth.StatusId = $("#proStatusid").val(),
        proGridWidth.Status = $("#proStatuswidth").val(),
        proGridWidth.ActionsId = $("#proActionsid").val(),
        proGridWidth.Actions = $("#proActionswidth").val(),

        $('#ResizeProjectgrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveProjectGridWidth",
        data: JSON.stringify(proGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Project Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#proGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function keyGridWidthSave() {
    var CheckError = false;
    $("#keyNameError").css("display", "none");
    $("#keyControlTypeError").css("display", "none");
    $("#keyEntryError").css("display", "none");
    $("#keyActionsError").css("display", "none");
    $("#keyerror").css("display", "none");

    $("#keyGridthsave").prop("disabled", true);


    if (parseFloat($("#keyNamewidth").val()) < 10 || $("#keyNamewidth").val() == "" || isFloat(parseFloat($("#keyNamewidth").val()))) {
        $("#keyNameError").css("display", "block");
        $("#keyGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#keyControlTypewidth").val()) < 10 || $("#keyControlTypewidth").val() == "" || isFloat(parseFloat($("#keyControlTypewidth").val()))) {
        $("#keyControlTypeError").css("display", "block");
        $("#keyGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#keyEntrywidth").val()) < 10 || $("#keyEntrywidth").val() == "" || isFloat(parseFloat($("#keyEntrywidth").val()))) {
        $("#keyEntryError").css("display", "block");
        $("#keyGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#keyActionswidth").val()) < 10 || $("#keyActionswidth").val() == "" || isFloat(parseFloat($("#keyActionswidth").val()))) {
        $("#keyActionsError").css("display", "block");
        $("#keyGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#keyNamewidth").val())) + Number(parseFloat($("#keyControlTypewidth").val())) +
        Number(parseFloat($("#keyEntrywidth").val())) + Number(parseFloat($("#keyActionswidth").val()));

    if (sum != 100) {
        $("#keyerror").css("display", "block");
        $("#keyGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var keyGridWidth = {};
    keyGridWidth.GridId = $("#keyGridid").val(),
        keyGridWidth.NameId = $("#keyNameid").val(),
        keyGridWidth.Name = $("#keyNamewidth").val(),
        keyGridWidth.ControlTypeId = $("#keyControlTypeid").val(),
        keyGridWidth.ControlType = $("#keyControlTypewidth").val(),
        keyGridWidth.EntryDataId = $("#keyEntryid").val(),
        keyGridWidth.EntryData = $("#keyEntrywidth").val(),
        keyGridWidth.ActionsId = $("#keyActionsid").val(),
        keyGridWidth.Actions = $("#keyActionswidth").val(),

        $('#ResizeKeywordgrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveKeywordGridWidth",
        data: JSON.stringify(keyGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Keyword Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#keyGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function TSGridWidthSave() {
    var CheckError = false;
    $("#TSNameError").css("display", "none");
    $("#TSDescriptionError").css("display", "none");
    $("#TSApplicationError").css("display", "none");
    $("#TSProjectError").css("display", "none");
    $("#TSActionsError").css("display", "none");
    $("#TSerror").css("display", "none");

    $("#TSGridthsave").prop("disabled", true);

    if (parseFloat($("#TSNamewidth").val()) < 10 || $("#TSNamewidth").val() == "" || isFloat(parseFloat($("#TSNamewidth").val()))) {
        $("#TSNameError").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TSDescriptionwidth").val()) < 10 || $("#TSDescriptionwidth").val() == "" || isFloat(parseFloat($("#TSDescriptionwidth").val()))) {
        $("#TSDescriptionError").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TSApplicationwidth").val()) < 10 || $("#TSApplicationwidth").val() == "" || isFloat(parseFloat($("#TSApplicationwidth").val()))) {
        $("#TSApplicationError").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TSProjectwidth").val()) < 10 || $("#TSProjectwidth").val() == "" || isFloat(parseFloat($("#TSProjectwidth").val()))) {
        $("#TSProjectError").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TSActionswidth").val()) < 10 || $("#TSActionswidth").val() == "" || isFloat(parseFloat($("#TSActionswidth").val()))) {
        $("#TSActionsError").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#TSNamewidth").val())) + Number(parseFloat($("#TSDescriptionwidth").val())) +
        Number(parseFloat($("#TSApplicationwidth").val())) + Number(parseFloat($("#TSProjectwidth").val())) +
        Number(parseFloat($("#TSActionswidth").val()));

    if (sum != 100) {
        $("#TSerror").css("display", "block");
        $("#TSGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var TsGridWidth = {};
    TsGridWidth.GridId = $("#TSGridid").val(),
        TsGridWidth.NameId = $("#TSNameid").val(),
        TsGridWidth.Name = $("#TSNamewidth").val(),
        TsGridWidth.DescriptionId = $("#TSDescriptionid").val(),
        TsGridWidth.Description = $("#TSDescriptionwidth").val(),
        TsGridWidth.ApplicationId = $("#TSApplicationid").val(),
        TsGridWidth.Application = $("#TSApplicationwidth").val(),
        TsGridWidth.ProjectId = $("#TSProjectid").val(),
        TsGridWidth.Project = $("#TSProjectwidth").val(),
        TsGridWidth.ActionsId = $("#TSActionsid").val(),
        TsGridWidth.Actions = $("#TSActionswidth").val(),

        $('#ResizeTestSuitegrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveTestSuiteGridWidth",
        data: JSON.stringify(TsGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Test Suite Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#TSGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function TCGridWidthSave() {
    var CheckError = false;
    $("#TCNameError").css("display", "none");
    $("#TCDescriptionError").css("display", "none");
    $("#TCApplicationError").css("display", "none");
    $("#TCTestSuiteError").css("display", "none");
    $("#TCActionsError").css("display", "none");
    $("#TCerror").css("display", "none");

    $("#TCGridthsave").prop("disabled", true);

    if (parseFloat($("#TCNamewidth").val()) < 10 || $("#TCNamewidth").val() == "" || isFloat(parseFloat($("#TCNamewidth").val()))) {
        $("#TCNameError").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCDescriptionwidth").val()) < 10 || $("#TCDescriptionwidth").val() == "" || isFloat(parseFloat($("#TCDescriptionwidth").val()))) {
        $("#TCDescriptionError").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCApplicationwidth").val()) < 10 || $("#TCApplicationwidth").val() == "" || isFloat(parseFloat($("#TCApplicationwidth").val()))) {
        $("#TCApplicationError").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCTestSuitewidth").val()) < 10 || $("#TCTestSuitewidth").val() == "" || isFloat(parseFloat($("#TCTestSuitewidth").val()))) {
        $("#TCTestSuiteError").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCActionswidth").val()) < 10 || $("#TCActionswidth").val() == "" || isFloat(parseFloat($("#TCActionswidth").val()))) {
        $("#TCActionsError").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#TCNamewidth").val())) + Number(parseFloat($("#TCDescriptionwidth").val())) +
        Number(parseFloat($("#TCApplicationwidth").val())) + Number(parseFloat($("#TCTestSuitewidth").val())) +
        Number(parseFloat($("#TCActionswidth").val()));

    if (sum != 100) {
        $("#TCerror").css("display", "block");
        $("#TCGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var TCGridWidth = {};
    TCGridWidth.GridId = $("#TCGridid").val(),
        TCGridWidth.NameId = $("#TCNameid").val(),
        TCGridWidth.Name = $("#TCNamewidth").val(),
        TCGridWidth.DescriptionId = $("#TCDescriptionid").val(),
        TCGridWidth.Description = $("#TCDescriptionwidth").val(),
        TCGridWidth.ApplicationId = $("#TCApplicationid").val(),
        TCGridWidth.Application = $("#TCApplicationwidth").val(),
        TCGridWidth.TestSuiteId = $("#TCTestSuiteid").val(),
        TCGridWidth.TestSuite = $("#TCTestSuitewidth").val(),
        TCGridWidth.ActionsId = $("#TCActionsid").val(),
        TCGridWidth.Actions = $("#TCActionswidth").val(),

        $('#ResizeTestCasegrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveTestCaseGridWidth",
        data: JSON.stringify(TCGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Test Case Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#TCGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function ObjGridWidthSave() {
    var CheckError = false;
    $("#objNameError").css("display", "none");
    $("#objInternalAccessError").css("display", "none");
    $("#objTypeError").css("display", "none");
    $("#objPegwindowError").css("display", "none");
    $("#objActionsError").css("display", "none");
    $("#objSelectError").css("display", "none");
    $("#objerror").css("display", "none");

    $("#objGridthsave").prop("disabled", true);

    if (parseFloat($("#objNamewidth").val()) < 10 || $("#objNamewidth").val() == "" || isFloat(parseFloat($("#objNamewidth").val()))) {
        $("#objNameError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#objInternalAccesswidth").val()) < 10 || $("#objInternalAccesswidth").val() == "" || isFloat(parseFloat($("#objInternalAccesswidth").val()))) {
        $("#objInternalAccessError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#objTypewidth").val()) < 10 || $("#objTypewidth").val() == "" || isFloat(parseFloat($("#objTypewidth").val()))) {
        $("#objTypeError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#objPegwindowwidth").val()) < 10 || $("#objPegwindowwidth").val() == "" || isFloat(parseFloat($("#objPegwindowwidth").val()))) {
        $("#objPegwindowError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#objActionswidth").val()) < 10 || $("#objActionswidth").val() == "" || isFloat(parseFloat($("#objActionswidth").val()))) {
        $("#objActionsError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#objSelectwidth").val()) < 10 || $("#objSelectwidth").val() == "" || isFloat(parseFloat($("#objSelectwidth").val()))) {
        $("#objSelectError").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    var sum = Number(parseFloat($("#objNamewidth").val())) + Number(parseFloat($("#objInternalAccesswidth").val())) +
        Number(parseFloat($("#objTypewidth").val())) + Number(parseFloat($("#objPegwindowwidth").val())) +
        Number(parseFloat($("#objActionswidth").val())) + Number(parseFloat($("#objSelectwidth").val()));

    if (sum != 100) {
        $("#objerror").css("display", "block");
        $("#objGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var ObjGridWidth = {};
    ObjGridWidth.GridId = $("#objGridid").val(),
        ObjGridWidth.NameId = $("#objNameid").val(),
        ObjGridWidth.Name = $("#objNamewidth").val(),
        ObjGridWidth.InternalAccessId = $("#objInternalAccessid").val(),
        ObjGridWidth.InternalAccess = $("#objInternalAccesswidth").val(),
        ObjGridWidth.TypeId = $("#objTypeid").val(),
        ObjGridWidth.Type = $("#objTypewidth").val(),
        ObjGridWidth.PegwindowId = $("#objPegwindowid").val(),
        ObjGridWidth.Pegwindow = $("#objPegwindowwidth").val(),
        ObjGridWidth.ActionsId = $("#objActionsid").val(),
        ObjGridWidth.Actions = $("#objActionswidth").val(),
        ObjGridWidth.SelectId = $("#objSelectid").val(),
        ObjGridWidth.Select = $("#objSelectwidth").val();

    $('#ResizeObjectgrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveObjectGridWidth",
        data: JSON.stringify(ObjGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Object Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#objGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function VarGridWidthSave() {
    var CheckError = false;
    $("#VarNameError").css("display", "none");
    $("#VarTypeError").css("display", "none");
    $("#VarValueError").css("display", "none");
    $("#VarStatusError").css("display", "none");
    $("#VarActionsError").css("display", "none");
    $("#Varerror").css("display", "none");

    $("#VarGridthsave").prop("disabled", true);

    if (parseFloat($("#VarNamewidth").val()) < 10 || $("#VarNamewidth").val() == "" || isFloat(parseFloat($("#VarNamewidth").val()))) {
        $("#VarNameError").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#VarTypewidth").val()) < 10 || $("#VarTypewidth").val() == "" || isFloat(parseFloat($("#VarTypewidth").val()))) {
        $("#VarTypeError").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#VarValuewidth").val()) < 10 || $("#VarValuewidth").val() == "" || isFloat(parseFloat($("#VarValuewidth").val()))) {
        $("#VarValueError").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#VarStatuswidth").val()) < 10 || $("#VarStatuswidth").val() == "" || isFloat(parseFloat($("#VarStatuswidth").val()))) {
        $("#VarStatusError").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#VarActionswidth").val()) < 10 || $("#VarActionswidth").val() == "" || isFloat(parseFloat($("#VarActionswidth").val()))) {
        $("#VarActionsError").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#VarNamewidth").val())) + Number(parseFloat($("#VarTypewidth").val())) +
        Number(parseFloat($("#VarValuewidth").val())) + Number(parseFloat($("#VarStatuswidth").val())) +
        Number(parseFloat($("#VarActionswidth").val()));

    if (sum != 100) {
        $("#Varerror").css("display", "block");
        $("#VarGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var VarGridWidth = {};
    VarGridWidth.GridId = $("#VarGridid").val(),
        VarGridWidth.NameId = $("#VarNameid").val(),
        VarGridWidth.Name = $("#VarNamewidth").val(),
        VarGridWidth.TypeId = $("#VarTypeid").val(),
        VarGridWidth.Type = $("#VarTypewidth").val(),
        VarGridWidth.ValueId = $("#VarValueid").val(),
        VarGridWidth.Value = $("#VarValuewidth").val(),
        VarGridWidth.StatusId = $("#VarStatusid").val(),
        VarGridWidth.Status = $("#VarStatuswidth").val(),
        VarGridWidth.ActionsId = $("#VarActionsid").val(),
        VarGridWidth.Actions = $("#VarActionswidth").val(),

        $('#ResizeVariablegrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveVaribleGridWidth",
        data: JSON.stringify(VarGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Varible Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#VarGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function userGridWidthSave() {
    var CheckError = false;
    $("#userFNameError").css("display", "none");
    $("#userMNameError").css("display", "none");
    $("#userLNameError").css("display", "none");
    $("#userNameError").css("display", "none");
    $("#userEmailError").css("display", "none");
    $("#userComError").css("display", "none");
    $("#userStatusError").css("display", "none");
    $("#userActionsError").css("display", "none");
    $("#usererror").css("display", "none");

    $("#UserGridthsave").prop("disabled", true);

    if (parseFloat($("#userFNamewidth").val()) < 10 || $("#userFNamewidth").val() == "" || isFloat(parseFloat($("#userFNamewidth").val()))) {
        $("#userFNameError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userMNamewidth").val()) < 10 || $("#userMNamewidth").val() == "" || isFloat(parseFloat($("#userMNamewidth").val()))) {
        $("#userMNameError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userLNamewidth").val()) < 10 || $("#userLNamewidth").val() == "" || isFloat(parseFloat($("#userLNamewidth").val()))) {
        $("#userLNameError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userNamewidth").val()) < 10 || $("#userNamewidth").val() == "" || isFloat(parseFloat($("#userNamewidth").val()))) {
        $("#userNameError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userEmailwidth").val()) < 10 || $("#userEmailwidth").val() == "" || isFloat(parseFloat($("#userEmailwidth").val()))) {
        $("#userEmailError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userComwidth").val()) < 10 || $("#userComwidth").val() == "" || isFloat(parseFloat($("#userComwidth").val()))) {
        $("#userComError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userStatuswidth").val()) < 10 || $("#userStatuswidth").val() == "" || isFloat(parseFloat($("#userStatuswidth").val()))) {
        $("#userStatusError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#userActionswidth").val()) < 10 || $("#userActionswidth").val() == "" || isFloat(parseFloat($("#userActionswidth").val()))) {
        $("#userActionsError").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }

    var sum = Number(parseFloat($("#userFNamewidth").val())) + Number(parseFloat($("#userMNamewidth").val())) +
        Number(parseFloat($("#userLNamewidth").val())) + Number(parseFloat($("#userNamewidth").val())) +
        Number(parseFloat($("#userEmailwidth").val())) + Number(parseFloat($("#userComwidth").val())) +
        Number(parseFloat($("#userStatuswidth").val()) + Number(parseFloat($("#userActionswidth").val())));

    if (sum != 100) {
        $("#usererror").css("display", "block");
        $("#UserGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var UserGridWidth = {};
    UserGridWidth.GridId = $("#userGridid").val(),
        UserGridWidth.FNameId = $("#userFNameid").val(),
        UserGridWidth.FName = $("#userFNamewidth").val(),
        UserGridWidth.MNameId = $("#userMNameid").val(),
        UserGridWidth.MName = $("#userMNamewidth").val(),
        UserGridWidth.LNameId = $("#userLNameid").val(),
        UserGridWidth.LName = $("#userLNamewidth").val(),
        UserGridWidth.NameId = $("#userNameid").val(),
        UserGridWidth.Name = $("#userNamewidth").val(),
        UserGridWidth.EmailId = $("#userEmailid").val(),
        UserGridWidth.Email = $("#userEmailwidth").val(),
        UserGridWidth.CompanyId = $("#userComid").val(),
        UserGridWidth.Company = $("#userComwidth").val(),
        UserGridWidth.StatusId = $("#userStatusid").val(),
        UserGridWidth.Status = $("#userStatuswidth").val(),
        UserGridWidth.ActionsId = $("#userActionsid").val(),
        UserGridWidth.Actions = $("#userActionswidth").val();

    $('#ResizeUsergrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveUserGridWidth",
        data: JSON.stringify(UserGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Users Grid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#UserGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function TCPGridWidthSave() {
    var CheckError = false;
    $("#TCPKeywordError").css("display", "none");
    $("#TCPObjectError").css("display", "none");
    $("#TCPParametersError").css("display", "none");
    $("#TCPCommentError").css("display", "none");
    $("#TCPDataSetsError").css("display", "none");

    $("#TCPGridthsave").prop("disabled", true);

    if (parseFloat($("#TCPKeywordwidth").val()) < 20 || $("#TCPKeywordwidth").val() == "" || isFloat(parseFloat($("#TCPKeywordwidth").val()))) {
        $("#TCPKeywordError").css("display", "block");
        $("#TCPGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCPObjectwidth").val()) < 20 || $("#TCPObjectwidth").val() == "" || isFloat(parseFloat($("#TCPObjectwidth").val()))) {
        $("#TCPObjectError").css("display", "block");
        $("#TCPGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCPParameterswidth").val()) < 20 || $("#TCPParameterswidth").val() == "" || isFloat(parseFloat($("#TCPParameterswidth").val()))) {
        $("#TCPParametersError").css("display", "block");
        $("#TCPGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#TCPCommentwidth").val()) < 20 || $("#TCPCommentwidth").val() == "" || isFloat(parseFloat($("#TCPCommentwidth").val()))) {
        $("#TCPCommentError").css("display", "block");
        $("#TCPGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var TCPGridWidth = {};
    TCPGridWidth.GridId = $("#TCPGridid").val(),
        TCPGridWidth.KeywordId = $("#TCPKeywordid").val(),
        TCPGridWidth.Keyword = $("#TCPKeywordwidth").val(),
        TCPGridWidth.ObjectId = $("#TCPObjectid").val(),
        TCPGridWidth.Object = $("#TCPObjectwidth").val(),
        TCPGridWidth.ParametersId = $("#TCPParametersid").val(),
        TCPGridWidth.Parameters = $("#TCPParameterswidth").val(),
        TCPGridWidth.CommentId = $("#TCPCommentid").val(),
        TCPGridWidth.Comment = $("#TCPCommentwidth").val(),

        $('#ResizeTestCasePagegrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveTestCasePqGridWidth",
        data: JSON.stringify(TCPGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted TestCase PqGrid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#TCPGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function SPpqGridWidthSave() {
    var CheckError = false;
    $("#SPActionError").css("display", "none");
    $("#SPStepsError").css("display", "none");
    $("#SPTestSuiteError").css("display", "none");
    $("#SPTestCaseError").css("display", "none");
    $("#SPDatasetError").css("display", "none");
    $("#SPBaseResultError").css("display", "none");
    $("#SPBaseErrorCauseError").css("display", "none");
    $("#SPBaseScriptStartsError").css("display", "none");
    $("#SPBaseScriptDurationError").css("display", "none");
    $("#SPComResultError").css("display", "none");
    $("#SPComErrorCauseError").css("display", "none");
    $("#SPComScriptStartError").css("display", "none");
    $("#SPComScriptDurationError").css("display", "none");
    $("#SPDependencyError").css("display", "none");
    $("#SPDescriptionError").css("display", "none");

    $("#SPpqGridthsave").prop("disabled", true);

    if (parseFloat($("#SPActionwidth").val()) < 20 || $("#SPActionwidth").val() == "" || isFloat(parseFloat($("#SPActionwidth").val()))) {
        $("#SPActionError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPStepswidth").val()) < 20 || $("#SPStepswidth").val() == "" || isFloat(parseFloat($("#SPStepswidth").val()))) {
        $("#SPStepsError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPTestSuitewidth").val()) < 20 || $("#SPTestSuitewidth").val() == "" || isFloat(parseFloat($("#SPTestSuitewidth").val()))) {
        $("#SPTestSuiteError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPTestCasewidth").val()) < 20 || $("#SPTestCasewidth").val() == "" || isFloat(parseFloat($("#SPTestCasewidth").val()))) {
        $("#SPTestCaseError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPDatasetwidth").val()) < 20 || $("#SPDatasetwidth").val() == "" || isFloat(parseFloat($("#SPDatasetwidth").val()))) {
        $("#SPDatasetError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPBaseResultwidth").val()) < 20 || $("#SPBaseResultwidth").val() == "" || isFloat(parseFloat($("#SPBaseResultwidth").val()))) {
        $("#SPBaseResultError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPBaseErrorCausewidth").val()) < 20 || $("#SPBaseErrorCausewidth").val() == "" || isFloat(parseFloat($("#SPBaseErrorCausewidth").val()))) {
        $("#SPBaseErrorCauseError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPBaseScriptStartswidth").val()) < 20 || $("#SPBaseScriptStartswidth").val() == "" || isFloat(parseFloat($("#SPBaseScriptStartswidth").val()))) {
        $("#SPBaseScriptStartsError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPBaseScriptDurationwidth").val()) < 20 || $("#SPBaseScriptDurationwidth").val() == "" || isFloat(parseFloat($("#SPBaseScriptDurationwidth").val()))) {
        $("#SPBaseScriptDurationError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPComResultwidth").val()) < 20 || $("#SPComResultwidth").val() == "" || isFloat(parseFloat($("#SPComResultwidth").val()))) {
        $("#SPComResultError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPComErrorCausewidth").val()) < 20 || $("#SPComErrorCausewidth").val() == "" || isFloat(parseFloat($("#SPComErrorCausewidth").val()))) {
        $("#SPComErrorCauseError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPComScriptStartwidth").val()) < 20 || $("#SPComScriptStartwidth").val() == "" || isFloat(parseFloat($("#SPComScriptStartwidth").val()))) {
        $("#SPComScriptStartError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPComScriptDurationwidth").val()) < 20 || $("#SPComScriptDurationwidth").val() == "" || isFloat(parseFloat($("#SPComScriptDurationwidth").val()))) {
        $("#SPComScriptDurationError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPDependencywidth").val()) < 20 || $("#SPDependencywidth").val() == "" || isFloat(parseFloat($("#SPDependencywidth").val()))) {
        $("#SPDependencyError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (parseFloat($("#SPDescriptionwidth").val()) < 20 || $("#SPDescriptionwidth").val() == "" || isFloat(parseFloat($("#SPDescriptionwidth").val()))) {
        $("#SPDescriptionError").css("display", "block");
        $("#SPpqGridthsave").attr("disabled", false);
        CheckError = true;
    }
    if (CheckError) { return false; }

    var SPGridWidth = {};
    SPGridWidth.GridId = $("#SPGridid").val(),
        SPGridWidth.ActionId = $("#SPActionid").val(),
        SPGridWidth.Action = $("#SPActionwidth").val(),
        SPGridWidth.StepsId = $("#SPStepsid").val(),
        SPGridWidth.Steps = $("#SPStepswidth").val(),
        SPGridWidth.TestSuiteId = $("#SPTestSuiteid").val(),
        SPGridWidth.TestSuite = $("#SPTestSuitewidth").val(),
        SPGridWidth.TestCaseId = $("#SPTestCaseid").val(),
        SPGridWidth.TestCase = $("#SPTestCasewidth").val(),
        SPGridWidth.DatasetId = $("#SPDatasetid").val(),
        SPGridWidth.Dataset = $("#SPDatasetwidth").val(),
        SPGridWidth.BResultId = $("#SPBaseResultid").val(),
        SPGridWidth.BResult = $("#SPBaseResultwidth").val(),
        SPGridWidth.BErrorCauseId = $("#SPBaseErrorCauseid").val(),
        SPGridWidth.BErrorCause = $("#SPBaseErrorCausewidth").val(),
        SPGridWidth.BScriptStartId = $("#SPBaseScriptStartsid").val(),
        SPGridWidth.BScriptStart = $("#SPBaseScriptStartswidth").val(),
        SPGridWidth.BScriptDurationId = $("#SPBaseScriptDurationsid").val(),
        SPGridWidth.BScriptDuration = $("#SPBaseScriptDurationwidth").val(),
        SPGridWidth.CResultId = $("#SPComResultid").val(),
        SPGridWidth.CResult = $("#SPComResultwidth").val(),
        SPGridWidth.CErrorCauseId = $("#SPComErrorCauseid").val(),
        SPGridWidth.CErrorCause = $("#SPComErrorCausewidth").val(),
        SPGridWidth.CScriptStartId = $("#SPComScriptStartid").val(),
        SPGridWidth.CScriptStart = $("#SPComScriptStartwidth").val(),
        SPGridWidth.CScriptDurationId = $("#SPComScriptDurationid").val(),
        SPGridWidth.CScriptDuration = $("#SPComScriptDurationwidth").val(),
        SPGridWidth.DependencyId = $("#SPDependencyid").val(),
        SPGridWidth.Dependency = $("#SPDependencywidth").val(),
        SPGridWidth.DescriptionId = $("#SPDescriptionid").val(),
        SPGridWidth.Description = $("#SPDescriptionwidth").val(),

        $('#ResizeStoryboardPqgrid').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveStoryboardPqGridWidth",
        data: JSON.stringify(SPGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            if (result) {
                stoploader();
                swal.fire({
                    "title": "",
                    "text": "Successfully submitted Storyboard PqGrid Width.",
                    "type": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
                $("#SPpqGridthsave").prop("disabled", false);
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}

function ResizeLeftPanel() {
    var CheckError = false;
    $("#RError").css("display", "none");
    if (parseFloat($("#resizeid").val()) < 150 || $("#resizeid").val() == "" || isFloat(parseFloat($("#resizeid").val()))) {
        $("#RError").css("display", "block");
        CheckError = true;
    }

    if (CheckError) { return false; }
    var lwidth = $("#resizeid").val();
    var RGridWidth = {};
        RGridWidth.GridId = $("#RGridid").val(),
        RGridWidth.ResizeId = $("#RLeftGridid").val(),
        RGridWidth.Resize = $("#resizeid").val(),

        $('#ResizeLeftPanel').modal('hide');
    startloader();
    $.ajax({
        url: "/ConfigurationGrid/SaveLeftPanelGridWidth",
        data: JSON.stringify(RGridWidth),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "HTML",
        success: function (result) {
            debugger
            if (result) {
                stoploader();
                $(".kt-aside").css("width", lwidth + "px");
                $("#kt_header").css("left", lwidth + "px");
                $(".kt-wrapper").css("padding-left", lwidth + "px");
                $(".kt-subheader").css("left", lwidth + "px");
                $("#hdnLeftPanelWidth").val(lwidth);
                $('.ULtablist li').each(function (index, value) {
                    if ($(value).children().first().attr("class").indexOf("active") > -1) {
                        var tabobj = $(value).children().first();
                        if (tabobj.attr("data-tab") == "TestCase") {
                            var lTestCaseName = tabobj.attr("data-testcasename");
                            setTimeout(function () { gridobj[".grid" + lTestCaseName].reset({ filter: true }); }, 500);
                        }
                        if (tabobj.attr("data-tab") == "Storyboard") {
                            var lStoryboardName = tabobj.attr("data-storyboardname");
                            setTimeout(function () { gridobj[".grid" + lStoryboardName].reset({ filter: true }); }, 500);
                        }
                    }
                });
            }
        },
        error: function (x, y, z) {
            stoploader();
            swal.fire(
                'Error while deleting',
                'error'
            )
        }
    });
}
