$(document).ready(function () {

    const loginMessage = localStorage.getItem('loginSuccessMessage');
    const registerMessage = localStorage.getItem('registerSuccessMessage');

    const successMessage = loginMessage || registerMessage;

    if (successMessage) {
        const Toast = Swal.mixin({
            toast: true,
            position: "top-end",
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: (toast) => {
                toast.onmouseenter = Swal.stopTimer;
                toast.onmouseleave = Swal.resumeTimer;
            }
        });
        Toast.fire({
            icon: "success",
            title: successMessage
        });

        // Clear the messages after displaying them
        localStorage.removeItem('loginSuccessMessage');
        localStorage.removeItem('registerSuccessMessage');
    }

    const success = localStorage.getItem('success');
    if (success) {
        toastr.success(success);
        localStorage.removeItem('success');
    }

    // api to fetch categories

    $.ajax({
        url: 'api/Categories/getAllCategories',
        method: 'GET',
        dataType: 'json', // Expect JSON response
        success: function (response) {
            console.log(response.categories);
            renderCategories(response.categories);
        },
        error: function () {
            console.error('Error fetching categories');
        }
    });

    let username = '';
    const urlParams = new URLSearchParams(window.location.search);
    username = urlParams.get('username');
  

    if (username !== null) {
        fetchPosts(null, username, null);
    } else if (window.location.pathname.endsWith("profile-view.html")) {
        username = getUsernameFromToken();
        fetchPosts(null, username, null);
    }
    else {
        fetchPosts();
    }

    function renderCategories(categories) {
        const categoriesContainer = $('#categories');

        // Add "All Categories" button
        categoriesContainer.append(`
        <h6 class="px-3 py-2 btn text-uppercase cursor-pointer category-item active-category" 
            data-category="" data-username="${username}">
            All Categories
        </h6>
    `);

        // Add each category as a clickable item
        categories.forEach(function (category) {
            categoriesContainer.append(`
            <span 
                class="btn rounded-pill m-1 px-3 py-2 text-uppercase category-item"
                data-category="${category.categoryId}" 
                data-username="${username}">
                ${category.categoryName}
            </span>
        `);
        });

        $('.category-item').on('click', function () {
            $('.category-item').removeClass('active-category'); // Remove underline from all
            $(this).addClass('active-category'); // Add underline to the clicked one

            const categoryId = $(this).data('category');
            const username = $(this).data('username');
            fetchPosts(null, username, categoryId);
        });
    }



    // Handle the edit post btn click
    $(document).on('click', '#editPostBtn', function () {
        // Navigate to the addPost page with postId as a query parameter
        const postId = $(this).data("postid");
        window.location.href = `new-post.html?postId=${postId}`;
    });

    $(document).on("click", "#deletePostBtn", function () {
        postId = $(this).data("postid");
    });

    $("#confirmDeleteBtn").on("click", function () {
        deletePost(postId);
        $("#delete_modal").modal("hide");
    });

    $(document).on("click", ".share-button", function () {
        postId = $(this).data("postid");
        const shareUrl = `${window.location.origin}/post-view.html?postId=${postId}`;

        // Populate the modal with the sharing options
        $('#copyLink').data('url', shareUrl); // Store the URL for copying

        // Open the modal
        $('#shareModal').modal('show');

        // Generate Facebook, Twitter, and WhatsApp links
        const facebookShareUrl = `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(shareUrl)}`;
        const twitterShareUrl = `https://twitter.com/intent/tweet?url=${encodeURIComponent(shareUrl)}&text=Check out this post!`;
        const whatsappShareUrl = `https://wa.me/?text=${encodeURIComponent(`Check out this post: ${shareUrl}`)}`;

        // Set the href attributes for sharing links
        $('#shareFacebook').attr('href', facebookShareUrl).attr('target', '_blank');
        $('#shareTwitter').attr('href', twitterShareUrl).attr('target', '_blank');
        $('#shareWhatsApp').attr('href', whatsappShareUrl).attr('target', '_blank');
    });

    // Handle "Copy Link" click
    $('#copyLink').on('click', function (e) {
        e.preventDefault();

        // Get the share URL
        const shareUrl = $(this).data('url');

        // Copy the URL to the clipboard
        navigator.clipboard.writeText(shareUrl).then(() => {
            alert('Link copied to clipboard!');
        }).catch(err => {
            console.error('Failed to copy link: ', err);
        });
    });

});

let currentSearch = '';
let currentCategoryId = '';
let currenUsername = '';
let currentPage = 1; // Track the current page
let pageSize = 5; // Number of posts per page

function fetchPosts(search = null, username = null , categoryId = null,currentPage = 1, pageSize = 2) {
    const apiUrl = 'api/Post/getPosts'; // Base API URL
    const postsContainer = $('#postsContainer'); // Container for posts

    // Update global variables if new values are provided
    if (search !== null) currentSearch = search;
    if (categoryId !== null) currentCategoryId = categoryId;
    if (username !== null) currenUsername = username;
    console.log(currenUsername)

    // Build query parameters
    const params = {
        page: currentPage,
        pageSize: pageSize
    };
    if (currentSearch) params.search = currentSearch;
    if (currentCategoryId) params.categoryId = currentCategoryId;
    if (currenUsername) {
        params.userName = currenUsername
    }

    // AJAX request to fetch posts
    $.ajax({
        url: apiUrl,
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        data: params, // Pass the query parameters
        dataType: 'json', // Expect JSON response
        success: function (response) {
            renderPosts(response.items); // Render posts dynamically
            renderPagination(response.currentPage, response.totalPages, response.items.username);
        },
        error: function () {
            postsContainer.html('<p>Error loading posts. Please try again.</p>');
        }
    });
}


function renderPosts(posts) {
    const postsContainer = $('#postsContainer');
    postsContainer.empty(); // Clear existing posts

    if (!posts.length) {
        postsContainer.html('<p>No posts found.</p>');
        return;
    }

    // Loop through and display posts
    posts.forEach(function (post) {
        postsContainer.append(`
        <div class="col-md-8" data-post-id="${post.postId}">
    <div class="media g-mb-30 media-comment" data-post-username="${post.userName}">
      <a href="profile-view.html?username=${post.userName}">
    <img
        class="d-flex g-width-50 g-height-50 rounded-circle g-mt-3 g-mr-15"
        src="${post.profilePicture}" 
        alt="Image Description">
</a>

        <div class="media-body u-shadow-v18 g-bg-secondary g-pa-30">
            <div class="g-mb-15 d-flex justify-content-between align-items-center">
               <h5 class="h5 g-color-gray-dark-v1 mb-0">
                    <a href="/post-view.html?postId=${post.postId}" class="post-title-link">
                        ${post.postTitle}
                    </a>
                </h5>
${post.isMyPost ? `
<div class="btn-group dropleft">
    <button type="button" class="btn action-icon" data-toggle="dropdown" aria-expanded="false">
        <i class="fa fa-ellipsis-v"></i>
    </button>
    <ul class="dropdown-menu dropdown-menu-right">
        <li><a class="dropdown-item" id="editPostBtn" data-postId="${post.postId}"><i class="fa fa-pencil"></i> Edit</a></li>
        <li>
            <a class="dropdown-item" id="deletePostBtn" data-toggle="modal" data-target="#delete_modal" data-postId="${post.postId}">
                <i class="fa fa-trash"></i> Delete
            </a>
        </li>
    </ul>
</div>
` : ''}

            </div>
            <span class="g-color-gray-dark-v4 g-font-size-12 ">${post.postedAtRelative}</span>

            <div class="mt-3 description">
                ${post.postDescription}
            </div>

            <ul class="list-inline d-sm-flex my-0">
                <li class="list-inline-item g-mr-20">
                    <a class="u-link-v5 g-color-gray-dark-v4 g-color-primary--hover" 
                       href="#!" 
                       onclick="vote(${post.postId}, true)">
                        <i class="like-button fa fa-thumbs-up g-pos-rel g-top-1 g-mr-3 ${post.isLiked ? 'text-primary' : ''}"></i>
                        <span class="like-count">${post.upvoteCount}</span>
                    </a>
                </li>
                <li class="list-inline-item g-mr-20">
                    <a class="u-link-v5 g-color-gray-dark-v4 g-color-primary--hover" 
                       href="#!" 
                       onclick="vote(${post.postId}, false)">
                        <i class="dislike-button fa fa-thumbs-down g-pos-rel g-top-1 g-mr-3 ${post.isDisliked ? 'text-danger' : ''}"></i>
                        <span class="dislike-count">${post.downvoteCount}</span>
                    </a>
                </li>
                <li class="list-inline-item g-mr-20">
                    <a class="u-link-v5 g-color-gray-dark-v4 g-color-primary--hover" href="/post-view.html?postId=${post.postId}&openComment=true">
                        <i class="fa fa-comment g-pos-rel g-top-1 g-mr-3"></i>
                        ${post.commentsCount}
                    </a>
                </li>
                <li class="list-inline-item ml-auto">
                    <a class="u-link-v5 g-color-gray-dark-v4 g-color-primary--hover share-button" href="#!" data-postId="${post.postId}">
                        <i class="fa fa-share g-pos-rel g-top-1 g-mr-3"></i>
                        share
                    </a>
                </li>
            </ul>
        </div>
    </div>
</div>

        `);
    });

}




function vote(postId, isUpvote) {
    const apiUrl = 'api/Vote/vote'; // API endpoint
    const payload = { postId, isUpvote }; // Data to send to the API

    // Find the button container for the given post
    const postContainer = $(`[data-post-id="${postId}"]`);
    const likeButton = postContainer.find('.like-button');
    const dislikeButton = postContainer.find('.dislike-button');
    const likeCount = postContainer.find('.like-count');
    const dislikeCount = postContainer.find('.dislike-count');

    // AJAX request to handle the vote
    $.ajax({
        url: apiUrl,
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`, // Include auth token if needed
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(payload), // Convert payload to JSON string
        success: function () {
            const isLiked = likeButton.hasClass('text-primary'); // Check if already liked
            const isDisliked = dislikeButton.hasClass('text-danger'); // Check if already disliked

            if (isUpvote) {
                if (isLiked) {
                    
                    // If already liked, undo the like
                    likeButton.removeClass('text-primary').addClass('g-color-gray-dark-v4');
                    likeCount.text(Math.max(0, parseInt(likeCount.text()) - 1));
                } else {
                    // Like the post
                    likeButton.removeClass('g-color-gray-dark-v4').addClass('text-primary');
                    likeCount.text(parseInt(likeCount.text()) + 1);

                    if (isDisliked) {
                        // Undo dislike if previously disliked
                        dislikeButton.removeClass('text-danger').addClass('g-color-gray-dark-v4');
                        dislikeCount.text(Math.max(0, parseInt(dislikeCount.text()) - 1));
                    }
                }
            } else {
                if (isDisliked) {
                    // If already disliked, undo the dislike
                    dislikeButton.removeClass('text-danger').addClass('g-color-gray-dark-v4');
                    dislikeCount.text(Math.max(0, parseInt(dislikeCount.text()) - 1));
                } else {
                    // Dislike the post
                    dislikeButton.removeClass('g-color-gray-dark-v4').addClass('text-danger');
                    dislikeCount.text(parseInt(dislikeCount.text()) + 1);

                    if (isLiked) {
                        // Undo like if previously liked
                        likeButton.removeClass('text-primary').addClass('g-color-gray-dark-v4');
                        likeCount.text(Math.max(0, parseInt(likeCount.text()) - 1));
                    }
                }
            }
        },
        error: function () {
            alert('An error occurred while voting. Please try again.');
        }
    });
}
function renderPagination(currentPage, totalPages, username) {
    const paginationContainer = $('.block-27 ul'); // Pagination container
    paginationContainer.empty(); // Clear existing pagination

    if (totalPages <= 1) return; // If only one page, no pagination needed

    // Previous Button
    if (currentPage > 1) {
        const prevButton = $('<li>')
            .append($('<a>', { href: '#', text: '<' }))
            .on('click', function (e) {
                e.preventDefault();
                fetchPosts(null, username, null, currentPage - 1); // Fetch previous page
            });
        paginationContainer.append(prevButton);
    } else {
        paginationContainer.append(
            $('<li>').addClass('disabled').append($('<span>', { text: '<' }))
        );
    }

    // Page Numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === currentPage) {
            paginationContainer.append(
                $('<li>').addClass('active').append($('<span>', { text: i }))
            );
        } else {
            const pageButton = $('<li>')
                .append($('<a>', { href: '#', text: i }))
                .on('click', function (e) {
                    e.preventDefault();
                    fetchPosts(null, username,null, i); // Fetch selected page
                });
            paginationContainer.append(pageButton);
        }
    }

    // Next Button
    if (currentPage < totalPages) {
        const nextButton = $('<li>')
            .append($('<a>', { href: '#', text: '>' }))
            .on('click', function (e) {
                e.preventDefault();
                fetchPosts(null, username, null, currentPage + 1); // Fetch next page
            });
        paginationContainer.append(nextButton);
    } else {
        paginationContainer.append(
            $('<li>').addClass('disabled').append($('<span>', { text: '>' }))
        );
    }
}

function deletePost(postId) {
    $.ajax({
        url: `api/Post/deletePost/${postId}`,
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
        },
        success: function (response) {
            localStorage.setItem('success', 'Post deleted successfully!')
            window.location.href = '/index.html';

        },
        error: function (xhr, status, error) {
            console.error("Failed to delete this Post:", error);
        },
    });
}

function getUsernameFromToken() {
    const token = localStorage.getItem('authToken');
    if (!token) return null; // No token found

    try {
        // Extract the payload from the token
        const payload = JSON.parse(atob(token.split('.')[1])); // Decode Base64 and parse JSON

        // Extract the username using the correct claim key
        return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || null;
    } catch (error) {
        console.error("Error decoding token:", error);
        return null;
    }
}

