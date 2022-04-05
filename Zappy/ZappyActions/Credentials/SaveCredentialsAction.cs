using CredentialManagement;
using System.ComponentModel;
using Zappy.InputData;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;

namespace Zappy.ZappyActions.Credentials
{
    [Description("SaveCredentials")]
    public class SaveCredentialsAction : TemplateAction
    {
        public SaveCredentialsAction()
        {
            CredentialType = CredentialType.Generic;
            PersistanceType = PersistanceType.LocalComputer;
        }

        bool _IsSaved;

        [Category("Input")]
        public DynamicTextProperty CredentialName { get; set; }

        [Category("Input")]
        public string Username { get; set; }

        [Category("Input")]
        public string Password { get; set; }

        [Category("Input")]
        public CredentialType CredentialType { get; set; }

        [Category("Input")]
        public PersistanceType PersistanceType { get; set; }

        [Category("Output")]
        public bool Saved { get { return _IsSaved; } set {; } }

                                                public override void Invoke(IZappyExecutionContext context, ZappyTaskActionInvoker actionInvoker)
        {
                        try
            {
                using (var cred = new Credential())
                {
                    cred.Password = Password;
                    cred.Username = Username;
                    cred.Target = CredentialName;
                    cred.Type = this.CredentialType;
                    cred.PersistanceType = this.PersistanceType;
                    _IsSaved = cred.Save();
                }
            }
            catch
            {
                _IsSaved = false;
                throw;
            }
        }

        public override string AuditInfo()
        {
            return base.AuditInfo() + " CredentialName:" + this.CredentialName + " Username:" + this.Username;
        }
    }

}
