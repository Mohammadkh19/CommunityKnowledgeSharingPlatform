$(document).ready(function () {
    const success = localStorage.getItem('success');
    if (success) {
        toastr.success(success);
        localStorage.removeItem('success');
    }

    $('form').on('submit', function (e) {
        e.preventDefault(); // Prevent default form submission

        const email = $('#inputEmail').val();
        const password = $('#inputPassword').val();

        if (!email || !password) {
            alert('Email and password are required!');
            return;
        }

        $.ajax({
            url: '/api/auth/login',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ usernameOrEmail: email, password: password }),
            success: function (response) {
                localStorage.setItem('authToken', response.token); // Save token
                localStorage.setItem('loginSuccessMessage', 'Signed in successfully!')
                window.location.href = '/index.html'; // Redirect
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Login failed. Please try again.');
            },
        });
    });
});

