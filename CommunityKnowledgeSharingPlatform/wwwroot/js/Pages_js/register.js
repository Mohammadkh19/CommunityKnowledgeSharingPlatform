
$(document).ready(function () {

    // Handle form submission
    $('form').on('submit', function (e) {
        e.preventDefault();

        // Collect input values
        const email = $('#inputEmail4').val();
        const username = $('#username').val();
        const password = $('#inputPassword5').val();
        const confirmPassword = $('#inputPassword6').val();

        // Validate inputs
        if (!email || !username || !password || !confirmPassword) {
            alert('All fields are required!');
            return;
        }

        if (password !== confirmPassword) {
            alert('Passwords do not match!');
            return;
        }

        $.ajax({
            url: '/api/Auth/Register', 
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                email: email,
                username: username,
                password: password,
                confirmPassword: confirmPassword
            }),
            success: function (response) {
                alert('Registration successful! You can now log in.');
                localStorage.setItem('authToken', response.token); // Save token
                window.location.href = '/index.html'; // Redirect
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Registration failed. Please try again.');
            },
        });
    });
});

