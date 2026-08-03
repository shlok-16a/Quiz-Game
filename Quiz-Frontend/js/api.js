const API_BASE_URL = "http://localhost:5260";

const ADMIN_USER = "admin";
const ADMIN_PASS = "admin123";

function requireAdmin() {
    if (localStorage.getItem("adminLoggedIn") !== "true") {
        window.location.href = "index.html";
    }
}

function adminLogout() {
    localStorage.removeItem("adminLoggedIn");
    window.location.href = "index.html";
}

function userLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("quiz");
    localStorage.removeItem("quizQuestionNumber");
    localStorage.removeItem("resultSession");
    window.location.href = "index.html";
}

async function apiGet(path) {
    const response = await fetch(`${API_BASE_URL}${path}`);
    if (!response.ok) throw new Error(await response.text());
    return response.json();
}

async function apiSend(path, method, body) {
    const response = await fetch(`${API_BASE_URL}${path}`, {
        method,
        headers: { "Content-Type": "application/json" },
        body: body ? JSON.stringify(body) : undefined
    });

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || "Request failed");
    }

    if (response.status === 204) return null;
    return response.json();
}
