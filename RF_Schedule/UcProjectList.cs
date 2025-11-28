using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
    public partial class UcProjectList : DevExpress.XtraEditors.XtraUserControl
    {
        public class Project
        {
            public string ProjectName { get; set; }
            public string Customer { get; set; }
            public string Priority { get; set; }
            public string Status { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Note { get; set; }
            public DateTime CreatedDate { get; set; }

            public List<Regulation> Regulations { get; set; } = new List<Regulation>();
        }

        public class Regulation
        {
            public string RegulationName { get; set; }
            public string Status { get; set; }
            public string Note { get; set; }

            public List<TestItem> TestItems { get; set; } = new List<TestItem>();
        }

        public class TestItem
        {
            public string TestItemName { get; set; }
            public string Status { get; set; }
            public decimal EstimatedHours { get; set; }

            public List<TestItemEngineer> Engineers { get; set; } = new List<TestItemEngineer>();
        }

        public class TestItemEngineer
        {
            public string EngineerName { get; set; }
            public string RoleType { get; set; }
            public decimal AssignedHours { get; set; }
        }


        public UcProjectList()
        {
            InitializeComponent();
        }

        private void UcProjectList_Load(object sender, EventArgs e)
        {
            // ==========================
            // ✔ 1. 產生假資料：Project → Regulation → TestItem → Engineer
            // ==========================

            var dummy = new List<Project>();
            var rand = new Random();

            // --- Base Project Data (Your original two) ---
            dummy.Add(new Project
            {
                ProjectName = "TE-2511000011",
                Customer = "ABC",
                Priority = "High",
                Status = "進行中",
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddDays(5),
                Note = "目前進度正常",
                CreatedDate = DateTime.Today.AddDays(-12),
                Regulations = GenerateFakeRegulations(rand)
            });

            dummy.Add(new Project
            {
                ProjectName = "TE-2511000026",
                Customer = "XYZ",
                Priority = "Medium",
                Status = "草稿",
                StartDate = null,
                EndDate = null,
                Note = "新客戶專案，尚未排程",
                CreatedDate = DateTime.Today.AddDays(-1),
                Regulations = GenerateFakeRegulations(rand)
            });

            // --- Auto Generate Project 3~30 ---
            for (int i = 3; i <= 30; i++)
            {
                dummy.Add(new Project
                {
                    ProjectName = $"TE-25110{i:0000}",
                    Customer = $"Customer-{i:00}",
                    Priority = (i % 3 == 0) ? "High" : (i % 3 == 1) ? "Medium" : "Low",
                    Status = (i % 4) switch
                    {
                        0 => "草稿",
                        1 => "進行中",
                        2 => "待審核",
                        _ => "完成"
                    },
                    StartDate = DateTime.Today.AddDays(-i),
                    EndDate = DateTime.Today.AddDays(+i / 2),
                    Note = (i % 5) switch
                    {
                        0 => "客戶文件補件中",
                        1 => "樣品等待中",
                        2 => "排程與 WiFi 場地衝突",
                        3 => "測試中，尚有部份項目未完成",
                        _ => "進度正常"
                    },
                    CreatedDate = DateTime.Today.AddDays(-i - 2),
                    Regulations = GenerateFakeRegulations(rand)
                });
            }

            gridControl1.DataSource = dummy;


            // ============================
            // ⬇⬇⬇ 在這裡開始放 Master-Detail 設定 ⬇⬇⬇
            // ============================

            // 1. 建立第一層：Project → Regulations
            gridControl1.LevelTree.Nodes.Add("Regulations", viewRegulation);

            // 2. 第二層：Regulation → TestItems
            gridControl1.LevelTree.Nodes["Regulations"]
                .Nodes.Add("TestItems", viewTestItem);

            // 3. 第三層：TestItem → Engineers
            gridControl1.LevelTree.Nodes["Regulations"]
                .Nodes["TestItems"]
                .Nodes.Add("Engineers", viewEngineer);

            // --- 綁定事件 ---
            // Project → Regulations
            viewProject.MasterRowGetChildList += viewProject_MasterRowGetChildList;
            viewProject.MasterRowGetRelationCount += viewProject_MasterRowGetRelationCount;
            viewProject.MasterRowGetRelationName += viewProject_MasterRowGetRelationName;

            // 讓子 View 不顯示 Caption 列 (Regulations / TestItems / Engineers 那條灰條)

            // 法規層
            viewRegulation.ViewCaption = string.Empty;
            viewRegulation.ViewCaptionHeight = 0;

            // 測試項目層
            viewTestItem.ViewCaption = string.Empty;
            viewTestItem.ViewCaptionHeight = 0;

            // 工程師層
            viewEngineer.ViewCaption = string.Empty;
            viewEngineer.ViewCaptionHeight = 0;
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        // 1. 回傳子集合
        private void viewProject_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var project = (Project)viewProject.GetRow(e.RowHandle);
            if (e.RelationIndex == 0)
                e.ChildList = project.Regulations;
        }

        // 2. 回傳子關聯數量（你有多少個子層）
        private void viewProject_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1; // Project 只有 Regulations 這一層
        }

        // 3. 回傳 RelationName（要跟 Level1 的 RelationName 一致）
        private void viewProject_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            if (e.RelationIndex == 0)
                e.RelationName = "Regulations";
        }

        private void viewRegulation_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var regulation = (Regulation)viewRegulation.GetRow(e.RowHandle);
            if (e.RelationIndex == 0)
                e.ChildList = regulation.TestItems;
        }

        private void viewRegulation_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void viewRegulation_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            if (e.RelationIndex == 0)
                e.RelationName = "TestItems";
        }

        private void viewTestItem_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var item = (TestItem)viewTestItem.GetRow(e.RowHandle);
            if (e.RelationIndex == 0)
                e.ChildList = item.Engineers;
        }

        private void viewTestItem_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void viewTestItem_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            if (e.RelationIndex == 0)
                e.RelationName = "Engineers";
        }

        private List<Regulation> GenerateFakeRegulations(Random rand)
        {
            string[] regulationNames = { "FCC Part 22", "FCC Part 24", "FCC Part 27", "NCC PLMN", "CE RED", "IC RSS" };

            var list = new List<Regulation>();
            int count = rand.Next(2, 4); // 每個 Project 隨機 2~3 個 regulation

            for (int i = 0; i < count; i++)
            {
                list.Add(new Regulation
                {
                    RegulationName = regulationNames[rand.Next(regulationNames.Length)],
                    Status = (i % 3 == 0) ? "進行中" : (i % 3 == 1) ? "草稿" : "完成",
                    Note = "自動產生法規",
                    TestItems = GenerateFakeTestItems(rand)
                });
            }

            return list;
        }


        private List<TestItem> GenerateFakeTestItems(Random rand)
        {
           string[] items =
           {
            "Conducted Emission",
            "Radiated Spurious",
            "Blocking",
            "Receiver Sensitivity",
            "EIRP",
            "Power Density",
            "OOB Test"
           };

         var list = new List<TestItem>();
         int count = rand.Next(2, 5); // 每個 Regulation 2~4 個 TestItem

        for (int i = 0; i < count; i++)
        {
           list.Add(new TestItem
           {
              TestItemName = items[rand.Next(items.Length)],
              Status = (i % 3 == 0) ? "未開始" : (i % 3 == 1) ? "進行中" : "完成",
              EstimatedHours = rand.Next(8, 40),
              Engineers = GenerateFakeEngineers(rand)
           });
        }

           return list;
        }

        private List<TestItemEngineer> GenerateFakeEngineers(Random rand)
        {
            string[] names = { "Allen", "Brian", "Cindy", "Derek", "Emily", "Frank", "Grace" };
            string[] roles = { "Main1", "Main2", "Support" };

            var list = new List<TestItemEngineer>();
            int count = rand.Next(1, 3); // 1~2 位工程師

            for (int i = 0; i < count; i++)
            {
                list.Add(new TestItemEngineer
                {
                    EngineerName = names[rand.Next(names.Length)],
                    RoleType = roles[rand.Next(roles.Length)],
                    AssignedHours = rand.Next(1, 10)
                });
            }
            return list;
        }

    }
}
