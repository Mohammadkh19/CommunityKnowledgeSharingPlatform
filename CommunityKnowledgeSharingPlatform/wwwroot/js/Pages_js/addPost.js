$(document).ready(function () {
    // Fetch categories when the page loads
    fetchCategories();

    $('.new-post').on('click', function () {
        $('#addPostForm').submit();
    });


    // Form submission handler
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

    function validateForm() {
        const title = $('#title').val();
        const categoryId = $('#category').val();
        const description = $('#description').val();

        if (!title || !categoryId || !description) {
            alert('All fields are required.');
            return false;
        }
        return true;
    }


    function addPost() {
        if (!validateForm()) return;

        const postData = {
            postTitle: $('#title').val(),
            categoryId: $('#category').val(),
            postDescription: $('#description').val(),
        };

        // Make API call to add the post
        $.ajax({
            url: '/api/Post/createPost', // Replace with your API endpoint
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(postData),
            headers: {
                Authorization: `Bearer ${localStorage.getItem('authToken')}`, // Include auth token if required
            },
            success: function (response) {
                alert('Post added successfully!');
                // Optionally, reset the form
                $('#addPostForm')[0].reset();
            },
            error: function (xhr) {
                alert(xhr.responseJSON?.message || 'Failed to add post. Please try again.');
            },
        });
    }
});
