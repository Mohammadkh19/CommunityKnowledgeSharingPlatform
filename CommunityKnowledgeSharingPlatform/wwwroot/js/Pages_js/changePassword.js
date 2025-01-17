$(document).ready(function () {
    $('form').on('submit', function (e) {
        e.preventDefault();

        const oldPassword = $('#oldPassword').val();
        const newPassword = $('#newPassword').val();
        const confirmNewPassword = $('#confirmNewPassword').val();

        if (newPassword !== confirmNewPassword) {
            alert('New password and confirmation do not match.');
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
                alert('Password updated successfully!');
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Failed to update password.');
            },
        });
    });
});