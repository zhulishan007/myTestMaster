var $DataSourceGrid = null;


function createDefaultDataSourceTabs() {
    try {
        alert("createDefaultDataSourceTabs");
        //$("kt-container #dataSourceTabs").removeAttr("class");
        //$("kt-container #dataSourceTabs").removeAttr("style");
        $("kt-container #dataSourceTabs").tabs();
    } catch (e) {
        alert(e);
    }
    
}

function attachColumn2Render(datasourceTable) {
    datasourceTable.colModel[3].render = function (ui) {
        var rowData = ui.rowData,
            dataIndx = ui.dataIndx;
        if (dataIndx != "DATA_SOURCE_TYPE") return;
        if (rowData["DATA_SOURCE_TYPE"] == "1") {
            rowData["Type"] = "Mars Datacompare Data";
        } else {
            rowData["Type"] = "N/A";
        }
    }
    
}

function DataSourceDelete(datasourceId, pg) {
    alert("delete " + datasourceId);
    if (typeof(pg) == 'undefined') return;
    if (pg == null) {
        return null;
    }
    
    if ((datasourceId == -1)
        || (datasourceId == "-1")) {
        alert("");
    }
}

function AddRow() {
    
}