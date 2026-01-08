using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.DirectoryServices.ActiveDirectory;

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
            DeleteOthersBlenderTools(vrcRootDir.FullName);
            CreateSymbolicLinks(vrcRootDir.FullName, BlenderToolsDirectory_TextBox.Text);
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
            if (string.IsNullOrWhiteSpace(vrcRootDirPath))
            {
                return;
            }

            try
            {
                var rootDir = new DirectoryInfo(vrcRootDirPath);
                if (!rootDir.Exists)
                {
                    MessageBox.Show(this, "指定されたルートフォルダが存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int deletedCount = 0;
                int failedCount = 0;

                // 探索は1階層分のみ（rootDir の直下のフォルダ）
                foreach (var child in rootDir.GetDirectories())
                {
                    var candidate = Path.Combine(child.FullName, "BlenderTools");
                    if (Directory.Exists(candidate))
                    {
                        try
                        {
                            // 属性が読み取り専用などで削除できない場合があるため属性をクリアしてから削除する
                            ClearReadOnlyAttributes(new DirectoryInfo(candidate));
                            Directory.Delete(candidate, true);
                            deletedCount++;
                        }
                        catch (Exception)
                        {
                            failedCount++;
                        }
                    }
                }

                if (deletedCount == 0 && failedCount == 0)
                {
                    // なにも見つからなかった
                    MessageBox.Show(this, "子フォルダ内に 'BlenderTools' は見つかりませんでした。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var msg = $"削除成功: {deletedCount} 件\n失敗: {failedCount} 件";
                    MessageBox.Show(this, msg, "処理完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "処理中にエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ClearReadOnlyAttributes(DirectoryInfo dir)
        {
            // ディレクトリとファイルの属性を再帰的に通常に戻す
            try
            {
                foreach (var subDir in dir.GetDirectories("*", SearchOption.AllDirectories))
                {
                    subDir.Attributes = FileAttributes.Normal;
                }

                foreach (var file in dir.GetFiles("*", SearchOption.AllDirectories))
                {
                    file.Attributes = FileAttributes.Normal;
                }

                dir.Attributes = FileAttributes.Normal;
            }
            catch
            {
                // 属性変更に失敗しても削除処理に任せる（上位で捕捉される）
            }
        }

        private void CreateSymbolicLinks(string vrcRootDirPath, string blenderToolsDirPath)
        {
            if (string.IsNullOrWhiteSpace(vrcRootDirPath))
            {
                return;
            }

            try
            {
                var rootDir = new DirectoryInfo(vrcRootDirPath);
                if (!rootDir.Exists)
                {
                    MessageBox.Show(this, "指定されたルートフォルダが存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 探索は1階層分のみ（rootDir の直下のフォルダ）
                foreach (var child in rootDir.GetDirectories())
                {
                    bool isAvaterDir = File.Exists(Path.Join(child.FullName, "VRC.SDK3A.csproj"));
                    if (isAvaterDir) { continue; }

                    var linkPath = Path.Combine(child.FullName, "BlenderTools");
                    if ( Directory.Exists(linkPath) || File.Exists(linkPath)) { continue; }
                    CreateSymbolicLink(blenderToolsDirPath, linkPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "シンボリックリンク作成中にエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSymbolicLink(string from, string to)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c mklink /D \"{from}\" \"{to}\"",
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
