using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Zappy.Helpers;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using static Zappy.Plugins.ChromeBrowser.Chrome.ChromeAction;

namespace Zappy.Plugins.ChromeBrowser.Chrome
{
    public class ChromeAction_extractTableFromWebPage : TemplateAction
    {

        public DynamicProperty<DataTable> DataTable { get; set; }

        public ChromeAction_extractTableFromWebPage()
        {
        }

        [Category("Input")]
        [Description("CSV file name where user store")]
        public DynamicTextProperty CsvFilePath { get; set; }

        [Category("Input")]
        [Description("Extract Table from Web Page")]
        [Editor(typeof(HtmlTablePickerEditor), typeof(UITypeEditor))]
        public string ExtractTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        ChromeAction sendKeysAction = new ChromeAction();
            sendKeysAction.CommandName = "sendKeys";
            sendKeysAction.CommandValue = "${Copy}";
            List<string> extractTblList = new List<string>();
            extractTblList.Add(ExtractTable);
            sendKeysAction.CommandTarget = extractTblList;
            sendKeysAction.TargetToSend = ChromeTarget.Xpath;
            sendKeysAction.Invoke(context, actionInvoker);
            string ClipboardData = Clipboard.GetText();

                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(ClipboardData);
            
            List<List<string>> table = new List<List<string>>();
                                    table = doc.DocumentNode
                       .Descendants("tr")
                       .Skip(1)
                       .Where(tr => tr.Elements("td").Count() > 1)
                       .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                       .ToList();


            DataTable dt = new DataTable();
            table.First().ForEach(colname => dt.Columns.Add(colname));
                                                                                                            
            foreach (List<string> row in table)
                dt.Rows.Add(row.ToArray());
            
            this.DataTable = dt;

                        string DataTableInCSVString = WriteDataTableToCsv(DataTable);

            if (string.IsNullOrEmpty(CsvFilePath))
                CsvFilePath = Path.Combine(CrapyConstants.TempFolder, "temp.csv");
            string __FileName = CsvFilePath;
            string _DirPath = Path.GetDirectoryName(__FileName);

            if (!Directory.Exists(_DirPath))
                Directory.CreateDirectory(_DirPath);

            if (DataTable.Value != null && !string.IsNullOrEmpty(__FileName) && DataTable.Value.Columns.Count > 0)
            {
                File.WriteAllText(__FileName, DataTableInCSVString);

            }
        }
        string WriteDataTableToCsv(DataTable dtDataTable)
        {
            StringBuilder sb = new StringBuilder();
                        {                  int _ColumnCount = dtDataTable.Columns.Count;
                int __ColumnCountMinus_1 = _ColumnCount - 1;

                                                                                                                                                                

                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < __ColumnCountMinus_1; i++)
                    {
                        WriteColumnValue(sb, dr, i);
                        sb.Append(",");
                    }
                                        WriteColumnValue(sb, dr, __ColumnCountMinus_1);
                    sb.Append(Environment.NewLine);
                }
            }
            return sb.ToString();
        }

                                                        void WriteColumnValue(StringBuilder sb, DataRow dr, int ColumnIndex)
        {
            if (!Convert.IsDBNull(dr[ColumnIndex]))
            {
                string value = dr[ColumnIndex].ToString();
                if (value.Contains(','))
                    value = string.Format("\"{0}\"", value);
                sb.Append(value);
            }
        }
    }
}
