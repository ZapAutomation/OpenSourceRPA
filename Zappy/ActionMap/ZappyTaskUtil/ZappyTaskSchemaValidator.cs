using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal static class ZappyTaskSchemaValidator
    {
        internal static EventHandler<ValidationResult> OnSchemaValidationCompleted;

        private static void ZappyTaskSchemaValidationHandler(object sender, ValidationEventArgs e)
        {
            if (OnSchemaValidationCompleted != null)
            {
                ValidationResult result = new ValidationResult
                {
                    Severity = (int)e.Severity
                };
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        result.Message = e.Message;
                        result.Exception = e.Exception;
                        break;
                }
                OnSchemaValidationCompleted(sender, result);
            }
        }

        public static void ValidateZappyTaskFileStream(Stream uitaskStream, Stream xsdStream)
        {
            XmlTextReader reader = new XmlTextReader(xsdStream)
            {
                DtdProcessing = DtdProcessing.Prohibit
            };
            XmlSchema schema = XmlSchema.Read(reader, null);
            XmlDocument document = new XmlDocument();
            document.Schemas.Add(schema);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit
            };
            using (XmlReader reader2 = XmlReader.Create(uitaskStream, settings))
            {
                document.Load(reader2);
            }
            document.Validate(ZappyTaskSchemaValidationHandler);
        }
    }
}