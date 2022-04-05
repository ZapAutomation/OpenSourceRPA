using CredentialManagement;
using System.ComponentModel;
using System.Security;
using System.Xml.Serialization;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Credentials
{
    [Description(" Get Credentials")]
    public class GetCredentialsAction : TemplateAction
    {
        public GetCredentialsAction() : base()
        {
            CredentialType = CredentialType.Generic;
            PersistanceType = PersistanceType.LocalComputer;
        }
        bool _IsValid;

        [Category("Input")]
        [Description("The username to be used with the credentials")]
        public DynamicTextProperty CredentialName { get; set; }

        [Category("Input")]
        [Description("Credential Types: The type of credential to be added")]
        public CredentialType CredentialType { get; set; }

        [Category("Input")]
        [Description("Defines the rules according to which the given credentials are stored")]
        public PersistanceType PersistanceType { get; set; }

        [Category("Output")]
        [Description("If password is match with username can return true otherwise false")]
        public bool IsValid { get { return _IsValid; } set {; } }

        [Category("Output")]
        [Description("Provided user name")]
        [XmlIgnore] public string Username { get; set; }

        [Category("Output")]
        [Description("Represents text that should be kept confidential, such as by deleting it from computer memory when no longer needed")]
        [XmlIgnore, Newtonsoft.Json.JsonIgnore] public SecureString SecurePassword { get; set; }

        [Category("Output")]
        [Description("Password is in encrypted format")]
        [XmlIgnore, Newtonsoft.Json.JsonIgnore] public string Password { get; set; }

                                                        public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        try
            {
                using (var cred = new Credential())
                {
                    cred.Target = CredentialName;
                    cred.PersistanceType = this.PersistanceType;
                    cred.Type = this.CredentialType;
                    _IsValid = cred.Load();
                    if (_IsValid)
                    {
                        this.Username = cred.Username;
                        SecurePassword = cred.SecurePassword;
                        Password = cred.Password;
                    }
                }
            }
            catch
            {
                _IsValid = false;
                throw;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " CredentialName:" + this.CredentialName + " Username:" + this.Username;
        }
    }

}
