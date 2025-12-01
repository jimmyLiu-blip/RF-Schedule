using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;

namespace RF_Schedule
{
    public partial class UcProjectGanttChart : DevExpress.XtraEditors.XtraUserControl
    {
        public UcProjectGanttChart()
        {
            InitializeComponent();
            ConfigureGantt();   // 設定 Mapping
            LoadSampleData();   // 先塞假資料看畫面
        }

        private void ganttControl1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        // === 這是你的甘特資料節點 ===
        public class GanttNode
        {
            public int Id { get; set; }          // 唯一編號
            public int ParentId { get; set; }    // 上層 Id（做樹狀用）

            public string ColumnName { get; set; } = "";      // 顯示名稱：場地 / 案件 / 法規 / 測項
            public string NodeType { get; set; } = "";  // Location / Project / Regulation / TestItem

            public DateTime? StartDate { get; set; }    // 只有測項會有日期，其它可以是 null
            public DateTime? EndDate { get; set; }

            public string Status { get; set; } = "";    // 測項狀態：NotStarted / InProgress / Completed / Fail
            public string Engineer { get; set; } = "";  // 負責工程師（可先空白）
        }

        private void ConfigureGantt()
        {
            ganttControl1.TreeListMappings.KeyFieldName = "Id";
            ganttControl1.TreeListMappings.ParentFieldName = "ParentId";
            ganttControl1.TreeListMappings.TreeViewFieldName = "ColumnName";
            ganttControl1.ChartMappings.StartDateFieldName = "StartDate";
            ganttControl1.ChartMappings.FinishDateFieldName = "EndDate";

            ganttControl1.Columns.Clear();

            var col0 = ganttControl1.Columns.AddField("ColumnName");
            col0.Caption = "名稱";
            col0.Visible = true;
            col0.VisibleIndex = 0;
            col0.Width = 150;  // 明確設定寬度

            var col1 = ganttControl1.Columns.AddField("StartDate");
            col1.Caption = "開始日期";
            col1.Visible = true;
            col1.VisibleIndex = 1;
            col1.Width = 100;

            var col2 = ganttControl1.Columns.AddField("EndDate");
            col2.Caption = "結束日期";
            col2.Visible = true;
            col2.VisibleIndex = 2;
            col2.Width = 100;

            var col3 = ganttControl1.Columns.AddField("Status");
            col3.Caption = "狀態";
            col3.Visible = true;
            col3.VisibleIndex = 3;
            col3.Width = 80;

            var col4 = ganttControl1.Columns.AddField("Engineer");
            col4.Caption = "工程師";
            col4.Visible = true;
            col4.VisibleIndex = 4;
            col4.Width = 80;
        }


        private void LoadSampleData()
        {
            var list = new List<GanttNode>();

            int id = 1;

            // 工具函數：快速建立節點
            GanttNode Node(string name, string type, int parent,
                           DateTime? s = null, DateTime? e = null,
                           string status = "", string eng = "")
            {
                return new GanttNode
                {
                    Id = id++,
                    ParentId = parent,
                    ColumnName = name,
                    NodeType = type,
                    StartDate = s,
                    EndDate = e,
                    Status = status,
                    Engineer = eng
                };
            }

            //===============================
            // 場地清單
            //===============================
            var locations = new[]
            {
        "Conducted 1", "Conducted 2", "Conducted 3",
        "Conducted 4", "Conducted 5", "Conducted 6",
        "966-1", "966-2", "966-3", "1166"
            };

            // Level 1：場地
            var locId = new Dictionary<string, int>();

            foreach (var loc in locations)
            {
                var n = Node(loc, "Location", 0);
                list.Add(n);
                locId[loc] = n.Id;
            }

            //===============================
            // 每個場地給一個 Project（示例）
            //===============================
            string projectName = "TE-2511000026";

            //===============================
            // 多種測項名稱
            //===============================
            var WIFI = "WIFI_Conducted";
            var WWAN = "WWAN_Conducted";
            var BLOCK = "Rx-Blocking";
            var ADAPT = "Adaptivity";
            var DFS = "DFS";
            var PWS = "PWS";
            var RAD = "Radiated";

            //===============================
            // 場地對應測項
            //===============================
            var map = new Dictionary<string, string[]>
            {
                ["Conducted 1"] = new[] { WIFI },
                ["Conducted 2"] = new[] { WIFI },
                ["Conducted 3"] = new[] { WWAN },
                ["Conducted 4"] = new[] { WWAN },
                ["Conducted 5"] = new[] { BLOCK, ADAPT },
                ["Conducted 6"] = new[] { DFS, PWS },
                ["966-1"] = new[] { RAD },
                ["966-2"] = new[] { RAD },
                ["966-3"] = new[] { RAD },
                ["1166"] = new[] { RAD },
            };

            Random rnd = new Random();

            string[] engineers = { "東海", "小賴", "利特", "神童", "銀赫" };
            string[] statuses = { "NotStarted", "InProgress", "Completed", "Fail" };

            //===============================
            // 逐一建立 Project → Regulation → TestItem
            //===============================
            foreach (var loc in locations)
            {
                int locNodeId = locId[loc];

                // Project
                var p = Node(projectName, "Project", locNodeId);
                list.Add(p);

                // Regulation（為簡化用 FCC）
                var r = Node("FCC", "Regulation", p.Id);
                list.Add(r);

                // 該場地負責哪些 TestItem
                foreach (var ti in map[loc])
                {
                    // 隨機 3~7 天的甘特條
                    var start = new DateTime(2025, 12, rnd.Next(1, 10));
                    var end = start.AddDays(rnd.Next(2, 6));

                    list.Add(Node(
                        ti,
                        "TestItem",
                        r.Id,
                        start,
                        end,
                        statuses[rnd.Next(statuses.Length)],
                        engineers[rnd.Next(engineers.Length)]
                    ));
                }
            }

            ganttControl1.DataSource = list;

            ganttControl1.RefreshDataSource();
            ganttControl1.BestFitColumns();
        }

    }
}
