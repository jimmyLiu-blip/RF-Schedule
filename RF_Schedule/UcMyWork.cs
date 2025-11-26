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
        public UcMyWork()
        {
            InitializeComponent();
        }

        private void UcMyWork_Load(object sender, EventArgs e)
        {

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
