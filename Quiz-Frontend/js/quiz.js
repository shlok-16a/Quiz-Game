const quiz = JSON.parse(localStorage.getItem("quiz"));

if (!quiz) {
    alert("No quiz found. Start a quiz from Categories.");
    window.location.href = "categories.html";
}

const sessionId = quiz.sessionId;
let currentQuestion = quiz.firstQuestion;
let isSubmitting = false;

showQuestion(currentQuestion);

function showQuestion(question) {
    if (!question) {
        console.error("showQuestion called with empty question");
        return;
    }

    const questionText = question.questionText ?? question.QuestionText;
    const option1Text = question.option1 ?? question.Option1;
    const option2Text = question.option2 ?? question.Option2;
    const option3Text = question.option3 ?? question.Option3;
    const questionId = question.id ?? question.Id;

    // Normalize so submitAnswer always uses camelCase fields
    currentQuestion = {
        id: questionId,
        questionText: questionText,
        option1: option1Text,
        option2: option2Text,
        option3: option3Text
    };

    document.getElementById("question").textContent = questionText;
    document.getElementById("option1").textContent = option1Text;
    document.getElementById("option2").textContent = option2Text;
    document.getElementById("option3").textContent = option3Text;

    setButtonsDisabled(false);
}

function setButtonsDisabled(disabled) {
    document.getElementById("option1").disabled = disabled;
    document.getElementById("option2").disabled = disabled;
    document.getElementById("option3").disabled = disabled;
}

async function submitAnswer(selectedOption) {
    if (isSubmitting) return;
    isSubmitting = true;
    setButtonsDisabled(true);

    try {
        const token = localStorage.getItem("token");

        const response = await fetch(`${API_BASE_URL}/api/Quiz/answer`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`
            },
            body: JSON.stringify({
                sessionId: sessionId,
                questionId: currentQuestion.id,
                selectedOption: selectedOption
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error("Answer API failed:", response.status, errorText);
            alert("Failed to submit answer. Check console.");
            setButtonsDisabled(false);
            return;
        }

        const data = await response.json();
        console.log("Submit API Response:", data);

        const quizCompleted = data.quizCompleted ?? data.QuizCompleted;
        const nextQuestion = data.nextQuestion ?? data.NextQuestion;

        if (quizCompleted) {
            localStorage.setItem("resultSession", sessionId);
            window.location.href = "result.html";
            return;
        }

        if (!nextQuestion) {
            console.error("No nextQuestion in response:", data);
            alert("No next question returned from API.");
            setButtonsDisabled(false);
            return;
        }

        showQuestion(nextQuestion);
    } catch (err) {
        console.error(err);
        alert("Unable to submit answer.");
        setButtonsDisabled(false);
    } finally {
        isSubmitting = false;
    }
}

document.getElementById("option1").onclick = () => submitAnswer(1);
document.getElementById("option2").onclick = () => submitAnswer(2);
document.getElementById("option3").onclick = () => submitAnswer(3);
