namespace RF_Schedule
{
    partial class UcProjectGanttChart
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
            ganttControl1 = new DevExpress.XtraGantt.GanttControl();
            coltreeListColumn0 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)ganttControl1).BeginInit();
            SuspendLayout();
            // 
            // ganttControl1
            // 
            ganttControl1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { coltreeListColumn0, coltreeListColumn1, coltreeListColumn2, coltreeListColumn3, coltreeListColumn4 });
            ganttControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            ganttControl1.Location = new System.Drawing.Point(0, 0);
            ganttControl1.Name = "ganttControl1";
            ganttControl1.Size = new System.Drawing.Size(1843, 1027);
            ganttControl1.SplitterPosition = 604;
            ganttControl1.TabIndex = 0;
            ganttControl1.ViewStyle = DevExpress.XtraTreeList.TreeListViewStyle.TreeList;
            ganttControl1.FocusedNodeChanged += ganttControl1_FocusedNodeChanged;
            // 
            // coltreeListColumn0
            // 
            coltreeListColumn0.Caption = "名稱";
            coltreeListColumn0.FieldName = "ColumnName";
            coltreeListColumn0.Name = "coltreeListColumn0";
            coltreeListColumn0.OptionsColumn.AllowEdit = false;
            coltreeListColumn0.OptionsColumn.AllowFocus = false;
            coltreeListColumn0.OptionsColumn.ReadOnly = true;
            coltreeListColumn0.Visible = true;
            coltreeListColumn0.VisibleIndex = 0;
            coltreeListColumn0.Width = 192;
            // 
            // coltreeListColumn1
            // 
            coltreeListColumn1.Caption = "開始日期";
            coltreeListColumn1.FieldName = "StartDate";
            coltreeListColumn1.Format.FormatString = "yyyy/MM/dd";
            coltreeListColumn1.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            coltreeListColumn1.Name = "coltreeListColumn1";
            coltreeListColumn1.OptionsColumn.AllowEdit = false;
            coltreeListColumn1.OptionsColumn.AllowFocus = false;
            coltreeListColumn1.OptionsColumn.ReadOnly = true;
            coltreeListColumn1.Visible = true;
            coltreeListColumn1.VisibleIndex = 1;
            coltreeListColumn1.Width = 116;
            // 
            // coltreeListColumn2
            // 
            coltreeListColumn2.Caption = "結束日期";
            coltreeListColumn2.FieldName = "EndDate";
            coltreeListColumn2.Format.FormatString = "yyyy/MM/dd";
            coltreeListColumn2.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            coltreeListColumn2.Name = "coltreeListColumn2";
            coltreeListColumn2.OptionsColumn.AllowEdit = false;
            coltreeListColumn2.OptionsColumn.AllowFocus = false;
            coltreeListColumn2.OptionsColumn.ReadOnly = true;
            coltreeListColumn2.Visible = true;
            coltreeListColumn2.VisibleIndex = 2;
            coltreeListColumn2.Width = 116;
            // 
            // coltreeListColumn3
            // 
            coltreeListColumn3.Caption = "狀態";
            coltreeListColumn3.FieldName = "Status";
            coltreeListColumn3.Name = "coltreeListColumn3";
            coltreeListColumn3.OptionsColumn.AllowEdit = false;
            coltreeListColumn3.OptionsColumn.ReadOnly = true;
            coltreeListColumn3.Visible = true;
            coltreeListColumn3.VisibleIndex = 3;
            coltreeListColumn3.Width = 91;
            // 
            // coltreeListColumn4
            // 
            coltreeListColumn4.Caption = "工程師";
            coltreeListColumn4.FieldName = "Engineer";
            coltreeListColumn4.Name = "coltreeListColumn4";
            coltreeListColumn4.OptionsColumn.AllowEdit = false;
            coltreeListColumn4.OptionsColumn.ReadOnly = true;
            coltreeListColumn4.Visible = true;
            coltreeListColumn4.VisibleIndex = 4;
            coltreeListColumn4.Width = 89;
            // 
            // UcProjectGanttChart
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(ganttControl1);
            Name = "UcProjectGanttChart";
            Size = new System.Drawing.Size(1843, 1027);
            ((System.ComponentModel.ISupportInitialize)ganttControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGantt.GanttControl ganttControl1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn0;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn4;
    }
}
