using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.Decode.Mssa
{
    public static class OrderOfInvoke
    {
        private static bool isEnabled = true;
        private static bool needsReset = true;

        private static Dictionary<ITaskActivityElement, int> orderMap = new Dictionary<ITaskActivityElement, int>(new TopLevelComparer());

        private static Dictionary<IntPtr, int> orderMapPlayback = new Dictionary<IntPtr, int>();
        private static readonly Regex regExOrder = new Regex(@"( && )*FilterCondition\(OrderOfInvocation=\'(?<digit>-?\d+)\'\)", RegexOptions.CultureInvariant | RegexOptions.Compiled);


        public static void ClearCache()
        {
            orderMap.Clear();
            orderMapPlayback.Clear();
        }


        internal static int GetOrderOfInvocation(ITaskActivityElement element, AndCondition elementQueryElement)
        {
            if (!IsEnabled)
            {
                return 0;
            }
            string windowText = NativeMethods.GetWindowText(element.WindowHandle);
            if (string.IsNullOrEmpty(windowText))
            {
                return 1;
            }
            if (!OrderMap.ContainsKey(element))
            {
                List<ITaskActivityElement> list = new List<ITaskActivityElement>();
                int num = 1;
                foreach (KeyValuePair<ITaskActivityElement, int> pair in OrderMap)
                {
                    IntPtr windowHandle = pair.Key.WindowHandle;
                    if (!NativeMethods.IsWindow(windowHandle))
                    {
                        list.Add(pair.Key);
                        continue;
                    }
                    IQueryCondition[] conditions = elementQueryElement.Conditions;
                    IQueryCondition condition = pair.Key.QueryId.Condition;
                    bool? nullable = false;
                    object obj2 = null;
                    if (element != null && pair.Key != null)
                    {
                        if (string.Equals(NativeMethods.GetWindowText(windowHandle), windowText, StringComparison.Ordinal))
                        {
                            nullable = true;
                        }
                        if (nullable.Value)
                        {
                            foreach (QueryCondition condition2 in conditions)
                            {
                                PropertyCondition condition3 = condition2 as PropertyCondition;
                                if (condition3 != null && !condition3.PropertyName.Equals("Name", StringComparison.OrdinalIgnoreCase) && (obj2 = condition.GetPropertyValue(condition3.PropertyName)) != null && !string.Equals(obj2.ToString(), condition3.Value.ToString(), StringComparison.Ordinal))
                                {
                                    nullable = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (nullable.Value)
                    {
                        num++;
                    }
                }
                foreach (ITaskActivityElement element2 in list)
                {
                    OrderMap.Remove(element2);
                }
                SetOrderMap(element, num);
            }
            return OrderMap[element];
        }


        public static void Initialize()
        {
            needsReset = false;
            orderMap.Clear();
            orderMapPlayback.Clear();
        }

        internal static int ParseQueryId(string queryId, out string fixedQid)
        {
            int num = 0;
            fixedQid = queryId;
            Match match = regExOrder.Match(queryId);
            if (match.Success)
            {
                fixedQid = queryId.Replace(match.Value, "");
                num = int.Parse(match.Groups["digit"].Value, CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
            if (!isEnabled)
            {
                return 0;
            }
            return num;
        }


        internal static void SetOrderMap(ITaskActivityElement element, int value)
        {
            orderMap[element] = value;
        }


        internal static void SetOrderMapPlayback(IntPtr handle, int value)
        {
            orderMapPlayback[handle] = value;
        }

        internal static bool IsEnabled
        {
            get =>
                isEnabled;
            set
            {
                isEnabled = value;
            }
        }

        internal static bool NeedsReset =>
            needsReset;


        public static Dictionary<ITaskActivityElement, int> OrderMap =>
            orderMap;


        public static Dictionary<IntPtr, int> OrderMapPlayback =>
            orderMapPlayback;

        private class TopLevelComparer : IEqualityComparer<ITaskActivityElement>
        {
            public bool Equals(ITaskActivityElement objLeft, ITaskActivityElement objRight) =>
                objLeft != null && objRight != null && objLeft.WindowHandle == objRight.WindowHandle;

            public int GetHashCode(ITaskActivityElement obj)
            {
                if (obj != null)
                {
                    return obj.WindowHandle.GetHashCode();
                }
                return -1;
            }
        }
    }
}