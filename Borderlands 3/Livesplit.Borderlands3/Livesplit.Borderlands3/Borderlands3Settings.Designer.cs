namespace Livesplit.Borderlands3
{
    partial class Borderlands3Settings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      this.levelSplitsCheckbox = new System.Windows.Forms.CheckBox();
      this.versionLabel = new System.Windows.Forms.Label();
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.sqCounterCheckbox = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.sqCounterTextBox = new System.Windows.Forms.TextBox();
      this.dividerLabel = new System.Windows.Forms.Label();
      this.sqCounterTextTooltip = new System.Windows.Forms.ToolTip(this.components);
      this.flowLayoutPanel.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // levelSplitsCheckbox
      // 
      this.levelSplitsCheckbox.AutoSize = true;
      this.levelSplitsCheckbox.Location = new System.Drawing.Point(0, 0);
      this.levelSplitsCheckbox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
      this.levelSplitsCheckbox.Name = "levelSplitsCheckbox";
      this.levelSplitsCheckbox.Size = new System.Drawing.Size(144, 17);
      this.levelSplitsCheckbox.TabIndex = 0;
      this.levelSplitsCheckbox.Text = "Split on Level Transitions";
      this.levelSplitsCheckbox.UseVisualStyleBackColor = true;
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(3, 61);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(129, 13);
      this.versionLabel.TabIndex = 1;
      this.versionLabel.Text = "Game Version: Not Found";
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.AutoSize = true;
      this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel.Controls.Add(this.levelSplitsCheckbox);
      this.flowLayoutPanel.Controls.Add(this.tableLayoutPanel1);
      this.flowLayoutPanel.Controls.Add(this.dividerLabel);
      this.flowLayoutPanel.Controls.Add(this.versionLabel);
      this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
      this.flowLayoutPanel.Size = new System.Drawing.Size(183, 77);
      this.flowLayoutPanel.TabIndex = 4;
      // 
      // sqCounterCheckbox
      // 
      this.sqCounterCheckbox.AutoSize = true;
      this.sqCounterCheckbox.Location = new System.Drawing.Point(0, 5);
      this.sqCounterCheckbox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 3);
      this.sqCounterCheckbox.Name = "sqCounterCheckbox";
      this.sqCounterCheckbox.Size = new System.Drawing.Size(77, 17);
      this.sqCounterCheckbox.TabIndex = 6;
      this.sqCounterCheckbox.Text = "Count SQs";
      this.sqCounterCheckbox.UseVisualStyleBackColor = true;
      this.sqCounterCheckbox.CheckedChanged += new System.EventHandler(this.sqCounterCheckbox_CheckedChanged);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.sqCounterTextBox, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.sqCounterCheckbox, 0, 0);
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 20);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(183, 26);
      this.tableLayoutPanel1.TabIndex = 8;
      // 
      // sqCounterTextBox
      // 
      this.sqCounterTextBox.Enabled = false;
      this.sqCounterTextBox.Location = new System.Drawing.Point(80, 3);
      this.sqCounterTextBox.Name = "sqCounterTextBox";
      this.sqCounterTextBox.Size = new System.Drawing.Size(100, 20);
      this.sqCounterTextBox.TabIndex = 1;
      this.sqCounterTextBox.Text = "SQs:";
      this.sqCounterTextTooltip.SetToolTip(this.sqCounterTextBox, "The first Counter Component with this same Counter Text will be selected.");
      // 
      // dividerLabel
      // 
      this.dividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.dividerLabel.Location = new System.Drawing.Point(0, 54);
      this.dividerLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 5);
      this.dividerLabel.Name = "dividerLabel";
      this.dividerLabel.Size = new System.Drawing.Size(183, 2);
      this.dividerLabel.TabIndex = 5;
      // 
      // Borderlands3Settings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.flowLayoutPanel);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "Borderlands3Settings";
      this.Size = new System.Drawing.Size(183, 77);
      this.flowLayoutPanel.ResumeLayout(false);
      this.flowLayoutPanel.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox levelSplitsCheckbox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label dividerLabel;
        private System.Windows.Forms.CheckBox sqCounterCheckbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox sqCounterTextBox;
        private System.Windows.Forms.ToolTip sqCounterTextTooltip;
    }
}
