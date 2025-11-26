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
    public partial class FrmWorkLogReport : DevExpress.XtraEditors.XtraForm
    {
        public FrmWorkLogReport()
        {
            InitializeComponent();
        }

        private void lblTestItemName_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("工時已成功回報！（Prototype）",
                        "成功",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
        }

        private void cboStatus_EditValueChanged(object sender, EventArgs e)
        {
            string status = cboStatus.EditValue?.ToString();

            if (status == "Delayed")
            {
                cboDelayReason.Enabled = true;
            }
            else
            {
                cboDelayReason.Enabled = false;
                cboDelayReason.EditValue = null; // 清空
            }
        }
    }
}