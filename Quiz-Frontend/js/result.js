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
        `Skipped Questions : ${data.skippedAnswers}`;

    document.getElementById("percentage").innerText =
        `Percentage : ${data.percentage}%`;
}

function playAgain() {

    localStorage.removeItem("quiz");
    localStorage.removeItem("resultSession");

    window.location.href = "categories.html";

}   