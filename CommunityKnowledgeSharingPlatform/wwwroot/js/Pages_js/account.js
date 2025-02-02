$(document).ready(function () {

    $.ajax({
        url: 'api/Auth/GetUserDetails',
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        dataType: 'json', // Expect JSON response
        success: function (response) {
            $('#username').val(response.username);
            $('#email').val(response.email);
        },
        error: function () {
            console.error('Error fetching categories');
        }
    });


    $("#changeAccountInfoForm").submit(function (event) {
        event.preventDefault(); // Prevent default form submission

        // Serialize form data
        var formData = {
            newUsername: $("#username").val(),
            newEmail: $("#email").val()
        };

        // Call API using AJAX
        $.ajax({
            url: "api/Auth/UpdateDetails", // API endpoint
            type: "PUT", // HTTP method
            contentType: "application/json", // Content type
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            data: JSON.stringify(formData), // Convert formData to JSON
            success: function (response) {
                localStorage.setItem('success', 'Updated successfully! Please login again')
                localStorage.removeItem('authToken');
                window.location.href = '/auth-login.html';
            },
            error: function (xhr, status, error) {
                toastr.error("Error updating user details: " + xhr.responseText);
                console.error(xhr, status, error);
            }
        });
    });

});