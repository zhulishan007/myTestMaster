function ExecuteEngine(lDataBase) {
    var lStoryboardId = $("#hdnExeStoryboardId").val();
    var lStoryboardName = $("#hdnExeStoryboardName").val();
    lStoryboardName = RecusrsiveTab(lStoryboardName);// lStoryboardName.replace(" ", "_");

    var lLoginUser = $("#hdnExeLoginUName").val();
    var lMode = $("#drpExecMode").val();
    var lAppIds = $("#drpExeApp").val();
    var lContinue = "False";
    var lIgnoreError = "False";
    if ($('#chkExeConTest').is(":checked")) {
        lContinue = "True";
    }
    if ($('#chkExeIgnoreError').is(":checked")) {
        lIgnoreError = "True";
    }
    console.log("Login User: " + lLoginUser);
    console.log("StoryboardName: " + lStoryboardName);
    console.log("StoryboardId: " + lStoryboardId);
    console.log("AppId: " + lAppIds);
    console.log("Mode: " + lMode);
    console.log("Continue: " + lContinue);
    console.log("IgnoreError: " + lIgnoreError);
    console.log("Database: " + lDataBase);
    $("#ExecutePopup").modal("toggle");
    //window.open("/StoryBoard/DownlaodBatchFile?lLoginUser=" + lLoginUser + "&lStoryboardName=" + lStoryboardName + "&lStoryboardId=" + lStoryboardId + "&lAppId=" + lAppIds + "&lMode=" + lMode + "&lContinue=" + lContinue + "&lIgnoreError=" + lIgnoreError);
    ///Added begin
    ///  Creator: changed by tiger
    ///  date   : 2020/08/11
    ///  reason : To enable MARS Engine to be invoked
    ///  version: 1.0.0.0	
    var engineURI = window.location.origin + "/MARSENGINE/Mars.AutoTestingDriver.application?userName=" + lLoginUser + "&command=-S&storyBoadName=" + encodeURI($("#hdnExeStoryboardName").val()) + "&storyBoardId=" + lStoryboardId + "&app=" + lAppIds + "&Mode=" + lMode + "&Continue=" + lContinue + "&IgnoreError=" + lIgnoreError + "&currentDB=" + lDataBase;
    window.open(engineURI);
    ///added End 
}

function ExecuteEngineTCstep(lDataBase, MarsengineURL) {
    $("#txtareacopy").css("display", "block");
    $("#txtareacopy")[0].select();
    window.document.execCommand("Copy");
    var lStoryboardId = $("#hdnExeTCStoryboardId").val();
    var lStoryboardName = $("#hdnExeTCStoryboardName").val();
    var lLoginUser = $("#hdnExeTCLoginUName").val();
    var lAppIds = $("#drpExeAppTC").val();
    var lguid = $("#hdnExeTCguid").val(); 
    $("#ExecutePopupTC").modal("toggle");
    var engineURI = window.location.origin + MarsengineURL + "?userName=" + lLoginUser + "&command=-FromClipboard&storyBoadName=" + lStoryboardName + "&storyBoardId=" + lStoryboardId + "&app=" + lAppIds + "&guid=" + lguid + "&currentDB=" + lDataBase;
    window.open(engineURI);
}