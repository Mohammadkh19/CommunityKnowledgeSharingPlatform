function requireAuth() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        alert('You must log in first!');
        window.location.href = '/auth-login.html';
    }
}
