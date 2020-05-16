using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Timers;
using Uno.Diagnostics;

namespace Uno.Logging
{
    public class LogState : IDisposable
    {
        const double _fps = 18.0;
        const double _interval = 1000.0 / _fps;

        static readonly string _cr = "\r" + new string(' ', GetNumberOfSpaces()) + "\r";

        static int GetNumberOfSpaces()
        {
            try
            {
                return Math.Max(Console.WindowWidth - 1, 0);
            }
            catch (System.IO.IOException)
            {
                return 80; // not attached to a terminal?
            }
        }

        static readonly string _spinner = PlatformDetection.IsWindows
                                        ? "|/-\\" // The Windows shell can't print the dot characters
                                        : "⠋⠙⠹⠸⠼⠴⠦⠧⠇⠏";
        static readonly Stopwatch _w = Stopwatch.StartNew();
        static LogState _animated;
        static Timer _timer;

        internal bool SkipLine, DisableSkip = true;
        internal bool DeferLine => _deferLine != null;
        internal double Time => _w.Elapsed.TotalSeconds;

        ConsoleColor? _deferColor;
        ConsoleColor _resetColor;
        TextWriter _deferWriter;
        string _deferLine;
        double _deferTime;
        bool _resetLine;

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
            Flush(null);
            SetColor(null, null);
        }

        internal void SetColor(TextWriter writer, ConsoleColor? value)
        {
            if (writer == TextWriter.Null)
                return;

            try
            {
                if (value == null)
                {
                    Console.ResetColor();
                    return;
                }

                var color = value.Value;
                // adjust colors when background is white (or non-dark)
                if ((int)Console.BackgroundColor > 8 && (int)color > 8)
                    color = color == ConsoleColor.White
                        ? ConsoleColor.Black    // white -> black
                        : color - 8;            // light -> dark
                Console.ForegroundColor = color;
            }
            catch
            {
                // Swallow exception
            }
        }

        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        internal IDisposable BeginDeferred(TextWriter writer, string line, ConsoleColor? color, bool animate)
        {
            if (writer == TextWriter.Null)
                return writer;

            Flush(writer);
            SetColor(writer, color);
            writer.Write(line);

            _deferLine = line;
            _deferTime = Time;
            _deferWriter = writer;
            _deferColor = color;

            if (animate)
            {
                if (_timer == null)
                {
                    _timer = new Timer(_interval);
                    _timer.Elapsed += (_, __) => {
                        lock (_animated)
                            if (_timer.Enabled) // Check race condition
                                lock (_animated._deferWriter) // Don't race with i.e. Stuff.Log
                                    _animated.Tick();         // (or anything locking Console.Out)
                    };
                }

                _animated = this;
                _timer.Enabled = true;
            }

            return new Deferred(this);
        }

        internal void EndDeferred(TextWriter writer, string line, ConsoleColor? color, double start)
        {
            if (writer == TextWriter.Null)
                return;

            if (_timer != null)
                _timer.Enabled = false;

            if (_deferLine != null && _deferLine != line)
                Flush(writer);

            if (_resetLine)
            {
                writer.Write(_cr);
                _resetLine = false;
                SetColor(writer, color);
                writer.Write(_deferLine);
            }

            SetColor(writer, GetDark(color));
            SkipLine = true;

            if (_deferLine == null)
            {
                writer.Write("> ");
                writer.WriteLine(TimeSince(start));
            }
            else
            {
                writer.Write("  ");
                writer.Write(TimeSince(start));
                Flush(writer);
                DisableSkip = true;
            }
        }

        internal void Flush(TextWriter writer)
        {
            if (writer == TextWriter.Null)
                return;

            if (_resetLine)
            {
                SetColor(writer, _deferColor);
                _deferWriter.Write(_cr);
                _deferWriter.Write(_deferLine);
                SafeForegroundColor = _resetColor;
                _resetLine = false;
            }

            if (_deferLine != null)
                _deferWriter.WriteLine();
            else if (SkipLine && !DisableSkip)
                writer?.WriteLine();

            SkipLine = false;
            DisableSkip = false;
            _deferLine = null;
        }

        void Tick()
        {
            if (_resetLine)
            {
                SetColor(_deferWriter, _deferColor);
                _deferWriter.Write(_cr);
                _deferWriter.Write(_deferLine);
            }
            else
                _resetColor = SafeForegroundColor;

            SetColor(_deferWriter, GetDark(_deferColor));
            _deferWriter.Write(_deferLine != null ? "  " : "> ");
            _deferWriter.Write(TimeSince(_deferTime));
            _deferWriter.Write(' ');
            _deferWriter.Write(_spinner[(int) (Time * _fps) % _spinner.Length]);
            _resetLine = true;
        }

        static string TimeSince(double start)
        {
            var s = _w.Elapsed.TotalSeconds - start;

            if (Log.Default.IsVeryVerbose)
                return (s * 1000.0).ToString("#,##0.00 ms").PadLeft(11);
            if (s < 60.0)
                return s.ToString("0.00 s");

            var t = TimeSpan.FromSeconds(s);
            return t.Minutes + " m, " + (s - t.Minutes * 60.0).ToString("0.00 s");
        }

        static ConsoleColor GetDark(ConsoleColor? color)
        {
            return color == ConsoleColor.Green
                       ? ConsoleColor.DarkGreen :
                   color == ConsoleColor.Cyan
                       ? ConsoleColor.DarkCyan :
                   color == ConsoleColor.Blue
                       ? ConsoleColor.DarkBlue :
                   color
                      ?? ConsoleColor.DarkGray;
        }

        static ConsoleColor SafeForegroundColor
        {
            get
            {
                try
                {
                    return Console.ForegroundColor;
                }
                catch
                {
                    return ConsoleColor.White;
                }
            }
            set
            {
                try
                {
                    Console.ForegroundColor = value;
                }
                catch
                {
                    // Swallow exception
                }
            }
        }

        class Deferred : IDisposable
        {
            readonly LogState _state;
            readonly TextWriter _writer;
            readonly ConsoleColor? _color;
            readonly string _line;
            readonly double _time;

            public Deferred(LogState state)
            {
                _state = state;
                _writer = state._deferWriter;
                _color = state._deferColor;
                _line = state._deferLine;
                _time = state._deferTime;
            }

            public void Dispose()
            {
                lock (_state)
                    _state.EndDeferred(_writer, _line, _color, _time);
            }
        }
    }
}