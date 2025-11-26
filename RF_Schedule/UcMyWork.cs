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
            // Index 0 = 回報工時（已經有）
            if (e.Button.Index == 0)
            {
                MessageBox.Show("回報工時 按下（之後會跳出工時回報視窗）");
                return;
            }

            // Index 1 = 完成（現在要新增）
            if (e.Button.Index == 1)
            {
                // 取得目前點擊的列
                int rowHandle = gridView1.FocusedRowHandle;

                // 模擬：把狀態改為 Completed
                gridView1.SetRowCellValue(rowHandle, "Status", "Completed");

                // 讓 UI 重新繪製
                gridView1.RefreshRow(rowHandle);

                MessageBox.Show("此測項已標記為 Completed（UI 模擬用）");
            }
        }
    }
}
