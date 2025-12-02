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
            // TreeList 結構綁定
            ganttControl1.TreeListMappings.KeyFieldName = "Id";
            ganttControl1.TreeListMappings.ParentFieldName = "ParentId";
            ganttControl1.TreeListMappings.TreeViewFieldName = "ColumnName";

            // 甘特條綁定
            ganttControl1.ChartMappings.StartDateFieldName = "StartDate";
            ganttControl1.ChartMappings.FinishDateFieldName = "EndDate";
        }



        private void LoadSampleData()
        {
            var list = new List<GanttNode>();
            int id = 1;

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

            // 固定 mapping（你指定的）
            var locationMap = new Dictionary<string, (string[] testItems, string[] engineers)>
            {
                ["Conducted 1"] = (new[] { "WIFI_Conducted" }, new[] { "Chris" }),
                ["Conducted 2"] = (new[] { "WIFI_Conducted" }, new[] { "Ian" }),
                ["Conducted 3"] = (new[] { "WWAN_Conducted" }, new[] { "Thomas", "Darren" }),
                ["Conducted 4"] = (new[] { "WWAN_Conducted" }, new[] { "Brian" }),
                ["Conducted 5"] = (new[] { "Rx-Blocking", "Adaptivity" }, new[] { "Billy" }),
                ["Conducted 6"] = (new[] { "DFS", "PWS" }, new[] { "Zen" }),
                ["966-1"] = (new[] { "Radiated" }, new[] { "WJ", "Alex" }),
                ["966-2"] = (new[] { "Radiated" }, new[] { "Bob", "GN" }),
                ["966-3"] = (new[] { "Radiated" }, new[] { "Nick", "Dante" }),
                ["1166"] = (new[] { "Radiated" }, new[] { "Jack", "Kane" }),
            };

            var regulations = new[] { "FCC", "NCC", "CE", "IC", "TELEC" };
            var statuses = new[] { "NotStarted", "InProgress", "Fail" };

            Random rnd = new Random();

            // ——————————————————————————
            // ① 加入場地（root node）
            // ——————————————————————————
            var locId = new Dictionary<string, int>();

            foreach (var loc in locationMap.Keys)
            {
                var n = Node(loc, "Location", 0);
                list.Add(n);
                locId[loc] = n.Id;
            }

            // ——————————————————————————
            // ② 每個場地下 2～5 組假資料
            // ——————————————————————————
            foreach (var loc in locationMap.Keys)
            {
                int locNodeId = locId[loc];
                var (testItems, engineers) = locationMap[loc];

                int projectCount = rnd.Next(2, 6);  // 2～5

                for (int pIdx = 0; pIdx < projectCount; pIdx++)
                {
                    string projName = $"TE2511-{rnd.Next(100000, 999999)}";

                    // Project
                    var project = Node(projName, "Project", locNodeId);
                    list.Add(project);

                    // 每個專案 2～4 法規
                    int regCount = rnd.Next(2, 5);

                    for (int rIdx = 0; rIdx < regCount; rIdx++)
                    {
                        string reg = regulations[rnd.Next(regulations.Length)];

                        var regNode = Node(reg, "Regulation", project.Id);
                        list.Add(regNode);

                        // 每個法規底下：場地固定的測項們
                        foreach (var ti in testItems)
                        {
                            // 日期：限制到 2026/01/20
                            var start = RandomDate(new DateTime(2025, 12, 1), new DateTime(2026, 1, 10));
                            var end = start.AddDays(rnd.Next(2, 8));

                            if (end > new DateTime(2026, 1, 20))
                                end = new DateTime(2026, 1, 20);

                            string eng = engineers[rnd.Next(engineers.Length)];

                            list.Add(Node(
                                ti,
                                "TestItem",
                                regNode.Id,
                                start,
                                end,
                                statuses[rnd.Next(statuses.Length)],
                                eng
                            ));
                        }
                    }
                }
            }

            // ——————————————————————————
            // ③ 組合顯示欄位（甘特圖左側顯示用）
            // ——————————————————————————
            foreach (var node in list)
            {
                if (node.NodeType == "TestItem")
                {
                    string sDate = node.StartDate?.ToString("MM/dd") ?? "";
                    string eDate = node.EndDate?.ToString("MM/dd") ?? "";

                    node.ColumnName =
                        $"{node.ColumnName}   [{sDate} ~ {eDate}]   ({node.Status})   - {node.Engineer}";
                }
            }

            ganttControl1.DataSource = list;
        }

        // ——————————————————————————
        // 亂數日期 (含結束限制邏輯)
        // ——————————————————————————
        private DateTime RandomDate(DateTime min, DateTime max)
        {
            var rnd = new Random();
            int range = (max - min).Days;
            return min.AddDays(rnd.Next(range));
        }
    }
}
