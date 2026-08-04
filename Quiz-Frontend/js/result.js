window.onload = loadResult;

async function loadResult() {

    const token = localStorage.getItem("token");

    const sessionId = localStorage.getItem("resultSession");

    const response = await fetch(
        `${API_BASE_URL}/api/Quiz/result/${sessionId}`,
        {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });

    const data = await response.json();

    document.getElementById("score").innerText =
        `Score : ${data.score}`;

    document.getElementById("correct").innerText =
        `Correct Answers : ${data.correctAnswers}`;

    document.getElementById("wrong").innerText =
        `Wrong Answers : ${data.wrongAnswers}`;

    document.getElementById("skipped").innerText =
        `Skipped Questions : ${data.skippedAnswers ?? data.SkippedAnswers ?? 0}`;

    const bonusPoints = Number(data.bonusPoints ?? data.BonusPoints ?? 0);
    const bonusAnswers = Number(data.bonusAnswers ?? data.BonusAnswers ?? 0);
    document.getElementById("bonus").innerText = bonusPoints > 0
        ? `Bonus Points : +${bonusPoints} (from ${bonusAnswers} fast correct answer${bonusAnswers === 1 ? "" : "s"})`
        : `Bonus Points : 0`;

    document.getElementById("percentage").innerText =
        `Percentage : ${data.percentage ?? data.Percentage}%`;
}

function goBack() {
    localStorage.removeItem("quiz");
    localStorage.removeItem("quizQuestionNumber");
    localStorage.removeItem("resultSession");
    window.location.href = "categories.html";
}
