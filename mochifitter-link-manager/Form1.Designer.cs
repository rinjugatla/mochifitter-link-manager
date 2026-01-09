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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                processingTimer?.Dispose();
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
            ProgressStatus_Label = new Label();
            Progress_ProgressBar = new ProgressBar();
            ProcessingTime_Label = new Label();
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
            BrowseBlenderToolsDirectory_Button.Click += BrowseBlenderToolsDirectory_Button_Click;
            // 
            // BlenderToolsDirectory_TextBox
            // 
            BlenderToolsDirectory_TextBox.Location = new Point(145, 12);
            BlenderToolsDirectory_TextBox.Name = "BlenderToolsDirectory_TextBox";
            BlenderToolsDirectory_TextBox.Size = new Size(480, 23);
            BlenderToolsDirectory_TextBox.TabIndex = 4;
            BlenderToolsDirectory_TextBox.TextChanged += BlenderToolsDirectory_TextBox_TextChanged;
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
            // ProgressStatus_Label
            // 
            ProgressStatus_Label.AutoSize = true;
            ProgressStatus_Label.Location = new Point(12, 105);
            ProgressStatus_Label.Name = "ProgressStatus_Label";
            ProgressStatus_Label.Size = new Size(0, 15);
            ProgressStatus_Label.TabIndex = 6;
            // 
            // Progress_ProgressBar
            // 
            Progress_ProgressBar.Location = new Point(12, 130);
            Progress_ProgressBar.Name = "Progress_ProgressBar";
            Progress_ProgressBar.Size = new Size(708, 23);
            Progress_ProgressBar.TabIndex = 7;
            Progress_ProgressBar.Visible = false;
            // 
            // ProcessingTime_Label
            // 
            ProcessingTime_Label.AutoSize = true;
            ProcessingTime_Label.Location = new Point(12, 160);
            ProcessingTime_Label.Name = "ProcessingTime_Label";
            ProcessingTime_Label.Size = new Size(0, 15);
            ProcessingTime_Label.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(731, 185);
            Controls.Add(ProcessingTime_Label);
            Controls.Add(Progress_ProgressBar);
            Controls.Add(ProgressStatus_Label);
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
        private Label ProgressStatus_Label;
        private ProgressBar Progress_ProgressBar;
        private Label ProcessingTime_Label;
    }
}
