using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Excel
{
                [Description("Copied Excel Cells To DataTable")]
    public class CopiedExcelCellsToDataTable : TemplateAction
    {
                                [Category("Input")]
        [Description("Check First Row Has ColumnNames")]
        public DynamicProperty<bool> FirstRowHasColumnNames { get; set; }

                                [Category("Output")]
        [Description("Gets DataTable from excel")]
        public DataTable Table { get; set; }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            Table = clipboardExcelToDataTable(FirstRowHasColumnNames);
        }


                                                                private XDocument fwToXDocument(XmlDocument xmlDocument)
        {
            using (XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument))
            {
                xmlNodeReader.MoveToContent();
                return XDocument.Load(xmlNodeReader);
            }
        }

                                                        private DataTable clipboardExcelToDataTable(bool blnFirstRowHasHeader = false)
        {
            var clipboard = Clipboard.GetDataObject();
            if (!clipboard.GetDataPresent("XML Spreadsheet")) return null;

            StreamReader streamReader = new StreamReader((MemoryStream)clipboard.GetData("XML Spreadsheet"));
            streamReader.BaseStream.SetLength(streamReader.BaseStream.Length - 1);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(streamReader.ReadToEnd());

            XNamespace ssNs = "urn:schemas-microsoft-com:office:spreadsheet";
            DataTable dtData = new DataTable();

            var linqRows = fwToXDocument(xmlDocument).Descendants(ssNs + "Row").ToList<XElement>();

            for (int x = 0; x < linqRows.Max(a => a.Descendants(ssNs + "Cell").Count()); x++)
                dtData.Columns.Add("Column " + (x + 1).ToString());

            int intCol = 0;

            DataRow drCurrent;

            linqRows.ForEach(rowElement =>
            {
                intCol = 0;
                drCurrent = dtData.Rows.Add();
                rowElement.Descendants(ssNs + "Cell")
                    .ToList<XElement>()
                    .ForEach(cell => drCurrent[intCol++] = cell.Value);
            });

            if (blnFirstRowHasHeader)
            {
                int x = 0;
                foreach (DataColumn dcCurrent in dtData.Columns)
                    dcCurrent.ColumnName = dtData.Rows[0][x++].ToString();

                dtData.Rows.RemoveAt(0);
            }

            return dtData;
        }
    }
}
