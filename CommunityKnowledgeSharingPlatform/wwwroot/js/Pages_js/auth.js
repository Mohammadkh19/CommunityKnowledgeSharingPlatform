function requireAuth() {
    const token = localStorage.getItem('authToken');
    if (!token) {
        // Redirect if token is missing
        window.location.href = '/auth-login.html';
        return;
    }

    // Decode the token to check its expiration (works for JWT)
    const payloadBase64 = token.split('.')[1]; // JWT format: header.payload.signature
    const payload = JSON.parse(atob(payloadBase64)); // Decode the payload from Base64

    const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
    if (payload.exp && payload.exp < currentTime) {
        // Token has expired
        localStorage.removeItem('authToken'); // Optional: clear the expired token
        window.location.href = '/auth-login.html';
        return;
    }
}
