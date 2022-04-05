using System;
using System.Collections.Generic;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ActionMap.HelperClasses
{
    internal class ElementQueryComparer : IEqualityComparer<ITaskActivityElement>
    {
        private static bool ElementEquals(ITaskActivityElement x, ITaskActivityElement y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return Object.Equals(x, y) || x.Equals(y) && QueryIdEquals(x, y);
        }

        public bool Equals(ITaskActivityElement x, ITaskActivityElement y) =>
            ElementEquals(x, y);

        public int GetHashCode(ITaskActivityElement obj)
        {
            ZappyTaskUtilities.CheckForNull(obj, "obj");
            return GetHashCodeRecursive(obj);
        }

        private int GetHashCodeRecursive(ITaskActivityElement obj)
        {
            int hashCode = obj.GetHashCode();
            if (obj.QueryId != null)
            {
                hashCode ^= obj.QueryId.GetHashCode();
                if (obj.QueryId.Ancestor != null)
                {
                    hashCode ^= GetHashCodeRecursive(obj.QueryId.Ancestor);
                }
            }
            return hashCode;
        }

        private static bool QueryIdEquals(ITaskActivityElement x, ITaskActivityElement y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            if (x.QueryId == null && y.QueryId == null)
            {
                return true;
            }
            if (x.QueryId == null || y.QueryId == null)
            {
                return false;
            }
            return x.QueryId.Equals(y.QueryId) && QueryIdEquals(x.QueryId.Ancestor, y.QueryId.Ancestor);
        }
    }
}
