using System;
using System.IO;
using System.Windows.Forms;

namespace mochifitter_link_manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Validate initial state and update when the text changes
            BlenderToolsDirectory_TextBox.TextChanged += BlenderToolsDirectory_TextBox_TextChanged;
            UpdateCreateLinkButtonState();
        }

        private void BrowseVrcRootDirectory_Button_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.Multiselect = false;
                    var result = dialog.ShowDialog();
                    if (result == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                    {
                        var selected = dialog.FileName;
                        if (string.Equals(System.IO.Path.GetFileName(selected), "BlenderTools", StringComparison.OrdinalIgnoreCase))
                        {
                            BlenderToolsDirectory_TextBox.Text = selected;
                            // Ensure CreateLink button state is updated immediately after setting the text
                            UpdateCreateLinkButtonState();
                        }
                        else
                        {
                            MessageBox.Show(this, "フォルダ名は 'BlenderTools' を選択してください。", "無効なフォルダ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "フォルダ選択ダイアログを開けませんでした: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BlenderToolsDirectory_TextBox_TextChanged(object? sender, EventArgs e)
        {
            UpdateCreateLinkButtonState();
        }

        private void UpdateCreateLinkButtonState()
        {
            var path = BlenderToolsDirectory_TextBox.Text;
            bool enabled = false;

            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    // Use DirectoryInfo to get the last segment reliably (handles trailing separators)
                    var dirInfo = new DirectoryInfo(path);
                    var folderName = dirInfo.Name;

                    if (string.Equals(folderName, "BlenderTools", StringComparison.OrdinalIgnoreCase)
                        && Directory.Exists(path))
                    {
                        enabled = true;
                    }
                }
                catch
                {
                    // Any exceptions (e.g. invalid path format) result in disabled state
                    enabled = false;
                }
            }

            CreateLink_Button.Enabled = enabled;
        }
    }
}
