using System;
using System.Drawing;
using System.Windows.Forms;

namespace Aries.Desktop
{
    internal sealed class FrameSplash : Form
    {
        private readonly Label _status;

        public FrameSplash()
        {
            Text = "Aries Contador";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = true;
            Size = new Size(440, 140);
            MaximizeBox = false;
            MinimizeBox = false;

            _status = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Text = "Iniciando…"
            };
            Controls.Add(_status);
        }

        public void SetStatus(string text)
        {
            _status.Text = text ?? string.Empty;
            Refresh();
            Application.DoEvents();
        }
    }
}
