using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Zappy.Decode.Helper;

namespace Zappy.Invoker.ZappyControls
{
    internal class ZappyTreeView : System.Windows.Forms.TreeView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        
        
        
        
        
                
                

        public ToolTip ButtonTooltip { get; set; }

        public event Action<TreeNode, bool> PinnedNodeChanged;

        public ZappyTreeView()
        {
            if (!DesignMode)
            {
                                ButtonTooltip = new ToolTip();
                                                
                                                
                                                                                                                                                                                                
                this.CheckBoxes = true;
                                this.FullRowSelect = true;
                this.HotTracking = true;
                this.Indent = 22;
                this.ItemHeight = 22;
                this.LabelEdit = true;
                this.LineColor = this.BackColor;
            }

        }

                        
                                                
                                

                


        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            DrawTreeNodeEventArgs de = new DrawTreeNodeEventArgs(
                e.Graphics,
                e.Node,
                new Rectangle(e.Bounds.Location,
                new Size(ClientSize.Width, e.Bounds.Height)), e.State);
            base.OnDrawNode(de);
        }

                        
                                                                                        
                                
                                                                                                        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(this.Handle, "explorer", null);
        }

        #region Events

        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestDisplayText;

        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestEditText;

        #endregion

        #region Overridden Members

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null)             {
                NodeRequestTextEventArgs displayTextArgs = new NodeRequestTextEventArgs(e.Node, e.Label);
                RequestDisplayText?.Invoke(this, displayTextArgs);

                e.CancelEdit = true; 
                if (!displayTextArgs.Cancel)
                    e.Node.Text = displayTextArgs.Label;
            }

            base.OnAfterLabelEdit(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (RequestEditText != null)
            {
                NodeRequestTextEventArgs editTextArgs = new NodeRequestTextEventArgs(e.Node, e.Node.Text);
                RequestEditText?.Invoke(this, editTextArgs);
                                if (editTextArgs.Cancel)
                    e.CancelEdit = true;

                                if (!e.CancelEdit)
                {
                    IntPtr editHandle;

                    editHandle = NativeMethods.SendMessage(this.Handle, NativeMethods.TVM_GETEDITCONTROL, IntPtr.Zero,
                        IntPtr.Zero);                     if (editHandle != IntPtr.Zero)
                        NativeMethods.SendMessage(editHandle, NativeMethods.WM_SETTEXT, IntPtr.Zero,
                            editTextArgs.Label);                 }
            }
            base.OnBeforeLabelEdit(e);
        }

        #endregion

        #region Members

                                        protected virtual void OnRequestDisplayText(NodeRequestTextEventArgs e)
        {
            EventHandler<NodeRequestTextEventArgs> handler;

            handler = this.RequestDisplayText;

            handler?.Invoke(this, e);
        }



        #endregion


    }
}
