angular.module('AddEditUser', []).controller('Accounts', function ($scope, $http, $location, $window) {
    $scope.message = '';
    $scope.Header = '';
    $scope.result = "color-default";
    $scope.isViewLoading = false;
    $scope.user = {
        TESTER_ID: 0,
        TESTER_NAME_F: '',
        TESTER_NAME_M: '',
        TESTER_NAME_LAST: '',
        COMPANY_ID: 0,
        TESTER_MAIL: '',
        TESTER_LOGIN_NAME: '',
        ACTIVE: 0,
        TESTER_PWD: ''
    };
    $scope.listCompany = [];
    //get called when user submits the form  
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    // Read query string to an object - vars
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    if (vars.lid != undefined && vars.lid != null && vars.lid != "" && vars.lid != "0") {
        $scope.Header = "Update";
    }
    else {
        $scope.Header = "Create";
    }
    // Load City Dropdown
    $http
        ({
            method: 'GET',
            url: 'http://CS-DC-17/MARSApi/api/GetCompanies',
            asyc: false
        }).then(
            function (data, status, headers, config) {
                if (data != undefined && data != null && data.data != undefined && data.data != null) {
                    $scope.listCompany = data.data;
                }
            }
        );
    if (vars != null && vars.length > 0 && vars.lid != null) {
        $http
            ({
                method: 'GET',
                url: 'http://CS-DC-17/MARSApi/api/GetUserById?lid=' + vars.lid
            }).then(
                function (data, status, headers, config) {
                    if (data != undefined && data != null && data.data != undefined && data.data != null) {
                        $scope.user = data.data;
                    }
                }
            );
    }
    $scope.submitForm = function () {
        //debugger;
        console.log('Form is submitted with:', $scope.user);
        //$http service that send or receive data from the remote server  
        if ($scope.user.STATUS == true) {
            $scope.user.STATUS = 1;
        }
        else {
            $scope.user.STATUS = 0;
        }
        var value = $scope.user;
        value = JSON.stringify(value);
        $.ajax({
            url: 'http://localhost/MARSApi/api/AddEditUser1',
            data: value,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (result) {
                //debugger;
                if (result == "success") {
                    window.open('/Accounts/ListOfUsers', '_self');
                }
            }
        });
        //$http(
        //    {
        //        method: 'POST',
        //        url: 'http://CS-DC-17/MARSApi/api/AddEditUser1',
        //        data: value
        //    }).than(function (data, status, headers, config) {
        //        debugger;
        //        $window.location.reload();
        //    });
        return false;
    };
});