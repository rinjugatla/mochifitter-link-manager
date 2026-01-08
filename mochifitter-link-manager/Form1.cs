using System;
using System.IO;
using System.Diagnostics;
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
            CreateLink_Button.Enabled = ValidateBlenderToolsDirectory(BlenderToolsDirectory_TextBox.Text);
        }

        private bool ValidateBlenderToolsDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return false; }
                
            try
            {
                var dirInfo = new DirectoryInfo(path);
                var folderName = dirInfo.Name;
                return string.Equals(folderName, "BlenderTools", StringComparison.OrdinalIgnoreCase)
                       && Directory.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        private void CreateLink_Button_Click(object sender, EventArgs e)
        {
            bool isValidBlenderToolsDir = ValidateBlenderToolsDirectory(BlenderToolsDirectory_TextBox.Text);
            if (!isValidBlenderToolsDir)
            {
                MessageBox.Show(this, "BlenderToolsフォルダのパスが無効です。正しいフォルダを指定してください。", "無効なフォルダ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // フォルダ構成
            // VRCRoot/VRchat Avatar Dir/BlenderTools
            var vrcRootDir = new DirectoryInfo(BlenderToolsDirectory_TextBox.Text)?.Parent?.Parent;
            if (vrcRootDir == null || !vrcRootDir.Exists)
            {
                MessageBox.Show(this, "VRCRootフォルダが見つかりません。BlenderToolsフォルダの正しいパスを指定してください。", "無効なフォルダ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MoveBlenderToolsToRoot(BlenderToolsDirectory_TextBox.Text, vrcRootDir.FullName);
        }

        private void MoveBlenderToolsToRoot(string blenderToolsDirPath, string vrcRootDirPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(blenderToolsDirPath) || string.IsNullOrWhiteSpace(vrcRootDirPath))
                {
                    MessageBox.Show(this, "パスが無効です。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var sourceDir = new DirectoryInfo(blenderToolsDirPath);
                if (!sourceDir.Exists)
                {
                    MessageBox.Show(this, "指定されたBlenderToolsフォルダが見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var targetPath = Path.Combine(vrcRootDirPath, "BlenderTools");

                var sourceFull = Path.GetFullPath(sourceDir.FullName).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var targetFull = Path.GetFullPath(targetPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (string.Equals(sourceFull, targetFull, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this, "BlenderToolsは既にVRCRootにあります。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Directory.Exists(targetFull) || File.Exists(targetFull))
                {
                    MessageBox.Show(this, "VRCRootに既にBlenderToolsが存在します。操作を中止します。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Move the directory to VRCRoot
                Directory.Move(sourceFull, targetFull);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "移動中にエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteOthersBlenderTools(string vrcRootDirPath)
        {

        }

        private void CreateSymbolicLink(string targetPath, string linkPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink /D \"{linkPath}\" \"{targetPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas" // 管理者権限で実行
            };

            if (processInfo == null)
            {
                throw new InvalidOperationException("プロセス情報の作成に失敗しました。");
            }

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException("シンボリックリンクの作成に失敗しました: " + error);
                }
            }
        }
    }
}
