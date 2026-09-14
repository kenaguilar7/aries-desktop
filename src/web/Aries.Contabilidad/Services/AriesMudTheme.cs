using MudBlazor;

namespace Aries.Contabilidad.Services
{
    public static class AriesMudTheme
    {
        public static MudTheme Create() => new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#1f883d",
                PrimaryContrastText = "#ffffff",
                Secondary = "#656d76",
                SecondaryContrastText = "#ffffff",
                AppbarBackground = "#f6f8fa",
                AppbarText = "#1f2328",
                Background = "#ffffff",
                BackgroundGray = "#f6f8fa",
                Surface = "#ffffff",
                DrawerBackground = "#f6f8fa",
                DrawerText = "#1f2328",
                TextPrimary = "#1f2328",
                TextSecondary = "#656d76",
                ActionDefault = "#656d76",
                ActionDisabled = "#8c959f",
                Divider = "#d0d7de",
                DividerLight = "#d8dee4",
                LinesDefault = "#d0d7de",
                TableLines = "#d0d7de",
                TableStriped = "#f6f8fa",
                TableHover = "#f3f4f6",
                Success = "#1a7f37",
                Error = "#d1242f",
                Warning = "#9a6700",
                Info = "#0969da",
                Dark = "#24292f",
                OverlayDark = "rgba(31,35,40,0.45)"
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#3fb950",
                PrimaryContrastText = "#0d1117",
                Secondary = "#8d96a0",
                SecondaryContrastText = "#0d1117",
                AppbarBackground = "#010409",
                AppbarText = "#e6edf3",
                Background = "#0d1117",
                BackgroundGray = "#161b22",
                Surface = "#161b22",
                DrawerBackground = "#0d1117",
                DrawerText = "#e6edf3",
                TextPrimary = "#e6edf3",
                TextSecondary = "#8d96a0",
                ActionDefault = "#8d96a0",
                ActionDisabled = "#6e7681",
                Divider = "#30363d",
                DividerLight = "#21262d",
                LinesDefault = "#30363d",
                TableLines = "#30363d",
                TableStriped = "#161b22",
                TableHover = "#1c2128",
                Success = "#3fb950",
                Error = "#f85149",
                Warning = "#d29922",
                Info = "#4493f8",
                Dark = "#010409",
                OverlayDark = "rgba(1,4,9,0.65)"
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "6px"
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = new[] { "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Noto Sans", "Helvetica", "Arial", "sans-serif" },
                    FontSize = "0.875rem"
                }
            }
        };
    }
}
