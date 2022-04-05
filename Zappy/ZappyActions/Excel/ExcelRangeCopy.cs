using System;
using System.ComponentModel;
using System.Data;
using Zappy.Decode.LogManager;
using Zappy.Plugins.Excel;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Excel.Helper;
using ZappyMessages.ExcelMessages;

namespace Zappy.ZappyActions.Excel
{
    public class ExcelRangeCopy : TemplateAction
    {
        [Category("Input")]
        [Description("Start row index of excel")]
        public DynamicProperty<int> StartRowIndex { get; set; }

        [Category("Input")]
        [Description("Start column index of excel")]
        public DynamicProperty<int> StartColumnIndex { get; set; }

        [Category("Input")]
        [Description("End row index of excel")]
        public DynamicProperty<int> EndRowIndex { get; set; }

        [Category("Input")]
        [Description("End row index of excel")]
        public DynamicProperty<int> EndColumnIndex { get; set; }

        [Category("Input")]
        [Description("ExSheet name of excel")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName name")]
        public DynamicProperty<string> WorkbookName { get; set; }

        [Category("Output")]
        [Description("Gets DataTable from the excel file")]
        public DataTable DataTable { get; set; }

        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            DataTable = CopyRange(SheetName, WorkbookName);
        }
        private DataTable CopyRange(string sheetName, string Path)
        {
                                                
            try
            {
                

                
                                                
                
                DataTable dt = new DataTable();

                ExcelCellInfo cellInfo = null;
                int ColumnIndexNo = 1;

                for (int i = StartColumnIndex; i <= EndColumnIndex; i++)
                {
                                                            dt.Columns.Add("Column"+ ColumnIndexNo++);
                }
                DataRow dr = null;
                int k = 0;
                for (int i = StartRowIndex; i <= EndRowIndex; i++)
                {
                    k = StartColumnIndex;
                    dr = dt.NewRow();

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        cellInfo = ExcelHelper.CreateExcelCellInfo(i, k, SheetName, WorkbookName);
                        string data = ExcelCommunicator.Instance.GetCellProperty(cellInfo, PropertyNames.PropertyNameEnum.Text.ToString()).ToString();
                        dr[j] = data;
                        k++;
                    }

                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);

                throw new Exception(ex.Message);

                            }
            finally
            {
                                                            }
        }
        public override string AuditInfo()
        {
            return base.AuditInfo() + " SheetName:" + this.SheetName + " Excel File location:" + this.WorkbookName + " Output" + this.DataTable;
        }
    }
}
