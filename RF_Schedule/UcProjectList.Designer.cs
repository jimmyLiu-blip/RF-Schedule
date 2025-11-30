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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode2 = new DevExpress.XtraGrid.GridLevelNode();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode3 = new DevExpress.XtraGrid.GridLevelNode();
            viewRegulation = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            viewTestItem = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            viewEngineer = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            viewProject = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            panelFilter = new DevExpress.XtraEditors.PanelControl();
            panelGrid = new DevExpress.XtraEditors.PanelControl();
            treeProject = new DevExpress.XtraTreeList.TreeList();
            coltreeListColumn0 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            coltreeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            ((System.ComponentModel.ISupportInitialize)viewRegulation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)viewTestItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)viewEngineer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)viewProject).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelFilter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)panelGrid).BeginInit();
            panelGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)treeProject).BeginInit();
            SuspendLayout();
            // 
            // viewRegulation
            // 
            viewRegulation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn9, gridColumn10 });
            viewRegulation.GridControl = gridControl1;
            viewRegulation.Name = "viewRegulation";
            viewRegulation.OptionsBehavior.Editable = false;
            viewRegulation.OptionsBehavior.ReadOnly = true;
            viewRegulation.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            viewRegulation.OptionsDetail.AllowZoomDetail = false;
            viewRegulation.OptionsDetail.ShowDetailTabs = false;
            viewRegulation.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn9
            // 
            gridColumn9.Caption = "法規";
            gridColumn9.FieldName = "RegulationName";
            gridColumn9.MinWidth = 30;
            gridColumn9.Name = "gridColumn9";
            gridColumn9.Visible = true;
            gridColumn9.VisibleIndex = 0;
            gridColumn9.Width = 100;
            // 
            // gridColumn10
            // 
            gridColumn10.Caption = "狀態";
            gridColumn10.FieldName = "Status";
            gridColumn10.MinWidth = 30;
            gridColumn10.Name = "gridColumn10";
            gridColumn10.Visible = true;
            gridColumn10.VisibleIndex = 1;
            gridColumn10.Width = 80;
            // 
            // gridControl1
            // 
            gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = viewRegulation;
            gridLevelNode1.RelationName = "Level1";
            gridLevelNode2.LevelTemplate = viewTestItem;
            gridLevelNode2.RelationName = "Level2";
            gridLevelNode3.LevelTemplate = viewEngineer;
            gridLevelNode3.RelationName = "Level3";
            gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] { gridLevelNode1, gridLevelNode2, gridLevelNode3 });
            gridControl1.Location = new System.Drawing.Point(2, 2);
            gridControl1.MainView = viewProject;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(1806, 854);
            gridControl1.TabIndex = 0;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewTestItem, viewEngineer, viewProject, viewRegulation });
            gridControl1.Click += gridControl1_Click;
            // 
            // viewTestItem
            // 
            viewTestItem.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn11, gridColumn13 });
            viewTestItem.GridControl = gridControl1;
            viewTestItem.Name = "viewTestItem";
            viewTestItem.OptionsBehavior.Editable = false;
            viewTestItem.OptionsBehavior.ReadOnly = true;
            viewTestItem.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            viewTestItem.OptionsDetail.AllowZoomDetail = false;
            viewTestItem.OptionsDetail.ShowDetailTabs = false;
            viewTestItem.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn11
            // 
            gridColumn11.Caption = "測試項目";
            gridColumn11.FieldName = "TestItemName";
            gridColumn11.MinWidth = 30;
            gridColumn11.Name = "gridColumn11";
            gridColumn11.Visible = true;
            gridColumn11.VisibleIndex = 0;
            gridColumn11.Width = 112;
            // 
            // gridColumn13
            // 
            gridColumn13.Caption = "狀態";
            gridColumn13.FieldName = "Status";
            gridColumn13.MinWidth = 30;
            gridColumn13.Name = "gridColumn13";
            gridColumn13.Visible = true;
            gridColumn13.VisibleIndex = 1;
            gridColumn13.Width = 112;
            // 
            // viewEngineer
            // 
            viewEngineer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn14, gridColumn15, gridColumn16 });
            viewEngineer.GridControl = gridControl1;
            viewEngineer.Name = "viewEngineer";
            viewEngineer.OptionsBehavior.Editable = false;
            viewEngineer.OptionsBehavior.ReadOnly = true;
            viewEngineer.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            viewEngineer.OptionsDetail.AllowZoomDetail = false;
            viewEngineer.OptionsDetail.ShowDetailTabs = false;
            viewEngineer.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn14
            // 
            gridColumn14.Caption = "工程師";
            gridColumn14.FieldName = "EngineerName";
            gridColumn14.MinWidth = 30;
            gridColumn14.Name = "gridColumn14";
            gridColumn14.Visible = true;
            gridColumn14.VisibleIndex = 0;
            gridColumn14.Width = 112;
            // 
            // gridColumn15
            // 
            gridColumn15.Caption = "實際測試時數";
            gridColumn15.FieldName = "AssignHours";
            gridColumn15.MinWidth = 30;
            gridColumn15.Name = "gridColumn15";
            gridColumn15.Visible = true;
            gridColumn15.VisibleIndex = 1;
            gridColumn15.Width = 112;
            // 
            // gridColumn16
            // 
            gridColumn16.Caption = "給工程師的話";
            gridColumn16.FieldName = "Note";
            gridColumn16.MinWidth = 30;
            gridColumn16.Name = "gridColumn16";
            gridColumn16.Visible = true;
            gridColumn16.VisibleIndex = 2;
            gridColumn16.Width = 112;
            // 
            // viewProject
            // 
            viewProject.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
            viewProject.Appearance.HeaderPanel.Options.UseFont = true;
            viewProject.Appearance.HeaderPanel.Options.UseTextOptions = true;
            viewProject.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            viewProject.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            viewProject.Appearance.Row.Options.UseFont = true;
            viewProject.Appearance.Row.Options.UseTextOptions = true;
            viewProject.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            viewProject.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { gridColumn8, gridColumn1, gridColumn2, gridColumn3, gridColumn4, gridColumn5, gridColumn6, gridColumn7 });
            viewProject.GridControl = gridControl1;
            viewProject.Name = "viewProject";
            viewProject.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            viewProject.OptionsDetail.AllowZoomDetail = false;
            viewProject.OptionsDetail.ShowDetailTabs = false;
            viewProject.OptionsView.RowAutoHeight = true;
            viewProject.OptionsView.ShowGroupPanel = false;
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
            gridColumn8.Width = 200;
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
            gridColumn1.Width = 150;
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
            gridColumn2.Width = 120;
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
            gridColumn3.Width = 220;
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
            gridColumn4.Width = 265;
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
            gridColumn5.Width = 265;
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
            gridColumn6.Width = 265;
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
            gridColumn7.Width = 285;
            // 
            // panelFilter
            // 
            panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            panelFilter.Location = new System.Drawing.Point(0, 0);
            panelFilter.Name = "panelFilter";
            panelFilter.Size = new System.Drawing.Size(1810, 149);
            panelFilter.TabIndex = 0;
            panelFilter.Paint += panelFilter_Paint;
            // 
            // panelGrid
            // 
            panelGrid.Controls.Add(treeProject);
            panelGrid.Controls.Add(gridControl1);
            panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            panelGrid.Location = new System.Drawing.Point(0, 149);
            panelGrid.Name = "panelGrid";
            panelGrid.Size = new System.Drawing.Size(1810, 858);
            panelGrid.TabIndex = 1;
            // 
            // treeProject
            // 
            treeProject.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] { coltreeListColumn0, coltreeListColumn1, coltreeListColumn2 });
            treeProject.Dock = System.Windows.Forms.DockStyle.Fill;
            treeProject.KeyFieldName = "Id";
            treeProject.Location = new System.Drawing.Point(2, 2);
            treeProject.Name = "treeProject";
            treeProject.ParentFieldName = "ParentId";
            treeProject.Size = new System.Drawing.Size(1806, 854);
            treeProject.TabIndex = 1;
            treeProject.Visible = false;
            treeProject.FocusedNodeChanged += treeProject_FocusedNodeChanged;
            // 
            // coltreeListColumn0
            // 
            coltreeListColumn0.Caption = "名稱";
            coltreeListColumn0.FieldName = "Name";
            coltreeListColumn0.Name = "coltreeListColumn0";
            coltreeListColumn0.Visible = true;
            coltreeListColumn0.VisibleIndex = 0;
            // 
            // coltreeListColumn1
            // 
            coltreeListColumn1.Caption = "狀態";
            coltreeListColumn1.FieldName = "Status";
            coltreeListColumn1.Name = "coltreeListColumn1";
            coltreeListColumn1.Visible = true;
            coltreeListColumn1.VisibleIndex = 1;
            // 
            // coltreeListColumn2
            // 
            coltreeListColumn2.Caption = "工時";
            coltreeListColumn2.FieldName = "Hours";
            coltreeListColumn2.Name = "coltreeListColumn2";
            coltreeListColumn2.Visible = true;
            coltreeListColumn2.VisibleIndex = 2;
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
            ((System.ComponentModel.ISupportInitialize)viewRegulation).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)viewTestItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)viewEngineer).EndInit();
            ((System.ComponentModel.ISupportInitialize)viewProject).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelFilter).EndInit();
            ((System.ComponentModel.ISupportInitialize)panelGrid).EndInit();
            panelGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)treeProject).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.PanelControl panelGrid;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView viewProject;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Views.Grid.GridView viewRegulation;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Views.Grid.GridView viewTestItem;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Views.Grid.GridView viewEngineer;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraTreeList.TreeList treeProject;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn0;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltreeListColumn2;
    }
}
