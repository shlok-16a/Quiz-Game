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

    const rank = data.rank ?? data.Rank;
    const total = data.totalCompletions ?? data.TotalCompletions ?? 0;
    document.getElementById("rank").innerText = rank
        ? `Rank : #${rank} of ${total}`
        : `Rank : —`;

    const durationSeconds = Number(data.durationSeconds ?? data.DurationSeconds ?? 0);
    const mins = Math.floor(durationSeconds / 60);
    const secs = durationSeconds % 60;
    const durationLabel = mins > 0
        ? `${mins}m ${String(secs).padStart(2, "0")}s`
        : `${secs}s`;
    document.getElementById("duration").innerText =
        `Time Taken : ${durationLabel}`;
}

function goBack() {
    localStorage.removeItem("quiz");
    localStorage.removeItem("quizQuestionNumber");
    localStorage.removeItem("resultSession");
    window.location.href = "categories.html";
}
