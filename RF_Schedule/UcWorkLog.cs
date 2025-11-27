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
            public bool IsSupportOther { get; set; }
            public string SupportEngineerName { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        public UcWorkLog()
        {
            InitializeComponent();
            this.Load += UcWorkLog_Load;
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
                    IsSupportOther = false,
                    SupportEngineerName = "",
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
                    IsSupportOther = false,
                    SupportEngineerName = "",
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
                    RevisionName = "v2",
                    StatusName = "延遲",
                    DelayReason = "設備故障",
                    Comment = "設備校正延後",
                    IsSupportOther = false,
                    SupportEngineerName = "",
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
                    StatusName = "非測試",
                    DelayReason = "",
                    Comment = "教育訓練",
                    IsSupportOther = false,
                    SupportEngineerName = "",
                    CreatedDate = DateTime.Now.AddDays(-4),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-5),
                    ActualHours = 3.5m,
                    WorkTypeName = "測試",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "CE",
                    TestItemName = "Wifi Condcted",
                    RevisionName = "v1",
                    StatusName = "進行中",
                    DelayReason = "",
                    Comment = "",
                    IsSupportOther = true,
                    SupportEngineerName = "王小明",
                    CreatedDate = DateTime.Now.AddDays(-5),
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today,
                    ActualHours = 8.0m,
                    WorkTypeName = "進行中",
                    ProjectNumber = "TE-2511000026",
                    RegulationName = "CE",
                    TestItemName = "Adaptivity",
                    RevisionName = "v3",
                    StatusName = "進行中",
                    DelayReason = "",
                    Comment = "今日完成 Adaptivity Pre-check",
                    IsSupportOther = false,
                    SupportEngineerName = "",
                    CreatedDate = DateTime.Now,
                },

                new WorkLogDto
                {
                    WorkDate = DateTime.Today.AddDays(-7),
                    ActualHours = 2.0m,
                    WorkTypeName = "非測試",
                    ProjectNumber = "",
                    RegulationName = "",
                    TestItemName = "",
                    RevisionName = "",
                    StatusName = "非測試",
                    DelayReason = "",
                    Comment = "開會",
                    IsSupportOther = false,
                    SupportEngineerName = "",
                    CreatedDate = DateTime.Now.AddDays(-7),
                }
            };

            gridWorkLog.DataSource = dummyList;
        }
    }
}
