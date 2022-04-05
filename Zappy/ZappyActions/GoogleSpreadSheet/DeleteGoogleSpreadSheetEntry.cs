using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.ComponentModel;
using System.IO;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.GoogleSpreadSheet
{
    [Description("Delete the spreadsheet entry")]
    public class DeleteGoogleSpreadSheetEntry : TemplateAction
    {

        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        [Category("Input")]
        [Description("Id of created Google SpreadSheet")]
        public DynamicProperty<string> SpreadsheetId { get; set; }

        [Category("Input")]
        [Description("Google SpreadSheet name")]
        public DynamicProperty<string> SheetName { get; set; }

                                                [Category("Input")]
        [Description("Range with row and column as ")]
        public DynamicProperty<string> Range { get; set; }

        [Category("Input")]
        [Description("Json file path of the created spredsheet")]
        public string JsonFilePathofSheet { get; set; }

        public static GoogleCredential HttpClientInitializer;
        private static SheetsService service;
        public string ApplicationName;


        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(JsonFilePathofSheet, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

                        service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            DeleteEntry();
        }

        void DeleteEntry()
        {
            var range = Range;
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetId, range);
            var deleteReponse = deleteRequest.Execute();
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " SpreadsheetId:" + this.SpreadsheetId + " SheetName:" + this.SheetName + " Range:" + this.Range + " JsonFilePathofSheet:" + this.JsonFilePathofSheet;
        }

    }
}
