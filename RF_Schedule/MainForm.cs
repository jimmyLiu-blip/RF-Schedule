using DevExpress.XtraBars.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RF_Schedule
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            InitializeComponent();

        }

        private void panelMainContent_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void accordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
            if (e.Element == accordionControlElement5)
            {
                LoadUserControl(new UcMyWork());
            }
        }

        private void accordionControlElement22_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmOtherWorkLog())
            {
                frm.ShowDialog(this);
            }
        }

        private void accordionControlElement6_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcWorkLog());
        }

        public void LoadUserControl(UserControl uc)
        {
            panelMainContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelMainContent.Controls.Add(uc);
        }

        private void accordionControlElement8_Click(object sender, EventArgs e)
        {
            LoadUserControl(new UcProjectList());
        }
    }
}
