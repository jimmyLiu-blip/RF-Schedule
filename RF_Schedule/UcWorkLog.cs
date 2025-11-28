using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RF_Schedule
{
    public partial class UcWorkLog : DevExpress.XtraEditors.XtraUserControl
    {
        public class WorkLogDto
        {
            public DateTime WorkDate { get; set; }
            public decimal ActualHours { get; set; }
            public string WorkTypeName { get; set; } = string.Empty;
            public string ProjectNumber { get; set; } = string.Empty;
            public string RegulationName { get; set; } = string.Empty;
            public string TestItemName { get; set; } = string.Empty;
            public string RevisionName { get; set; } = string.Empty;
            public string StatusName { get; set; } = string.Empty;
            public string DelayReason { get; set; } = string.Empty;
            public string Comment { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        public UcWorkLog()
        {
            InitializeComponent();
            this.Load += UcWorkLog_Load;

            var view = gridView1; // 你的 GridView 名稱

            view.OptionsBehavior.Editable = false;    // 預設不可編輯（不要讓工程師亂修改）

            gridView1.Columns["Comment"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["Comment"].AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;

            // 日期欄位格式
            view.Columns["WorkDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            view.Columns["WorkDate"].DisplayFormat.FormatString = "yyyy-MM-dd";

            // 工時欄位
            view.Columns["ActualHours"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            view.Columns["ActualHours"].DisplayFormat.FormatString = "0.00";

            // 建立時間
            view.Columns["CreatedDate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            view.Columns["CreatedDate"].DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";          

            // 設定欄位寬度（你可微調）
            view.Columns["WorkDate"].Width = 150;
            view.Columns["ActualHours"].Width = 70;
            view.Columns["WorkTypeName"].Width = 70;
            view.Columns["ProjectNumber"].Width = 180;
            view.Columns["RegulationName"].Width = 85;
            view.Columns["TestItemName"].Width = 180;
            view.Columns["RevisionName"].Width = 100;
            view.Columns["StatusName"].Width = 70;
            view.Columns["DelayReason"].Width = 150;
            view.Columns["Comment"].Width = 200;
            view.Columns["CreatedDate"].Width = 150;

            gridView1.RowCellClick += GridView1_RowCellClick;
        }

        private void gridWorkLog_Click(object sender, EventArgs e)
        {

        }

        private void UcWorkLog_Load(object sender, EventArgs e)
        {
            var dummyList = new List<WorkLogDto>
            {
                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-1),
                    ActualHours = 6.5m,
                    WorkTypeName = "測試",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "FCC",
                    TestItemName = "WWAN_Conducted",
                    RevisionName = "v1",
                    StatusName = "進行中",
                    DelayReason = "",
                    Comment = "完成一半測試項目",
                    CreatedDate = DateTime.Now.AddDays(-1),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-2),
                    ActualHours = 7.0m,
                    WorkTypeName = "測試",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "FCC",
                    TestItemName = "Radiated",
                    RevisionName = "v1",
                    StatusName = "完成",
                    DelayReason = "",
                    Comment = "測試完成",
                    CreatedDate = DateTime.Now.AddDays(-2),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-3),
                    ActualHours = 8.0m,
                    WorkTypeName = "測試",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "CE",
                    TestItemName = "Rx-Blocking",
                    RevisionName = "v2 - 補測(Command)",
                    StatusName = "延遲",
                    DelayReason = "設備故障",
                    Comment = "設備校正延後",
                    CreatedDate = DateTime.Now.AddDays(-3),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-4),
                    ActualHours = 4.0m,
                    WorkTypeName = "非測試",
                    ProjectNumber = "",
                    RegulationName = "",
                    TestItemName = "",
                    RevisionName = "",
                    StatusName = "Pending",
                    DelayReason = "",
                    Comment = "教育訓練",
                    CreatedDate = DateTime.Now.AddDays(-4),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-5),
                    ActualHours = 3.5m,
                    WorkTypeName = "測試",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "CE",
                    TestItemName = "Wifi_Condcted",
                    RevisionName = "v1",
                    StatusName = "進行中",
                    DelayReason = "",
                    Comment = "",
                    CreatedDate = DateTime.Now.AddDays(-5),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-9),
                    ActualHours = 8.0m,
                    WorkTypeName = "進行中",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "CE",
                    TestItemName = "Adaptivity",
                    RevisionName = "v3 - 重測",
                    StatusName = "進行中",
                    DelayReason = "",
                    Comment = "今日完成 Adaptivity Pre-check",
                    CreatedDate = DateTime.Now.AddDays(-9),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-10),
                    ActualHours = 2.0m,
                    WorkTypeName = "非測試",
                    ProjectNumber = "",
                    RegulationName = "",
                    TestItemName = "",
                    RevisionName = "",
                    StatusName = "Pending",
                    DelayReason = "",
                    Comment = "開會",
                    CreatedDate = DateTime.Now.AddDays(-10),
                }
            };

            gridWorkLog.DataSource = dummyList;
        }

        private void panelFilter_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            // 如果點到的是「操作」欄位
            if (e.Column.Caption == "操作")
            {
                var row = gridView1.GetRow(e.RowHandle) as WorkLogDto;

                if (row == null) return;

                MessageBox.Show($"你點到 {row.WorkDate:yyyy-MM-dd} 的操作按鈕！");

                // 之後改成 -->
                // new FrmWorkLogEdit(row).ShowDialog();
            }
        }

    }
}
