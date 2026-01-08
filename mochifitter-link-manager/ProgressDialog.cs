using System;
using System.Windows.Forms;

namespace mochifitter_link_manager
{
    public class ProgressDialog : Form
    {
        private readonly Label _label;
        private readonly ProgressBar _progressBar;

        public ProgressDialog()
        {
            Text = "èàóùíÜ";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Width = 420;
            Height = 140;
            ControlBox = false;
            ShowInTaskbar = false;

            _label = new Label
            {
                Left = 12,
                Top = 12,
                Width = 380,
                Height = 40,
                AutoEllipsis = true,
                Text = "èÄîıíÜ..."
            };

            _progressBar = new ProgressBar
            {
                Left = 12,
                Top = 60,
                Width = 380,
                Height = 20,
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };

            Controls.Add(_label);
            Controls.Add(_progressBar);
        }

        public void UpdateStatus(string message, int percent)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string, int>(UpdateStatus), message, percent);
                return;
            }

            _label.Text = message;
            _progressBar.Value = Math.Max(0, Math.Min(100, percent));
        }
    }
}
