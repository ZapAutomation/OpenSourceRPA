using System;
using System.Collections.Generic;
using System.Linq;
using Zappy.Decode.LogManager;
using Zappy.ZappyTaskEditor.EditorPage.ElementPicker;
using Zappy.ZappyTaskEditor.ExecutionHelpers;
using Zappy.ZappyTaskEditor.Helper;

namespace Zappy.ZappyActions.ElementPicker.Helper
{
    public static class ElementBot
    {
                                        
        
                                
                
        public static Element GetElement(this ZappyExecutionContext context, string value)
        {
            var query = new ElementQuery(XmlSerializerHelper.ToObject<List<Condition>>(value));
            if (query == null)
                throw new Exception("Input 'Element' is required.");

            var elements = WinContext.Shared.GetElementsFromQuery(query);

            if (elements.Count() == 0)
                throw new Exception("No element found.");
            if (elements.Count() > 1)
                CrapyLogger.log.Error("Too many elements found.");

            return elements.First();
        }

        public static int CountElements(this ZappyExecutionContext context, string input)
        {
            var query = new ElementQuery(XmlSerializerHelper.ToObject<List<Condition>>(input));
            if (query == null)
                throw new Exception("Input 'Element' is required.");

            var elements = WinContext.Shared.GetElementsFromQuery(query);

            return elements.Count();
        }

    }
}
