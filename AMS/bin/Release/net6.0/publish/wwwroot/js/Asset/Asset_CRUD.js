$(document).ready(function () {
    $("#AssignEmployeeId").select2();
});

var Details = function (id) {
    var url = "/Asset/Details?id=" + id;
    $('#titleBigModal').html("Asset Details");
    loadBigModal(url);
};

var AssignEmployeeInfo = function (id) {
    if (id == 0) {
        FieldValidationAlert(null, 'This asset is not assigned yet to employees. ', "info");
    }
    else {
        var url = "/Employee/Details?id=" + id;
        $('#titleBigModal').html("Employee Details");
        loadBigModal(url);
    }
};

var UnassignedAssetWarning = function (id) {
    FieldValidationAlert(null, 'This asset is not assigned yet to employees. ', "info");
};


var UpdateAssignee = function (id) {
    var url = "/Asset/AddEdit?id=" + id;
    $('#titleExtraBigModal').html("Edit Asset Assignee");
    loadExtraBigModal(url);
    setTimeout(function () {
        activaTab('divOtherTab');
    }, 300);
};


var Delete = function (id) {
    Swal.fire({
        title: 'Do you want to delete this item?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "DELETE",
                url: "/Asset/Delete?id=" + id,
                success: function (result) {
                    var message = "Asset has been deleted successfully. Asset ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblAsset').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};


var AddNewComment = function (IsView) {
    var _CommentMessage = $("#CommentMessage").val();
    if (_CommentMessage == "" || _CommentMessage == null) {
        Swal.fire({
            title: 'New comment field can not be null or empty.',
            icon: "warning",
            onAfterClose: () => {
                $("#CommentMessage").focus();
            }
        });
        return;
    }

    var _AssetId = $("#tmpAssetId").val();
    var _FormData = new FormData()
    _FormData.append('AssetId', _AssetId)
    _FormData.append('Message', $("#CommentMessage").val())

    $.ajax({
        type: "POST",
        url: "/Comment/AddNewComment",
        data: _FormData,
        processData: false,
        contentType: false,
        success: function (result) {
            UIRoutSwitch(IsView);
            //$('#tblCommentHistory').DataTable().ajax.reload();
        }
    });
};


var DeleteComment = function (id, IsView) {
    Swal.fire({
        title: 'Do you want to delete this item?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "DELETE",
                url: "/Comment/Delete?id=" + id,
                success: function (result) {
                    Swal.fire({
                        title: result,
                        icon: 'info',
                        onAfterClose: () => {
                            UIRoutSwitch(IsView);
                            //$('#tblCommentHistory').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};


var UIRoutSwitch = function (IsView) {
    var _AssetId = $("#tmpAssetId").val();
    if (IsView) {
        Details(_AssetId);
        setTimeout(function () {
            activaTab('divCommentHistoryInView');
        }, 100);
    }
    else {
        AddEdit(_AssetId);
        setTimeout(function () {
            activaTab('divCommentHistoryTab');
        }, 100);
    }
}


var PrintAsset = function (id) {
    location.href = "/Asset/PrintAsset?id=" + id;
};

var AddEdit = function (id) {
    var url = "/Asset/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Edit Asset");
    }
    else {
        $('#titleExtraBigModal').html("Add Asset");
    }
    loadExtraBigModal(url);
};

var SaveAsset = function () {
    if (!FieldValidation('#Name')) {
        FieldValidationAlert('#Name', 'Asset Name is Required.', "warning");
        return;
    }
    if (!FieldValidation('#Category')) {
        FieldValidationAlert('#Category', 'Asset Category is Required.', "warning");
        return;
    }

    $("#btnSave").prop('value', 'Please Wait');
    $('#btnSave').prop('disabled', true);

    $.ajax({
        type: "POST",
        url: "/Asset/AddEdit",
        data: PreparedFormObj(),
        processData: false,
        contentType: false,
        success: function (result) {
            $('#btnSave').prop('disabled', false);
            Swal.fire({
                title: result.AlertMessage,
                icon: "success"
            }).then(function () {
                document.getElementById("btnClose").click();
                if (result.CurrentURL == "/") {
                    setTimeout(function () {
                        $("#tblAsset").load("/ #tblAsset");
                    }, 1000);
                }
                else {
                    $('#tblAsset').DataTable().ajax.reload();
                }
            });
        },
        error: function (errormessage) {
            $("#btnSave").prop('value', 'Save');
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}


var UpdateBarcode = function () {
    var _AssetId = $("#AssetId").val();
    if (_AssetId.length > 10) {
        FieldValidationAlert('#AssetId', 'Max lenght is 10.', "warning");
        return;
    }

    Swal.fire({
        title: 'Barcode Updated',
        icon: "success"
    }).then(function () {
        $("#Barcode").JsBarcode(_AssetId);
    });
}


var PreparedFormObj = function () {
    var _FormData = new FormData()
    _FormData.append('Id', $("#Id").val())
    _FormData.append('CreatedDate', $("#CreatedDate").val())
    _FormData.append('CreatedBy', $("#CreatedBy").val())
    _FormData.append('AssetId', $("#AssetId").val())
    _FormData.append('AssetModelNo', $("#AssetModelNo").val())
    _FormData.append('Name', $("#Name").val())

    _FormData.append('Description', $("#Description").val())
    _FormData.append('UnitPrice', $("#UnitPrice").val())
    _FormData.append('Category', $("#Category").val())
    _FormData.append('SubCategory', $("#SubCategory").val())
    _FormData.append('Supplier', $("#Supplier").val())

    _FormData.append('Department', $("#Department").val())
    _FormData.append('SubDepartment', $("#SubDepartment").val())
    _FormData.append('DateOfManufacture', $("#DateOfManufacture").val())
    _FormData.append('YearOfValuation', $("#YearOfValuation").val())
    _FormData.append('WarranetyInMonth', $("#WarranetyInMonth").val())

    _FormData.append('DepreciationInMonth', $("#DepreciationInMonth").val())
    _FormData.append('Location', $("#Location").val())
    _FormData.append('ImageURL', $("#ImageURL").val())
    _FormData.append('ImageURLDetails', $('#ImageURLDetails')[0].files[0])
    _FormData.append('AssetStatus', $("#AssetStatus").val())
    _FormData.append('Note', $("#Note").val())
    _FormData.append('Barcode', $('#Barcode').attr('src'))
    _FormData.append('AssignEmployeeId', $("#AssignEmployeeId").val())
    _FormData.append('CurrentURL', $("#CurrentURL").val())
    return _FormData;
}