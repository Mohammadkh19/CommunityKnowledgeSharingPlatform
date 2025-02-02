$(document).ready(function () {
    const $avatar = $('.avatar-img');
    var username = '';
    const urlParams = new URLSearchParams(window.location.search);
    username = urlParams.get('username');

    if (!username) {
        $("#myPostsLink").addClass('active');
    }


    const apiUrl = username ? `/api/Profile/GetProfile?username=${username}` : '/api/Profile/GetProfile'
    function fetchProfile() {
        $.ajax({
            url: apiUrl,
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            success: function (data) {
                if (data) {
                    $('#profileInfo').show();
                    $('#line').show();
                    $('#userName').text(`${data.firstName} ${data.lastName}`);
                    $('#userAddress').text(data.address);
                    $('#userDescription').text(data.bio);
                    // Profile exists, populate form fields
                    $avatar.attr('src', data.profilePicturePath || './assets/avatars/default.png');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching profile:', error);
            }
        });
    }

    fetchProfile();

});

