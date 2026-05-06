$(document).on("click", ".perm-btn", function () {

    const btn = $(this);
    const permission = btn.data("permission");
    const userId = btn.closest(".permissions").data("user-id");

    $.ajax({
        url: "/Admin/TogglePermissionByName",
        type: "POST",
        data: {
            userId: userId,
            permissionName: permission
        },
        success: function () {

            btn.toggleClass("btn-primary btn-secondary");

        },
        error: function () {
            alert("Failed to update permission");
        }
    });

});