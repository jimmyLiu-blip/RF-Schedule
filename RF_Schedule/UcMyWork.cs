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
            if (e.Button.Index == 0)
            {
                MessageBox.Show("回報工時 按下（之後會跳出工時回報視窗）");
            }
            else if (e.Button.Index == 1)
            {
                MessageBox.Show("完成 按下（之後會把測項狀態改為 Completed）");
            }
        }
    }
}
