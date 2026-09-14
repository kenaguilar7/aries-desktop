window.ariesTheme = {
    apply: function (theme) {
        var value = theme === "dark" ? "dark" : "light";
        document.documentElement.setAttribute("data-theme", value);
        document.documentElement.setAttribute("data-bs-theme", value);
    },
    get: function () {
        try {
            return localStorage.getItem("aries-theme");
        } catch (e) {
            return null;
        }
    },
    set: function (theme) {
        var value = theme === "dark" ? "dark" : "light";
        try {
            localStorage.setItem("aries-theme", value);
        } catch (e) { }
        this.apply(value);
    },
    prefersDark: function () {
        return !!(window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches);
    }
};
