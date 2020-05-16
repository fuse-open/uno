using System;
using System.Collections;
using System.Collections.Generic;
using Uno.UX.Markup.Common;

namespace Uno.UX.Markup.Tests.Helpers
{
    public class ListMarkupErrorLogSource : IEquatable<ListMarkupErrorLogSource>
    {
        public static ListMarkupErrorLogSource Unknown => new ListMarkupErrorLogSource("(unknown)", 0);

        public string Path { get; }
        public int Line { get; }

        public ListMarkupErrorLogSource(string path, int line)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Path = path;
            Line = line;
        }

        public override string ToString()
        {
            return Path + ": " + Line;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ListMarkupErrorLogSource);
        }

        public bool Equals(ListMarkupErrorLogSource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Path.Equals(other.Path) && Line.Equals(other.Line);
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ Line.GetHashCode();
        }
    }

    public class ListMarkupErrorLogEntry : IEquatable<ListMarkupErrorLogEntry>
    {
        public string Message { get; }
        public ListMarkupErrorLogSource Source { get; }

        public ListMarkupErrorLogEntry(string message, ListMarkupErrorLogSource source)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (source == null) throw new ArgumentNullException(nameof(source));

            Message = message;
            Source = source;
        }

        public override string ToString()
        {
            return Message + (Source != null ? " (at " + Source + ")" : "");
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ListMarkupErrorLogEntry);
        }

        public bool Equals(ListMarkupErrorLogEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Message.Equals(other.Message) && Source.Equals(other.Source);
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode() ^ Source.GetHashCode();
        }
    }

    public class ListMarkupErrorLog : IMarkupErrorLog
    {
        private readonly List<ListMarkupErrorLogEntry> _errors = new List<ListMarkupErrorLogEntry>();
        public IEnumerable Errors => _errors;

        private readonly List<ListMarkupErrorLogEntry> _warnings = new List<ListMarkupErrorLogEntry>();
        public IEnumerable Warnings => _warnings;

        public void ReportError(string message)
        {
            _errors.Add(new ListMarkupErrorLogEntry(message, ListMarkupErrorLogSource.Unknown));
        }

        public void ReportWarning(string message)
        {
            _warnings.Add(new ListMarkupErrorLogEntry(message, ListMarkupErrorLogSource.Unknown));
        }

        public void ReportError(string path, int line, string message)
        {
            _errors.Add(new ListMarkupErrorLogEntry(message, new ListMarkupErrorLogSource(path, line)));
        }

        public void ReportWarning(string path, int line, string message)
        {
            _warnings.Add(new ListMarkupErrorLogEntry(message, new ListMarkupErrorLogSource(path, line)));
        }
    }
}
