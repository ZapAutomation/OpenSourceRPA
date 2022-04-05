using System.Collections.Generic;

namespace Zappy.Helpers
{
    internal class InternalNode
    {

        public int MillisecsSpent { get; set; }

        public int StartMillisec { get; set; }

        List<InternalNode> _Nodes;

        public string Text { get; private set; }

        public string AltText { get; set; }


        public List<InternalNode> Nodes
        {
            get
            {
                if (_Nodes == null)
                    _Nodes = new List<InternalNode>();
                return _Nodes;
            }
        }

        public InternalNode Parent { get; set; }
        public object Tag { get; set; }

        public InternalNode(string Text)
        {
            this.Text = Text;
        }
    }
}