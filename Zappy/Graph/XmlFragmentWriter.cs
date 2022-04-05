using System;
using System.IO;
using System.Xml;

namespace Zappy.Graph
{
    internal class XmlFragmentWriter : XmlTextWriter
    {
        public XmlFragmentWriter(TextWriter w) : base(w)
        {
                                            }

                                        
                                                

        private bool _skip;
        public override void WriteRaw(string data)
        {
                                    base.WriteRaw(data);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)

        {
            
            
            if (prefix != null && prefix.StartsWith("xmlns") && (localName == "xsd" || localName == "xsi")
            )             {
                _skip = true;

                return;
            }

            base.WriteStartAttribute(prefix, localName, "");
        }


        public override void WriteString(string text)

        {
            if (_skip) return;            if (text != Environment.NewLine)
                text = text.Replace(Environment.NewLine, "<EOS>");
            base.WriteString(text);
        }


        public override void WriteEndAttribute()

        {
            if (_skip)

            {
                
                _skip = false;

                return;
            }

            base.WriteEndAttribute();
        }


        public override void WriteStartDocument()
        {
                    }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
                                                base.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteEndElement()
        {
                                    base.WriteEndElement();
        }

    }
}