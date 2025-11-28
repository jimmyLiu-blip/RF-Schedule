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

    public partial class UcMyWork : DevExpress.XtraEditors.XtraUserControl
    {
        public class WorkItem
        {
            public string ProjectName { get; set; }
            public string RegulationName { get; set; }
            public string TestItemName { get; set; }
            public string Status { get; set; }
            public string Priority { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal RemainingHours { get; set; }
            public string Action { get; set; }
        }

        public UcMyWork()
        {
            InitializeComponent();
        }

        private void UcMyWork_Load(object sender, EventArgs e)
        {
            var data = new List<WorkItem>
    {
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "CE",
            TestItemName = "WWAN : Conducted",
            Status = "InProgress",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(2),
            RemainingHours = 3,
            Action = "回報工時"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "FCC",
            TestItemName = "WWAN : Conducted",
            Status = "NotStarted",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(5),
            RemainingHours = 5,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "NCC",
            TestItemName = "Wifi : Conducted",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(3),
            EndDate = DateTime.Today.AddDays(7),
            RemainingHours = 30,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "FCC",
            TestItemName = "Wifi : Conducted",
            Status = "NotStarted",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(10),
            EndDate = DateTime.Today.AddDays(12),
            RemainingHours = 60,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "NCC",
            TestItemName = "Radiated",
            Status = "InProgress",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(-3),
            EndDate = DateTime.Today.AddDays(1),
            RemainingHours = 12,
            Action = "回報工時"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "FCC",
            TestItemName = "Radiated",
            Status = "NotStarted",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(9),
            RemainingHours = 52,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "CE",
            TestItemName = "Adaptivity",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(12),
            EndDate = DateTime.Today.AddDays(15),
            RemainingHours = 56,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "CE",
            TestItemName = "Rx-Blocking",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(15),
            EndDate = DateTime.Today.AddDays(17),
            RemainingHours = 24,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000026",
            RegulationName = "Other",
            TestItemName = "自定義",
            Status = "NotStarted",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(20),
            EndDate = DateTime.Today.AddDays(21),
            RemainingHours = 24,
            Action = "未開始"
        },

        // 再來 10 筆不同專案的資料
        new WorkItem
        {
            ProjectName = "TE-2511000041",
            RegulationName = "CE",
            TestItemName = "WWAN : Conducted",
            Status = "InProgress",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(-2),
            EndDate = DateTime.Today.AddDays(3),
            RemainingHours = 4,
            Action = "回報工時"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000041",
            RegulationName = "FCC",
            TestItemName = "Radiated",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(4),
            EndDate = DateTime.Today.AddDays(7),
            RemainingHours = 18,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000041",
            RegulationName = "NCC",
            TestItemName = "Adaptivity",
            Status = "InProgress",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1),
            RemainingHours = 6,
            Action = "回報工時"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000041",
            RegulationName = "CE",
            TestItemName = "Rx-Blocking",
            Status = "NotStarted",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(8),
            EndDate = DateTime.Today.AddDays(10),
            RemainingHours = 28,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000052",
            RegulationName = "FCC",
            TestItemName = "Wifi : Conducted",
            Status = "InProgress",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(-4),
            EndDate = DateTime.Today.AddDays(1),
            RemainingHours = 2,
            Action = "回報工時"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000052",
            RegulationName = "NCC",
            TestItemName = "Radiated",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(6),
            EndDate = DateTime.Today.AddDays(9),
            RemainingHours = 40,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000052",
            RegulationName = "Other",
            TestItemName = "自定義測試",
            Status = "NotStarted",
            Priority = "Medium",
            StartDate = DateTime.Today.AddDays(12),
            EndDate = DateTime.Today.AddDays(14),
            RemainingHours = 12,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000063",
            RegulationName = "CE",
            TestItemName = "Blocking",
            Status = "NotStarted",
            Priority = "Low",
            StartDate = DateTime.Today.AddDays(3),
            EndDate = DateTime.Today.AddDays(4),
            RemainingHours = 10,
            Action = "未開始"
        },
        new WorkItem
        {
            ProjectName = "TE-2511000063",
            RegulationName = "FCC",
            TestItemName = "WWAN : Conducted",
            Status = "InProgress",
            Priority = "High",
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1),
            RemainingHours = 1,
            Action = "回報工時"
        }
    };

            gridControl1.DataSource = data;
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void repositoryActionButtons_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            int rowHandle = gridView1.FocusedRowHandle;

            // Index 0 = 回報工時（已經有）
            if (e.Button.Index == 0)
            {
                using (var dlg = new FrmWorkLogReport())
                {
                    // ⭐ 一定要先 ShowDialog() ⭐
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        // 使用者按了「送出」
                        gridView1.SetRowCellValue(rowHandle, "ActualHours", dlg.EnteredHours);
                        gridView1.SetRowCellValue(rowHandle, "Status", dlg.SelectedStatus);
                        gridView1.SetRowCellValue(rowHandle, "Comment", dlg.Comment);
                        gridView1.SetRowCellValue(rowHandle, "WorkDate", dlg.WorkDate);

                        gridView1.RefreshRow(rowHandle);

                        XtraMessageBox.Show("工時已成功回報（Prototype）");
                    }
                }

                return;
            }

            // Index 1 = 完成（現在要新增）
            if (e.Button.Index == 1)
            {
                // 模擬：把狀態改為 Completed
                gridView1.SetRowCellValue(rowHandle, "Status", "Completed");

                // 讓 UI 重新繪製
                gridView1.RefreshRow(rowHandle);

                XtraMessageBox.Show("此測項已標記為 Completed（UI Prototype）");
                return;
            }
        }

        private void panelFilter_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
