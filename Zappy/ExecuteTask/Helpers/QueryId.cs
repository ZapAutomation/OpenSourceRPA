using System;
using System.Collections.Generic;
using System.Text;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{
    internal class QueryId
    {
        private const char EscapeCharacter = '\\';
        private List<SingleQueryId> queryIds;
        private const char QueryIdSeparator = ';';
        private const char SingleQuotes = '\'';

        public QueryId(string queryIdString)
        {
            ZappyTaskUtilities.CheckForNull(queryIdString, "queryIdString");
            object[] args = { queryIdString };
            
            queryIds = new List<SingleQueryId>();
            foreach (string str in ParseQueryId(queryIdString))
            {
                queryIds.Add(new SingleQueryId(str));
            }
        }

        public int GetIndexOfExpandableElement(int startIndex)
        {
            while (startIndex < SingleQueryIds.Count - 1)
            {
                if (SingleQueryIds[startIndex].IsExpandable)
                {
                    return startIndex;
                }
                startIndex++;
            }
            return startIndex;
        }

        public string GetQueryString(int startIndex, int lastIndex)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = startIndex; i <= lastIndex && i < SingleQueryIds.Count; i++)
            {
                builder.Append(SingleQueryIds[i]);
            }
            return builder.ToString();
        }

        internal static List<string> ParseQueryId(string queryIdString)
        {
            ZappyTaskUtilities.CheckForNull(queryIdString, "queryIdString");
            List<string> list = new List<string>();
            if (!Equals(queryIdString[0], ';'))
            {
                CrapyLogger.log.ErrorFormat("ParseQueryId : Invalid Query Id : Doesn't start with a delimiter");
                throw new ArgumentException(Resources.InvalidQueryString, "queryIdString");
            }
            bool flag = false;
            int startIndex = 1;
            for (int i = 1; i <= queryIdString.Length; i++)
            {
                if (i == queryIdString.Length || Equals(queryIdString[i], ';') && !flag)
                {
                    string str = queryIdString.Substring(startIndex, i - startIndex).Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        list.Add(str);
                    }
                    startIndex = i + 1;
                }
                else if (Equals(queryIdString[i], '\\'))
                {
                    i++;
                }
                else if (Equals(queryIdString[i], '\''))
                {
                    flag = !flag;
                }
            }
            if (flag)
            {
                CrapyLogger.log.ErrorFormat("ParseQueryId : Invalid Query Id");
                throw new ArgumentException(Resources.InvalidQueryString, "queryIdString");
            }
            return list;
        }

        public override string ToString() =>
            GetQueryString(0, SingleQueryIds.Count);

        public IList<SingleQueryId> SingleQueryIds =>
            queryIds;
    }
}