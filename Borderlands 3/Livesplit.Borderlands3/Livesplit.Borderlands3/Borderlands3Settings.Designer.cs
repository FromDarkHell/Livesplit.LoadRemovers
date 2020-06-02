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
      this.levelSplitsCheckbox = new System.Windows.Forms.CheckBox();
      this.versionLabel = new System.Windows.Forms.Label();
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.dividerLabel = new System.Windows.Forms.Label();
      this.flowLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // levelSplitsCheckbox
      // 
      this.levelSplitsCheckbox.AutoSize = true;
      this.levelSplitsCheckbox.Location = new System.Drawing.Point(0, 0);
      this.levelSplitsCheckbox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
      this.levelSplitsCheckbox.Name = "levelSplitsCheckbox";
      this.levelSplitsCheckbox.Size = new System.Drawing.Size(144, 17);
      this.levelSplitsCheckbox.TabIndex = 0;
      this.levelSplitsCheckbox.Text = "Split on Level Transitions";
      this.levelSplitsCheckbox.UseVisualStyleBackColor = true;
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(3, 32);
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
      this.flowLayoutPanel.Controls.Add(this.dividerLabel);
      this.flowLayoutPanel.Controls.Add(this.versionLabel);
      this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
      this.flowLayoutPanel.Size = new System.Drawing.Size(144, 48);
      this.flowLayoutPanel.TabIndex = 4;
      // 
      // dividerLabel
      // 
      this.dividerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dividerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.dividerLabel.Location = new System.Drawing.Point(0, 25);
      this.dividerLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
      this.dividerLabel.Name = "dividerLabel";
      this.dividerLabel.Size = new System.Drawing.Size(144, 2);
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
      this.Size = new System.Drawing.Size(144, 48);
      this.flowLayoutPanel.ResumeLayout(false);
      this.flowLayoutPanel.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox levelSplitsCheckbox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label dividerLabel;
    }
}
