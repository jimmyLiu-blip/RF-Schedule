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
        public string SelectedRevision { get; set; }
        public decimal EnteredHours { get; set; }
        public string SelectedStatus { get; set; }
        public string SelectedDelayReason { get; set; }
        public string Comment { get; set; }
        public DateTime WorkDate { get; set; }

        public FrmWorkLogReport()
        {
            InitializeComponent();
        }

        private void lblTestItemName_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SelectedRevision = cboRevision.Text;
            EnteredHours = Convert.ToDecimal(spinWorkHours.Value);
            SelectedStatus = cboStatus.Text;
            SelectedDelayReason = cboDelayReason.Text;
            Comment = memoComment.Text;
            WorkDate = dateWorkDate.DateTime;
            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private void FrmWorkLogReport_Load(object sender, EventArgs e)
        {

        }

        private void cboRevision_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}