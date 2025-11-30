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
    public partial class UcProjectGanttChart : DevExpress.XtraEditors.XtraUserControl
    {
        public UcProjectGanttChart()
        {
            InitializeComponent();
            SetupGanttMappings();
            LoadSampleData();
        }

        private void ganttControl1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        private void SetupGanttMappings()
        {
            ganttControl1.TreeListMappings.KeyFieldName = "Id";
            ganttControl1.TreeListMappings.ParentFieldName = "ParentId";

            ganttControl1.TreeListMappings.TreeViewFieldName = "Name";
            ganttControl1.ChartMappings.StartDateFieldName = "StartDate";
            ganttControl1.ChartMappings.FinishDateFieldName = "EndDate";
        }

        private void LoadSampleData()
        {
            var list = new List<GanttData>()
            {
        new GanttData { Id = 1, ParentId = 0, Name="Conducted_1", StartDate=DateTime.Parse("2025/11/20"), EndDate=DateTime.Parse("2025/12/10") },
        new GanttData { Id = 2, ParentId = 1, Name="FCC", StartDate=DateTime.Parse("2025/11/20"), EndDate=DateTime.Parse("2025/11/30") },
        new GanttData { Id = 3, ParentId = 2, Name="WIFI_Conducted", StartDate=DateTime.Parse("2025/11/20"), EndDate=DateTime.Parse("2025/11/30") },
        new GanttData { Id = 4, ParentId = 1, Name="NCC", StartDate=DateTime.Parse("2025/11/30"), EndDate=DateTime.Parse("2025/12/10") },
        new GanttData { Id = 5, ParentId = 4, Name="WIFI_Conducted", StartDate=DateTime.Parse("2025/11/30"), EndDate=DateTime.Parse("2025/12/10") }
            };

            ganttControl1.DataSource = list;
        }
        public class GanttData
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

    }
}
