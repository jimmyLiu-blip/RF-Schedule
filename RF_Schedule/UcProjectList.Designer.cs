namespace RF_Schedule
{
    partial class UcProjectList
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
            panelGrid = new DevExpress.XtraEditors.PanelControl();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)panelFilter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelGrid).BeginInit();
            panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // panelFilter
            // 
            panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            panelFilter.Location = new System.Drawing.Point(0, 0);
            panelFilter.Name = "panelFilter";
            panelFilter.Size = new System.Drawing.Size(1810, 149);
            panelFilter.TabIndex = 0;
            // 
            // panelGrid
            // 
            panelGrid.Controls.Add(gridControl1);
            panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            panelGrid.Location = new System.Drawing.Point(0, 149);
            panelGrid.Name = "panelGrid";
            panelGrid.Size = new System.Drawing.Size(1810, 858);
            panelGrid.TabIndex = 1;
            // 
            // gridControl1
            // 
            gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            gridControl1.Location = new System.Drawing.Point(2, 2);
            gridControl1.MainView = gridView1;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(1806, 854);
            gridControl1.TabIndex = 0;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            gridControl1.Click += gridControl1_Click;
            // 
            // gridView1
            // 
            gridView1.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView1.Appearance.Row.Options.UseTextOptions = true;
            gridView1.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn8, gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6, gridColumn7 });
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn8
            // 
            gridColumn8.Caption = "專案編號";
            gridColumn8.FieldName = "ProjectName";
            gridColumn8.MinWidth = 30;
            gridColumn8.Name = "gridColumn8";
            gridColumn8.OptionsColumn.AllowEdit = false;
            gridColumn8.OptionsColumn.ReadOnly = true;
            gridColumn8.Visible = true;
            gridColumn8.VisibleIndex = 0;
            gridColumn8.Width = 111;
            // 
            // gridColumn1
            // 
            gridColumn1.Caption = "客戶";
            gridColumn1.FieldName = "Customer";
            gridColumn1.MinWidth = 30;
            gridColumn1.Name = "gridColumn1";
            gridColumn1.OptionsColumn.AllowEdit = false;
            gridColumn1.OptionsColumn.ReadOnly = true;
            gridColumn1.Visible = true;
            gridColumn1.VisibleIndex = 1;
            gridColumn1.Width = 111;
            // 
            // gridColumn2
            // 
            gridColumn2.AppearanceCell.Options.UseTextOptions = true;
            gridColumn2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridColumn2.Caption = "優先級";
            gridColumn2.FieldName = "Priority";
            gridColumn2.MinWidth = 30;
            gridColumn2.Name = "gridColumn2";
            gridColumn2.OptionsColumn.AllowEdit = false;
            gridColumn2.OptionsColumn.ReadOnly = true;
            gridColumn2.Visible = true;
            gridColumn2.VisibleIndex = 2;
            gridColumn2.Width = 111;
            // 
            // gridColumn3
            // 
            gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridColumn3.Caption = "狀態";
            gridColumn3.FieldName = "Status";
            gridColumn3.MinWidth = 30;
            gridColumn3.Name = "gridColumn3";
            gridColumn3.OptionsColumn.AllowEdit = false;
            gridColumn3.OptionsColumn.ReadOnly = true;
            gridColumn3.Visible = true;
            gridColumn3.VisibleIndex = 3;
            gridColumn3.Width = 111;
            // 
            // gridColumn4
            // 
            gridColumn4.Caption = "預計開始日期";
            gridColumn4.DisplayFormat.FormatString = "yyyy-MM-dd";
            gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridColumn4.FieldName = "StartDate";
            gridColumn4.MinWidth = 30;
            gridColumn4.Name = "gridColumn4";
            gridColumn4.OptionsColumn.AllowEdit = false;
            gridColumn4.OptionsColumn.ReadOnly = true;
            gridColumn4.Visible = true;
            gridColumn4.VisibleIndex = 4;
            gridColumn4.Width = 111;
            // 
            // gridColumn5
            // 
            gridColumn5.Caption = "預計結束日期";
            gridColumn5.DisplayFormat.FormatString = "yyyy-MM-dd";
            gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridColumn5.FieldName = "EndDate";
            gridColumn5.MinWidth = 30;
            gridColumn5.Name = "gridColumn5";
            gridColumn5.OptionsColumn.AllowEdit = false;
            gridColumn5.OptionsColumn.ReadOnly = true;
            gridColumn5.Visible = true;
            gridColumn5.VisibleIndex = 5;
            gridColumn5.Width = 111;
            // 
            // gridColumn6
            // 
            gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            gridColumn6.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gridColumn6.Caption = "備註";
            gridColumn6.FieldName = "Note";
            gridColumn6.MinWidth = 30;
            gridColumn6.Name = "gridColumn6";
            gridColumn6.Visible = true;
            gridColumn6.VisibleIndex = 6;
            gridColumn6.Width = 111;
            // 
            // gridColumn7
            // 
            gridColumn7.Caption = "建立時間";
            gridColumn7.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
            gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridColumn7.FieldName = "CreatedDate";
            gridColumn7.MinWidth = 30;
            gridColumn7.Name = "gridColumn7";
            gridColumn7.OptionsColumn.AllowEdit = false;
            gridColumn7.OptionsColumn.ReadOnly = true;
            gridColumn7.Visible = true;
            gridColumn7.VisibleIndex = 7;
            gridColumn7.Width = 111;
            // 
            // UcProjectList
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panelGrid);
            Controls.Add(panelFilter);
            Name = "UcProjectList";
            Size = new System.Drawing.Size(1810, 1007);
            Load += UcProjectList_Load;
            ((System.ComponentModel.ISupportInitialize)panelFilter).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelGrid).EndInit();
            panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.PanelControl panelGrid;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
    }
}
