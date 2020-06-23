using Uno.Collections;

namespace Uno.Internal
{
    public class FormatStringTokenizer
    {
        private enum State { Beginning, Literal, FirstCurly, FormatItem, End}
        int _index = 0;
        int _prevIndex = 0;
        List<FormatStringToken> _tokens;
        State _state;
        string _format;

        public FormatStringTokenizer(string format)
        {
            _format = format;
        }

        public static List<FormatStringToken> TokenizeFormatString(string format)
        {
            return new FormatStringTokenizer(format).TokenizeFormatStringInternal();
        }

        private List<FormatStringToken> TokenizeFormatStringInternal()
        {
            _index = 0;
            _prevIndex = 0;
            _state = State.Beginning;
            _tokens = new List<FormatStringToken>();
            while (_index <= _format.Length)
            {
                switch (_state)
                {
                    case State.Beginning:
                        if (AtEnd())
                            _state = State.End;
                        else if (_format[_index] == '{')
                            _state = State.FirstCurly;
                        else if (_format[_index] == '}')
                            Throw();
                        else
                            _state = State.Literal;
                        break;
                    case State.FirstCurly:
                        if (AtEnd())
                            Throw();
                        else if (_format[_index] == '{')
                            DiscardCharacterAndGoTo(State.Literal);
                        else if (_format[_index] == '}')
                            Throw();
                        else
                            _state = State.FormatItem;
                        break;
                    case State.FormatItem:
                        if (AtEnd())
                            Throw();
                        else if (_format[_index] == '}')
                            AddFormatItemAndGoTo(State.Beginning);
                        else if (_format[_index] == '{')
                            Throw();
                        break;
                    case State.Literal:
                        if (AtEnd())
                            AddLiteralAndGoTo(State.End);
                        else if (_format[_index] == '{')
                            AddLiteralAndGoTo(State.FirstCurly);
                        else if (_format[_index] == '}')
                            Throw();
                        break;
                }
                _index++;
            }
            return _tokens;
        }

        private void AddFormatItemAndGoTo(State state)
        {
            _tokens.Add(new FormatStringItem(_format.Substring(_prevIndex, _index - _prevIndex + 1)));
            _prevIndex = _index +1;
            _state = state;
        }

        private void AddLiteralAndGoTo(State state)
        {
            _tokens.Add(new FormatStringLiteral(_format.Substring(_prevIndex, _index - _prevIndex)));
            _prevIndex = _index;
            _state = state;
        }

        private void DiscardCharacterAndGoTo(State state)
        {
            _prevIndex++;
            _state = state;
        }

        private void Throw()
        {
            throw new FormatException("Input string was not in a correct format");
        }

        private bool AtEnd()
        {
            return _index == _format.Length;
        }
    }
}
