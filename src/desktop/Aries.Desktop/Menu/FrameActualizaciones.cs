using System;
using System.Drawing;
using System.Linq;
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
        private readonly Label _lblAppVersion;
        private readonly Label _lblAppFeed;
        private readonly Label _lblAppStatus;
        private readonly Label _lblDbConnection;
        private readonly Label _lblDbVersion;
        private readonly Label _lblDbStatus;
        private readonly ListBox _lstPending;
        private readonly Button _btnUpdateApp;
        private readonly Button _btnUpdateDb;
        private readonly Button _btnClose;
        private bool _loaded;

        public FrameActualizaciones()
        {
            Text = "Actualizaciones";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ShowIcon = false;
            ClientSize = new Size(560, 520);
            Font = new Font("Segoe UI", 9F);

            var grpApp = new GroupBox
            {
                Text = "Sistema",
                Location = new Point(12, 12),
                Size = new Size(536, 150)
            };
            _lblAppVersion = MakeValueLabel(grpApp, "Versión instalada:", 28);
            _lblAppFeed = MakeValueLabel(grpApp, "Canal de actualización:", 60);
            _lblAppStatus = MakeValueLabel(grpApp, "Estado:", 92, 40);

            var grpDb = new GroupBox
            {
                Text = "Base de datos",
                Location = new Point(12, 174),
                Size = new Size(536, 280)
            };
            _lblDbConnection = MakeValueLabel(grpDb, "Conexión:", 28);
            _lblDbVersion = MakeValueLabel(grpDb, "Esquema:", 60);
            _lblDbStatus = MakeValueLabel(grpDb, "Estado:", 92, 36);

            var lblPending = new Label
            {
                AutoSize = true,
                Location = new Point(16, 136),
                Text = "Migraciones pendientes:"
            };
            _lstPending = new ListBox
            {
                Location = new Point(16, 158),
                Size = new Size(504, 104),
                IntegralHeight = false
            };
            grpDb.Controls.Add(lblPending);
            grpDb.Controls.Add(_lstPending);

            _btnUpdateApp = new Button
            {
                Text = "Actualizar sistema",
                Location = new Point(12, 470),
                Size = new Size(160, 32),
                UseVisualStyleBackColor = true
            };
            _btnUpdateApp.Click += BtnUpdateApp_Click;

            _btnUpdateDb = new Button
            {
                Text = "Actualizar base de datos",
                Location = new Point(178, 470),
                Size = new Size(180, 32),
                UseVisualStyleBackColor = true
            };
            _btnUpdateDb.Click += BtnUpdateDb_Click;

            _btnClose = new Button
            {
                Text = "Cerrar",
                Location = new Point(448, 470),
                Size = new Size(100, 32),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(grpApp);
            Controls.Add(grpDb);
            Controls.Add(_btnUpdateApp);
            Controls.Add(_btnUpdateDb);
            Controls.Add(_btnClose);
            CancelButton = _btnClose;
            AcceptButton = _btnClose;

            Shown += FrameActualizaciones_Shown;
        }

        private static Label MakeValueLabel(GroupBox group, string caption, int top, int height = 18)
        {
            var captionLabel = new Label
            {
                AutoSize = true,
                Location = new Point(16, top),
                Text = caption
            };
            var value = new Label
            {
                AutoEllipsis = height <= 18,
                Location = new Point(180, top),
                Size = new Size(340, height)
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
                _lblAppVersion.Text = AppUpdater.FileVersion;
                _lblAppFeed.Text = "(sin datos)";
                _lblAppStatus.Text = "No se pudo consultar el canal de actualización.";
                return;
            }

            var beta = GlobalConfig.IsBeta ? " Beta" : string.Empty;
            _lblAppVersion.Text = app.InstalledVersion + "  [" + GlobalConfig.EnvironmentName + beta + "]";
            _lblAppFeed.Text = app.HasFeed
                ? (string.IsNullOrWhiteSpace(app.FeedUrl) ? "(ninguno)" : app.FeedUrl)
                : "(ninguno — este equipo no busca actualizaciones)";

            if (!string.IsNullOrWhiteSpace(app.Error))
            {
                _lblAppStatus.Text = "No se pudo consultar el feed: " + app.Error;
                _btnUpdateApp.Enabled = app.HasFeed;
                return;
            }

            if (!app.HasFeed)
            {
                _lblAppStatus.Text = "No hay UpdateUrl configurado.";
                _btnUpdateApp.Enabled = false;
                return;
            }

            if (app.HasUpdate)
            {
                _lblAppStatus.Text = "Hay una versión nueva: " + (app.AvailableVersion ?? "(desconocida)");
                _btnUpdateApp.Enabled = true;
            }
            else
            {
                _lblAppStatus.Text = "El sistema está al día"
                    + (string.IsNullOrWhiteSpace(app.AvailableVersion)
                        ? "."
                        : " (feed: " + app.AvailableVersion + ").");
                _btnUpdateApp.Enabled = true;
            }
        }

        private void BindDb(MigrationStatus db, string error)
        {
            _lblDbConnection.Text = GlobalConfig.MySqlServer + " / " + GlobalConfig.MySqlDatabase;

            if (!string.IsNullOrWhiteSpace(error))
            {
                _lblDbVersion.Text = "(sin datos)";
                _lblDbStatus.Text = "No se pudo leer el esquema: " + error;
                _lstPending.Items.Clear();
                _btnUpdateDb.Enabled = false;
                return;
            }

            if (db == null)
            {
                _lblDbVersion.Text = "(sin datos)";
                _lblDbStatus.Text = "No se pudo leer el esquema.";
                _lstPending.Items.Clear();
                _btnUpdateDb.Enabled = false;
                return;
            }

            _lblDbVersion.Text = db.HistoryTableExists
                ? "v." + db.AppliedVersion + " aplicada  /  v." + db.CatalogVersion + " en esta versión del sistema"
                    + "  (" + db.Applied.Count + " de " + (db.Applied.Count + db.Pending.Count) + " migraciones)"
                : "Sin historial  /  catálogo v." + db.CatalogVersion;

            _lstPending.Items.Clear();
            foreach (var pending in db.Pending)
                _lstPending.Items.Add(pending.Id + " — " + pending.Description);

            if (db.ChecksumMismatches.Count > 0)
            {
                _lblDbStatus.Text = "El SQL de una migración aplicada cambió: "
                    + string.Join(", ", db.ChecksumMismatches.ToArray());
            }
            else if (!db.HistoryTableExists)
            {
                _lblDbStatus.Text = db.Pending.Count == 0
                    ? "No hay historial de esquema."
                    : "Sin historial. Hay " + db.Pending.Count + " migraciones por aplicar.";
            }
            else if (db.IsUpToDate)
            {
                _lblDbStatus.Text = "El esquema está al día.";
            }
            else
            {
                _lblDbStatus.Text = db.Pending.Count == 1
                    ? "Hay 1 migración pendiente."
                    : "Hay " + db.Pending.Count + " migraciones pendientes.";
            }

            if (!GlobalConfig.CanApplySchemaMigrations)
            {
                _lblDbStatus.Text += " Solo un administrador puede aplicar migraciones.";
                _btnUpdateDb.Enabled = false;
            }
            else
            {
                _btnUpdateDb.Enabled = true;
            }
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
