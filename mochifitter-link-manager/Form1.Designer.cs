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
            BrowseBlenderToolsDirectory_Button = new Button();
            BlenderToolsDirectory_TextBox = new TextBox();
            BlenderToolsDirectory_Label = new Label();
            CreateLink_Button = new Button();
            SuspendLayout();
            // 
            // BrowseBlenderToolsDirectory_Button
            // 
            BrowseBlenderToolsDirectory_Button.Location = new Point(645, 11);
            BrowseBlenderToolsDirectory_Button.Name = "BrowseBlenderToolsDirectory_Button";
            BrowseBlenderToolsDirectory_Button.Size = new Size(75, 23);
            BrowseBlenderToolsDirectory_Button.TabIndex = 2;
            BrowseBlenderToolsDirectory_Button.Text = "参照";
            BrowseBlenderToolsDirectory_Button.UseVisualStyleBackColor = true;
            BrowseBlenderToolsDirectory_Button.Click += BrowseVrcRootDirectory_Button_Click;
            // 
            // BlenderToolsDirectory_TextBox
            // 
            BlenderToolsDirectory_TextBox.Location = new Point(145, 12);
            BlenderToolsDirectory_TextBox.Name = "BlenderToolsDirectory_TextBox";
            BlenderToolsDirectory_TextBox.Size = new Size(480, 23);
            BlenderToolsDirectory_TextBox.TabIndex = 4;
            // 
            // BlenderToolsDirectory_Label
            // 
            BlenderToolsDirectory_Label.AutoSize = true;
            BlenderToolsDirectory_Label.Location = new Point(12, 15);
            BlenderToolsDirectory_Label.Name = "BlenderToolsDirectory_Label";
            BlenderToolsDirectory_Label.Size = new Size(127, 15);
            BlenderToolsDirectory_Label.TabIndex = 5;
            BlenderToolsDirectory_Label.Text = "BlenderToolsフォルダパス";
            // 
            // CreateLink_Button
            // 
            CreateLink_Button.Location = new Point(12, 41);
            CreateLink_Button.Name = "CreateLink_Button";
            CreateLink_Button.Size = new Size(708, 51);
            CreateLink_Button.TabIndex = 2;
            CreateLink_Button.Text = "BlenderToolsをおまとめ";
            CreateLink_Button.UseVisualStyleBackColor = true;
            CreateLink_Button.Click += CreateLink_Button_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(731, 104);
            Controls.Add(BlenderToolsDirectory_Label);
            Controls.Add(BlenderToolsDirectory_TextBox);
            Controls.Add(CreateLink_Button);
            Controls.Add(BrowseBlenderToolsDirectory_Button);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "もちふぃった～ BlenderTools まとめツール";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox BlenderToolsDirectory_TextBox;
        private Button BrowseBlenderToolsDirectory_Button;
        private Label BlenderToolsDirectory_Label;
        private Button CreateLink_Button;
    }
}
