using System.Text;

namespace Zappy.ZappyActions.Web.Rest
{
    public class DynamicProxyFactoryOptions
    {
        private ProxyCodeModifier codeModifier = null;
        private LanguageOptions lang = LanguageOptions.CS;
        private FormatModeOptions mode = FormatModeOptions.Auto;

        public override string ToString()
        {
            StringBuilder builder1 = new StringBuilder();
            builder1.Append("DynamicProxyFactoryOptions[");
            builder1.Append("Language=" + this.Language);
            builder1.Append(",FormatMode=" + this.FormatMode);
            builder1.Append(",CodeModifier=" + this.CodeModifier);
            builder1.Append("]");
            return builder1.ToString();
        }

        public ProxyCodeModifier CodeModifier
        {
            get =>
                this.codeModifier;
            set
            {
                this.codeModifier = value;
            }
        }

        public FormatModeOptions FormatMode
        {
            get =>
                this.mode;
            set
            {
                this.mode = value;
            }
        }

        public LanguageOptions Language
        {
            get =>
                this.lang;
            set
            {
                this.lang = value;
            }
        }

        public enum FormatModeOptions
        {
            Auto,
            XmlSerializer,
            DataContractSerializer
        }

        public enum LanguageOptions
        {
            CS,
            VB
        }
    }
}

