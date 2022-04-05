using System.ComponentModel;
using System.Windows.Forms;

namespace Zappy.Invoker
{
    internal class NodeRequestTextEventArgs : CancelEventArgs
    {

        #region Constructors

        public NodeRequestTextEventArgs(TreeNode node, string label)
            : this()
        {
            this.Node = node;
            this.Label = label;
        }

        protected NodeRequestTextEventArgs()
        { }

        #endregion

        #region Properties

        public string Label { get; set; }

        public TreeNode Node { get; protected set; }

        #endregion
    }
}


