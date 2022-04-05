using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.SharedInterface;
using Zappy.ZappyTaskEditor.ExecutionHelpers;

namespace Zappy.ZappyTaskEditor.EditorPage.Forms
{

    public static class ZappyTaskEditorHelper
    {
        public static List<PropertyInfo> GetPropertyInfos(IZappyAction _Action)
        {
            return _Action.GetType().GetProperties()
                .Where(x => !x.PropertyType.IsGenericType).ToList();
        }

                                                                                                        
                
                                
                                
        public static void
            SetInputsFromArguments(IZappyAction Action,
                List<Argument> args)         {
            foreach (var prop in GetPropertyInfos(Action))
            {
                Argument _Given = args.FirstOrDefault(x => x.Name == prop.Name);
                if (_Given != null)
                {
                    prop.SetValue(Action, _Given.Value);
                }
            }
        }

        public static List<Output> GetOutputs(IZappyAction Action)
        {
            var args = new List<Output>();
            foreach (var prop in GetPropertyInfos(Action))
            {
                object _Value = prop.GetValue(Action);
                args.Add(new Output()
                {
                    Name = prop.Name,
                    Type = prop.PropertyType.FullName,                    Value = (_Value ?? string.Empty).ToString()                 });
            }

            return args;
        }

        public static IEnumerable<Type> GetAllActivities()
        {
            return ActionTypeInfo.SupportedActionTypes;

                        
                                    

                    }

        public static IZappyAction CreateInstance(string activityId)
        {
            if (GetAllActivities().Where(x => x.FullName == activityId).FirstOrDefault() is Type activityType)
            {
                return Activator.CreateInstance(activityType) as IZappyAction;
            }
            else
            {
                throw new TypeLoadException(string.Format("Cannot create instance of '{0}' activity", activityId));
            }
        }

    }
}