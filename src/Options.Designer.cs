using RookieUI;
namespace KPColorChange
{
	partial class Options
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.cgExpiring = new RookieUI.CheckedGroupBox();
			this.cbUseDateOnly = new System.Windows.Forms.CheckBox();
			this.cbExpiringColors = new System.Windows.Forms.ComboBox();
			this.warningDays = new System.Windows.Forms.TextBox();
			this.lSoonExpiringThreshold = new System.Windows.Forms.Label();
			this.lSoonExpiringColor = new System.Windows.Forms.Label();
			this.bExpiring = new System.Windows.Forms.Button();
			this.lSoonExpiring = new System.Windows.Forms.Label();
			this.cgExpired = new RookieUI.CheckedGroupBox();
			this.hkcToggle = new KeePass.UI.HotKeyControlEx();
			this.cbHideExpired = new System.Windows.Forms.CheckBox();
			this.cbExpiredColors = new System.Windows.Forms.ComboBox();
			this.lExpiredColor = new System.Windows.Forms.Label();
			this.bExpired = new System.Windows.Forms.Button();
			this.lExpired = new System.Windows.Forms.Label();
			this.cgExpiring.SuspendLayout();
			this.cgExpired.SuspendLayout();
			this.SuspendLayout();
			// 
			// cgExpiring
			// 
			this.cgExpiring.CheckboxOffset = new System.Drawing.Point(6, 0);
			this.cgExpiring.Checked = true;
			this.cgExpiring.Controls.Add(this.cbUseDateOnly);
			this.cgExpiring.Controls.Add(this.cbExpiringColors);
			this.cgExpiring.Controls.Add(this.warningDays);
			this.cgExpiring.Controls.Add(this.lSoonExpiringThreshold);
			this.cgExpiring.Controls.Add(this.lSoonExpiringColor);
			this.cgExpiring.Controls.Add(this.bExpiring);
			this.cgExpiring.Controls.Add(this.lSoonExpiring);
			this.cgExpiring.Dock = System.Windows.Forms.DockStyle.Top;
			this.cgExpiring.Location = new System.Drawing.Point(10, 141);
			this.cgExpiring.Name = "cgExpiring";
			this.cgExpiring.Size = new System.Drawing.Size(612, 152);
			this.cgExpiring.TabIndex = 20;
			this.cgExpiring.Text = "Expiring soon";
			// 
			// cbUseDateOnly
			// 
			this.cbUseDateOnly.AutoSize = true;
			this.cbUseDateOnly.Location = new System.Drawing.Point(15, 122);
			this.cbUseDateOnly.Name = "cbUseDateOnly";
			this.cbUseDateOnly.Size = new System.Drawing.Size(270, 24);
			this.cbUseDateOnly.TabIndex = 40;
			this.cbUseDateOnly.Text = "Ignore time fraction of expiry date";
			this.cbUseDateOnly.UseVisualStyleBackColor = true;
			// 
			// cbExpiringColors
			// 
			this.cbExpiringColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbExpiringColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cbExpiringColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbExpiringColors.FormattingEnabled = true;
			this.cbExpiringColors.Location = new System.Drawing.Point(474, 58);
			this.cbExpiringColors.Name = "cbExpiringColors";
			this.cbExpiringColors.Size = new System.Drawing.Size(120, 27);
			this.cbExpiringColors.TabIndex = 20;
			this.cbExpiringColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.drawItem);
			// 
			// warningDays
			// 
			this.warningDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.warningDays.Location = new System.Drawing.Point(545, 92);
			this.warningDays.MaxLength = 3;
			this.warningDays.Name = "warningDays";
			this.warningDays.Size = new System.Drawing.Size(50, 26);
			this.warningDays.TabIndex = 30;
			this.warningDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.warningDays.Validating += new System.ComponentModel.CancelEventHandler(this.OptionsValidating);
			// 
			// lSoonExpiringThreshold
			// 
			this.lSoonExpiringThreshold.Location = new System.Drawing.Point(15, 94);
			this.lSoonExpiringThreshold.Name = "lSoonExpiringThreshold";
			this.lSoonExpiringThreshold.Size = new System.Drawing.Size(160, 25);
			this.lSoonExpiringThreshold.TabIndex = 40;
			this.lSoonExpiringThreshold.Text = "Warning (days):";
			// 
			// lSoonExpiringColor
			// 
			this.lSoonExpiringColor.AutoSize = true;
			this.lSoonExpiringColor.Location = new System.Drawing.Point(15, 62);
			this.lSoonExpiringColor.Name = "lSoonExpiringColor";
			this.lSoonExpiringColor.Size = new System.Drawing.Size(50, 20);
			this.lSoonExpiringColor.TabIndex = 20;
			this.lSoonExpiringColor.Text = "Color:";
			// 
			// bExpiring
			// 
			this.bExpiring.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bExpiring.Location = new System.Drawing.Point(560, 22);
			this.bExpiring.Margin = new System.Windows.Forms.Padding(0);
			this.bExpiring.Name = "bExpiring";
			this.bExpiring.Size = new System.Drawing.Size(34, 35);
			this.bExpiring.TabIndex = 10;
			this.bExpiring.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.bExpiring.UseVisualStyleBackColor = true;
			this.bExpiring.Click += new System.EventHandler(this.IconButtonClick);
			// 
			// lSoonExpiring
			// 
			this.lSoonExpiring.Location = new System.Drawing.Point(15, 28);
			this.lSoonExpiring.Name = "lSoonExpiring";
			this.lSoonExpiring.Size = new System.Drawing.Size(50, 25);
			this.lSoonExpiring.TabIndex = 0;
			this.lSoonExpiring.Text = "Icon:";
			// 
			// cgExpired
			// 
			this.cgExpired.CheckboxOffset = new System.Drawing.Point(6, 0);
			this.cgExpired.Checked = true;
			this.cgExpired.Controls.Add(this.hkcToggle);
			this.cgExpired.Controls.Add(this.cbHideExpired);
			this.cgExpired.Controls.Add(this.cbExpiredColors);
			this.cgExpired.Controls.Add(this.lExpiredColor);
			this.cgExpired.Controls.Add(this.bExpired);
			this.cgExpired.Controls.Add(this.lExpired);
			this.cgExpired.Dock = System.Windows.Forms.DockStyle.Top;
			this.cgExpired.Location = new System.Drawing.Point(10, 9);
			this.cgExpired.Name = "cgExpired";
			this.cgExpired.Size = new System.Drawing.Size(612, 132);
			this.cgExpired.TabIndex = 10;
			this.cgExpired.Text = "Expired";
			// 
			// hkcToggle
			// 
			this.hkcToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.hkcToggle.Location = new System.Drawing.Point(444, 94);
			this.hkcToggle.Name = "hkcToggle";
			this.hkcToggle.Size = new System.Drawing.Size(150, 26);
			this.hkcToggle.TabIndex = 40;
			// 
			// cbHideExpired
			// 
			this.cbHideExpired.AutoSize = true;
			this.cbHideExpired.Location = new System.Drawing.Point(15, 94);
			this.cbHideExpired.Name = "cbHideExpired";
			this.cbHideExpired.Size = new System.Drawing.Size(267, 24);
			this.cbHideExpired.TabIndex = 30;
			this.cbHideExpired.Text = "Hide expired entries, toggle with: ";
			this.cbHideExpired.UseVisualStyleBackColor = true;
			// 
			// cbExpiredColors
			// 
			this.cbExpiredColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbExpiredColors.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cbExpiredColors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbExpiredColors.FormattingEnabled = true;
			this.cbExpiredColors.Location = new System.Drawing.Point(474, 58);
			this.cbExpiredColors.Name = "cbExpiredColors";
			this.cbExpiredColors.Size = new System.Drawing.Size(120, 27);
			this.cbExpiredColors.TabIndex = 20;
			this.cbExpiredColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.drawItem);
			// 
			// lExpiredColor
			// 
			this.lExpiredColor.AutoSize = true;
			this.lExpiredColor.Location = new System.Drawing.Point(15, 62);
			this.lExpiredColor.Name = "lExpiredColor";
			this.lExpiredColor.Size = new System.Drawing.Size(50, 20);
			this.lExpiredColor.TabIndex = 20;
			this.lExpiredColor.Text = "Color:";
			// 
			// bExpired
			// 
			this.bExpired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bExpired.Location = new System.Drawing.Point(560, 22);
			this.bExpired.Margin = new System.Windows.Forms.Padding(0);
			this.bExpired.Name = "bExpired";
			this.bExpired.Size = new System.Drawing.Size(34, 35);
			this.bExpired.TabIndex = 10;
			this.bExpired.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.bExpired.UseVisualStyleBackColor = true;
			this.bExpired.Click += new System.EventHandler(this.IconButtonClick);
			// 
			// lExpired
			// 
			this.lExpired.Location = new System.Drawing.Point(15, 28);
			this.lExpired.Name = "lExpired";
			this.lExpired.Size = new System.Drawing.Size(50, 25);
			this.lExpired.TabIndex = 0;
			this.lExpired.Text = "Icon:";
			// 
			// Options
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.cgExpiring);
			this.Controls.Add(this.cgExpired);
			this.Name = "Options";
			this.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
			this.Size = new System.Drawing.Size(632, 422);
			this.Validating += new System.ComponentModel.CancelEventHandler(this.OptionsValidating);
			this.cgExpiring.ResumeLayout(false);
			this.cgExpiring.PerformLayout();
			this.cgExpired.ResumeLayout(false);
			this.cgExpired.PerformLayout();
			this.ResumeLayout(false);

		}

		//private System.Windows.Forms.GroupBox gExpiring;
		internal CheckedGroupBox cgExpired;
		private System.Windows.Forms.Label lExpiredColor;
		private System.Windows.Forms.Button bExpired;
		private System.Windows.Forms.Label lExpired;
		private System.Windows.Forms.ComboBox cbExpiredColors;
		internal CheckedGroupBox cgExpiring;
		private System.Windows.Forms.ComboBox cbExpiringColors;
		private System.Windows.Forms.TextBox warningDays;
		private System.Windows.Forms.Label lSoonExpiringThreshold;
		private System.Windows.Forms.Label lSoonExpiringColor;
		private System.Windows.Forms.Button bExpiring;
		private System.Windows.Forms.Label lSoonExpiring;
		private System.Windows.Forms.CheckBox cbUseDateOnly;
		internal KeePass.UI.HotKeyControlEx hkcToggle;
		private System.Windows.Forms.CheckBox cbHideExpired;
	}
}
