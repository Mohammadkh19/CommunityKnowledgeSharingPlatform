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
});
