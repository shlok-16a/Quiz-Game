window.onload = loadQuizzes;

async function loadQuizzes() {
    const token = localStorage.getItem("token");

    if (!token) {
        window.location.href = "index.html";
        return;
    }

    const response = await fetch(`${API_BASE_URL}/api/Quiz/available`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    if (!response.ok) {
        alert("Unable to load quizzes. Please login again.");
        window.location.href = "index.html";
        return;
    }

    const quizzes = await response.json();
    const container = document.getElementById("quizzes");
    container.innerHTML = "";

    if (!quizzes.length) {
        container.innerHTML = `<p class="muted">No active quizzes available right now.</p>`;
        return;
    }

    quizzes.forEach(quiz => {
        const mins = Math.max(1, Math.round(quiz.durationSeconds / 60));
        container.innerHTML += `
            <div class="card">
                <h3>${quiz.title}</h3>
                <p>Category: ${quiz.categoryName}</p>
                <p>Questions: ${quiz.assignedQuestions}</p>
                <p>Duration: ${mins} min</p>
                <button onclick="startQuiz(${quiz.id})">Start Quiz</button>
            </div>
            <br>
        `;
    });
}

async function startQuiz(quizId) {
    const token = localStorage.getItem("token");

    const response = await fetch(`${API_BASE_URL}/api/Quiz/start`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ quizId })
    });

    const data = await response.json();

    if (!response.ok) {
        alert(data.title || data || "Unable to start quiz");
        return;
    }

    localStorage.setItem("quiz", JSON.stringify(data));
    localStorage.setItem("quizQuestionNumber", "1");
    window.location.href = "quiz.html";
}
