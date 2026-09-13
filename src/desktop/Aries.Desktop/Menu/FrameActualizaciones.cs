using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aries.Desktop.Utils;
using AriesContador.Data;
using AriesContador.Data.Migrations;
using Aries.Reporting.Textos;

namespace Aries.Desktop
{
    public sealed class FrameActualizaciones : Form
    {
        private readonly TextBox _txtAppVersion;
        private readonly TextBox _txtAppFeed;
        private readonly TextBox _txtAppStatus;
        private readonly TextBox _txtDbConnection;
        private readonly TextBox _txtDbVersion;
        private readonly TextBox _txtDbStatus;
        private readonly TextBox _txtPending;
        private readonly Button _btnUpdateApp;
        private readonly Button _btnUpdateDb;
        private readonly Button _btnCopy;
        private readonly Button _btnClose;
        private bool _loaded;

        public FrameActualizaciones()
        {
            Text = "Actualizaciones";
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ShowIcon = false;
            MinimumSize = new Size(720, 620);
            ClientSize = new Size(700, 580);
            Font = new Font("Segoe UI", 9F);

            var grpApp = new GroupBox
            {
                Text = "Sistema",
                Location = new Point(12, 12),
                Size = new Size(676, 168),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            _txtAppVersion = MakeValueBox(grpApp, "Versión instalada:", 24, 22);
            _txtAppFeed = MakeValueBox(grpApp, "Canal de actualización:", 52, 40);
            _txtAppStatus = MakeValueBox(grpApp, "Estado:", 98, 52);

            var grpDb = new GroupBox
            {
                Text = "Base de datos",
                Location = new Point(12, 188),
                Size = new Size(676, 330),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            _txtDbConnection = MakeValueBox(grpDb, "Conexión:", 24, 36);
            _txtDbVersion = MakeValueBox(grpDb, "Esquema:", 66, 36);
            _txtDbStatus = MakeValueBox(grpDb, "Estado:", 108, 36);

            var lblPending = new Label
            {
                AutoSize = true,
                Location = new Point(16, 152),
                Text = "Migraciones pendientes:"
            };
            _txtPending = new TextBox
            {
                Location = new Point(16, 174),
                Size = new Size(644, 138),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = SystemColors.Window
            };
            grpDb.Controls.Add(lblPending);
            grpDb.Controls.Add(_txtPending);

            _btnUpdateApp = new Button
            {
                Text = "Actualizar sistema",
                Location = new Point(12, 532),
                Size = new Size(160, 32),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                UseVisualStyleBackColor = true
            };
            _btnUpdateApp.Click += BtnUpdateApp_Click;

            _btnUpdateDb = new Button
            {
                Text = "Actualizar base de datos",
                Location = new Point(178, 532),
                Size = new Size(180, 32),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                UseVisualStyleBackColor = true
            };
            _btnUpdateDb.Click += BtnUpdateDb_Click;

            _btnCopy = new Button
            {
                Text = "Copiar datos",
                Location = new Point(364, 532),
                Size = new Size(120, 32),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                UseVisualStyleBackColor = true
            };
            _btnCopy.Click += BtnCopy_Click;

            _btnClose = new Button
            {
                Text = "Cerrar",
                Location = new Point(588, 532),
                Size = new Size(100, 32),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(grpApp);
            Controls.Add(grpDb);
            Controls.Add(_btnUpdateApp);
            Controls.Add(_btnUpdateDb);
            Controls.Add(_btnCopy);
            Controls.Add(_btnClose);
            CancelButton = _btnClose;
            AcceptButton = _btnClose;

            Shown += FrameActualizaciones_Shown;
        }

        private static TextBox MakeValueBox(GroupBox group, string caption, int top, int height)
        {
            var captionLabel = new Label
            {
                AutoSize = true,
                Location = new Point(16, top + 3),
                Text = caption
            };
            var value = new TextBox
            {
                Location = new Point(180, top),
                Size = new Size(480, height),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = SystemColors.Window,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Multiline = height > 24,
                WordWrap = height > 24,
                ScrollBars = height > 40 ? ScrollBars.Vertical : ScrollBars.None
            };
            value.Click += delegate
            {
                if (value.SelectionLength == 0)
                    value.SelectAll();
            };
            group.Controls.Add(captionLabel);
            group.Controls.Add(value);
            return value;
        }

        private async void FrameActualizaciones_Shown(object sender, EventArgs e)
        {
            if (_loaded)
                return;
            _loaded = true;
            await RefreshStatusAsync();
        }

        private async Task RefreshStatusAsync()
        {
            _btnUpdateApp.Enabled = false;
            _btnUpdateDb.Enabled = false;

            AppUpdateCheck app = null;
            MigrationStatus db = null;
            string dbError = null;

            await UiBusy.Run(this, async () =>
            {
                app = await AppUpdater.CheckAsync();
                try
                {
                    db = await new DatabaseMigrator(GlobalConfig.ConnectionString.MySQLDefault)
                        .GetStatusAsync()
                        .ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    dbError = ex.GetBaseException().Message;
                }
            }, "Consultando versiones…");

            BindApp(app);
            BindDb(db, dbError);
        }

        private void BindApp(AppUpdateCheck app)
        {
            if (app == null)
            {
                _txtAppVersion.Text = AppUpdater.FileVersion;
                _txtAppFeed.Text = "(sin datos)";
                _txtAppStatus.Text = "No se pudo consultar el canal de actualización.";
                return;
            }

            var beta = GlobalConfig.IsBeta ? " Beta" : string.Empty;
            _txtAppVersion.Text = app.InstalledVersion + "  [" + GlobalConfig.EnvironmentName + beta + "]";
            _txtAppFeed.Text = app.HasFeed
                ? (string.IsNullOrWhiteSpace(app.FeedUrl) ? "(ninguno)" : app.FeedUrl)
                : "(ninguno — este equipo no busca actualizaciones)";

            if (!string.IsNullOrWhiteSpace(app.Error))
            {
                _txtAppStatus.Text = "No se pudo consultar el feed: " + app.Error;
                _btnUpdateApp.Enabled = app.HasFeed;
                return;
            }

            if (!app.HasFeed)
            {
                _txtAppStatus.Text = "No hay UpdateUrl configurado.";
                _btnUpdateApp.Enabled = false;
                return;
            }

            if (app.HasUpdate)
            {
                _txtAppStatus.Text = "Hay una versión nueva: " + (app.AvailableVersion ?? "(desconocida)");
                _btnUpdateApp.Enabled = true;
            }
            else
            {
                _txtAppStatus.Text = "El sistema está al día"
                    + (string.IsNullOrWhiteSpace(app.AvailableVersion)
                        ? "."
                        : " (feed: " + app.AvailableVersion + ").");
                _btnUpdateApp.Enabled = true;
            }
        }

        private void BindDb(MigrationStatus db, string error)
        {
            _txtDbConnection.Text = GlobalConfig.MySqlServer + " / " + GlobalConfig.MySqlDatabase;

            if (!string.IsNullOrWhiteSpace(error))
            {
                _txtDbVersion.Text = "(sin datos)";
                _txtDbStatus.Text = "No se pudo leer el esquema: " + error;
                _txtPending.Text = string.Empty;
                _btnUpdateDb.Enabled = false;
                return;
            }

            if (db == null)
            {
                _txtDbVersion.Text = "(sin datos)";
                _txtDbStatus.Text = "No se pudo leer el esquema.";
                _txtPending.Text = string.Empty;
                _btnUpdateDb.Enabled = false;
                return;
            }

            _txtDbVersion.Text = db.HistoryTableExists
                ? "v." + db.AppliedVersion + " aplicada  /  v." + db.CatalogVersion + " en esta versión del sistema"
                    + "  (" + db.Applied.Count + " de " + (db.Applied.Count + db.Pending.Count) + " migraciones)"
                : "Sin historial  /  catálogo v." + db.CatalogVersion;

            if (db.Pending.Count == 0)
            {
                _txtPending.Text = "(ninguna)";
            }
            else
            {
                var pending = new StringBuilder();
                foreach (var item in db.Pending)
                    pending.AppendLine(item.Id + " — " + item.Description);
                _txtPending.Text = pending.ToString();
            }

            if (db.ChecksumMismatches.Count > 0)
            {
                _txtDbStatus.Text = "El SQL de una migración aplicada cambió: "
                    + string.Join(", ", db.ChecksumMismatches);
            }
            else if (!db.HistoryTableExists)
            {
                _txtDbStatus.Text = db.Pending.Count == 0
                    ? "No hay historial de esquema."
                    : "Sin historial. Hay " + db.Pending.Count + " migraciones por aplicar.";
            }
            else if (db.IsUpToDate)
            {
                _txtDbStatus.Text = "El esquema está al día.";
            }
            else
            {
                _txtDbStatus.Text = db.Pending.Count == 1
                    ? "Hay 1 migración pendiente."
                    : "Hay " + db.Pending.Count + " migraciones pendientes.";
            }

            if (!GlobalConfig.CanApplySchemaMigrations)
            {
                _txtDbStatus.Text += " Solo un administrador puede aplicar migraciones.";
                _btnUpdateDb.Enabled = false;
            }
            else
            {
                _btnUpdateDb.Enabled = true;
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            var text = new StringBuilder();
            text.AppendLine("Versión instalada: " + _txtAppVersion.Text);
            text.AppendLine("Canal de actualización: " + _txtAppFeed.Text);
            text.AppendLine("Estado sistema: " + _txtAppStatus.Text);
            text.AppendLine("Conexión: " + _txtDbConnection.Text);
            text.AppendLine("Esquema: " + _txtDbVersion.Text);
            text.AppendLine("Estado base: " + _txtDbStatus.Text);
            text.AppendLine("Migraciones pendientes:");
            text.Append(_txtPending.Text);
            try
            {
                Clipboard.SetText(text.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo copiar.\n\n" + ex.GetBaseException().Message,
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                "Datos copiados al portapapeles.",
                TextoGeneral.NombreApp,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private async void BtnUpdateApp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GlobalConfig.UpdateUrl))
            {
                MessageBox.Show(
                    "No hay canal de actualización configurado.",
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            AppUpdateApplyResult result = null;
            try
            {
                await UiBusy.Run(this, async () =>
                {
                    result = await AppUpdater.ApplyAsync();
                }, "Buscando e instalando la actualización…");
            }
            catch (Exception ex)
            {
                StartupLog.Write("Actualización de sistema: " + ex);
                MessageBox.Show(
                    "No se pudo actualizar el sistema.\n\n" + ex.GetBaseException().Message,
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (result == null || !result.Applied)
            {
                MessageBox.Show(
                    "El sistema ya está en la última versión del canal.",
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                await RefreshStatusAsync();
                return;
            }

            StartupLog.Write("Squirrel aplicó " + result.Version + " desde " + GlobalConfig.UpdateUrl);
            MessageBox.Show(
                "Se instaló la versión " + result.Version + ". Aries se reiniciará.",
                TextoGeneral.NombreApp,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            AppUpdater.Restart();
        }

        private async void BtnUpdateDb_Click(object sender, EventArgs e)
        {
            if (!GlobalConfig.CanApplySchemaMigrations)
            {
                MessageBox.Show(
                    "Solo un administrador puede actualizar el esquema de la base de datos.",
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MigrationStatus status;
            try
            {
                status = await new DatabaseMigrator(GlobalConfig.ConnectionString.MySQLDefault).GetStatusAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo leer el esquema.\n\n" + ex.GetBaseException().Message,
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (status.IsUpToDate)
            {
                MessageBox.Show(
                    "El esquema de la base de datos ya está al día.",
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                await RefreshStatusAsync();
                return;
            }

            if (!ConfirmMigrate(status))
                return;

            try
            {
                MigrationResult migrated = null;
                await UiBusy.Run(this, async () =>
                {
                    migrated = await new DatabaseMigrator(GlobalConfig.ConnectionString.MySQLDefault)
                        .ApplyPendingAsync()
                        .ConfigureAwait(true);
                }, "Aplicando migraciones…");

                if (migrated != null && migrated.HadPending)
                    StartupLog.Write(
                        "Migraciones aplicadas por "
                        + (GlobalConfig.User?.UserName ?? "(admin)")
                        + ": " + string.Join(", ", migrated.AppliedIds));
                else
                    StartupLog.Write("Esquema al día al actualizar desde el menú");

                MessageBox.Show(
                    migrated != null && migrated.HadPending
                        ? "Se aplicaron: " + string.Join(", ", migrated.AppliedIds)
                        : "El esquema ya estaba al día.",
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                StartupLog.Write("Migración (menú): " + ex);
                MessageBox.Show(
                    "No se pudo actualizar la base de datos.\n\n" + ex.GetBaseException().Message,
                    TextoGeneral.NombreApp,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            await RefreshStatusAsync();
        }

        private static bool ConfirmMigrate(MigrationStatus status)
        {
            var text = new StringBuilder();
            text.AppendLine("Se van a aplicar migraciones en:");
            text.AppendLine("  Servidor: " + GlobalConfig.MySqlServer);
            text.AppendLine("  Base: " + GlobalConfig.MySqlDatabase);
            text.AppendLine("  Usuario: " + (GlobalConfig.Usuario != null ? GlobalConfig.Usuario.ToString() : "(admin)"));
            text.AppendLine();
            if (status.Pending.Count == 0)
            {
                text.AppendLine("Se creará el historial de esquema si hace falta.");
            }
            else
            {
                text.AppendLine("Pendientes:");
                foreach (var pending in status.Pending)
                    text.AppendLine("  • " + pending.Id + " — " + pending.Description);
            }

            if (!MySqlConnectionInfo.IsLoopback(GlobalConfig.ConnectionString.MySQLDefault))
            {
                text.AppendLine();
                text.AppendLine("Esta no es una base local. El cambio es permanente.");
            }

            text.AppendLine();
            text.Append("¿Continuar?");

            return MessageBox.Show(
                text.ToString(),
                TextoGeneral.NombreApp,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }
    }
}
