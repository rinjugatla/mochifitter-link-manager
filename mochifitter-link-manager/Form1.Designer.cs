namespace mochifitter_link_manager
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BrowseVrcRootDirectory_Button = new Button();
            VrcRootDirectory_TextBox = new TextBox();
            VrcRootDirectory_Label = new Label();
            CreateLink_Button = new Button();
            SuspendLayout();
            // 
            // BrowseVrcRootDirectory_Button
            // 
            this.BrowseVrcRootDirectory_Button.Location = new Point(645, 11);
            this.BrowseVrcRootDirectory_Button.Name = "BrowseVrcRootDirectory_Button";
            this.BrowseVrcRootDirectory_Button.Size = new Size(75, 23);
            this.BrowseVrcRootDirectory_Button.TabIndex = 2;
            this.BrowseVrcRootDirectory_Button.Text = "参照";
            this.BrowseVrcRootDirectory_Button.UseVisualStyleBackColor = true;
            // 
            // VrcRootDirectory_TextBox
            // 
            this.VrcRootDirectory_TextBox.Location = new Point(159, 12);
            this.VrcRootDirectory_TextBox.Name = "VrcRootDirectory_TextBox";
            this.VrcRootDirectory_TextBox.Size = new Size(480, 23);
            this.VrcRootDirectory_TextBox.TabIndex = 4;
            // 
            // VrcRootDirectory_Label
            // 
            VrcRootDirectory_Label.AutoSize = true;
            VrcRootDirectory_Label.Location = new Point(12, 15);
            VrcRootDirectory_Label.Name = "VrcRootDirectory_Label";
            VrcRootDirectory_Label.Size = new Size(141, 15);
            VrcRootDirectory_Label.TabIndex = 5;
            VrcRootDirectory_Label.Text = "VRCプロジェクトルートフォルダ";
            // 
            // CreateLink_Button
            // 
            CreateLink_Button.Location = new Point(12, 41);
            CreateLink_Button.Name = "CreateLink_Button";
            CreateLink_Button.Size = new Size(708, 51);
            CreateLink_Button.TabIndex = 2;
            CreateLink_Button.Text = "BlenderToolsをおまとめ";
            CreateLink_Button.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(731, 104);
            Controls.Add(VrcRootDirectory_Label);
            Controls.Add(this.VrcRootDirectory_TextBox);
            Controls.Add(CreateLink_Button);
            Controls.Add(this.BrowseVrcRootDirectory_Button);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "もちふぃった～ BlenderTools まとめツール";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox VrcRootDirectory_TextBox;
        private Button BrowseVrcRootDirectory_Button;
        private Label VrcRootDirectory_Label;
        private Button CreateLink_Button;
    }
}
