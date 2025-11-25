namespace RF_Schedule
{
    partial class UcMyWork
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelFilter = new DevExpress.XtraEditors.PanelControl();
            lblProject = new DevExpress.XtraEditors.LabelControl();
            cmbProject = new DevExpress.XtraEditors.ComboBoxEdit();
            lblRegulation = new DevExpress.XtraEditors.LabelControl();
            cmbRegulation = new DevExpress.XtraEditors.ComboBoxEdit();
            lblStatus = new DevExpress.XtraEditors.LabelControl();
            cmbStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            lblTestType = new DevExpress.XtraEditors.LabelControl();
            cmbTestType = new DevExpress.XtraEditors.ComboBoxEdit();
            txtSearch = new DevExpress.XtraEditors.TextEdit();
            btnSearch = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)panelFilter).BeginInit();
            panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)cmbProject.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbRegulation.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbTestType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // panelFilter
            // 
            panelFilter.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
            panelFilter.Appearance.Options.UseBackColor = true;
            panelFilter.Controls.Add(labelControl1);
            panelFilter.Controls.Add(lblProject);
            panelFilter.Controls.Add(cmbProject);
            panelFilter.Controls.Add(btnSearch);
            panelFilter.Controls.Add(txtSearch);
            panelFilter.Controls.Add(cmbTestType);
            panelFilter.Controls.Add(lblTestType);
            panelFilter.Controls.Add(cmbStatus);
            panelFilter.Controls.Add(lblStatus);
            panelFilter.Controls.Add(cmbRegulation);
            panelFilter.Controls.Add(lblRegulation);
            panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            panelFilter.Location = new System.Drawing.Point(0, 0);
            panelFilter.Name = "panelFilter";
            panelFilter.Size = new System.Drawing.Size(2070, 80);
            panelFilter.TabIndex = 0;
            // 
            // lblProject
            // 
            lblProject.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblProject.Appearance.Options.UseFont = true;
            lblProject.Location = new System.Drawing.Point(50, 14);
            lblProject.Name = "lblProject";
            lblProject.Size = new System.Drawing.Size(60, 25);
            lblProject.TabIndex = 0;
            lblProject.Text = "專案：";
            // 
            // cmbProject
            // 
            cmbProject.Location = new System.Drawing.Point(116, 11);
            cmbProject.Name = "cmbProject";
            cmbProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbProject.Size = new System.Drawing.Size(150, 28);
            cmbProject.TabIndex = 1;
            // 
            // lblRegulation
            // 
            lblRegulation.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblRegulation.Appearance.Options.UseFont = true;
            lblRegulation.Location = new System.Drawing.Point(298, 15);
            lblRegulation.Name = "lblRegulation";
            lblRegulation.Size = new System.Drawing.Size(60, 25);
            lblRegulation.TabIndex = 2;
            lblRegulation.Text = "法規：";
            // 
            // cmbRegulation
            // 
            cmbRegulation.Location = new System.Drawing.Point(364, 14);
            cmbRegulation.Name = "cmbRegulation";
            cmbRegulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbRegulation.Size = new System.Drawing.Size(150, 28);
            cmbRegulation.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblStatus.Appearance.Options.UseFont = true;
            lblStatus.Location = new System.Drawing.Point(298, 46);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(60, 25);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "狀態：";
            // 
            // cmbStatus
            // 
            cmbStatus.Location = new System.Drawing.Point(364, 47);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbStatus.Size = new System.Drawing.Size(150, 28);
            cmbStatus.TabIndex = 4;
            // 
            // lblTestType
            // 
            lblTestType.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblTestType.Appearance.Options.UseFont = true;
            lblTestType.Location = new System.Drawing.Point(10, 46);
            lblTestType.Name = "lblTestType";
            lblTestType.Size = new System.Drawing.Size(100, 25);
            lblTestType.TabIndex = 2;
            lblTestType.Text = "測試類型：";
            // 
            // cmbTestType
            // 
            cmbTestType.Location = new System.Drawing.Point(116, 45);
            cmbTestType.Name = "cmbTestType";
            cmbTestType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbTestType.Size = new System.Drawing.Size(150, 28);
            cmbTestType.TabIndex = 5;
            // 
            // txtSearch
            // 
            txtSearch.Location = new System.Drawing.Point(634, 27);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new System.Drawing.Size(200, 28);
            txtSearch.TabIndex = 2;
            // 
            // btnSearch
            // 
            btnSearch.Location = new System.Drawing.Point(840, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new System.Drawing.Size(80, 51);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "搜尋";
            // 
            // labelControl1
            // 
            labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelControl1.Appearance.Options.UseFont = true;
            labelControl1.Location = new System.Drawing.Point(583, 27);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(45, 25);
            labelControl1.TabIndex = 6;
            labelControl1.Text = "搜尋:";
            // 
            // gridControl1
            // 
            gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl1.Location = new System.Drawing.Point(0, 80);
            gridControl1.MainView = gridView1;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(2070, 970);
            gridControl1.TabIndex = 1;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn1 });
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "gridColumn1";
            gridColumn1.MinWidth = 30;
            gridColumn1.Name = "gridColumn1";
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 0;
            gridColumn1.Width = 112;
            // 
            // UcMyWork
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(gridControl1);
            Controls.Add(panelFilter);
            Name = "UcMyWork";
            Size = new System.Drawing.Size(2070, 1050);
            Load += UcMyWork_Load;
            ((System.ComponentModel.ISupportInitialize)panelFilter).EndInit();
            panelFilter.ResumeLayout(false);
            panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)cmbProject.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbRegulation.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbStatus.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbTestType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSearch.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.LabelControl lblProject;
        private DevExpress.XtraEditors.ComboBoxEdit cmbProject;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cmbRegulation;
        private DevExpress.XtraEditors.LabelControl lblRegulation;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraEditors.ComboBoxEdit cmbTestType;
        private DevExpress.XtraEditors.LabelControl lblTestType;
        private DevExpress.XtraEditors.ComboBoxEdit cmbStatus;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
    }
}
