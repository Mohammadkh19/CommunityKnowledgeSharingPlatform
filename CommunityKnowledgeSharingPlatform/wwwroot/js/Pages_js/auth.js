function requireAuth() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        window.location.href = '/auth-login.html';
    }
}
