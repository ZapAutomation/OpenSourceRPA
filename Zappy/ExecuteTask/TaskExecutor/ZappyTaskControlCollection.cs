using System;
using System.Collections.Generic;
using System.Globalization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Properties;

namespace Zappy.ExecuteTask.TaskExecutor
{
    public class ZappyTaskControlCollection : List<ZappyTaskControl>
    {

        private void AddTechnologyElement(TaskActivityElement technologyElement, ZappyTaskControl parentElement,
            Dictionary<string, int> countPerControlType)
        {
            try
            {
                string controlTypeName = technologyElement.ControlTypeName;
                if (!countPerControlType.ContainsKey(controlTypeName))
                {
                    countPerControlType.Add(controlTypeName, 0);
                }
                int num = countPerControlType[controlTypeName];
                object[] args = { technologyElement.Framework, controlTypeName };
                string queryIdForRefetch = string.Format(CultureInfo.InvariantCulture, ";[{0}]ControlType='{1}'", args);
                ZappyTaskControl item =
                    ZappyTaskControl.FromTechnologyElement(technologyElement, parentElement, queryIdForRefetch);
                item.Instance = num;
                item.MaxDepth = 1;
                countPerControlType[controlTypeName] = ++num;
                Add(item);
            }
            catch (ZappyTaskControlNotAvailableException)
            {
            }
        }





        public T[] GetPropertyValuesOfControls<T>(string propertyName)
        {
            ZappyTaskUtilities.CheckForNull(propertyName, "propertyName");
            T[] localArray = new T[Count];
            for (int i = 0; i < localArray.Length; i++)
            {
                T local;
                if (!ZappyTaskUtilities.TryConvertToType(this[i].GetProperty(propertyName), out local))
                {
                    object[] args = { propertyName, typeof(T).Name };
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetPropertyValuesValueTypeIncorrectMessage, args));
                }
                localArray[i] = local;
            }
            return localArray;
        }




        internal static ZappyTaskControlCollection FromChildren(ZappyTaskControl parentElement)
        {
            ZappyTaskControlCollection controls = new ZappyTaskControlCollection();
            IEnumerator<TaskActivityElement> children = ZappyTaskService.Instance.GetChildren(parentElement.TechnologyElement, null);
            Dictionary<string, int> countPerControlType = new Dictionary<string, int>(ControlType.NameComparer);
            while (children.MoveNext())
            {
                TaskActivityElement current = children.Current;
                if (current != null)
                {
                    controls.AddTechnologyElement(current, parentElement, countPerControlType);
                }
            }
            return controls;
        }


        public string[] GetNamesOfControls() =>
            GetPropertyValuesOfControls<string>(ZappyTaskControl.PropertyNames.Name);


        public string[] GetValuesOfControls() =>
            GetPropertyValuesOfControls<string>(ZappyTaskControl.PropertyNames.Value);






    }

}