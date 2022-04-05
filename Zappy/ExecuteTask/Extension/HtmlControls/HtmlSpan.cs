﻿using System;
using Zappy.ActionMap.HelperClasses;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.ExecuteTask.Extension.HtmlControls
{
    [CLSCompliant(true)]
    public class HtmlSpan : HtmlControl
    {
        public HtmlSpan() : this(null)
        {
        }

        public HtmlSpan(ZappyTaskControl parent) : base(parent)
        {
            SearchProperties.Add(ZappyTaskControl.PropertyNames.ControlType, ControlType.Pane.Name);
            SearchProperties.Add(HtmlControl.PropertyNames.TagName, "SPAN");
        }

        public virtual string DisplayText =>
            (string)GetProperty(HtmlDiv.PropertyNames.DisplayText);

        [CLSCompliant(true)]
        public abstract class PropertyNames : HtmlControl.PropertyNames
        {
            public static readonly string DisplayText = "DisplayText";
        }
    }
}

