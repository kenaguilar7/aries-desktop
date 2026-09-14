window.ariesAsientos = {
    download: function (fileName, content, mime) {
        const blob = new Blob([content], { type: mime || "text/csv;charset=utf-8;" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(url);
    },
    bindShortcuts: function (element, dotNetRef) {
        if (!element || element._ariesAsientosBound) return;
        element._ariesAsientosBound = true;
        element.addEventListener("keydown", function (e) {
            const ctrl = e.ctrlKey || e.metaKey;
            if (!ctrl) {
                if (e.key === "Enter" && e.target && e.target.tagName !== "BUTTON" && e.target.tagName !== "TEXTAREA") {
                    e.preventDefault();
                    dotNetRef.invokeMethodAsync("OnEnterAsTab");
                }
                return;
            }
            const key = e.key.toLowerCase();
            if (key === "s") {
                e.preventDefault();
                dotNetRef.invokeMethodAsync("OnShortcut", "account");
            } else if (key === "d") {
                e.preventDefault();
                dotNetRef.invokeMethodAsync("OnShortcut", "debit");
            } else if (key === "c" && e.shiftKey) {
                e.preventDefault();
                dotNetRef.invokeMethodAsync("OnShortcut", "credit");
            } else if (key === "enter") {
                e.preventDefault();
                dotNetRef.invokeMethodAsync("OnShortcut", "submit");
            }
        }, true);
    },
    focusNext: function (container) {
        if (!container) return;
        const focusable = Array.from(container.querySelectorAll("input:not([disabled]), select:not([disabled]), button:not([disabled]), textarea:not([disabled])"))
            .filter(function (el) { return el.offsetParent !== null && el.tabIndex !== -1; });
        const i = focusable.indexOf(document.activeElement);
        if (i >= 0 && i < focusable.length - 1) {
            focusable[i + 1].focus();
            if (typeof focusable[i + 1].select === "function") {
                focusable[i + 1].select();
            }
        }
    }
};
