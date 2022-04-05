using System.ComponentModel;
using Zappy.ZappyTaskEditor.EditorPage;

namespace Zappy.ZappyActions.Core.Helper
{
    [Description("Run Zappy TaskHelper")]
    internal class RunZappyTaskHelper
    {
        [Description("Run Zappy Task which one you want to start")]
        internal RunZappyTask runZappyTask { get; set; }

        [Description("Parent TaskEditor Page")]
        internal TaskEditorPage ParentTaskEditorPage { get; set; }
        
    }
}