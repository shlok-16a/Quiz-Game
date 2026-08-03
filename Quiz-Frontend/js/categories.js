window.onload = loadQuizzes;

let quizzesById = {};
let pendingQuizId = null;

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
    quizzesById = {};
    quizzes.forEach(q => { quizzesById[q.id] = q; });

    const container = document.getElementById("quizzes");
    container.innerHTML = "";

    if (!quizzes.length) {
        container.innerHTML = `<p class="muted">No active quizzes available right now.</p>`;
        return;
    }

    quizzes.forEach(quiz => {
        const timerSec = Math.max(
            1,
            Number(quiz.questionTimerSeconds ?? quiz.durationSeconds) || 10
        );
        const from = formatQuizIst(quiz.startDate);
        const until = formatQuizIst(quiz.endDate);
        const action = quiz.hasAttempted
            ? `<button type="button" disabled>Already Attempted</button>`
            : `<button type="button" onclick="openStartModal(${quiz.id})">Start Quiz</button>`;

        container.innerHTML += `
            <div class="card">
                <h3>${escapeHtml(quiz.title)}</h3>
                <p>Category: ${escapeHtml(quiz.categoryName)}</p>
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

function escapeHtml(text) {
    return String(text ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;");
}

function openStartModal(quizId) {
    const quiz = quizzesById[quizId];
    if (!quiz) {
        alert("Quiz not found. Refresh and try again.");
        return;
    }

    if (quiz.hasAttempted) {
        alert("You have already attempted this quiz.");
        return;
    }

    pendingQuizId = quizId;

    const timerSec = Math.max(
        1,
        Number(quiz.questionTimerSeconds ?? quiz.durationSeconds) || 10
    );
    const totalQuestions = quiz.assignedQuestions || quiz.questionCount || 0;
    const rules = (quiz.rulesText || "").trim();

    document.getElementById("modalTitle").innerText = quiz.title || "Quiz";
    document.getElementById("modalQuestions").innerText = String(totalQuestions);
    document.getElementById("modalCorrect").innerText = String(quiz.correctPoints ?? 0);
    document.getElementById("modalWrong").innerText = String(quiz.wrongPoints ?? 0);
    document.getElementById("modalTimer").innerText = String(timerSec);

    const rulesBlock = document.getElementById("modalRulesBlock");
    const rulesEl = document.getElementById("modalRules");
    if (rules) {
        rulesEl.innerText = rules;
        rulesBlock.style.display = "block";
    } else {
        rulesEl.innerText = "";
        rulesBlock.style.display = "none";
    }

    document.getElementById("quizStartModal").style.display = "flex";
    document.getElementById("modalOkBtn").focus();
}

function closeStartModal() {
    pendingQuizId = null;
    document.getElementById("quizStartModal").style.display = "none";
    document.getElementById("countdownOverlay").style.display = "none";
    const okBtn = document.getElementById("modalOkBtn");
    const cancelBtn = document.getElementById("modalCancelBtn");
    okBtn.disabled = false;
    okBtn.innerText = "Okay";
    cancelBtn.disabled = false;
}

function onModalBackdrop(event) {
    if (event.target === document.getElementById("quizStartModal")) {
        closeStartModal();
    }
}

function wait(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function runStartCountdown(seconds = 5) {
    const overlay = document.getElementById("countdownOverlay");
    const numberEl = document.getElementById("countdownNumber");
    overlay.style.display = "flex";

    for (let n = seconds; n >= 1; n--) {
        numberEl.innerText = String(n);
        numberEl.classList.remove("countdown-pop");
        // Force reflow so animation can replay each second
        void numberEl.offsetWidth;
        numberEl.classList.add("countdown-pop");
        await wait(1000);
    }

    numberEl.innerText = "Go!";
    await wait(400);
}

async function confirmStartQuiz() {
    if (!pendingQuizId) return;

    const quizId = pendingQuizId;
    const okBtn = document.getElementById("modalOkBtn");
    const cancelBtn = document.getElementById("modalCancelBtn");
    okBtn.disabled = true;
    cancelBtn.disabled = true;
    okBtn.innerText = "Get ready...";

    try {
        document.getElementById("quizStartModal").style.display = "none";
        await runStartCountdown(5);
        await beginQuiz(quizId);
    } catch (err) {
        document.getElementById("countdownOverlay").style.display = "none";
        document.getElementById("quizStartModal").style.display = "flex";
        alert(err.message || "Unable to start quiz");
        okBtn.disabled = false;
        cancelBtn.disabled = false;
        okBtn.innerText = "Okay";
    }
}

async function beginQuiz(quizId) {
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
        throw new Error(data.title || data || "Unable to start quiz");
    }

    localStorage.setItem("quiz", JSON.stringify(data));
    localStorage.setItem("quizQuestionNumber", "1");
    window.location.href = "quiz.html";
}
