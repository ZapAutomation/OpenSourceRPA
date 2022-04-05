using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Binding = System.ServiceModel.Channels.Binding;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;

namespace Zappy.ZappyActions.Web.Rest
{
    public class DynamicProxyFactory
    {
        private IEnumerable<Binding> bindings;
        private CodeCompileUnit codeCompileUnit;
        private CodeDomProvider codeDomProvider;
        private IEnumerable<MetadataConversionError> codegenWarnings;
        private IEnumerable<CompilerError> compilerWarnings;
        private ServiceContractGenerator contractGenerator;
        private IEnumerable<ContractDescription> contracts;
        internal const string DefaultNamespace = "http://tempuri.org/";
        private ServiceEndpointCollection endpoints;
        private IEnumerable<MetadataConversionError> importWarnings;
        private Collection<MetadataSection> metadataCollection;
        private DynamicProxyFactoryOptions options;
        public Assembly proxyAssembly;
        private string proxyCode;
        private string wsdlUri;

        public DynamicProxyFactory(string wsdlUri) : this(wsdlUri, new DynamicProxyFactoryOptions())
        {
        }

        public DynamicProxyFactory(string wsdlUri, DynamicProxyFactoryOptions options)
        {
            if (wsdlUri == null)
            {
                throw new ArgumentNullException("wsdlUri");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            this.wsdlUri = wsdlUri;
            this.options = options;
            this.DownloadMetadata();
            this.ImportMetadata();
            this.CreateProxy();
            this.WriteCode();
            this.CompileProxy();
        }

        private void AddAssemblyReference(Assembly referencedAssembly, StringCollection refAssemblies)
        {
            string fullPath = Path.GetFullPath(referencedAssembly.Location);
            string fileName = Path.GetFileName(fullPath);
            if (!refAssemblies.Contains(fileName) && !refAssemblies.Contains(fullPath))
            {
                refAssemblies.Add(fullPath);
            }
        }

        private void AddDocumentToResults(object document, Collection<MetadataSection> results)
        {
            ServiceDescription serviceDescription = document as ServiceDescription;
            XmlSchema schema = document as XmlSchema;
            XmlElement policy = document as XmlElement;
            if (serviceDescription != null)
            {
                results.Add(MetadataSection.CreateFromServiceDescription(serviceDescription));
            }
            else if (schema != null)
            {
                results.Add(MetadataSection.CreateFromSchema(schema));
            }
            else if ((policy != null) && (policy.LocalName == "Policy"))
            {
                results.Add(MetadataSection.CreateFromPolicy(policy, null));
            }
            else
            {
                MetadataSection item = new MetadataSection
                {
                    Metadata = document
                };
                results.Add(item);
            }
        }

        private void AddStateForDataContractSerializerImport(WsdlImporter importer)
        {
            XsdDataContractImporter importer2 = new XsdDataContractImporter(this.codeCompileUnit)
            {
                Options = new ImportOptions()
            };
            importer2.Options.ImportXmlType = this.options.FormatMode == DynamicProxyFactoryOptions.FormatModeOptions.DataContractSerializer;
            importer2.Options.CodeProvider = this.codeDomProvider;
            importer.State.Add(typeof(XsdDataContractImporter), importer2);
            foreach (DataContractSerializerMessageContractImporter importer3 in importer.WsdlImportExtensions)
            {
                if (importer3 != null)
                {
                    importer3.Enabled = this.options.FormatMode != DynamicProxyFactoryOptions.FormatModeOptions.XmlSerializer;
                }
            }
        }

        private void AddStateForXmlSerializerImport(WsdlImporter importer)
        {
            XmlSerializerImportOptions options = new XmlSerializerImportOptions(this.codeCompileUnit)
            {
                CodeProvider = this.codeDomProvider,
                WebReferenceOptions = new WebReferenceOptions()
            };
            options.WebReferenceOptions.CodeGenerationOptions = CodeGenerationOptions.GenerateOrder | CodeGenerationOptions.GenerateProperties;
            options.WebReferenceOptions.SchemaImporterExtensions.Add(typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);
            importer.State.Add(typeof(XmlSerializerImportOptions), options);
        }

        private void CompileProxy()
        {
            CompilerParameters options = new CompilerParameters();
            this.AddAssemblyReference(typeof(ServiceContractAttribute).Assembly, options.ReferencedAssemblies);
            this.AddAssemblyReference(typeof(ServiceDescription).Assembly, options.ReferencedAssemblies);
            this.AddAssemblyReference(typeof(DataContractAttribute).Assembly, options.ReferencedAssemblies);
            this.AddAssemblyReference(typeof(XmlElement).Assembly, options.ReferencedAssemblies);
            this.AddAssemblyReference(typeof(Uri).Assembly, options.ReferencedAssemblies);
            this.AddAssemblyReference(typeof(DataSet).Assembly, options.ReferencedAssemblies);
            string[] sources = new string[] { this.proxyCode };
            CompilerResults results = this.codeDomProvider.CompileAssemblyFromSource(options, sources);
            if ((results.Errors != null) && results.Errors.HasErrors)
            {
                ProxyException exception1 = new ProxyException("There was an error in compiling the proxy code.")
                {
                    CompilationErrors = ToEnumerable(results.Errors)
                };
                throw exception1;
            }
            this.compilerWarnings = ToEnumerable(results.Errors);
            this.proxyAssembly = Assembly.LoadFile(results.PathToAssembly);
        }

        private bool ContractNameMatch(ContractDescription cDesc, string name) =>
            (string.Compare(cDesc.Name, name, true) == 0);

        private bool ContractNsMatch(ContractDescription cDesc, string ns)
        {
            if (ns != null)
            {
                return (string.Compare(cDesc.Namespace, ns, true) == 0);
            }
            return true;
        }

        private void CreateCodeDomProvider()
        {
            this.codeDomProvider = CodeDomProvider.CreateProvider(this.options.Language.ToString());
        }

        private void CreateProxy()
        {
            this.CreateServiceContractGenerator();
            foreach (ContractDescription description in this.contracts)
            {
                this.contractGenerator.GenerateServiceContractType(description);
            }
            bool flag = true;
            this.codegenWarnings = this.contractGenerator.Errors;
            if (this.codegenWarnings != null)
            {
                using (IEnumerator<MetadataConversionError> enumerator2 = this.codegenWarnings.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        if (!enumerator2.Current.IsWarning)
                        {
                            flag = false;
                            goto Label_008A;
                        }
                    }
                }
            }
        Label_008A:
            if (!flag)
            {
                ProxyException exception1 = new ProxyException("There was an error in generating the proxy code.")
                {
                    CodeGenerationErrors = this.codegenWarnings
                };
                throw exception1;
            }
        }

        public DynamicProxy CreateProxy(ServiceEndpoint endpoint)
        {
            Type contractType = this.GetContractType(endpoint.Contract.Name, endpoint.Contract.Namespace);
            return new DynamicProxy(this.GetProxyType(contractType), endpoint.Binding, endpoint.Address);
        }

        public DynamicProxy CreateProxy(string contractName) =>
            this.CreateProxy(contractName, null);

        public DynamicProxy CreateProxy(string contractName, string contractNamespace)
        {
            ServiceEndpoint endpoint = this.GetEndpoint(contractName, contractNamespace);
            return this.CreateProxy(endpoint);
        }

        private void CreateServiceContractGenerator()
        {
            this.contractGenerator = new ServiceContractGenerator(this.codeCompileUnit);
            this.contractGenerator.Options |= ServiceContractGenerationOptions.ClientClass;
        }

        private void DownloadMetadata()
        {
            new EndpointAddress(this.wsdlUri);
            DiscoveryClientProtocol protocol1 = new DiscoveryClientProtocol
            {
                Credentials = CredentialCache.DefaultNetworkCredentials,
                AllowAutoRedirect = true
            };
            protocol1.DiscoverAny(this.wsdlUri);
            protocol1.ResolveAll();
            Collection<MetadataSection> results = new Collection<MetadataSection>();
            foreach (object obj2 in protocol1.Documents.Values)
            {
                this.AddDocumentToResults(obj2, results);
            }
            this.metadataCollection = results;
        }

        internal static XmlQualifiedName GetContractName(Type contractType, string name, string ns)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = contractType.Name;
            }
            if (ns == null)
            {
                ns = "http://tempuri.org/";
            }
            else
            {
                ns = Uri.EscapeUriString(ns);
            }
            return new XmlQualifiedName(name, ns);
        }

        private Type GetContractType(string contractName, string contractNamespace)
        {
            ServiceContractAttribute attribute = null;
            Type type = null;
            foreach (Type type2 in this.proxyAssembly.GetTypes())
            {
                if (type2.IsInterface)
                {
                    object[] customAttributes = type2.GetCustomAttributes(typeof(ServiceContractAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length != 0))
                    {
                        attribute = (ServiceContractAttribute)customAttributes[0];
                        XmlQualifiedName name = GetContractName(type2, attribute.Name, attribute.Namespace);
                        if ((string.Compare(name.Name, contractName, true) == 0) && (string.Compare(name.Namespace, contractNamespace, true) == 0))
                        {
                            type = type2;
                            break;
                        }
                    }
                }
            }
            if (type == null)
            {
                throw new ArgumentException("The specified contract is not found in the proxy assembly.");
            }
            return type;
        }

        public ServiceEndpoint GetEndpoint(string contractName) =>
            this.GetEndpoint(contractName, null);

        public ServiceEndpoint GetEndpoint(string contractName, string contractNamespace)
        {
            ServiceEndpoint endpoint = null;
            foreach (ServiceEndpoint endpoint2 in this.Endpoints)
            {
                if (this.ContractNameMatch(endpoint2.Contract, contractName) && this.ContractNsMatch(endpoint2.Contract, contractNamespace))
                {
                    endpoint = endpoint2;
                    break;
                }
            }
            if (endpoint == null)
            {
                throw new ArgumentException(string.Format("The endpoint associated with contract {1}:{0} is not found.", contractName, contractNamespace));
            }
            return endpoint;
        }

        private Type GetProxyType(Type contractType)
        {
            Type[] typeArguments = new Type[] { contractType };
            Type c = typeof(ClientBase<>).MakeGenericType(typeArguments);
            Type type2 = null;
            foreach (Type type3 in this.ProxyAssembly.GetTypes())
            {
                if ((type3.IsClass && contractType.IsAssignableFrom(type3)) && type3.IsSubclassOf(c))
                {
                    type2 = type3;
                    break;
                }
            }
            if (type2 == null)
            {
                throw new ProxyException($"The proxy that implements the service contract {contractType.FullName} is not found.");
            }
            return type2;
        }

        public bool HasEndpoint(string contractName, string contractNamespace = null)
        {
            foreach (ServiceEndpoint endpoint in this.Endpoints)
            {
                if (this.ContractNameMatch(endpoint.Contract, contractName) && this.ContractNsMatch(endpoint.Contract, contractNamespace))
                {
                    return true;
                }
            }
            return false;
        }

        private void ImportMetadata()
        {
            this.codeCompileUnit = new CodeCompileUnit();
            this.CreateCodeDomProvider();
            WsdlImporter importer = new WsdlImporter(new MetadataSet(this.metadataCollection));
            this.AddStateForDataContractSerializerImport(importer);
            this.AddStateForXmlSerializerImport(importer);
            this.bindings = importer.ImportAllBindings();
            this.contracts = importer.ImportAllContracts();
            this.endpoints = importer.ImportAllEndpoints();
            this.importWarnings = importer.Errors;
        }

        private static IEnumerable<CompilerError> ToEnumerable(CompilerErrorCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            List<CompilerError> list = new List<CompilerError>();
            foreach (CompilerError error in collection)
            {
                list.Add(error);
            }
            return list;
        }

        public static string ToString(IEnumerable<CompilerError> compilerErrors)
        {
            if (compilerErrors == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            foreach (CompilerError error in compilerErrors)
            {
                builder.AppendLine(error.ToString());
            }
            return builder.ToString();
        }

        public static string ToString(IEnumerable<MetadataConversionError> importErrors)
        {
            if (importErrors == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            foreach (MetadataConversionError error in importErrors)
            {
                if (error.IsWarning)
                {
                    builder.AppendLine("Warning : " + error.Message);
                }
                else
                {
                    builder.AppendLine("Error : " + error.Message);
                }
            }
            return builder.ToString();
        }

        private void WriteCode()
        {
            using (StringWriter writer = new StringWriter())
            {
                CodeGeneratorOptions options = new CodeGeneratorOptions
                {
                    BracingStyle = "C"
                };
                this.codeDomProvider.GenerateCodeFromCompileUnit(this.codeCompileUnit, writer, options);
                writer.Flush();
                this.proxyCode = writer.ToString();
            }
            if (this.options.CodeModifier != null)
            {
                this.proxyCode = this.options.CodeModifier(this.proxyCode);
            }
        }

        public IEnumerable<Binding> Bindings =>
            this.bindings;

        public IEnumerable<MetadataConversionError> CodeGenerationWarnings =>
            this.codegenWarnings;

        public IEnumerable<CompilerError> CompilationWarnings =>
            this.compilerWarnings;

        public IEnumerable<ContractDescription> Contracts =>
            this.contracts;

        public IEnumerable<ServiceEndpoint> Endpoints =>
            this.endpoints;

        public IEnumerable<MetadataSection> Metadata =>
            this.metadataCollection;

        public IEnumerable<MetadataConversionError> MetadataImportWarnings =>
            this.importWarnings;

        public Assembly ProxyAssembly =>
            this.proxyAssembly;

        public string ProxyCode =>
            this.proxyCode;
    }
}

