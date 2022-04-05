using System;
using System.Collections.Generic;
using Zappy.ZappyTaskEditor.Nodes;

namespace Zappy.ZappyTaskEditor.EditorPage
{
    public interface ITaskEditorPage
    {
        PageState State { get; }
        string Name { get; set; }

        List<Node> Nodes { get; set; }

        event EventHandler OnStateChanged;

        void Run();

            }
}