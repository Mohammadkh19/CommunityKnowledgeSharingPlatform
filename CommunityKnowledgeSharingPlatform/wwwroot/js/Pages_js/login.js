$(document).ready(function () {
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
                alert('Login successful!');
                localStorage.setItem('authToken', response.token); // Save token
                window.location.href = '/index.html'; // Redirect
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Login failed. Please try again.');
            },
        });
    });
});

