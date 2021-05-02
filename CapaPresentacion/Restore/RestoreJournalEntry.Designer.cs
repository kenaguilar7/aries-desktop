
namespace CapaPresentacion.Restore
{
    partial class RestoreJournalEntry
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstEndPeriod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstStarPeriod = new System.Windows.Forms.ComboBox();
            this.JournalEntryLineGrid = new System.Windows.Forms.DataGridView();
            this.RestoreJournalsTabControl = new System.Windows.Forms.TabControl();
            this.journalEntryLineTab = new System.Windows.Forms.TabPage();
            this.JournalEntryTap = new System.Windows.Forms.TabPage();
            this.JournalEntryGrid = new System.Windows.Forms.DataGridView();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JournalEntryLineGrid)).BeginInit();
            this.RestoreJournalsTabControl.SuspendLayout();
            this.journalEntryLineTab.SuspendLayout();
            this.JournalEntryTap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JournalEntryGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lstEndPeriod);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lstStarPeriod);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1252, 65);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "FILTROS";
            // 
            // lstEndPeriod
            // 
            this.lstEndPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstEndPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstEndPeriod.FormattingEnabled = true;
            this.lstEndPeriod.Location = new System.Drawing.Point(309, 25);
            this.lstEndPeriod.Name = "lstEndPeriod";
            this.lstEndPeriod.Size = new System.Drawing.Size(130, 24);
            this.lstEndPeriod.TabIndex = 16;
            this.lstEndPeriod.SelectedIndexChanged += new System.EventHandler(this.LstEndPostingPeriod_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Mes Final:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Mes Inicial:";
            // 
            // lstStarPeriod
            // 
            this.lstStarPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstStarPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstStarPeriod.FormattingEnabled = true;
            this.lstStarPeriod.Location = new System.Drawing.Point(99, 25);
            this.lstStarPeriod.Name = "lstStarPeriod";
            this.lstStarPeriod.Size = new System.Drawing.Size(130, 24);
            this.lstStarPeriod.TabIndex = 13;
            this.lstStarPeriod.SelectedIndexChanged += new System.EventHandler(this.LstFirstPostingPeriod_SelectedIndexChanged);
            // 
            // JournalEntryLineGrid
            // 
            this.JournalEntryLineGrid.AllowUserToAddRows = false;
            this.JournalEntryLineGrid.AllowUserToDeleteRows = false;
            this.JournalEntryLineGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JournalEntryLineGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.JournalEntryLineGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.JournalEntryLineGrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.JournalEntryLineGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.JournalEntryLineGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.JournalEntryLineGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JournalEntryLineGrid.EnableHeadersVisualStyles = false;
            this.JournalEntryLineGrid.GridColor = System.Drawing.SystemColors.ControlLight;
            this.JournalEntryLineGrid.Location = new System.Drawing.Point(3, 3);
            this.JournalEntryLineGrid.MultiSelect = false;
            this.JournalEntryLineGrid.Name = "JournalEntryLineGrid";
            this.JournalEntryLineGrid.ReadOnly = true;
            this.JournalEntryLineGrid.RowHeadersVisible = false;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JournalEntryLineGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.JournalEntryLineGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.JournalEntryLineGrid.Size = new System.Drawing.Size(1238, 469);
            this.JournalEntryLineGrid.TabIndex = 4;
            this.JournalEntryLineGrid.TabStop = false;
            this.JournalEntryLineGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.JournalEntryLineGrid_CellClick);
            // 
            // RestoreJournalsTabControl
            // 
            this.RestoreJournalsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RestoreJournalsTabControl.Controls.Add(this.journalEntryLineTab);
            this.RestoreJournalsTabControl.Controls.Add(this.JournalEntryTap);
            this.RestoreJournalsTabControl.Location = new System.Drawing.Point(12, 83);
            this.RestoreJournalsTabControl.Name = "RestoreJournalsTabControl";
            this.RestoreJournalsTabControl.SelectedIndex = 0;
            this.RestoreJournalsTabControl.Size = new System.Drawing.Size(1252, 501);
            this.RestoreJournalsTabControl.TabIndex = 13;
            this.RestoreJournalsTabControl.SelectedIndexChanged += new System.EventHandler(this.RestoreJournalsTabControl_SelectedIndexChanged);
            // 
            // journalEntryLineTab
            // 
            this.journalEntryLineTab.Controls.Add(this.JournalEntryLineGrid);
            this.journalEntryLineTab.Location = new System.Drawing.Point(4, 22);
            this.journalEntryLineTab.Name = "journalEntryLineTab";
            this.journalEntryLineTab.Padding = new System.Windows.Forms.Padding(3);
            this.journalEntryLineTab.Size = new System.Drawing.Size(1244, 475);
            this.journalEntryLineTab.TabIndex = 0;
            this.journalEntryLineTab.Text = "Entradas de asiento";
            this.journalEntryLineTab.UseVisualStyleBackColor = true;
            // 
            // JournalEntryTap
            // 
            this.JournalEntryTap.Controls.Add(this.JournalEntryGrid);
            this.JournalEntryTap.Location = new System.Drawing.Point(4, 22);
            this.JournalEntryTap.Name = "JournalEntryTap";
            this.JournalEntryTap.Padding = new System.Windows.Forms.Padding(3);
            this.JournalEntryTap.Size = new System.Drawing.Size(1244, 475);
            this.JournalEntryTap.TabIndex = 1;
            this.JournalEntryTap.Text = "Asientos";
            this.JournalEntryTap.UseVisualStyleBackColor = true;
            // 
            // JournalEntryGrid
            // 
            this.JournalEntryGrid.AllowUserToAddRows = false;
            this.JournalEntryGrid.AllowUserToDeleteRows = false;
            this.JournalEntryGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JournalEntryGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.JournalEntryGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.JournalEntryGrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.JournalEntryGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.JournalEntryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.JournalEntryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JournalEntryGrid.EnableHeadersVisualStyles = false;
            this.JournalEntryGrid.GridColor = System.Drawing.SystemColors.ControlLight;
            this.JournalEntryGrid.Location = new System.Drawing.Point(3, 3);
            this.JournalEntryGrid.MultiSelect = false;
            this.JournalEntryGrid.Name = "JournalEntryGrid";
            this.JournalEntryGrid.ReadOnly = true;
            this.JournalEntryGrid.RowHeadersVisible = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JournalEntryGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.JournalEntryGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.JournalEntryGrid.Size = new System.Drawing.Size(1238, 469);
            this.JournalEntryGrid.TabIndex = 5;
            this.JournalEntryGrid.TabStop = false;
            this.JournalEntryGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.JournalEntryGrid_CellClick);
            // 
            // RestoreJournalEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 596);
            this.Controls.Add(this.RestoreJournalsTabControl);
            this.Controls.Add(this.groupBox2);
            this.MinimumSize = new System.Drawing.Size(1066, 532);
            this.Name = "RestoreJournalEntry";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recuperar Asientos Eliminados";
            this.Load += new System.EventHandler(this.RestoreJournalEntry_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.JournalEntryLineGrid)).EndInit();
            this.RestoreJournalsTabControl.ResumeLayout(false);
            this.journalEntryLineTab.ResumeLayout(false);
            this.JournalEntryTap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.JournalEntryGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox lstEndPeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lstStarPeriod;
        private System.Windows.Forms.DataGridView JournalEntryLineGrid;
        private System.Windows.Forms.TabControl RestoreJournalsTabControl;
        private System.Windows.Forms.TabPage journalEntryLineTab;
        private System.Windows.Forms.TabPage JournalEntryTap;
        private System.Windows.Forms.DataGridView JournalEntryGrid;
    }
}