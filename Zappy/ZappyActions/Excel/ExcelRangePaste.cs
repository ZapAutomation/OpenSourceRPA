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
    public class ExcelRangePaste : TemplateAction
    {
        public ExcelRangePaste()
        {
            ExcelPropertyType = PropertyNames.PropertyNameEnum.Value;
        }

        [Category("Input")]
        [Description("Start row index of excel")]
        public DynamicProperty<int> StartRowIndex { get; set; }

        [Category("Input")]
        [Description("Start column index of excel")]
        public DynamicProperty<int> StartColumnIndex { get; set; }

        [Category("Input")]
        [Description("Sheet name of excel")]
        public DynamicProperty<string> SheetName { get; set; }

        [Category("Input")]
        [Description("WorkbookName")]
        public DynamicProperty<string> WorkbookName { get; set; }



        [Category("Input")]
        [Description("DataTable")]
        public DynamicProperty<DataTable> DataTable { get; set; }

        [Category("Input")]
        [Description("Sets excel property type")]
        public PropertyNames.PropertyNameEnum ExcelPropertyType { get; set; }


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            PasteRange(DataTable, SheetName, WorkbookName);
        }
        private void PasteRange(DataTable dataTable, string sheetName, string Path)
        {
                                                
            try
            {

                
                                                
                                
                
                int i = StartRowIndex;
                string data = string.Empty;
                ExcelCellInfo cellInfo = null;

                foreach (DataRow row in dataTable.Rows)
                {
                    int j = StartColumnIndex;
                    for (int k = 0; k < dataTable.Columns.Count; k++)
                    {
                        cellInfo = ExcelHelper.CreateExcelCellInfo(i, j, sheetName, Path);
                        data = row[k].ToString();
                                                ExcelCommunicator.Instance.SetCellProperty(cellInfo, ExcelPropertyType.ToString(), data);
                        j++;
                    }
                    i++;
                }

                                                                                                                                

                                
                
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
