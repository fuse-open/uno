using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Uno.Compiler;
using Uno.Logging;

namespace Uno.ProjectFormat
{
    public class UnoprojParser : JsonTextReader
    {
        public static UnoprojDocument Parse(string filename)
        {
            using (var p = new UnoprojParser(Log.Default, filename))
                return p.ParseDocument();
        }

        readonly Log _log;
        readonly SourceFile _file;
        readonly UnoprojDocument _document;

        UnoprojParser(Log log, string filename)
            : base(new StreamReader(filename))
        {
            _log = log;
            _file = new SourceFile(SourcePackage.Unknown, filename);
            _document = new UnoprojDocument();
        }

        UnoprojDocument ParseDocument()
        {
            var startErrorCount = _log.ErrorCount;

            try
            {
                if (ReadToken() != JsonToken.StartObject)
                    throw new FormatException("Expected '{' to open project");

                ParseObject("");

                if (ReadToken() != JsonToken.None)
                    throw new FormatException("Expected EOF");
            }
            catch (Exception e)
            {
                throw new SourceException(GetSource(), e.Message, e);
            }

            if (_log.ErrorCount > startErrorCount)
                throw new SourceException(GetSource(), "Failed to parse project.\n\n" + _log.GetErrorSummary(startErrorCount));

            return _document;
        }

        void ParseObject(string prefix)
        {
            for (; ; )
            {
                switch (ReadToken())
                {
                    case JsonToken.PropertyName:
                        ParseProperty(prefix + Value);
                        break;
                    case JsonToken.EndObject:
                        return;
                    default:
                        throw new FormatException("Expected property or '}' to close object");
                }
            }
        }

        void ParseProperty(string name)
        {
            var tmp = ReadToken();
            switch (tmp)
            {
                case JsonToken.Null:
                case JsonToken.Boolean:
                case JsonToken.Float:
                case JsonToken.Integer:
                case JsonToken.String:
                {
                    _document.Properties[name] = GetValue();
                    break;
                }
                case JsonToken.StartObject:
                {
                    ParseObject(name + ".");
                    break;
                }
                case JsonToken.StartArray:
                {
                    switch (name)
                    {
                        case "InternalsVisibleTo":
                        {
                            _document.OptionalInternalsVisibleTo = new List<SourceValue>();

                            foreach (var e in ReadArray())
                                _document.OptionalInternalsVisibleTo.Add(new SourceValue(GetSource(), e));

                            break;
                        }
                        case "Packages":
                        {
                            _document.OptionalPackages = new List<PackageReference>();

                            foreach (var e in ReadArray())
                            {
                                try
                                {
                                    _document.OptionalPackages.Add(PackageReference.FromString(GetSource(), e));
                                }
                                catch (Exception x)
                                {
                                    _log.Error(GetSource(), null, "Invalid 'Packages' element (" + e + "): " + x.Message);
                                }
                            }
                            break;
                        }
                        case "Projects":
                        {
                            _document.OptionalProjects = new List<ProjectReference>();

                            foreach (var e in ReadArray())
                            {
                                try
                                {
                                    _document.OptionalProjects.Add(ProjectReference.FromString(GetSource(), e));
                                }
                                catch (Exception x)
                                {
                                    _log.Error(GetSource(), null, "Invalid 'Projects' element (" + e + "): " + x.Message);
                                }
                            }
                            break;
                        }
                        case "Includes":
                        {
                            foreach (var e in ReadArray())
                            {
                                try
                                {
                                    _document.Includes.Add(IncludeItem.FromString(GetSource(), e));
                                }
                                catch (Exception x)
                                {
                                    _log.Error(GetSource(), null, "Invalid 'Includes' element (" + e + "): " + x.Message);
                                }
                            }
                            break;
                        }
                        case "Excludes":
                        {
                            _document.OptionalExcludes = new List<SourceValue>();

                            foreach (var e in ReadArray())
                                _document.OptionalExcludes.Add(new SourceValue(GetSource(), e));

                            break;
                        }
                        default:
                        {
                            // Don't allow arrays in the root object
                            if (name.IndexOf('.') == -1)
                                throw new FormatException("Invalid array " + name.Quote());

                            _document.Properties[name] = GetArray();
                            break;
                        }
                    }
                    break;
                }
                default:
                {
                    throw new FormatException("Invalid property " + name.Quote() + " (" + Value + ")");
                }
            }
        }

        IEnumerable<string> ReadArray()
        {
            for (; ; )
            {
                switch (ReadToken())
                {
                    case JsonToken.String:
                        yield return (string)Value;
                        break;
                    case JsonToken.EndArray:
                        yield break;
                    default:
                        throw new FormatException("Expected string or ']' to close array");
                }
            }
        }

        JsonToken ReadToken()
        {
            while (Read() && TokenType == JsonToken.Comment)
                ;

            return TokenType;
        }

        SourceValue GetArray()
        {
            return new SourceValue(GetSource(), string.Join("\n", ReadArray()));
        }

        SourceValue GetValue()
        {
            return new SourceValue(GetSource(), Value?.ToString());
        }

        Source GetSource()
        {
            return new Source(_file, LineNumber, LinePosition);
        }
    }
}
