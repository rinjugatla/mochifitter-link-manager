using System;
using System.Windows.Forms;

namespace mochifitter_link_manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
    }
}
