using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.Utils
{
    internal static class UiBusy
    {
        public static Task Run(Control control, Func<Task> action)
        {
            return Run(control, action, null);
        }

        public static async Task Run(Control control, Func<Task> action, string message)
        {
            var host = control as Form ?? control.FindForm() ?? control;
            var prevCursor = host.Cursor;
            Panel overlay = null;
            host.Cursor = Cursors.WaitCursor;
            host.UseWaitCursor = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(message))
                    overlay = ShowOverlay(host, message);

                await action();
            }
            finally
            {
                RestoreUi(host, overlay, prevCursor);
            }
        }

        private static Panel ShowOverlay(Control host, string message)
        {
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Name = "UiBusyOverlay"
            };
            overlay.Controls.Add(new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f)
            });
            host.Controls.Add(overlay);
            overlay.BringToFront();
            overlay.Update();
            return overlay;
        }

        private static void RestoreUi(Control host, Panel overlay, Cursor prevCursor)
        {
            void Restore()
            {
                if (host.IsDisposed)
                    return;
                if (overlay != null && !overlay.IsDisposed)
                {
                    host.Controls.Remove(overlay);
                    overlay.Dispose();
                }
                host.UseWaitCursor = false;
                host.Cursor = prevCursor;
            }

            if (host.IsDisposed)
                return;
            if (host.InvokeRequired)
                host.BeginInvoke((Action)Restore);
            else
                Restore();
        }
    }
}
