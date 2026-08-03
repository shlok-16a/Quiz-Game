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
        const timerSec = Math.max(1, Number(quiz.durationSeconds) || 10);
        const from = formatQuizIst(quiz.startDate);
        const until = formatQuizIst(quiz.endDate);
        const action = quiz.hasAttempted
            ? `<button type="button" disabled>Already Attempted</button>`
            : `<button type="button" onclick="startQuiz(${quiz.id})">Start Quiz</button>`;

        container.innerHTML += `
            <div class="card">
                <h3>${quiz.title}</h3>
                <p>Category: ${quiz.categoryName}</p>
                <p>Questions: ${quiz.assignedQuestions}</p>
                <p>Timer: ${timerSec} sec / question</p>
                ${from ? `<p class="muted">Available from: ${from}</p>` : ""}
                ${until ? `<p class="muted">Available until: ${until}</p>` : ""}
                ${action}
            </div>
            <br>
        `;
    });
}

function formatQuizIst(iso) {
    if (!iso) return null;
    const text = formatIst(iso);
    return text === "—" ? null : text;
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
