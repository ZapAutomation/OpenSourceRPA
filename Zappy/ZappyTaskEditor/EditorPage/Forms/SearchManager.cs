using ScintillaNET;
using System.Windows.Forms;

namespace Zappy.ZappyTaskEditor.EditorPage.Forms
{
    internal class SearchManager
    {

        public static Scintilla TextArea;
        public static TextBox SearchBox;

        public static string LastSearch = "";

        public static int LastSearchIndex;

        public static void Find(bool next, bool incremental)
        {
            bool first = LastSearch != SearchBox.Text;

            LastSearch = SearchBox.Text;
            if (LastSearch.Length > 0)
            {

                if (next)
                {

                    
                                        TextArea.TargetStart = LastSearchIndex - 1;
                    TextArea.TargetEnd = LastSearchIndex + (LastSearch.Length + 1);
                    TextArea.SearchFlags = SearchFlags.None;

                                        if (!incremental || TextArea.SearchInTarget(LastSearch) == -1)
                    {

                                                TextArea.TargetStart = TextArea.CurrentPosition;
                        TextArea.TargetEnd = TextArea.TextLength;
                        TextArea.SearchFlags = SearchFlags.None;

                                                if (TextArea.SearchInTarget(LastSearch) == -1)
                        {

                                                        TextArea.TargetStart = 0;
                            TextArea.TargetEnd = TextArea.TextLength;

                                                        if (TextArea.SearchInTarget(LastSearch) == -1)
                            {

                                                                TextArea.ClearSelections();
                                return;
                            }
                        }

                    }

                }
                else
                {

                    
                                        TextArea.TargetStart = 0;
                    TextArea.TargetEnd = TextArea.CurrentPosition;
                    TextArea.SearchFlags = SearchFlags.None;

                                        if (TextArea.SearchInTarget(LastSearch) == -1)
                    {

                                                TextArea.TargetStart = TextArea.CurrentPosition;
                        TextArea.TargetEnd = TextArea.TextLength;

                                                if (TextArea.SearchInTarget(LastSearch) == -1)
                        {

                                                        TextArea.ClearSelections();
                            return;
                        }
                    }

                }

                                LastSearchIndex = TextArea.TargetStart;
                TextArea.SetSelection(TextArea.TargetEnd, TextArea.TargetStart);
                TextArea.ScrollCaret();

            }

            SearchBox.Focus();
        }


    }
}