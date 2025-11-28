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
    public partial class UcProjectList : DevExpress.XtraEditors.XtraUserControl
    {
        public class ProjectDto
        {
            public string ProjectName { get; set; } = string.Empty;
            public string Customer { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Note { get; set; } = string.Empty;
            public DateTime CreatedDate { get; set; }
        }

        public UcProjectList()
        {
            InitializeComponent();
        }

        private void UcProjectList_Load(object sender, EventArgs e)
        {
            var dummy = new List<ProjectDto>();

            // 先放你原本兩筆
            dummy.Add(new ProjectDto
            {
                ProjectName = "TE-2511000011",
                Customer = "ABC",
                Priority = "High",
                Status = "進行中",
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddDays(5),
                Note = "目前進度正常",
                CreatedDate = DateTime.Today.AddDays(-12)
            });

            dummy.Add(new ProjectDto
            {
                ProjectName = "TE-2511000026",
                Customer = "XYZ",
                Priority = "Medium",
                Status = "草稿",
                StartDate = null,
                EndDate = null,
                Note = "新客戶專案，尚未排程",
                CreatedDate = DateTime.Today.AddDays(-1)
            });

            // 用迴圈多生一些資料（編號、客戶、日期都帶一點規則）
            for (int i = 3; i <= 30; i++)
            {
                dummy.Add(new ProjectDto
                {
                    ProjectName = $"TE-25110{i:0000}",
                    Customer = $"Customer-{i:00}",
                    Priority = (i % 3 == 0) ? "High" : (i % 3 == 1) ? "Medium" : "Low",
                    Status = (i % 4) switch
                    {
                        0 => "草稿",
                        1 => "進行中",
                        2 => "待審核",
                        _ => "完成"
                    },
                    StartDate = DateTime.Today.AddDays(-i),
                    EndDate = DateTime.Today.AddDays(+i / 2),
                    Note = (i % 5 == 0) ? "客戶文件補件中" :
                           (i % 5 == 1) ? "樣品等待中" :
                           (i % 5 == 2) ? "排程與 WiFi 場地衝突" :
                           (i % 5 == 3) ? "測試中，尚有部份項目未完成" :
                                          "進度正常",
                    CreatedDate = DateTime.Today.AddDays(-i - 2)
                });
            }

            gridControl1.DataSource = dummy;
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }
    }
}
