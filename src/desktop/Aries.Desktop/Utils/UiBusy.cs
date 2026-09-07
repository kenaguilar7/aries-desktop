using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aries.Desktop.Utils
{
    internal static class UiBusy
    {
        public static async Task Run(Control control, Func<Task> action)
        {
            var prev = control.Cursor;
            control.Cursor = Cursors.WaitCursor;
            try { await action(); }
            finally { control.Cursor = prev; }
        }
    }
}
