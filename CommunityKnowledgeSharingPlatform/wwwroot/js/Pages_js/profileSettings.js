$(document).ready(function () {
    const apiUrl = '/api/Profile'; 
    const $form = $('#profileForm');
    const $imagePreview = $('#imagePreview');
    const $avatar = $('.avatar-img'); 
    const $imageUpload = $('#imageUpload');
    let isProfileExisting = false;

    // Fetch the profile data on page load
    fetchProfile();
    function fetchProfile() {
        $.ajax({
            url: `${apiUrl}/GetProfile`,
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
                    isProfileExisting = true;
                    $('#firstname').val(data.firstName);
                    $('#lastname').val(data.lastName);
                    $('#bio').val(data.bio);
                    $('#address').val(data.address);
                    $avatar.attr('src', data.profilePicturePath || './assets/avatars/default.png');
                    $imagePreview.attr('src', data.profilePicturePath || './assets/avatars/default.jpg');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching profile:', error);
            }
        });
    }

    // Handle form submission
    $form.on('submit', function (event) {
        event.preventDefault(); // Prevent default form submission

        const formData = new FormData(this);
        if ($imageUpload[0].files[0]) {
            formData.append('profilePicture', $imageUpload[0].files[0]);
        }

        // Determine whether to call AddProfile or UpdateProfile
        const endpoint = isProfileExisting ? `${apiUrl}/UpdateProfile` : `${apiUrl}/AddProfile`;
        const method = isProfileExisting ? 'PUT' : 'POST';

        $.ajax({
            url: endpoint,
            method: method,
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('authToken')}` 
            },
            processData: false, // Prevent jQuery from processing the data
            contentType: false, // Prevent jQuery from setting the Content-Type header
            data: formData,
            success: function (data) {
                if (data) {
                    fetchProfile();
                    toastr.success(isProfileExisting ? 'Profile saved successfully!' : 'Profile created successfully!');
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error saving profile:', error);
            }
        });
    });


    $imageUpload.on('change', function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();

            // When the file is loaded, set the image source to the preview
            reader.onload = function (e) {
                $imagePreview.attr('src', e.target.result); // Set the preview image
            };

            reader.readAsDataURL(file); // Read the file as a data URL
        }
    });
});

