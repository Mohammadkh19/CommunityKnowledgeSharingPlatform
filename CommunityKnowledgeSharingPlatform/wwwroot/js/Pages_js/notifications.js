$(document).ready(function () {
    loadNotifications();

    // Refresh notifications every 30 seconds 
    setInterval(loadNotifications, 30000);

    $('#clearAllNotifications').on('click', function () {
        $('.notification-item').each(function () {
            const notificationId = $(this).data('id');
            markNotificationAsRead(notificationId, $(this));
          
        });
    });

});

// Function to load notifications from the API
function loadNotifications() {
    $.ajax({
        url: '/api/Notification/getNotifications',
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        dataType: 'json',
        success: function (notifications) {
            renderNotifications(notifications);
        },
        error: function (error) {
            console.error("Error fetching notifications:", error);
        }
    });
}

// Function to render notifications inside the modal
function renderNotifications(notifications) {
    const notificationsContainer = $('.modal-body .list-group'); // Target the list group in the modal
    notificationsContainer.empty(); // Clear old notifications

    if (notifications.length === 0) {
        notificationsContainer.append(`
            <div class="list-group-item text-center text-muted">
                No new notifications
            </div>
        `);
        return;
    }

    notifications.forEach(notification => {
        if (!notification.isRead) {  // Only show unread notifications
            notificationsContainer.append(`
                <div class="list-group-item bg-transparent notification-item" data-id="${notification.notificationId}">
                    <div class="row align-items-center">
                        <div class="col-auto">
                            <span class="fe fe-bell fe-24"></span>
                        </div>
                        <div class="col">
                            <small><strong>${notification.notificationText}</strong></small>
                            <div class="my-0 text-muted small">${new Date(notification.notificationDate).toLocaleString()}</div>
                            <small class="badge badge-pill badge-light text-muted">Unread</small>
                        </div>
                    </div>
                </div>
            `);
        }
    });

    // Attach click event to mark notification as read
    $('.notification-item').on('click', function () {
        const notificationId = $(this).data('id');
        markNotificationAsRead(notificationId, $(this));
    });
}

function markNotificationAsRead(notificationId, element) {
    $.ajax({
        url: `/api/Notification/markAsRead/${notificationId}`,
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        },
        success: function () {
            element.fadeOut(300, function () {
                $(this).remove(); // Remove the notification from the list
            });
        },
        error: function (error) {
            console.error("Error marking notification as read:", error);
        }
    });
}

