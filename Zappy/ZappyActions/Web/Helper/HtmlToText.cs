using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Zappy.ZappyActions.Web.Helper
{
    class HtmlToText
    {
                protected static Dictionary<string, string> _tags;
        protected static HashSet<string> _ignoreTags;

                protected TextBuilder _text;
        protected string _html;
        protected int _pos;

                static HtmlToText()
        {
            _tags = new Dictionary<string, string>();
            _tags.Add("address", "\n");
            _tags.Add("blockquote", "\n");
            _tags.Add("div", "\n");
            _tags.Add("dl", "\n");
            _tags.Add("fieldset", "\n");
            _tags.Add("form", "\n");
            _tags.Add("h1", "\n");
            _tags.Add("/h1", "\n");
            _tags.Add("h2", "\n");
            _tags.Add("/h2", "\n");
            _tags.Add("h3", "\n");
            _tags.Add("/h3", "\n");
            _tags.Add("h4", "\n");
            _tags.Add("/h4", "\n");
            _tags.Add("h5", "\n");
            _tags.Add("/h5", "\n");
            _tags.Add("h6", "\n");
            _tags.Add("/h6", "\n");
            _tags.Add("p", "\n");
            _tags.Add("/p", "\n");
            _tags.Add("table", "\n");
            _tags.Add("/table", "\n");
            _tags.Add("ul", "\n");
            _tags.Add("/ul", "\n");
            _tags.Add("ol", "\n");
            _tags.Add("/ol", "\n");
            _tags.Add("/li", "\n");
            _tags.Add("br", "\n");
            _tags.Add("/td", "\t");
            _tags.Add("/tr", "\n");
            _tags.Add("/pre", "\n");

            _ignoreTags = new HashSet<string>();
            _ignoreTags.Add("script");
            _ignoreTags.Add("noscript");
            _ignoreTags.Add("style");
            _ignoreTags.Add("object");
        }

                                                public string Convert(string html)
        {
                        _text = new TextBuilder();
            _html = html;
            _pos = 0;

                        while (!EndOfText)
            {
                if (Peek() == '<')
                {
                                        bool selfClosing;
                    string tag = ParseTag(out selfClosing);

                                        if (tag == "body")
                    {
                                                _text.Clear();
                    }
                    else if (tag == "/body")
                    {
                                                _pos = _html.Length;
                    }
                    else if (tag == "pre")
                    {
                                                _text.Preformatted = true;
                        EatWhitespaceToNextLine();
                    }
                    else if (tag == "/pre")
                    {
                                                _text.Preformatted = false;
                    }

                    string value;
                    if (_tags.TryGetValue(tag, out value))
                        _text.Write(value);

                    if (_ignoreTags.Contains(tag))
                        EatInnerContent(tag);
                }
                else if (Char.IsWhiteSpace(Peek()))
                {
                                        _text.Write(_text.Preformatted ? Peek() : ' ');
                    MoveAhead();
                }
                else
                {
                                        _text.Write(Peek());
                    MoveAhead();
                }
            }
                        return HttpUtility.HtmlDecode(_text.ToString());
        }

                        protected string ParseTag(out bool selfClosing)
        {
            string tag = String.Empty;
            selfClosing = false;

            if (Peek() == '<')
            {
                MoveAhead();

                                EatWhitespace();
                int start = _pos;
                if (Peek() == '/')
                    MoveAhead();
                while (!EndOfText && !Char.IsWhiteSpace(Peek()) &&
                       Peek() != '/' && Peek() != '>')
                    MoveAhead();
                tag = _html.Substring(start, _pos - start).ToLower();

                                while (!EndOfText && Peek() != '>')
                {
                    if (Peek() == '"' || Peek() == '\'')
                        EatQuotedValue();
                    else
                    {
                        if (Peek() == '/')
                            selfClosing = true;
                        MoveAhead();
                    }
                }
                MoveAhead();
            }
            return tag;
        }

                protected void EatInnerContent(string tag)
        {
            string endTag = "/" + tag;

            while (!EndOfText)
            {
                if (Peek() == '<')
                {
                                        bool selfClosing;
                    if (ParseTag(out selfClosing) == endTag)
                        return;
                                        if (!selfClosing && !tag.StartsWith("/"))
                        EatInnerContent(tag);
                }
                else MoveAhead();
            }
        }

                        protected bool EndOfText
        {
            get { return (_pos >= _html.Length); }
        }

                protected char Peek()
        {
            return (_pos < _html.Length) ? _html[_pos] : (char)0;
        }

                protected void MoveAhead()
        {
            _pos = Math.Min(_pos + 1, _html.Length);
        }

                        protected void EatWhitespace()
        {
            while (Char.IsWhiteSpace(Peek()))
                MoveAhead();
        }

                                protected void EatWhitespaceToNextLine()
        {
            while (Char.IsWhiteSpace(Peek()))
            {
                char c = Peek();
                MoveAhead();
                if (c == '\n')
                    break;
            }
        }

                protected void EatQuotedValue()
        {
            char c = Peek();
            if (c == '"' || c == '\'')
            {
                                MoveAhead();
                                int start = _pos;
                _pos = _html.IndexOfAny(new char[] { c, '\r', '\n' }, _pos);
                if (_pos < 0)
                    _pos = _html.Length;
                else
                    MoveAhead();                }
        }

                                protected class TextBuilder
        {
            private StringBuilder _text;
            private StringBuilder _currLine;
            private int _emptyLines;
            private bool _preformatted;

                        public TextBuilder()
            {
                _text = new StringBuilder();
                _currLine = new StringBuilder();
                _emptyLines = 0;
                _preformatted = false;
            }

                                                                        public bool Preformatted
            {
                get
                {
                    return _preformatted;
                }
                set
                {
                    if (value)
                    {
                                                                        if (_currLine.Length > 0)
                            FlushCurrLine();
                        _emptyLines = 0;
                    }
                    _preformatted = value;
                }
            }

                                                public void Clear()
            {
                _text.Length = 0;
                _currLine.Length = 0;
                _emptyLines = 0;
            }

                                                            public void Write(string s)
            {
                foreach (char c in s)
                    Write(c);
            }

                                                            public void Write(char c)
            {
                if (_preformatted)
                {
                                        _text.Append(c);
                }
                else
                {
                    if (c == '\r')
                    {
                                                                    }
                    else if (c == '\n')
                    {
                                                FlushCurrLine();
                    }
                    else if (Char.IsWhiteSpace(c))
                    {
                                                int len = _currLine.Length;
                        if (len == 0 || !Char.IsWhiteSpace(_currLine[len - 1]))
                            _currLine.Append(' ');
                    }
                    else
                    {
                                                _currLine.Append(c);
                    }
                }
            }

                        protected void FlushCurrLine()
            {
                                string line = _currLine.ToString().Trim();

                                string tmp = line.Replace("&nbsp;", String.Empty);
                if (tmp.Length == 0)
                {
                                        _emptyLines++;
                    if (_emptyLines < 2 && _text.Length > 0)
                        _text.AppendLine(line);
                }
                else
                {
                                        _emptyLines = 0;
                    _text.AppendLine(line);
                }

                                _currLine.Length = 0;
            }

                                                public override string ToString()
            {
                if (_currLine.Length > 0)
                    FlushCurrLine();
                return _text.ToString();
            }
        }
    }
}