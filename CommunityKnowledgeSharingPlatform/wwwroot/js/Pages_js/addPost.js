$(document).ready(function () {
    // Get postId from query parameters
    const urlParams = new URLSearchParams(window.location.search);
    const postId = urlParams.get('postId');

    if (postId) {
        // If postId exists, fetch the post details
        fetchPostDetails(postId);
        $('.page-title').text("Update Post")
    }
    else {
        $('.page-title').text("Add Post")
    }


    // Fetch categories when the page loads
    fetchCategories();

    $('.new-post').on('click', function () {
        $('#addPostForm').submit();
    });


    $('#addPostForm').on('submit', function (e) {
        e.preventDefault();
        addPost();
    });



    function fetchCategories() {
        $.ajax({
            url: '/api/categories/getAllCategories', 
            method: 'GET',
            success: function (response) {
                const categoryDropdown = $('#category');
                response.categories.forEach(category => {
                    categoryDropdown.append(
                        `<option value="${category.categoryId}">${category.categoryName}</option>`
                    );
                });
            },
            error: function () {
                alert('Failed to fetch categories. Please try again.');
            },
        });
    }

    function addPost() {

        const postData = {
            postId : postId ? postId : 0,
            postTitle: $('#title').val(),
            categoryId: $('#category').val(),
            postDescription: window.quill.root.innerHTML,
        };

        const url = postId
            ? `/api/Post/editPost/` // Update API for editing a post
            : '/api/Post/createPost'; // Create API for adding a new post

        const method = postId ? 'PUT' : 'POST';

        // Make API call to add the post
        $.ajax({
            url: url, 
            method: method,
            contentType: 'application/json',
            data: JSON.stringify(postData),
            headers: {
                Authorization: `Bearer ${localStorage.getItem('authToken')}`, 
            },
            success: function (response) {
                toastr.success(postId ? 'Post saved successfully!' : 'Post added successfully!');
                // Reset the form
                if (!postId) {
                    $('#addPostForm')[0].reset();
                    window.quill.root.innerHTML = '';
                }
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Failed to add post. Please try again.');
            },
        });
    }


    // Function to fetch post details
    function fetchPostDetails(postId) {
        $.ajax({
            url: `/api/Post/getPosts/${postId}`, // Backend API to fetch post details
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            success: function (response) {
                // Populate the form with the post data

                $('#title').val(response.postTitle);
                $('#category').val(response.categoryId).change(); // Set the category dropdown
                window.quill.clipboard.dangerouslyPasteHTML(response.postDescription);
            },
            error: function () {
                alert('Failed to fetch post details. Please try again.');
            },
        });
    }


});
