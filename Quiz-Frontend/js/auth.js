function onRoleChange() {
    const role = document.querySelector('input[name="role"]:checked').value;
    const emailInput = document.getElementById("email");
    const hint = document.getElementById("loginHint");

    if (role === "admin") {
        emailInput.placeholder = "Username";
        emailInput.type = "text";
        hint.innerText = "Admin demo: admin / admin123";
    } else {
        emailInput.placeholder = "Email";
        emailInput.type = "email";
        hint.innerText = "Player: use your registered email";
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
