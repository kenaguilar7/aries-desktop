using Microsoft.JSInterop;

namespace Aries.Contabilidad.Services
{
    public class ThemeService
    {
        public const string StorageKey = "aries-theme";

        private readonly IJSRuntime _jsRuntime;
        private bool _initialized;

        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public bool IsDarkMode { get; private set; }

        public string CurrentTheme => IsDarkMode ? "dark" : "light";

        public event Action? Changed;

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            try
            {
                var stored = await _jsRuntime.InvokeAsync<string?>("ariesTheme.get");
                if (stored == "dark")
                    IsDarkMode = true;
                else if (stored == "light")
                    IsDarkMode = false;
                else
                    IsDarkMode = await _jsRuntime.InvokeAsync<bool>("ariesTheme.prefersDark");

                await _jsRuntime.InvokeVoidAsync("ariesTheme.apply", CurrentTheme);
            }
            catch
            {
                try
                {
                    var stored = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
                    IsDarkMode = stored == "dark";
                }
                catch
                {
                    // Keep the default light theme if JS is unavailable.
                }
            }

            _initialized = true;
            Changed?.Invoke();
        }

        public async Task ToggleAsync()
        {
            await SetDarkModeAsync(!IsDarkMode);
        }

        public async Task SetDarkModeAsync(bool dark)
        {
            IsDarkMode = dark;
            try
            {
                await _jsRuntime.InvokeVoidAsync("ariesTheme.set", CurrentTheme);
            }
            catch
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, CurrentTheme);
                }
                catch
                {
                    // Keep in-memory state even if persistence fails.
                }
            }

            Changed?.Invoke();
        }
    }
}
