namespace RF_Schedule
{
    partial class FrmWorkLogReport
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
            lblTestItemName = new DevExpress.XtraEditors.LabelControl();
            txtTestItemName = new DevExpress.XtraEditors.TextEdit();
            lblWorkDate = new DevExpress.XtraEditors.LabelControl();
            dateWorkDate = new DevExpress.XtraEditors.DateEdit();
            lblWorkHours = new DevExpress.XtraEditors.LabelControl();
            spinWorkHours = new DevExpress.XtraEditors.SpinEdit();
            lblComment = new DevExpress.XtraEditors.LabelControl();
            memoComment = new DevExpress.XtraEditors.MemoEdit();
            btnOk = new DevExpress.XtraEditors.SimpleButton();
            btnCancel = new DevExpress.XtraEditors.SimpleButton();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            txtRegulation = new DevExpress.XtraEditors.TextEdit();
            txtPriority = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            labelControl4 = new DevExpress.XtraEditors.LabelControl();
            txtEstimateHours = new DevExpress.XtraEditors.TextEdit();
            labelControl5 = new DevExpress.XtraEditors.LabelControl();
            labelControl6 = new DevExpress.XtraEditors.LabelControl();
            cboRevision = new DevExpress.XtraEditors.ComboBoxEdit();
            cboStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            cboDelayReason = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)txtTestItemName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinWorkHours.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoComment.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtRegulation.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtPriority.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtEstimateHours.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboRevision.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboDelayReason.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblTestItemName
            // 
            lblTestItemName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblTestItemName.Location = new System.Drawing.Point(74, 66);
            lblTestItemName.Name = "lblTestItemName";
            lblTestItemName.Size = new System.Drawing.Size(90, 22);
            lblTestItemName.TabIndex = 0;
            lblTestItemName.Text = "測項名稱：";
            lblTestItemName.Click += lblTestItemName_Click;
            // 
            // txtTestItemName
            // 
            txtTestItemName.EditValue = "WWAN：Conducted";
            txtTestItemName.Enabled = false;
            txtTestItemName.Location = new System.Drawing.Point(201, 66);
            txtTestItemName.Name = "txtTestItemName";
            txtTestItemName.Properties.ContextImageOptions.SvgImageSize = new System.Drawing.Size(250, 0);
            txtTestItemName.Properties.ReadOnly = true;
            txtTestItemName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            txtTestItemName.Size = new System.Drawing.Size(234, 28);
            txtTestItemName.TabIndex = 1;
            // 
            // lblWorkDate
            // 
            lblWorkDate.Location = new System.Drawing.Point(74, 291);
            lblWorkDate.Name = "lblWorkDate";
            lblWorkDate.Size = new System.Drawing.Size(90, 22);
            lblWorkDate.TabIndex = 2;
            lblWorkDate.Text = "工作日期：";
            // 
            // dateWorkDate
            // 
            dateWorkDate.EditValue = null;
            dateWorkDate.Location = new System.Drawing.Point(201, 291);
            dateWorkDate.Name = "dateWorkDate";
            dateWorkDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Size = new System.Drawing.Size(150, 28);
            dateWorkDate.TabIndex = 3;
            // 
            // lblWorkHours
            // 
            lblWorkHours.Location = new System.Drawing.Point(38, 246);
            lblWorkHours.Name = "lblWorkHours";
            lblWorkHours.Size = new System.Drawing.Size(126, 22);
            lblWorkHours.TabIndex = 4;
            lblWorkHours.Text = "實際工時 (hr)：";
            // 
            // spinWorkHours
            // 
            spinWorkHours.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            spinWorkHours.Location = new System.Drawing.Point(201, 246);
            spinWorkHours.Name = "spinWorkHours";
            spinWorkHours.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinWorkHours.Properties.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            spinWorkHours.Properties.MaxValue = new decimal(new int[] { 12, 0, 0, 0 });
            spinWorkHours.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 131072 });
            spinWorkHours.Size = new System.Drawing.Size(150, 28);
            spinWorkHours.TabIndex = 5;
            // 
            // lblComment
            // 
            lblComment.Location = new System.Drawing.Point(110, 426);
            lblComment.Name = "lblComment";
            lblComment.Size = new System.Drawing.Size(54, 22);
            lblComment.TabIndex = 6;
            lblComment.Text = "備註：";
            // 
            // memoComment
            // 
            memoComment.Location = new System.Drawing.Point(92, 468);
            memoComment.Name = "memoComment";
            memoComment.Size = new System.Drawing.Size(428, 100);
            memoComment.TabIndex = 7;
            // 
            // btnOk
            // 
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.Location = new System.Drawing.Point(127, 656);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(120, 41);
            btnOk.TabIndex = 8;
            btnOk.Text = "送出";
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(329, 656);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(120, 41);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "取消";
            // 
            // labelControl1
            // 
            labelControl1.Location = new System.Drawing.Point(110, 21);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(54, 22);
            labelControl1.TabIndex = 10;
            labelControl1.Text = "法規：";
            // 
            // txtRegulation
            // 
            txtRegulation.EditValue = "CE";
            txtRegulation.Enabled = false;
            txtRegulation.Location = new System.Drawing.Point(201, 21);
            txtRegulation.Name = "txtRegulation";
            txtRegulation.Properties.ReadOnly = true;
            txtRegulation.Size = new System.Drawing.Size(150, 28);
            txtRegulation.TabIndex = 11;
            // 
            // txtPriority
            // 
            txtPriority.EditValue = "Medium";
            txtPriority.Enabled = false;
            txtPriority.Location = new System.Drawing.Point(201, 111);
            txtPriority.Name = "txtPriority";
            txtPriority.Properties.ReadOnly = true;
            txtPriority.Size = new System.Drawing.Size(150, 28);
            txtPriority.TabIndex = 12;
            // 
            // labelControl2
            // 
            labelControl2.Location = new System.Drawing.Point(92, 111);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(72, 22);
            labelControl2.TabIndex = 13;
            labelControl2.Text = "優先級：";
            // 
            // labelControl3
            // 
            labelControl3.Location = new System.Drawing.Point(110, 156);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(54, 22);
            labelControl3.TabIndex = 14;
            labelControl3.Text = "版本：";
            // 
            // labelControl4
            // 
            labelControl4.Location = new System.Drawing.Point(38, 201);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new System.Drawing.Size(126, 22);
            labelControl4.TabIndex = 15;
            labelControl4.Text = "預估工時 (hr)：";
            // 
            // txtEstimateHours
            // 
            txtEstimateHours.Enabled = false;
            txtEstimateHours.Location = new System.Drawing.Point(201, 201);
            txtEstimateHours.Name = "txtEstimateHours";
            txtEstimateHours.Properties.ReadOnly = true;
            txtEstimateHours.Size = new System.Drawing.Size(150, 28);
            txtEstimateHours.TabIndex = 16;
            // 
            // labelControl5
            // 
            labelControl5.Location = new System.Drawing.Point(110, 336);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new System.Drawing.Size(54, 22);
            labelControl5.TabIndex = 18;
            labelControl5.Text = "狀態：";
            // 
            // labelControl6
            // 
            labelControl6.Location = new System.Drawing.Point(74, 381);
            labelControl6.Name = "labelControl6";
            labelControl6.Size = new System.Drawing.Size(90, 22);
            labelControl6.TabIndex = 19;
            labelControl6.Text = "延遲原因：";
            // 
            // cboRevision
            // 
            cboRevision.Location = new System.Drawing.Point(201, 156);
            cboRevision.Name = "cboRevision";
            cboRevision.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboRevision.Properties.Items.AddRange(new object[] { "v1 - 原始", "v2 - 補測(Command)", "v3 - 補測(Retest)" });
            cboRevision.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboRevision.Size = new System.Drawing.Size(150, 28);
            cboRevision.TabIndex = 20;
            // 
            // cboStatus
            // 
            cboStatus.EditValue = "InProgress";
            cboStatus.Location = new System.Drawing.Point(201, 336);
            cboStatus.Name = "cboStatus";
            cboStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboStatus.Properties.Items.AddRange(new object[] { "InProgress", "Completed", "Delayed" });
            cboStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboStatus.Size = new System.Drawing.Size(150, 28);
            cboStatus.TabIndex = 21;
            cboStatus.EditValueChanged += cboStatus_EditValueChanged;
            // 
            // cboDelayReason
            // 
            cboDelayReason.Enabled = false;
            cboDelayReason.Location = new System.Drawing.Point(201, 381);
            cboDelayReason.Name = "cboDelayReason";
            cboDelayReason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            cboDelayReason.Properties.Items.AddRange(new object[] { "Equipment", "Customer", "Engineer", "Location", "Other" });
            cboDelayReason.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboDelayReason.Size = new System.Drawing.Size(150, 28);
            cboDelayReason.TabIndex = 23;
            // 
            // FrmWorkLogReport
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(603, 756);
            Controls.Add(cboDelayReason);
            Controls.Add(cboStatus);
            Controls.Add(cboRevision);
            Controls.Add(labelControl6);
            Controls.Add(labelControl5);
            Controls.Add(txtEstimateHours);
            Controls.Add(labelControl4);
            Controls.Add(labelControl3);
            Controls.Add(labelControl2);
            Controls.Add(txtPriority);
            Controls.Add(txtRegulation);
            Controls.Add(labelControl1);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(memoComment);
            Controls.Add(lblComment);
            Controls.Add(spinWorkHours);
            Controls.Add(lblWorkHours);
            Controls.Add(dateWorkDate);
            Controls.Add(lblWorkDate);
            Controls.Add(txtTestItemName);
            Controls.Add(lblTestItemName);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmWorkLogReport";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "回報工時";
            Load += FrmWorkLogReport_Load;
            ((System.ComponentModel.ISupportInitialize)txtTestItemName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinWorkHours.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoComment.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtRegulation.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtPriority.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtEstimateHours.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboRevision.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboDelayReason.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTestItemName;
        private DevExpress.XtraEditors.TextEdit txtTestItemName;
        private DevExpress.XtraEditors.LabelControl lblWorkDate;
        private DevExpress.XtraEditors.DateEdit dateWorkDate;
        private DevExpress.XtraEditors.LabelControl lblWorkHours;
        private DevExpress.XtraEditors.SpinEdit spinWorkHours;
        private DevExpress.XtraEditors.LabelControl lblComment;
        private DevExpress.XtraEditors.MemoEdit memoComment;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtRegulation;
        private DevExpress.XtraEditors.TextEdit txtPriority;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtEstimateHours;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.ComboBoxEdit cboRevision;
        private DevExpress.XtraEditors.ComboBoxEdit cboStatus;
        private DevExpress.XtraEditors.ComboBoxEdit cboDelayReason;
    }
}