$(document).ready(function () {


    // Get PostId from the url
    const urlParams = new URLSearchParams(window.location.search);
    const postId = urlParams.get('postId');
    const openComment = urlParams.get('openComment'); // Get the openComment parameter

    if (openComment === 'true') {
        // Automatically focus and open the comment input
        const commentInput = $('#commentText'); // Replace with your input ID
        if (commentInput) {
            commentInput.focus(); // Focus the input
        }
    }

    if (!postId) {
        // Redirect to index.html if no postId is found
        window.location.href = 'index.html';
        return;
    }

    fetchPostData(postId).done(function (postData) {
        if (postData) {
            populatePostDetails(postData);
        } else {
            alert('No post found with the given ID.');
        }
    });

    $('.reaction .like').on('click', function () {
        vote(postId, true); // true indicates "like"
    });

    // Dislike button click handler
    $('.reaction .dislike').on('click', function () {
        vote(postId, false); // false indicates "dislike"
    });

    const commentInput = $('#commentText');
    commentInput.on("keydown", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            // Prevent new line and confirm edit on Enter
            event.preventDefault();
            $("#postComment").click(); // Simulate button click to submit edit
        }
    });

    $("#postComment").on("click", function () {
        const commentText = $("#commentText").val();

        if (!commentText) {
            toastr.warning("Comment cannot be empty.");
            return;
        }

        postComment(postId, commentText);
    });

    var commentId = 0;
    var isPostDelete = false;
    var isCommentDelete = false;

    $(document).on("click", "#deleteCommentBtn", function () {
        commentId = $(this).data("commentid");
        isCommentDelete = true;
        $('.modal-title').text("Are you sure want to delete this Comment?");
    });

    $(document).on("click", "#deletePostBtn", function () {
        isPostDelete = true;
        $('.modal-title').text("Are you sure want to delete this Post?");
    });

    


    $("#confirmDeleteBtn").on("click", function () {
        if (isCommentDelete) {
            deleteComment(commentId);
            isCommentDelete = false;
        }
        else if (isPostDelete) {
            deletePost(postId);
            isPostDelete = false;
        }
        $("#delete_modal").modal("hide");
    });


    $(document).on("click", "#editCommentBtn", function () {
        const commentId = $(this).data("commentid");
        const commentElement = $(`#comment-${commentId}`);
        const currentText = commentElement.find(".comment-text").text().trim();

        // Replace the comment text with an input field and send button
        commentElement.find(".comment-text").replaceWith(`
            <div class="comment-box d-flex align-items-center">
                <textarea id="dynamicTextarea"
                          class="form-control comment-input"
                          rows="1"
                          >${currentText}</textarea>
                <button id="sendEditCommentBtn" class="btn edit-send-btn" data-commentid="${commentId}">
                    <i class="fa fa-paper-plane"></i>
                </button>
            </div>

            `);

        const textarea = commentElement.find("#dynamicTextarea");
        const button = $("#sendEditCommentBtn");


        textarea.focus();
        const textLength = textarea.val().length;
        textarea[0].setSelectionRange(textLength, textLength);


        // Listen for ESC key press to cancel the edit
        textarea.on("keydown", function (event) {
            if (event.key === "Escape") {
                // Revert back to the original text
                commentElement.find(".comment-box").replaceWith(`
                <span class="comment-text">${currentText}</span>
            `);
            }
            else if (event.key === "Enter" && !event.shiftKey) {
                // Prevent new line and confirm edit on Enter
                event.preventDefault();
                button.click(); // Simulate button click to submit edit
            }
        });


        // Listen for input in the textarea
        textarea.on("input", function () {
            if ($.trim(textarea.val()) === "") {
                // Disable the button if textarea is empty or contains only spaces
                button.prop("disabled", true);
            } else {
                // Enable the button if textarea contains text
                button.prop("disabled", false);
            }
        });
    });

    const textarea = $("#commentText");
    const button = $("#postComment");


    // Listen for input in the textarea
    textarea.on("input", function () {
        if ($.trim(textarea.val()) === "") {
            // Disable the button if textarea is empty or contains only spaces
            button.prop("disabled", true);
        } else {
            // Enable the button if textarea contains text
            button.prop("disabled", false);
        }
    });

    $(document).on("input", "#dynamicTextarea", function () {
        // Reset height to calculate new height dynamically
        this.style.height = "auto";

        // Set the new height based on the scroll height
        this.style.height = `${this.scrollHeight}px`;
    });

    $(document).on("input", "#commentText", function () {
        // Reset height to calculate new height dynamically
        this.style.height = "auto";

        // Set the new height based on the scroll height
        this.style.height = `${this.scrollHeight}px`;
    });


    // Handle the send button click
    $(document).on("click", "#sendEditCommentBtn", function () {
        const commentId = $(this).data("commentid");
        const commentElement = $(`#comment-${commentId}`);
        const updatedText = commentElement.find(".comment-input").val();
        updateComment(commentId, updatedText);
       
    });


    // Handle the edit post btn click
    $(document).on('click', '#editPostBtn', function () {
        // Navigate to the addPost page with postId as a query parameter
        window.location.href = `new-post.html?postId=${postId}`;
    });



});

function fetchPostData(postId) {
    return $.ajax({
        url: `api/Post/getPosts/${postId}`, 
        method: 'GET',
        dataType: 'json',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        error: function (xhr, status, error) {
            console.error(`Error fetching post: ${error}`);
            toastr.error('Failed to load post data.');
        }
    });
}


function populatePostDetails(postData) {
    // Update the post title, description, and metadata

    const formattedTime = formatToLocalDateTime(postData.postedAt);
    $('.postedAt').text(`${formattedTime}`);

    $('.profile-link').text(postData.postTitle);
    $('.post-text p').html(postData.postDescription);

    // Update profile picture
    $('.profile-photo-md').attr('src', postData.profilePicturePath || './assets/avatars/default.jpg');

    // Update reaction counts
    $('.reaction .like').html(`<i class="fa fa-thumbs-up like-button ${postData.isLiked ? 'text-primary' : ''}"></i><span class="like-count">${postData.upvoteCount} </span>`);
    $('.reaction .dislike').html(`<i class="fa fa-thumbs-down dislike-button ${postData.isDisliked ? 'text-danger' : ''}"></i> <span class="dislike-count"> ${postData.downvoteCount} </span>`);

    // Populate comments
    const commentsContainer = $('.post-detail');
    postData.comments.forEach(comment => {
        const isCurrentUser = comment.isCurrentUserComment;
        const commentHTML = `
        <div id="comment-${comment.commentId}" class="post-comment d-flex align-items-center justify-content-between">
            <div class="d-flex align-items-center w-100">
                <img src="${comment.profilePicturePath || './assets/avatars/default.jpg'}" alt="" class="profile-photo-sm">
                <a href="#" class="profile-link mr-2">${comment.username}</a>
                <span class="comment-text"> ${comment.text}</span>
            </div>

            ${(isCurrentUser || isPostOwner) ? `
            <div class="btn-group dropleft ml-auto">
                <button type="button" class="btn action-icon" data-toggle="dropdown" aria-expanded="false">
                    <i class="fa fa-ellipsis-v"></i>
                </button>
                <ul class="dropdown-menu dropdown-menu-right">
                    ${isCurrentUser ? `
                    <li><a class="dropdown-item" id="editCommentBtn" data-commentId="${comment.commentId}">
                        <i class="fa fa-pencil"></i> Edit
                    </a></li>
                    ` : ''} <!-- Show edit only if the comment belongs to the current user -->
                    
                    <li><a class="dropdown-item" id="deleteCommentBtn" data-toggle="modal" data-target="#delete_modal"
                        data-commentId="${comment.commentId}">
                        <i class="fa fa-trash"></i> Delete
                    </a></li> <!-- Delete is always available if it's the post owner or the comment owner -->
                </ul>
            </div>
            ` : ''}
         </div>`;
        commentsContainer.append(commentHTML);
    });

    const reactionsContainer = $('.reaction');
    if (postData.isMyPost) {
        // Append dropdown if it's the current user's post
        const dropdownHtml = `
           <div class="btn-group dropleft ml-auto">
                <button type="button" class="btn action-icon" data-toggle="dropdown" aria-expanded="false">
                    <i class="fa fa-ellipsis-v"></i>
                </button>
                <ul class="dropdown-menu dropdown-menu-right">
                    <li><a class="dropdown-item" id="editPostBtn" data-postId="${postData.postId}"><i class="fa fa-pencil"></i> Edit</a></li>
                    <li><a class="dropdown-item" id="deletePostBtn" data-toggle="modal" data-target="#delete_modal"
                    data-postId="${postData.postId}"><i class="fa fa-trash"></i> Delete</a></li>
                </ul>
            </div>
            `;
        reactionsContainer.append(dropdownHtml); // Append dropdown to the reaction div
    }
}

function formatToLocalDateTime(isoDateString) {
    const date = new Date(isoDateString); // Parse the ISO date string

    // Format the date and time to local time
    const options = {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false // Use true for 12-hour format
    };

    return date.toLocaleString(undefined, options); // Localize based on user's browser settings
}



function vote(postId, isUpvote) {
    const apiUrl = 'api/Vote/vote'; // API endpoint
    const payload = { postId, isUpvote }; // Data to send to the API

    // Find the button container for the given post
    const likeButton = $('.like-button');
    const dislikeButton = $('.dislike-button');
    const likeCount = $('.like-count');
    const dislikeCount = $('.dislike-count');

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
            console.log(isLiked);
            if (isUpvote) {
                if (isLiked) {
                    console.log(isLiked);
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
            toastr.error('An error occurred while voting. Please try again.');
        }
    });
}


function postComment(postId, content) {
    $.ajax({
        url: `api/Comment/AddComment`,
        method: "POST",
        headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
            "Content-Type": "application/json",
        },
        data: JSON.stringify({ postId, content }),
        success: function (response) {
            toastr.success('Comment added successfully!');

            // Clear the comment box
            $("#commentText").val("");
            $("#commentText").css("height", "");
            $("#postComment").prop("disabled", true);

            // Append the new comment to the comments container
            const newCommentHTML = `
                <div id="comment-${response.comment.commentId}" class="post-comment d-flex align-items-center">
                   <div class="d-flex align-items-center flex-grow-1">
                      <img src="${response.comment.profilePicturePath ? response.comment.profilePicturePath : './assets/avatars/default.jpg'}" alt="" class="profile-photo-sm">

                      <a href="#" class="profile-link mr-2">${response.comment.username}</a>
                      <span class="comment-text"> ${response.comment.commentText}</span>
                   </div>
                   <div class="btn-group dropup">
                       <button type="button" class="btn action-icon" data-toggle="dropdown" aria-expanded="false">
                           <i class="fa fa-ellipsis-v"></i>
                       </button>
                       <ul class="dropdown-menu dropdown-menu-right">
                           <li><a class="dropdown-item" id="editCommentBtn" data-commentId="${response.comment.commentId}"><i class="fa fa-pencil"></i> Edit</a></li>
                           <li><a class="dropdown-item" id="deleteCommentBtn" data-toggle="modal" data-target="#delete_modal"
                           data-commentId="${response.comment.commentId}"><i class="fa fa-trash"></i> Delete</a></li>
                       </ul>
                  </div>
                </div>
                `;
            $(".post-detail").append(newCommentHTML);

            // Hide the comment form and show the add comment button again
            $("#commentForm").fadeOut(function () {
                $("#addCommentBtnForm").fadeIn();
            });
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
            toastr.error("Failed to add comment. Please try again.");
        },
    });
}



function deleteComment(commentId) {
    const commentElement = $(`#comment-${commentId}`);

    $.ajax({
        url: `api/Comment/deleteComment/${commentId}`,
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
        },
        success: function (response) {
            commentElement.remove();
            toastr.success('Comment deleted successfully!');

        },
        error: function (xhr, status, error) {
            console.error("Failed to delete comment:", error);
        },
    });
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


function updateComment(commentId, updatedText) {
    if (!updatedText) {
        toastr.warning("Comment cannot be empty.");
        return;
    }

    // Send the updated comment to the server
    $.ajax({
        url: `api/Comment/editComment/`,
        method: "PUT",
        headers: {
            Authorization: `Bearer ${localStorage.getItem("authToken")}`,
            "Content-Type": "application/json",
        },
        data: JSON.stringify({ commentId: commentId, content: updatedText }),
        success: function (response) {
            // Replace the input field with the updated comment text
            const commentElement = $(`#comment-${commentId}`);
            commentElement.find(".comment-box").replaceWith(`
                <span class="comment-text">${updatedText}</span>
            `);
            toastr.success('Comment saved successfully!');
        },
        error: function (xhr) {
            console.error("Failed to update comment:", xhr.responseJSON || xhr.responseText);
            toastr.error("Failed to update comment. Please try again.");
        },
    });
}
