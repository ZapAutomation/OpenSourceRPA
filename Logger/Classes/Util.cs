using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZappyLogger.Classes.Filter;
using ZappyLogger.ColumnizaeLib;
using ZappyLogger.Config;

namespace ZappyLogger.Classes
{
    public class Util
    {
        #region Public methods

        public static string GetNameFromPath(string fileName)
        {
            int i = fileName.LastIndexOf('\\');
            if (i < 0)
            {
                i = fileName.LastIndexOf('/');
            }
            if (i < 0)
            {
                i = -1;
            }
            return fileName.Substring(i + 1);
        }

        public static string StripExtension(string fileName)
        {
            int i = fileName.LastIndexOf('.');
            if (i < 0)
            {
                i = fileName.Length - 1;
            }
            return fileName.Substring(0, i);
        }

        public static string GetExtension(string fileName)
        {
            int i = fileName.LastIndexOf('.');
            if (i < 0 || i >= fileName.Length - 1)
            {
                return "";
            }
            else
            {
                return fileName.Substring(i + 1);
            }
        }


        public static string GetFileSizeAsText(long size)
        {
            if (size < 1024)
            {
                return "" + size + " bytes";
            }
            else if (size < 1024 * 1024)
            {
                return "" + size / 1024 + " KB";
            }
            else
            {
                return "" + string.Format("{0:0.00}", (double)size / 1048576.0) + " MB";
            }
        }

        public static bool TestFilterCondition(FilterParams filterParams, ILogLine line,
            ILogLineColumnizerCallback columnizerCallback)
        {
            if (filterParams.lastLine.Equals(line.FullLine))
            {
                return filterParams.lastResult;
            }

            bool match = TestFilterMatch(filterParams, line, columnizerCallback);
            filterParams.lastLine = line.FullLine;

            if (filterParams.isRangeSearch)
            {
                if (!filterParams.isInRange)
                {
                    if (match)
                    {
                        filterParams.isInRange = true;
                    }
                }
                else
                {
                    if (!match)
                    {
                        match = true;
                    }
                    else
                    {
                        filterParams.isInRange = false;
                    }
                }
            }
            if (filterParams.isInvert)
            {
                match = !match;
            }
            filterParams.lastResult = match;
            return match;
        }


        public static int DamerauLevenshteinDistance(string src, string dest)
        {
            int[,] d = new int[src.Length + 1, dest.Length + 1];
            int i, j, cost;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    d[i, j] =
                        Math.Min(d[i - 1, j] + 1, // Deletion
                            Math.Min(d[i, j - 1] + 1, // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if (i > 1 && j > 1 && str1[i - 1] == str2[j - 2] && str1[i - 2] == str2[j - 1])
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }
            return d[str1.Length, str2.Length];
        }


        public static unsafe int YetiLevenshtein(string s1, string s2)
        {
            fixed (char* p1 = s1)
            fixed (char* p2 = s2)
            {
                return YetiLevenshtein(p1, s1.Length, p2, s2.Length, 0); // substitutionCost = 1
            }
        }

        public static unsafe int YetiLevenshtein(string s1, string s2, int substitionCost)
        {
            int xc = substitionCost - 1;
            if (xc < 0 || xc > 1)
            {
                throw new ArgumentException("", "substitionCost");
            }

            fixed (char* p1 = s1)
            fixed (char* p2 = s2)
            {
                return YetiLevenshtein(p1, s1.Length, p2, s2.Length, xc);
            }
        }

        /// <summary>
        /// Cetin Sert, David Necas
        /// http://webcleaner.svn.sourceforge.net/viewvc/webcleaner/trunk/webcleaner2/wc/levenshtein.c?revision=6015&view=markup
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="l1"></param>
        /// <param name="s2"></param>
        /// <param name="l2"></param>
        /// <param name="xcost"></param>
        /// <returns></returns>
        public static unsafe int YetiLevenshtein(char* s1, int l1, char* s2, int l2, int xcost)
        {
            int i;
            //int *row;  /* we only need to keep one row of costs */
            int* end;
            int half;

            /* strip common prefix */
            while (l1 > 0 && l2 > 0 && *s1 == *s2)
            {
                l1--;
                l2--;
                s1++;
                s2++;
            }

            /* strip common suffix */
            while (l1 > 0 && l2 > 0 && s1[l1 - 1] == s2[l2 - 1])
            {
                l1--;
                l2--;
            }

            /* catch trivial cases */
            if (l1 == 0)
            {
                return l2;
            }
            if (l2 == 0)
            {
                return l1;
            }

            /* make the inner cycle (i.e. string2) the longer one */
            if (l1 > l2)
            {
                int nx = l1;
                char* sx = s1;
                l1 = l2;
                l2 = nx;
                s1 = s2;
                s2 = sx;
            }

            //check len1 == 1 separately
            if (l1 == 1)
            {
                //throw new NotImplementedException();
                if (xcost > 0)
                //return l2 + 1 - 2*(memchr(s2, *s1, l2) != NULL);
                {
                    return l2 + 1 - 2 * memchrRPLC(s2, *s1, l2);
                }
                else
                //return l2 - (memchr(s2, *s1, l2) != NULL);
                {
                    return l2 - memchrRPLC(s2, *s1, l2);
                }
            }

            l1++;
            l2++;
            half = l1 >> 1;

            /* initalize first row */
            //row = (int*)malloc(l2*sizeof(int));
            int* row = stackalloc int[l2];
            if (l2 < 0)
            //if (!row)
            {
                return (int)-1;
            }
            end = row + l2 - 1;
            for (i = 0; i < l2 - (xcost > 0 ? 0 : half); i++)
            {
                row[i] = i;
            }

            /* go through the matrix and compute the costs.  yes, this is an extremely
           * obfuscated version, but also extremely memory-conservative and
           * relatively fast.
           */
            if (xcost > 0)
            {
                for (i = 1; i < l1; i++)
                {
                    int* p = row + 1;
                    char char1 = s1[i - 1];
                    char* char2p = s2;
                    int D = i;
                    int x = i;
                    while (p <= end)
                    {
                        if (char1 == *char2p++)
                        {
                            x = --D;
                        }
                        else
                        {
                            x++;
                        }
                        D = *p;
                        D++;
                        if (x > D)
                        {
                            x = D;
                        }
                        *p++ = x;
                    }
                }
            }
            else
            {
                /* in this case we don't have to scan two corner triangles (of size len1/2)
                 * in the matrix because no best path can go throught them. note this
                 * breaks when len1 == len2 == 2 so the memchr() special case above is
                 * necessary */
                row[0] = l1 - half - 1;
                for (i = 1; i < l1; i++)
                {
                    int* p;
                    char char1 = s1[i - 1];
                    char* char2p;
                    int D, x;
                    /* skip the upper triangle */
                    if (i >= l1 - half)
                    {
                        int offset = i - (l1 - half);
                        int c3;

                        char2p = s2 + offset;
                        p = row + offset;
                        c3 = *p++ + (char1 != *char2p++ ? 1 : 0);
                        x = *p;
                        x++;
                        D = x;
                        if (x > c3)
                        {
                            x = c3;
                        }
                        *p++ = x;
                    }
                    else
                    {
                        p = row + 1;
                        char2p = s2;
                        D = x = i;
                    }
                    /* skip the lower triangle */
                    if (i <= half + 1)
                    {
                        end = row + l2 + i - half - 2;
                    }
                    /* main */
                    while (p <= end)
                    {
                        int c3 = --D + (char1 != *char2p++ ? 1 : 0);
                        x++;
                        if (x > c3)
                        {
                            x = c3;
                        }
                        D = *p;
                        D++;
                        if (x > D)
                        {
                            x = D;
                        }
                        *p++ = x;
                    }
                    /* lower triangle sentinel */
                    if (i <= half)
                    {
                        int c3 = --D + (char1 != *char2p ? 1 : 0);
                        x++;
                        if (x > c3)
                        {
                            x = c3;
                        }
                        *p = x;
                    }
                }
            }

            i = *end;
            return i;
        }


        public static ILogLineColumnizer FindColumnizerByName(string name, IList<ILogLineColumnizer> list)
        {
            foreach (ILogLineColumnizer columnizer in list)
            {
                if (columnizer.GetName().Equals(name))
                {
                    return columnizer;
                }
            }
            return null;
        }

        public static ILogLineColumnizer CloneColumnizer(ILogLineColumnizer columnizer)
        {
            if (columnizer == null)
            {
                return null;
            }
            ConstructorInfo cti = columnizer.GetType().GetConstructor(Type.EmptyTypes);
            if (cti != null)
            {
                object o = cti.Invoke(new object[] { });
                if (o is IColumnizerConfigurator)
                {
                    ((IColumnizerConfigurator)o).LoadConfig(ConfigManager.ConfigDir);
                }
                return (ILogLineColumnizer)o;
            }
            return null;
        }

        /// <summary>
        /// Returns true, if the given string is null or empty
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool IsNull(string toTest)
        {
            return toTest == null || toTest.Length == 0;
        }

        /// <summary>
        /// Returns true, if the given string is null or empty or contains only spaces
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool IsNullOrSpaces(string toTest)
        {
            return toTest == null || toTest.Trim().Length == 0;
        }

        [Conditional("DEBUG")]
        public static void AssertTrue(bool condition, string msg)
        {
            if (!condition)
            {
                MessageBox.Show("Assertion: " + msg);
                throw new Exception(msg);
            }
        }

        public string GetWordFromPos(int xPos, string text, Graphics g, Font font)
        {
            string[] words = text.Split(new char[] { ' ', '.', ':', ';' });
            int i = 0;
            int index = 0;
            List<CharacterRange> crList = new List<CharacterRange>();
            for (i = 0; i < words.Length; ++i)
            {
                crList.Add(new CharacterRange(index, words[i].Length));
                index += words[i].Length;
            }
            CharacterRange[] crArray = crList.ToArray();
            StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
            stringFormat.Trimming = StringTrimming.None;
            stringFormat.FormatFlags = StringFormatFlags.NoClip;
            stringFormat.SetMeasurableCharacterRanges(crArray);
            RectangleF rect = new RectangleF(0, 0, 3000, 20);
            Region[] stringRegions = g.MeasureCharacterRanges(text,
                font, rect, stringFormat);
            bool found = false;
            i = 0;
            foreach (Region regio in stringRegions)
            {
                if (regio.IsVisible(xPos, 3, g))
                {
                    found = true;
                    break;
                }
                i++;
            }
            if (found)
            {
                return words[i];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        private static bool TestFilterMatch(FilterParams filterParams, ILogLine line,
            ILogLineColumnizerCallback columnizerCallback)
        {
            string lowerSearchText;
            string searchText;
            Regex rex;
            if (filterParams.isInRange)
            {
                lowerSearchText = filterParams.lowerRangeSearchText;
                searchText = filterParams.rangeSearchText;
                rex = filterParams.rangeRex;
            }
            else
            {
                lowerSearchText = filterParams.lowerSearchText;
                searchText = filterParams.searchText;
                rex = filterParams.rex;
            }

            if (searchText == null || lowerSearchText == null || searchText.Length == 0)
            {
                return false;
            }

            if (filterParams.columnRestrict)
            {
                IColumnizedLogLine columns = filterParams.currentColumnizer.SplitLine(columnizerCallback, line);
                bool found = false;
                foreach (int colIndex in filterParams.columnList)
                {
                    if (colIndex < columns.ColumnValues.Length
                    ) // just to be sure, maybe the columnizer has changed anyhow
                    {
                        if (columns.ColumnValues[colIndex].FullValue.Trim().Length == 0)
                        {
                            if (filterParams.emptyColumnUsePrev)
                            {
                                string prevValue = (string)filterParams.lastNonEmptyCols[colIndex];
                                if (prevValue != null)
                                {
                                    if (TestMatchSub(filterParams, prevValue, lowerSearchText, searchText, rex,
                                        filterParams.exactColumnMatch))
                                    {
                                        found = true;
                                    }
                                }
                            }
                            else if (filterParams.emptyColumnHit)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            filterParams.lastNonEmptyCols[colIndex] = columns.ColumnValues[colIndex].FullValue;
                            if (TestMatchSub(filterParams, columns.ColumnValues[colIndex].FullValue, lowerSearchText,
                                searchText, rex,
                                filterParams.exactColumnMatch))
                            {
                                found = true;
                            }
                        }
                    }
                }
                return found;
            }
            else
            {
                return TestMatchSub(filterParams, line.FullLine, lowerSearchText, searchText, rex, false);
            }
        }

        // 
        private static bool TestMatchSub(FilterParams filterParams, string line, string lowerSearchText,
            string searchText, Regex rex, bool exactMatch)
        {
            if (filterParams.isRegex)
            {
                if (rex.IsMatch(line))
                {
                    return true;
                }
            }
            else
            {
                if (!filterParams.isCaseSensitive)
                {
                    if (exactMatch)
                    {
                        if (line.ToLower().Trim().Equals(lowerSearchText))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (line.ToLower().Contains(lowerSearchText))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (exactMatch)
                    {
                        if (line.Equals(searchText))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (line.Contains(searchText))
                        {
                            return true;
                        }
                    }
                }

                if (filterParams.fuzzyValue > 0)
                {
                    int range = line.Length - searchText.Length;
                    if (range > 0)
                    {
                        for (int i = 0; i < range; ++i)
                        {
                            string src = line.Substring(i, searchText.Length);
                            if (!filterParams.isCaseSensitive)
                            {
                                src = src.ToLower();
                            }
                            string dest = filterParams.isCaseSensitive ? searchText : lowerSearchText;
                            int dist = DamerauLevenshteinDistance(src, searchText);
                            if ((float)(searchText.Length + 1) / (float)(dist + 1) >=
                                11F / (float)(filterParams.fuzzyValue + 1F))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        private static unsafe int memchrRPLC(char* buffer, char c, int count)
        {
            char* p = buffer;
            char* e = buffer + count;
            while (p++ < e)
            {
                if (*p == c)
                {
                    return 1;
                }
            }
            return 0;
        }

        #endregion
    }
}