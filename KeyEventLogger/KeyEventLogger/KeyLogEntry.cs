using System;

namespace KeyEventLogger
{
    public class KeyLogEntry
    {
        public DateTime Timestamp { get; }
        public string EventType { get; }
        public string KeyName { get; }
        public string Modifiers { get; }

        public KeyLogEntry(string eventType, string keyName, string modifiers)
        {
            Timestamp = DateTime.Now;
            EventType = eventType;
            KeyName = keyName;
            Modifiers = modifiers;
        }

        public override string ToString()
        {
            string modPart = string.IsNullOrWhiteSpace(Modifiers) ? string.Empty : $" [{Modifiers}]";
            return $"{Timestamp:HH:mm:ss.fff} | {EventType,-8} | Tuş: {KeyName}{modPart}";
        }
    }
}