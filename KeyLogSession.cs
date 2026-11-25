using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KeyEventLogger
{
    public class KeyLogSession
    {
        private readonly List<KeyLogEntry> _entries = new List<KeyLogEntry>();

        public bool IsRecording { get; private set; }
        public IReadOnlyList<KeyLogEntry> Entries => _entries;
        public int Count => _entries.Count;

        public void Start()
        {
            IsRecording = true;
        }

        public void Stop()
        {
            IsRecording = false;
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public KeyLogEntry? AddEntry(string eventType, string keyName, string modifiers = "")
        {
            if (!IsRecording)
                return null;

            var entry = new KeyLogEntry(eventType, keyName, modifiers);
            _entries.Add(entry);
            return entry;
        }

        public string ExportAsText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== YMH219 - KeyEventLogger Kayıt Dökümü ===");
            sb.AppendLine($"Oluşturulma Zamanı: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Toplam Kayıt: {_entries.Count}");
            sb.AppendLine(new string('-', 50));

            foreach (var entry in _entries)
            {
                sb.AppendLine(entry.ToString());
            }

            return sb.ToString();
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, ExportAsText(), Encoding.UTF8);
        }
    }
}