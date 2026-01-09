namespace mochifitter_link_manager
{
    public partial class Form1 : Form
    {
        /// <summary>アバターフォルダに固有で存在するファイル</summary>
        private readonly string avaterProjectFileName = "VRC.SDK3A.csproj";

        /// <summary>BlenderToolsフォルダの位置</summary>
        private enum BlenderToolsPlace
        {
            /// <summary>アバターフォルダ内に存在する</summary>
            InAvaterDirectory,
            /// <summary>すでにルートフォルダに移動済み</summary>
            InVRCRootDirectory,
        }

        public Form1()
        {
            InitializeComponent();
            // Validate initial state and update when the text changes
            UpdateCreateLinkButtonState();
        }

        /// <summary>
        /// BlenderToolsフォルダ参照ボタンのクリック処理
        /// </summary>
        private void BrowseBlenderToolsDirectory_Button_Click(object? sender, EventArgs e)
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

        /// <summary>
        /// フォルダパス変更時の処理
        /// </summary>
        private void BlenderToolsDirectory_TextBox_TextChanged(object? sender, EventArgs e)
        {
            UpdateCreateLinkButtonState();
        }

        /// <summary>
        /// まとめボタンの状態更新
        /// </summary>
        private void UpdateCreateLinkButtonState()
        {
            CreateLink_Button.Enabled = ValidateBlenderToolsDirectory(BlenderToolsDirectory_TextBox.Text);
        }

        /// <summary>
        /// BlenderToolsフォルダパスを検証
        /// </summary>
        /// <param name="path">BlenderToolsフォルダパス</param>
        private bool ValidateBlenderToolsDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return false; }
                
            try
            {
                var dirInfo = new DirectoryInfo(path);
                var folderName = dirInfo.Name;
                return string.Equals(folderName, "BlenderTools", StringComparison.OrdinalIgnoreCase)
                       && Directory.Exists(path) 
                       && !IsSymbolicLink(path);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// シンボリックリンクか
        /// </summary>
        /// <param name="path">フォルダパス</param>
        private bool IsSymbolicLink(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            return (dirInfo.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }

        /// <summary>
        /// BlenderToolsフォルダをVRCRoot直下に移動し、不要なBlenderToolsフォルダを削除し、シンボリックリンクを作成
        /// </summary>
        private async void CreateLink_Button_Click(object sender, EventArgs e)
        {
            var blenderToolsPath = BlenderToolsDirectory_TextBox.Text;
            bool isValidBlenderToolsDir = ValidateBlenderToolsDirectory(blenderToolsPath);
            if (!isValidBlenderToolsDir)
            {
                MessageBox.Show(this, "BlenderToolsフォルダのパスが無効です。正しいフォルダを指定してください。", "無効なフォルダ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // フォルダ構成
            // VRCRoot/BlenderTools
            // VRCRoot/VRchat Avatar Dir/BlenderTools
            var place = AnalyzeBlenderToolsPlacce(blenderToolsPath);
            var vrcRootDir = place switch
            {
                BlenderToolsPlace.InAvaterDirectory => new DirectoryInfo(blenderToolsPath).Parent?.Parent,
                BlenderToolsPlace.InVRCRootDirectory => new DirectoryInfo(blenderToolsPath).Parent,
            };

            if (vrcRootDir == null || !vrcRootDir.Exists)
            {
                MessageBox.Show(this, "VRCRootフォルダが見つかりません。BlenderToolsフォルダの正しいパスを指定してください。", "無効なフォルダ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var progressDialog = new ProgressDialog())
            {
                progressDialog.Show(this);
                progressDialog.UpdateStatus("準備中...", 0);

                var rootPath = vrcRootDir.FullName;

                int deletedCount = 0;
                int failedDeleteCount = 0;
                int linkCreatedCount = 0;

                var progress = new Progress<(string message, int percent)>(update =>
                {
                    progressDialog.UpdateStatus(update.message, update.percent);
                });

                try
                {
                    await Task.Run(() =>
                    {
                        // 0-10% Move
                        ReportProgress(progress, "BlenderTools を VRCRoot へ移動しています...", 5);
                        string movedBlenderToolsDirPath = place == BlenderToolsPlace.InAvaterDirectory ?
                            MoveBlenderToolsToRootCore(blenderToolsPath, rootPath) :
                            blenderToolsPath;
                        ReportProgress(progress, "移動完了", 10);

                        // 10-70% Delete others
                        (deletedCount, failedDeleteCount) = DeleteOthersBlenderToolsCore(rootPath, progress, 10, 70);

                        // 70-100% Create links
                        linkCreatedCount = CreateSymbolicLinksCore(rootPath, movedBlenderToolsDirPath, progress, 70, 100);
                        ReportProgress(progress, "リンク作成完了", 100);
                    });

                    string summary = $"削除成功: {deletedCount} 件\n失敗: {failedDeleteCount} 件\n作成したリンク: {linkCreatedCount} 件";
                    MessageBox.Show(this, summary, "処理完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "処理中にエラーが発生しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressDialog.Close();
                }
            }
        }

        private static void ReportProgress(IProgress<(string message, int percent)> progress, string message, int percent)
        {
            progress?.Report((message, Math.Max(0, Math.Min(100, percent))));
        }

        /// <summary>
        /// BlenderToolsフォルダの位置を解析
        /// </summary>
        /// <param name="blenderToolsDirPath">BlenderToolsフォルダパス</param>
        private BlenderToolsPlace AnalyzeBlenderToolsPlacce(string blenderToolsDirPath)
        {
            var parent = new DirectoryInfo(blenderToolsDirPath).Parent;
            bool isAvaterDir = parent != null && File.Exists(Path.Join(parent.FullName, avaterProjectFileName));
            return isAvaterDir ? BlenderToolsPlace.InAvaterDirectory : BlenderToolsPlace.InVRCRootDirectory;
        }

        /// <summary>
        /// BlenderToolsフォルダをVRCRoot直下に移動
        /// </summary>
        /// <param name="blenderToolsDirPath">BlenderToolsフォルダ</param>
        /// <param name="vrcRootDirPath">VRChatプロジェクト群のルートフォルダ</param>
        /// <returns>移動後のBlenderToolsフォルダパス</returns>
        /// <exception cref="ArgumentException">パスが無効</exception>
        /// <exception cref="DirectoryNotFoundException">フォルダが存在しない</exception>
        /// <exception cref="IOException">移動先にすでにフォルダが存在する</exception>
        private string MoveBlenderToolsToRootCore(string blenderToolsDirPath, string vrcRootDirPath)
        {
            if (string.IsNullOrWhiteSpace(blenderToolsDirPath) || string.IsNullOrWhiteSpace(vrcRootDirPath))
            {
                throw new ArgumentException("パスが無効です。");
            }

            var sourceDir = new DirectoryInfo(blenderToolsDirPath);
            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException("指定されたBlenderToolsフォルダが見つかりません。");
            }

            var targetPath = Path.Combine(vrcRootDirPath, "BlenderTools");

            var sourceFull = Path.GetFullPath(sourceDir.FullName).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var targetFull = Path.GetFullPath(targetPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (string.Equals(sourceFull, targetFull, StringComparison.OrdinalIgnoreCase))
            {
                return targetFull; // already in place
            }

            if (Directory.Exists(targetFull) || File.Exists(targetFull))
            {
                // ルートフォルダに既にフォルダが存在する場合、移動せずにそのパスを返す
                // 移動元のフォルダは後の削除処理で自動削除される
                return targetFull;
            }

            Directory.Move(sourceFull, targetFull);

            return targetFull;
        }

        /// <summary>
        /// 不要なBlenderToolsフォルダを削除
        /// </summary>
        /// <param name="vrcRootDirPath">VRChatプロジェクト群のルートフォルダ</param>
        /// <exception cref="DirectoryNotFoundException">ルートフォルダが存在しない</exception>
        private (int deletedCount, int failedCount) DeleteOthersBlenderToolsCore(string vrcRootDirPath, IProgress<(string message, int percent)> progress, int startPercent, int endPercent)
        {
            if (string.IsNullOrWhiteSpace(vrcRootDirPath))
            {
                return (0, 0);
            }

            var rootDir = new DirectoryInfo(vrcRootDirPath);
            if (!rootDir.Exists)
            {
                throw new DirectoryNotFoundException("指定されたルートフォルダが存在しません。");
            }

            int deletedCount = 0;
            int failedCount = 0;

            var children = rootDir.GetDirectories();
            int total = Math.Max(1, children.Length);
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                var candidate = Path.Combine(child.FullName, "BlenderTools");

                int percent = startPercent + (int)((i + 1) / (double)total * (endPercent - startPercent));
                ReportProgress(progress, $"不要な 'BlenderTools' を削除中... ({i + 1}/{total})", percent);
                
                bool needDelete = Directory.Exists(candidate) && !IsSymbolicLink(candidate);
                if (!needDelete) { continue; }

                try
                {
                    ClearReadOnlyAttributes(new DirectoryInfo(candidate));
                    Directory.Delete(candidate, true);
                    deletedCount++;
                }
                catch
                {
                    failedCount++;
                }
            }

            return (deletedCount, failedCount);
        }

        /// <summary>
        /// ディレクトリとファイルの属性を再帰的に通常に戻す
        /// </summary>
        /// <remarks>
        /// 削除時に属性が読み取り専用だと失敗する場合がある
        /// </remarks>
        /// <param name="dir"></param>
        private void ClearReadOnlyAttributes(DirectoryInfo dir)
        {
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

        /// <summary>
        /// シンボリックリンクを作成
        /// </summary>
        /// <param name="vrcRootDirPath">VRChatプロジェクト群のルートフォルダ</param>
        /// <param name="blenderToolsDirPath">BlenderToolsの実体フォルダパス</param>
        /// <exception cref="DirectoryNotFoundException">ルートフォルダが存在しない</exception>
        private int CreateSymbolicLinksCore(string vrcRootDirPath, string blenderToolsDirPath, IProgress<(string message, int percent)> progress, int startPercent, int endPercent)
        {
            if (string.IsNullOrWhiteSpace(vrcRootDirPath))
            {
                return 0;
            }

            var rootDir = new DirectoryInfo(vrcRootDirPath);
            if (!rootDir.Exists)
            {
                throw new DirectoryNotFoundException("指定されたルートフォルダが存在しません。");
            }

            var children = rootDir.GetDirectories();
            int total = Math.Max(1, children.Length);
            int created = 0;
            for (int i = 0; i < children.Length; i++)
            {
                var child = children[i];
                bool isAvaterDir = File.Exists(Path.Join(child.FullName, avaterProjectFileName));
                if (isAvaterDir)
                {
                    var linkPath = Path.Combine(child.FullName, "BlenderTools");
                    if (!Directory.Exists(linkPath) && !File.Exists(linkPath))
                    {
                        CreateSymbolicLink(linkPath, blenderToolsDirPath);
                        created++;
                    }
                }

                int percent = startPercent + (int)((i + 1) / (double)total * (endPercent - startPercent));
                ReportProgress(progress, $"シンボリックリンク作成中... ({i + 1}/{total})", percent);
            }

            return created;
        }

        /// <summary>
        /// シンボリックリンクを作成
        /// </summary>
        /// <param name="linkPath">リンク</param>
        /// <param name="targetPath">実体</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void CreateSymbolicLink(string linkPath, string targetPath)
        {
            try
            {
                Directory.CreateSymbolicLink(linkPath, targetPath);
            }
            catch (Exception ex)
            {
                // 既存のエラーハンドリングと同様にラップして上位に伝搬
                throw new InvalidOperationException("シンボリックリンクの作成に失敗しました: " + ex.Message, ex);
            }
        }
    }
}
