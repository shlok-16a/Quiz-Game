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
    localStorage.removeItem("quizRunningScore");
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

/** Treat datetime-local value as IST and convert to UTC ISO. */
function istLocalToIso(localValue) {
    if (!localValue) return null;
    const withSeconds = localValue.length === 16 ? `${localValue}:00` : localValue;
    const d = new Date(`${withSeconds}+05:30`);
    return Number.isNaN(d.getTime()) ? null : d.toISOString();
}

/** Parse API datetime as UTC (SQLite/EF often omit the Z). */
function parseUtcDate(iso) {
    if (!iso) return null;
    const raw = String(iso).trim();
    const hasZone = /([zZ]|[+-]\d{2}:\d{2})$/.test(raw);
    const d = new Date(hasZone ? raw : `${raw}Z`);
    return Number.isNaN(d.getTime()) ? null : d;
}

/** Convert UTC ISO to datetime-local value in IST. */
function isoToIstLocal(iso) {
    const d = parseUtcDate(iso);
    if (!d) return "";

    const parts = new Intl.DateTimeFormat("en-GB", {
        timeZone: "Asia/Kolkata",
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        hour12: false
    }).formatToParts(d);

    const get = (type) => parts.find(p => p.type === type)?.value || "00";
    let hour = get("hour");
    if (hour === "24") hour = "00";
    return `${get("year")}-${get("month")}-${get("day")}T${hour}:${get("minute")}`;
}

/** Format UTC ISO for display in IST. */
function formatIst(iso) {
    const d = parseUtcDate(iso);
    if (!d) return "—";
    const formatted = d.toLocaleString("en-IN", {
        timeZone: "Asia/Kolkata",
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true
    });
    return `${formatted} IST`;
}
