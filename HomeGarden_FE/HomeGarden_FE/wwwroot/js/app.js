"use strict";

const API_BASE = "https://localhost:7120/api";
const PAGE_SIZE = 10;
const apiUrl = (p) => API_BASE.replace(/\/+$/, "") + "/" + String(p).replace(/^\/+/, "");
const $ = (id) => document.getElementById(id);

function fmtDate(iso) {
    if (!iso) return "—";
    const d = new Date(iso);
    return d.toLocaleDateString();
}

function setText(id, txt) {
    const el = $(id);
    if (el) el.textContent = txt;
}
function setDisabled(id, dis) {
    const el = $(id);
    if (el) el.disabled = !!dis;
}

async function fetchJson(url, options = {}) {
    try {
        const res = await fetch(url, { ...options, headers: { Accept: "application/json" } });
        const body = await res.json().catch(() => ({}));
        const ok = res.ok && (body?.success !== false);
        return { ok, data: body?.data ?? body, message: body?.message ?? res.statusText };
    } catch {
        return { ok: false, data: null, message: "Không thể kết nối máy chủ." };
    }
}

let curPage = 1;

function renderPlantRows(items) {
    if (!Array.isArray(items) || items.length === 0)
        return `<tr><td colspan="6" class="px-4 py-6 text-center text-gray-500">Không có dữ liệu</td></tr>`;
    return items.map(it => `
        <tr>
            <td class="px-4 py-2 font-medium">${it.name ?? "—"}</td>
            <td class="px-4 py-2">${it.species ?? "—"}</td>
            <td class="px-4 py-2">${it.health ?? "—"}</td>
            <td class="px-4 py-2">${it.status ?? "—"}</td>
            <td class="px-4 py-2">${fmtDate(it.plantedDate)}</td>
            <td class="px-4 py-2">
                <a href="/App/Plant/${it.plantId}"
                   class="px-2 py-1 rounded-lg border border-gray-300 dark:border-gray-700 text-xs hover:bg-gray-50 dark:hover:bg-gray-800">
                    Chi tiết
                </a>
            </td>
        </tr>`).join("");
}

async function loadPlants(page = 1) {
    const tbody = $("#plantRows");
    tbody.innerHTML = `<tr><td colspan="6" class="px-4 py-6 text-center text-gray-500">Đang tải...</td></tr>`;

    // chỉ gọi page & size, KHÔNG search, KHÔNG filter
    const qs = new URLSearchParams({ page: String(page), size: String(PAGE_SIZE) }).toString();
    const { ok, data, message } = await fetchJson(apiUrl("plants") + "?" + qs);

    if (!ok) {
        tbody.innerHTML = `<tr><td colspan="6" class="px-4 py-6 text-center text-red-600">${message}</td></tr>`;
        return;
    }

    const items = data?.items ?? data?.data?.items ?? [];
    const total = data?.total ?? data?.data?.total ?? 0;
    const sizeNum = data?.size ?? PAGE_SIZE;
    const maxPage = Math.max(1, Math.ceil(total / sizeNum));

    tbody.innerHTML = renderPlantRows(items);
    curPage = page;
    setText("pageInfo", `Trang ${page} / ${maxPage}`);
    setText("plantTotal", `${total} cây`);
    setDisabled("prevPage", page <= 1);
    setDisabled("nextPage", page >= maxPage);
}

$("#prevPage")?.addEventListener("click", () => curPage > 1 && loadPlants(curPage - 1));
$("#nextPage")?.addEventListener("click", () => loadPlants(curPage + 1));

(async function init() {
    await loadPlants(1);
})();
