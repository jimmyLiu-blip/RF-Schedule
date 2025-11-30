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
            splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            treeList1 = new DevExpress.XtraTreeList.TreeList();
            NodeType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ColumnName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            Status = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            StartDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            EndDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ganttControl1 = new DevExpress.XtraGantt.GanttControl();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).BeginInit();
            splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).BeginInit();
            splitContainerControl1.Panel2.SuspendLayout();
            splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeList1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ganttControl1).BeginInit();
            SuspendLayout();
            // 
            // splitContainerControl1
            // 
            splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            splitContainerControl1.Panel1.Controls.Add(treeList1);
            splitContainerControl1.Panel1.Text = "Panel1";
            // 
            // splitContainerControl1.Panel2
            // 
            splitContainerControl1.Panel2.Controls.Add(ganttControl1);
            splitContainerControl1.Panel2.Text = "Panel2";
            splitContainerControl1.Size = new System.Drawing.Size(1833, 1005);
            splitContainerControl1.SplitterPosition = 537;
            splitContainerControl1.TabIndex = 0;
            // 
            // treeList1
            // 
            treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { NodeType, ColumnName, Status, StartDate, EndDate });
            treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            treeList1.Location = new System.Drawing.Point(0, 0);
            treeList1.Name = "treeList1";
            treeList1.BeginUnboundLoad();
            treeList1.AppendNode(new object[] { null, "Conducted_1", null, null, null }, -1);
            treeList1.AppendNode(new object[] { null, "TE0123456789", "IN-Progress", new System.DateTime(2025, 11, 20, 0, 0, 0), new System.DateTime(2025, 12, 10, 0, 0, 0) }, 0);
            treeList1.AppendNode(new object[] { null, "FCC", "Completed", new System.DateTime(2025, 11, 20, 0, 0, 0), new System.DateTime(2025, 11, 30, 0, 0, 0) }, 1);
            treeList1.AppendNode(new object[] { null, "WIFI_Conducted", "Completed", new System.DateTime(2025, 11, 20, 0, 0, 0), new System.DateTime(2025, 11, 30, 0, 0, 0) }, 2);
            treeList1.AppendNode(new object[] { null, "NCC", null, null, null }, 1);
            treeList1.AppendNode(new object[] { null, "WIFI_Conducted", "IN-Progress", new System.DateTime(2025, 11, 30, 0, 0, 0), new System.DateTime(2025, 12, 10, 0, 0, 0) }, 4);
            treeList1.EndUnboundLoad();
            treeList1.Size = new System.Drawing.Size(537, 1005);
            treeList1.TabIndex = 1;
            // 
            // NodeType
            // 
            NodeType.Caption = "類型";
            NodeType.FieldName = "treeListColumn0";
            NodeType.MinWidth = 16;
            NodeType.Name = "NodeType";
            NodeType.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            NodeType.Width = 16;
            // 
            // Status
            // 
            Status.Caption = "狀態";
            Status.FieldName = "treeListColumn2";
            Status.Name = "Status";
            Status.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            Status.Visible = true;
            Status.VisibleIndex = 1;
            Status.Width = 97;
            // 
            // StartDate
            // 
            StartDate.Caption = "開始日期";
            StartDate.FieldName = "treeListColumn3";
            StartDate.Format.FormatString = "yyyy/MM/dd";
            StartDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            StartDate.Name = "StartDate";
            StartDate.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.DateTime;
            StartDate.Visible = true;
            StartDate.VisibleIndex = 2;
            StartDate.Width = 116;
            // 
            // EndDate
            // 
            EndDate.Caption = "結束日期";
            EndDate.FieldName = "treeListColumn4";
            EndDate.Format.FormatString = "yyyy/MM/dd";
            EndDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            EndDate.Name = "EndDate";
            EndDate.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.DateTime;
            EndDate.Visible = true;
            EndDate.VisibleIndex = 3;
            EndDate.Width = 121;
            // 
            // ganttControl1
            // 
            ganttControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            ganttControl1.Location = new System.Drawing.Point(0, 0);
            ganttControl1.Name = "ganttControl1";
            ganttControl1.Size = new System.Drawing.Size(1281, 1005);
            ganttControl1.SplitterPosition = 189;
            ganttControl1.TabIndex = 0;
            ganttControl1.FocusedNodeChanged += ganttControl1_FocusedNodeChanged;
            // 
            // UcProjectGanttChart
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(splitContainerControl1);
            Name = "UcProjectGanttChart";
            Size = new System.Drawing.Size(1833, 1005);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel1).EndInit();
            splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1.Panel2).EndInit();
            splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerControl1).EndInit();
            splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeList1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ganttControl1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGantt.GanttControl ganttControl1;
        private DevExpress.XtraTreeList.TreeList treeList1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn NodeType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn ColumnName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn Status;
        private DevExpress.XtraTreeList.Columns.TreeListColumn StartDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn EndDate;
    }
}
