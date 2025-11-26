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
            ((System.ComponentModel.ISupportInitialize)txtTestItemName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)spinWorkHours.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)memoComment.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblTestItemName
            // 
            lblTestItemName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblTestItemName.Location = new System.Drawing.Point(20, 20);
            lblTestItemName.Name = "lblTestItemName";
            lblTestItemName.Size = new System.Drawing.Size(90, 22);
            lblTestItemName.TabIndex = 0;
            lblTestItemName.Text = "測項名稱：";
            lblTestItemName.Click += lblTestItemName_Click;
            // 
            // txtTestItemName
            // 
            txtTestItemName.EditValue = "Conducted Power";
            txtTestItemName.Location = new System.Drawing.Point(125, 17);
            txtTestItemName.Name = "txtTestItemName";
            txtTestItemName.Properties.ContextImageOptions.SvgImageSize = new System.Drawing.Size(250, 0);
            txtTestItemName.Properties.ReadOnly = true;
            txtTestItemName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            txtTestItemName.Size = new System.Drawing.Size(150, 28);
            txtTestItemName.TabIndex = 1;
            // 
            // lblWorkDate
            // 
            lblWorkDate.Location = new System.Drawing.Point(56, 64);
            lblWorkDate.Name = "lblWorkDate";
            lblWorkDate.Size = new System.Drawing.Size(54, 22);
            lblWorkDate.TabIndex = 2;
            lblWorkDate.Text = "日期：";
            // 
            // dateWorkDate
            // 
            dateWorkDate.EditValue = null;
            dateWorkDate.Location = new System.Drawing.Point(125, 58);
            dateWorkDate.Name = "dateWorkDate";
            dateWorkDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dateWorkDate.Size = new System.Drawing.Size(150, 28);
            dateWorkDate.TabIndex = 3;
            // 
            // lblWorkHours
            // 
            lblWorkHours.Location = new System.Drawing.Point(20, 111);
            lblWorkHours.Name = "lblWorkHours";
            lblWorkHours.Size = new System.Drawing.Size(90, 22);
            lblWorkHours.TabIndex = 4;
            lblWorkHours.Text = "工時 (hr)：";
            // 
            // spinWorkHours
            // 
            spinWorkHours.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            spinWorkHours.Location = new System.Drawing.Point(125, 108);
            spinWorkHours.Name = "spinWorkHours";
            spinWorkHours.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            spinWorkHours.Properties.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            spinWorkHours.Properties.MaxValue = new decimal(new int[] { 12, 0, 0, 0 });
            spinWorkHours.Size = new System.Drawing.Size(150, 28);
            spinWorkHours.TabIndex = 5;
            // 
            // lblComment
            // 
            lblComment.Location = new System.Drawing.Point(56, 157);
            lblComment.Name = "lblComment";
            lblComment.Size = new System.Drawing.Size(54, 22);
            lblComment.TabIndex = 6;
            lblComment.Text = "備註：";
            // 
            // memoComment
            // 
            memoComment.Location = new System.Drawing.Point(70, 206);
            memoComment.Name = "memoComment";
            memoComment.Size = new System.Drawing.Size(428, 100);
            memoComment.TabIndex = 7;
            // 
            // btnOk
            // 
            btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOk.Location = new System.Drawing.Point(125, 540);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(120, 41);
            btnOk.TabIndex = 8;
            btnOk.Text = "送出";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(306, 540);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(120, 41);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "取消";
            // 
            // FrmWorkLogReport
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(578, 633);
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
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "回報工時";
            ((System.ComponentModel.ISupportInitialize)txtTestItemName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dateWorkDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)spinWorkHours.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)memoComment.Properties).EndInit();
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
    }
}