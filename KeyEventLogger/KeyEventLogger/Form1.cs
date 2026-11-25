using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeyEventLogger
{
    public partial class Form1 : Form
    {
        private readonly KeyLogSession _session = new KeyLogSession();

        private Label _lblStatus = null!;
        private Label _lblInfo = null!;
        private ListBox _lstLogs = null!;
        private Button _btnStart = null!;
        private Button _btnStop = null!;
        private Button _btnClear = null!;
        private Button _btnSave = null!;

        public Form1()
        {
            InitializeComponent();
            BuildUi();
            UpdateStatusUi();
        }

        private void BuildUi()
        {
            Text = "YMH219 - KeyEventLogger (Yalnızca Pencere Odaklı)";
            ClientSize = new Size(640, 440);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(560, 380);
            KeyPreview = true;

            _lblStatus = new Label
            {
                Location = new Point(16, 14),
                Size = new Size(340, 26),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            _lblInfo = new Label
            {
                Location = new Point(16, 42),
                Size = new Size(608, 20),
                ForeColor = Color.DimGray,
                Text = "Not: Yalnızca bu pencere aktifken basılan tuşlar kaydedilir (global hook yoktur)."
            };

            _btnStart = new Button
            {
                Text = "Başlat",
                Location = new Point(16, 72),
                Size = new Size(110, 34),
                TabStop = false
            };
            _btnStart.Click += (_, _) =>
            {
                _session.Start();
                UpdateStatusUi();
                Focus();
            };

            _btnStop = new Button
            {
                Text = "Durdur",
                Location = new Point(134, 72),
                Size = new Size(110, 34),
                TabStop = false
            };
            _btnStop.Click += (_, _) =>
            {
                _session.Stop();
                UpdateStatusUi();
                Focus();
            };

            _btnClear = new Button
            {
                Text = "Temizle",
                Location = new Point(252, 72),
                Size = new Size(110, 34),
                TabStop = false
            };
            _btnClear.Click += (_, _) =>
            {
                _session.Clear();
                _lstLogs.Items.Clear();
                UpdateStatusUi();
                Focus();
            };

            _btnSave = new Button
            {
                Text = "TXT'ye Kaydet",
                Location = new Point(370, 72),
                Size = new Size(130, 34),
                TabStop = false
            };
            _btnSave.Click += (_, _) => SaveLogsToTxt();

            _lstLogs = new ListBox
            {
                Location = new Point(16, 118),
                Size = new Size(608, 304),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Consolas", 9.5f),
                HorizontalScrollbar = true
            };

            Controls.Add(_lblStatus);
            Controls.Add(_lblInfo);
            Controls.Add(_btnStart);
            Controls.Add(_btnStop);
            Controls.Add(_btnClear);
            Controls.Add(_btnSave);
            Controls.Add(_lstLogs);

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!_session.IsRecording)
                return;

            string mods = BuildModifiersText(e);
            var entry = _session.AddEntry("KeyDown", e.KeyCode.ToString(), mods);
            if (entry != null)
            {
                AppendEntry(entry);
            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (!_session.IsRecording)
                return;

            string mods = BuildModifiersText(e);
            var entry = _session.AddEntry("KeyUp", e.KeyCode.ToString(), mods);
            if (entry != null)
            {
                AppendEntry(entry);
            }
        }

        private static string BuildModifiersText(KeyEventArgs e)
        {
            System.Collections.Generic.List<string> parts = new System.Collections.Generic.List<string>();
            if (e.Control) parts.Add("Ctrl");
            if (e.Alt) parts.Add("Alt");
            if (e.Shift) parts.Add("Shift");
            return string.Join("+", parts);
        }

        private void AppendEntry(KeyLogEntry entry)
        {
            _lstLogs.Items.Add(entry.ToString());
            _lstLogs.TopIndex = Math.Max(0, _lstLogs.Items.Count - 1);
            UpdateStatusUi();
        }

        private void SaveLogsToTxt()
        {
            if (_session.Count == 0)
            {
                MessageBox.Show("Kaydedilecek tuş kaydı bulunmuyor.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Title = "Tuş Kayıtlarını Kaydet",
                Filter = "Metin Dosyası (*.txt)|*.txt",
                FileName = $"KeyLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                DefaultExt = "txt"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _session.SaveToFile(dialog.FileName);
                    MessageBox.Show("Kayıtlar başarıyla .txt dosyasına aktarıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya kaydedilirken hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateStatusUi()
        {
            if (_session.IsRecording)
            {
                _lblStatus.Text = $"Kayıt Durumu: Aktif ({_session.Count} kayıt)";
                _lblStatus.ForeColor = Color.ForestGreen;
            }
            else
            {
                _lblStatus.Text = $"Kayıt Durumu: Durduruldu ({_session.Count} kayıt)";
                _lblStatus.ForeColor = Color.Firebrick;
            }

            _btnStart.Enabled = !_session.IsRecording;
            _btnStop.Enabled = _session.IsRecording;
            _btnSave.Enabled = _session.Count > 0;
        }
    }
}