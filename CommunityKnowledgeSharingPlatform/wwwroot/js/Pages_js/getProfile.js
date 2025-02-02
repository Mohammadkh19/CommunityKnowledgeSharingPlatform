

$(document).ready(function () {
    // api to show profile image in dashboard
    const $avatar = $('.avatar-img');
    $.ajax({
        url: `api/Profile/GetProfile`,
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        success: function (data) {
            if (data) {
                $avatar.attr('src', data.profilePicturePath || './assets/avatars/default.png');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching profile:', error);
        }
    });
});