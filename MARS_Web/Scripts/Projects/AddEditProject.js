$(document).ready(function () {
    $("#frmproject").validate({
        rules: {
            projectname: {
                required: true
            },
            projectdesc: {
                required: true,
            },
            DrpProjStatus: {
                required: true
            }
        },
    });
    $('#DrpProjStatus').on('changed.bs.DrpProjStatus', function () {
        validator.element($(this)); // validate element
    });
});

function AddEditProject(Id, editObj) {
    var validator = $("#frmproject").validate();
    validator.resetForm();
    $("#projexampleModalLabel").text('');
    $("#projexampleModalLabel").text('Edit Project');
    $("#projectvalidate").css("display", "none");
    $("#existPappName").css("display", "none");
    $("#AddProject").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    $("#addedproject").attr("disabled", false);
    var lprojectId = Id;
    var Name = $(editObj).attr("data-name").replace(/###/g, "'");
    var Desc = $(editObj).attr("data-des").replace(/###/g, "'");
    var Status = $(editObj).attr("data-statusId");
    var AppId = $(editObj).attr("data-appid");

    $("#projectname").val(Name);
    $("#projectdesc").val(Desc);
    $("#hdnProjectId").val(lprojectId);
    $("#DrpProjStatus").val(Status);
    $("#sDrpApplication").val(AppId.split(","));
    $("#sDrpApplication").select2();

    if (!$("#projectvalidate").valid())
        return false;
}
function AddProject() {
    $("#projectvalidate").css("display", "none");
    $("#existPappName").css("display", "none");
    $("#addedproject").attr("disabled", false);
    $("#projexampleModalLabel").text('');
    $("#projexampleModalLabel").text('Add Project');
    $("#AddProject").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    $("#hdnProjectId").val("");
    $("#projectname").val("");
    $("#projectdesc").val("");
    $("#sDrpApplication").val("");
    $("#sDrpApplication").select2();
    var validator = $("#frmproject").validate();
    validator.resetForm();
}

function AddEditProjectSave() {
    var appvalue = $("#sDrpApplication").val();
    if (!$("#frmproject").valid()) {
        if (appvalue.length == 0) {
            $("#existPappName").css("display", "block");
            return false;
        }
        return false;
    }
    if (appvalue.length == 0) {
        $("#existPappName").css("display", "block");
        return false;
    }
    $("#addedproject").attr("disabled", true);
    var lId = $("#hdnProjectId").val();
    if (lId == null && lId == "")
        lId = 0;
    var regex = /^[a-zA-Z0-9-._(&)*  ]*$/;
    var ProjectModel = {};
    ProjectModel.ProjectId = lId,
        ProjectModel.ProjectName = $("#projectname").val(),
        ProjectModel.ProjectDescription = $("#projectdesc").val(),
        ProjectModel.ApplicationId = $("#sDrpApplication").val().toString(),
        ProjectModel.Status = $("#DrpProjStatus").val().toString();
    if (!regex.test(ProjectModel.ProjectName)) {
        $("#projectname").val("");
        $("#projectvalidate").css("display", "block");
        $("#addedproject").attr("disabled", false);
        return false;
    }
    $.ajax({
        url: "/Project/AddEditProject",
        data: JSON.stringify(ProjectModel),
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
            if (result.status == 1 && result.data) {
                $("#AddProject").modal("hide");
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "icon": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
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
            if (typeof ProjectTable !== 'undefined')
                ProjectTable.table().draw();            
            $("#addedproject").attr("disabled", false);
        },
        error: function (errormessage) {
            stoploader();
            alert(errormessage.responseText);
        }
    });
}
function CheckProjectNameExist() {
    var ProjectName = $("#projectname").val();
    var ProjectId = $("#hdnProjectId").val();
    $.ajax({
        url: "/Project/CheckDuplicateProjectNameExist",
        data: '{"ProjectName":"' + ProjectName + '","ProjectId":"' + ProjectId + '"}',
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
            if (result.status == 1 && result.data) {
                $("#projectname").val("");
                $("#existprojectname").css("display", "block");
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
        error: function (errormessage) {
            stoploader();
            alert(errormessage.responseText);
        }
    });
}