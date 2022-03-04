$(document).ready(function () {
    $("#frmapplication").validate({
        rules: {
            applicationname: {
                required: true
            },
            applicationdesc: {
                required: true
            },
            applicationversion: {
                required: true
            }
        },
    });
});

function AddEditApplication(Id, editObj) {
    var validator = $("#frmapplication").validate();
    validator.resetForm();
    $("#appexampleModalLabel").text('');
    $("#appexampleModalLabel").text('Edit Application');
    $("#addedapplication").prop("disabled", false);
    $("#applicationvalidate").css("display", "none");
    $("#AddApplication").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    var lapplicationId = Id;
    var Name = $(editObj).attr("data-name").replace(/###/g, "'");
    var Desc = $(editObj).attr("data-des").replace(/###/g, "'");
    var version = $(editObj).attr("data-ver");
    var Extra = $(editObj).attr("data-Extra");
    var Mode = $(editObj).attr("data-Mode");
    var BitsId = $(editObj).attr("data-BitsId");
    var appPath = $(editObj).attr("data-appPath");

    $("#applicationname").val(Name);
    $("#applicationdesc").val(Desc);
    $("#hdnApplicationId").val(lapplicationId);
    $("#applicationversion").val(version);
    $("#sDrpExtraReq").val(Extra.split(","));
    $("#sDrpExtraReq").select2();
    $("#DrpMode").val(Mode);
    $("#DrpBits").val(BitsId != null && BitsId != "" && BitsId != undefined ? parseInt(BitsId) : "");
    $("#applicationpath").val(appPath);

    if (!$("#applicationvalidate").valid())
        return false;
}

function AddApplication() {
    $("#applicationvalidate").css("display", "none");
    $("#addedapplication").attr("disabled", false);
    $("#appexampleModalLabel").text('');
    $("#appexampleModalLabel").text('Add Application');
    $("#AddApplication").modal("show");
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });
    $("#hdnApplicationId").val("");
    $("#applicationname").val("");
    $("#applicationdesc").val("");
    $("#applicationversion").val("");
    $("#sDrpExtraReq").val("");
    $("#sDrpExtraReq").select2();
    var validator = $("#frmapplication").validate();
    validator.resetForm();
}

function AddEditApplicationSave() {
    $("#addedapplication").prop("disabled", true);
    if (!$("#frmapplication").valid()) {
        $("#addedapplication").prop("disabled", false);
        return false;
    }
    var lId = $("#hdnApplicationId").val();
    if (lId == null && lId == "")
        lId = 0;
    var regex = /^[a-zA-Z0-9-._(&)*  ]*$/;
    var ApplicationModel = {};
    ApplicationModel.ApplicationId = lId,
    ApplicationModel.ApplicationName = $("#applicationname").val(),
    ApplicationModel.Description = $("#applicationdesc").val(),
    ApplicationModel.Version = $("#applicationversion").val().toString(),
    ApplicationModel.ExtraRequirement = $("#sDrpExtraReq").val().toString(),
    ApplicationModel.Mode = $("#DrpMode").val().toString(),
    ApplicationModel.STARTER_COMMAND = $("#applicationpath").val().toString(),
    ApplicationModel.BitsId = $("#DrpBits").val().toString();
    if (!regex.test(ApplicationModel.ApplicationName)) {
        $("#applicationname").val("");
        $("#applicationvalidate").css("display", "block");
        $("#addedapplication").attr("disabled", false);
        return false;
    }
    $.ajax({
        url: "/Application/AddEditApplication",
        data: JSON.stringify(ApplicationModel),
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
            $("#AddApplication").modal("hide");
            $("#addedapplication").attr("disabled", false);
            if (result.status == 1 && result.data) {
                swal.fire({
                    "title": "",
                    "text": result.message,
                    "icon": "success",
                    "onClose": function (e) {
                        console.log('on close event fired!');
                    }
                });
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
            if (typeof ApplicationTable !== 'undefined')
                ApplicationTable.table().draw();            
        },
        error: function (errormessage) {
            stoploader();
            alert(errormessage.responseText);
        }
    });
}

function CheckApplicationNameExist() {
    var applicationname = $("#applicationname").val();
    var ApplicationId = $("#hdnApplicationId").val();
    $.ajax({
        url: "/Application/CheckDuplicateApplicationNameExist",
        data: '{"applicationname":"' + applicationname + '","ApplicationId":"' + ApplicationId + '"}',
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
                $("#applicationname").val("");
                $("#existapplicationname").css("display", "block");
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
            stoploader();
            alert(errormessage.responseText);
        }
    });
}
$("#applicationname").on('keyup', function () {
    $("#existapplicationname").css("display", "none");
});
$("#applicationname").on('keyup', function () {
    $("#applicationvalidate").css("display", "none");
});
