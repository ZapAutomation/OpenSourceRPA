using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Text;

namespace Zappy.ZappyActions.Web.Rest
{
    [Serializable]
    public class ProxyException : ApplicationException
    {
        private IEnumerable<MetadataConversionError> CodegenErrors;
        private IEnumerable<CompilerError> CompilerErrors;
        private IEnumerable<MetadataConversionError> ImportErrors;

        public ProxyException(string message) : base(message)
        {
        }

        public ProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.ToString());
            if (this.MetadataImportErrors != null)
            {
                builder.AppendLine("Metadata Import Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(this.MetadataImportErrors));
            }
            if (this.CodeGenerationErrors != null)
            {
                builder.AppendLine("Code Generation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(this.CodeGenerationErrors));
            }
            if (this.CompilationErrors != null)
            {
                builder.AppendLine("Compilation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(this.CompilationErrors));
            }
            return builder.ToString();
        }

        public IEnumerable<MetadataConversionError> CodeGenerationErrors
        {
            get =>
                this.CodegenErrors;
            internal set
            {
                this.CodegenErrors = value;
            }
        }

        public IEnumerable<CompilerError> CompilationErrors
        {
            get =>
                this.CompilerErrors;
            internal set
            {
                this.CompilerErrors = value;
            }
        }

        public IEnumerable<MetadataConversionError> MetadataImportErrors
        {
            get =>
                this.ImportErrors;
            internal set
            {
                this.ImportErrors = value;
            }
        }
    }
}

