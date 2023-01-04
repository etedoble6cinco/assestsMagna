var Details = function (id) {
    var url = "/Employee/Details?id=" + id;
    $('#titleBigModal').html("Employee Details");
    loadBigModal(url);
};


var AddEdit = function (id) {
    var url = "/Employee/AddEdit?id=" + id;
    if (id > 0) {
        $('#titleExtraBigModal').html("Edit Employee");
    }
    else {
        $('#titleExtraBigModal').html("Add Employee");
    }
    loadExtraBigModal(url);
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
                url: "/Employee/Delete?id=" + id,
                success: function (result) {
                    var message = "Employee has been deleted successfully. Employee ID: " + result.Id;
                    Swal.fire({
                        title: message,
                        icon: 'info',
                        onAfterClose: () => {
                            $('#tblEmployee').DataTable().ajax.reload();
                        }
                    });
                }
            });
        }
    });
};
