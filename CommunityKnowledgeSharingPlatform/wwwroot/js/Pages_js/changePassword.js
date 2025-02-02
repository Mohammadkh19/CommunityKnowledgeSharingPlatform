$(document).ready(function () {
    $('form').on('submit', function (e) {
        e.preventDefault();

        const oldPassword = $('#oldPassword').val();
        const newPassword = $('#newPassword').val();
        const confirmNewPassword = $('#confirmNewPassword').val();

        // Clear any existing alerts
        clearAlerts();

        if (newPassword !== confirmNewPassword) {
            showAlert('danger', 'New password and confirmation do not match.');
            return;
        }

        if (!validatePassword(newPassword)) {
            showAlert('danger', 'The passwor should contain at least 8 characters, one capital letter, and one number.');
            return;
        }

        // Call the API
        $.ajax({
            url: '/api/auth/changePassword',
            method: 'POST',
            contentType: 'application/json',
            headers: {
                Authorization: `Bearer ${localStorage.getItem('authToken')}`,
            },
            data: JSON.stringify({
                currentPassword: oldPassword,
                newPassword: newPassword,
                confirmNewPassword: confirmNewPassword,
            }),
            success: function (response) {
                showAlert('success', 'Password updated successfully!');
                $('#changePasswordForm')[0].reset(); // Optionally reset the form
            },
            error: function (xhr) {
                const errorMessage = xhr.responseJSON?.message || 'Failed to update password.';
                showAlert('danger', errorMessage);
            },
        });
    });

    // Function to clear existing alerts
    function clearAlerts() {
        $('.alert').remove();
    }

    // Function to validate the password
    function validatePassword(password) {
        const passwordRegex = /^(?=.*[A-Z])(?=.*\d).{8,}$/;
        return passwordRegex.test(password);
    }


    // Function to show Bootstrap alerts
    function showAlert(type, message) {
        const alertHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        `;
        $('#changePasswordForm').prepend(alertHTML);

    }

});


