namespace RF_Schedule
{
    partial class FrmOtherWorkLog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            memoNote = new DevExpress.XtraEditors.MemoEdit();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            dateWorkDate = new DevExpress.XtraEditors.DateEdit();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            spinHours = new DevExpress.XtraEditors.SpinEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            cmbWorkType = new DevExpress.XtraEditors.ComboBoxEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)memoNote.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinHours.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cmbWorkType.Properties).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(309, 324);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(90, 32);
            btnCancel.TabIndex = 18;
            btnCancel.Text = "取消";
            // 
            // btnSubmit
            // 
            btnSubmit.Location = new System.Drawing.Point(139, 324);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(90, 32);
            btnSubmit.TabIndex = 17;
            btnSubmit.Text = "送出";
            // 
            // memoNote
            // 
            memoNote.Location = new System.Drawing.Point(124, 214);
            memoNote.Name = "memoNote";
            memoNote.Size = new System.Drawing.Size(298, 80);
            memoNote.TabIndex = 16;
            // 
            // labelControl4
            // 
            labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl4.Location = new System.Drawing.Point(124, 180);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(56, 28);
            labelControl4.TabIndex = 10;
            labelControl4.Text = "備註：";
            // 
            // dateWorkDate
            // 
            dateWorkDate.EditValue = "DateTime.Today";
            dateWorkDate.Location = new System.Drawing.Point(222, 138);
            dateWorkDate.Name = "dateWorkDate";
            dateWorkDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Classic;
            dateWorkDate.Properties.MaskSettings.Set("mask", "D");
            dateWorkDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            dateWorkDate.Size = new System.Drawing.Size(140, 28);
            dateWorkDate.TabIndex = 15;
            // 
            // labelControl3
            // 
            labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl3.Location = new System.Drawing.Point(124, 140);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(89, 23);
            labelControl3.TabIndex = 14;
            labelControl3.Text = "工作日期：";
            // 
            // spinHours
            // 
            spinHours.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            spinHours.Location = new System.Drawing.Point(222, 95);
            spinHours.Name = "spinHours";
            spinHours.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinHours.Properties.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            spinHours.Properties.MaskSettings.Set("mask", "0.00");
            spinHours.Properties.MaxValue = new decimal(new int[] { 12, 0, 0, 0 });
            spinHours.Size = new System.Drawing.Size(100, 28);
            spinHours.TabIndex = 13;
            // 
            // labelControl2
            // 
            labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl2.Location = new System.Drawing.Point(124, 97);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(92, 23);
            labelControl2.TabIndex = 12;
            labelControl2.Text = "工時 (hr)：";
            // 
            // cmbWorkType
            // 
            cmbWorkType.Location = new System.Drawing.Point(222, 52);
            cmbWorkType.Name = "cmbWorkType";
            cmbWorkType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cmbWorkType.Properties.Items.AddRange(new object[] { "開會", "教育訓練", "PM指派任務", "協助其他工程師", "公版維護", "其他" });
            cmbWorkType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cmbWorkType.Size = new System.Drawing.Size(200, 28);
            cmbWorkType.TabIndex = 11;
            // 
            // labelControl1
            // 
            labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            labelControl1.Location = new System.Drawing.Point(72, 51);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(144, 29);
            labelControl1.TabIndex = 9;
            labelControl1.Text = "非測試工時類型：";
            // 
            // FrmOtherWorkLog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(555, 441);
            Controls.Add(btnCancel);
            Controls.Add(btnSubmit);
            Controls.Add(memoNote);
            Controls.Add(labelControl4);
            Controls.Add(dateWorkDate);
            Controls.Add(labelControl3);
            Controls.Add(spinHours);
            Controls.Add(labelControl2);
            Controls.Add(cmbWorkType);
            Controls.Add(labelControl1);
            Name = "FrmOtherWorkLog";
            ShowInTaskbar = false;
            Text = "其他工時回報";
            Load += FrmOtherWorkLog_Load;
            ((System.ComponentModel.ISupportInitialize)memoNote.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinHours.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cmbWorkType.Properties).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraEditors.MemoEdit memoNote;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.DateEdit dateWorkDate;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SpinEdit spinHours;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit cmbWorkType;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}