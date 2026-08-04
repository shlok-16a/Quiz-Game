let authMode = "login"; // "login" | "register"

function onRoleChange() {
    const role = document.querySelector('input[name="role"]:checked').value;
    const emailInput = document.getElementById("email");
    const hint = document.getElementById("loginHint");
    const authSwitch = document.getElementById("authSwitch");

    if (role === "admin") {
        // Admin has no register flow
        if (authMode === "register") {
            setAuthMode("login");
        }
        emailInput.placeholder = "Username";
        emailInput.type = "text";
        hint.innerText = "Admin demo: admin / admin123";
        authSwitch.style.display = "none";
    } else {
        emailInput.placeholder = "Email";
        emailInput.type = "email";
        hint.innerText = authMode === "register"
            ? "Create a player account to start quizzes"
            : "Player: use your registered email";
        authSwitch.style.display = "block";
    }
}

function setAuthMode(mode) {
    authMode = mode;
    const isRegister = mode === "register";

    document.getElementById("pageTitle").innerText = isRegister ? "Create Account" : "Quiz Login";
    document.getElementById("registerFields").style.display = isRegister ? "block" : "none";
    document.getElementById("roleRow").style.display = isRegister ? "none" : "flex";
    document.getElementById("primaryBtn").innerText = isRegister ? "Register" : "Login";
    document.getElementById("authSwitchLink").innerText = isRegister ? "Login here" : "Register here";
    document.getElementById("authSwitchText").innerText = isRegister
        ? "Already have an account?"
        : "New player?";

    if (isRegister) {
        document.querySelector('input[name="role"][value="user"]').checked = true;
        document.getElementById("loginHint").innerText = "Create a player account to start quizzes";
    }

    onRoleChange();
}

function toggleAuthMode(event) {
    event.preventDefault();
    setAuthMode(authMode === "login" ? "register" : "login");
}

async function submitAuth() {
    if (authMode === "register") {
        await register();
        return;
    }
    await login();
}

async function register() {
    const fullName = document.getElementById("fullName").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;

    if (!fullName) {
        alert("Full name is required.");
        return;
    }
    if (!email) {
        alert("Email is required.");
        return;
    }
    if (!password || password.length < 4) {
        alert("Password must be at least 4 characters.");
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/Auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ fullName, email, password })
        });

        const data = await response.json().catch(() => null);

        if (!response.ok) {
            alert(
                (typeof data === "string" && data) ||
                data?.title ||
                data?.message ||
                "Unable to register."
            );
            return;
        }

        alert("Account created. Please login.");
        document.getElementById("password").value = "";
        setAuthMode("login");
        document.getElementById("email").value = email;
    } catch (err) {
        console.error(err);
        alert("Unable to connect to server.");
    }
}

async function login() {
    const role = document.querySelector('input[name="role"]:checked').value;
    const emailOrUser = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;

    if (role === "admin") {
        if (emailOrUser === ADMIN_USER && password === ADMIN_PASS) {
            localStorage.setItem("adminLoggedIn", "true");
            localStorage.removeItem("token");
            window.location.href = "admin-dashboard.html";
            return;
        }

        alert("Invalid admin credentials.");
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/Auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                email: emailOrUser,
                password
            })
        });

        if (!response.ok) {
            alert("Invalid email or password");
            return;
        }

        const data = await response.json();
        localStorage.setItem("token", data.token);
        localStorage.removeItem("adminLoggedIn");
        window.location.href = "categories.html";
    } catch (err) {
        console.error(err);
        alert("Unable to connect to server.");
    }
}

onRoleChange();
