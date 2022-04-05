using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.ExecuteTask.Extension;
using Zappy.Plugins.Excel;
using Zappy.Plugins.Uia.Uia;
using Zappy.Properties;

namespace Zappy.Decode.Helper
{
    internal class ZappyTaskExtensionPackageManager : IZappyTaskExtensionPackageManager, IDisposable
    {
        private readonly IList<ZappyTaskExtensionPackage> extensionPackageList;
        private readonly object lockObject = new object();
        private static string m_defaultPluginDirectoryPath;
        private static IZappyTaskExtensionPackageManager s_Instance;

        protected ZappyTaskExtensionPackageManager()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                extensionPackageList = new List<ZappyTaskExtensionPackage>();
                extensionPackageList.Add(new UiaExtensionPackage());
                                                extensionPackageList.Add(new ExcelExtensionPackage());
                                
                                
                extensionPackageList.Add(new MsaaExtensionPackage());
                extensionPackageList.Add(new ZappyTaskingExtensionPackage());


            }
        }

        protected static void AddFilePathsToDictionary(Dictionary<string, string> dictionary, IEnumerable<string> filePaths)
        {
            if (filePaths != null)
            {
                foreach (string str in filePaths)
                {
                    if (!string.IsNullOrEmpty(str) && !dictionary.ContainsKey(str))
                    {
                        dictionary.Add(str, str);
                    }
                }
            }
        }

        protected static string CombineMultiplePaths(string firstPath, params string[] paths)
        {
            string str = firstPath;
            foreach (string str2 in paths)
            {
                str = Path.Combine(str, str2);
            }
            return str;
        }

        public void Dispose()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                foreach (ZappyTaskExtensionPackage package in extensionPackageList)
                {
                    package.Dispose();
                }
                                GC.SuppressFinalize(this);
            }
        }

        protected static bool ExceptionHandled(Exception exception) =>
            exception is FileNotFoundException || exception is BadImageFormatException || exception is SecurityException || exception is ArgumentException || (exception is NotSupportedException || exception is MemberAccessException) || exception is TargetInvocationException || exception is FileLoadException || (exception is MissingMethodException || exception is MethodAccessException) || exception is PathTooLongException || exception is TypeLoadException;

                public IList<T> GetExtensions<T>() where T : class
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return GetExtensionsInternal<T>();
            }
        }

        protected IList<T> GetExtensionsInternal<T>() where T : class
        {
            IList<T> list = new List<T>();
            object[] args = { typeof(T) };
            
            foreach (ZappyTaskExtensionPackage package in extensionPackageList)
            {
                try
                {
                    object service = package.GetService(typeof(T));
                    T item = service as T;
                    if (item != null)
                    {
                        if (!list.Contains(item))
                        {
                            object[] objArray2 = { package.PackageName, typeof(T), item.GetType() };
                            
                            list.Add(item);
                        }
                    }
                    else
                    {
                        T[] localArray = service as T[];
                        if (localArray != null)
                        {
                            foreach (T local2 in localArray)
                            {
                                if (!list.Contains(local2))
                                {
                                    object[] objArray3 = { package.PackageName, typeof(T), local2.GetType() };
                                    
                                    list.Add(local2);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (!ExceptionHandled(exception))
                    {
                        throw;
                    }
                    object[] objArray4 = { package.PackageName, ZappyTaskUtilities.GetTypeAssembly(package.GetType()).FullName, exception.Message };
                    CrapyLogger.log.ErrorFormat("ExtensionFramework : Error getting requested extension type : ExtensionPackageName {0}, {1}, Detailed Info {2}", objArray4);
                    object[] objArray5 = { package.PackageName, package.PackageVersion, package.PackageVendor, ZappyTaskUtilities.GetTypeAssembly(package.GetType()).FullName, exception };
                    throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.ExtensionPackageLoadFailure, objArray5));
                }
            }
            return list;
        }

                
                                                        
                                
                                                                                
                                




        
        
                                
                                                                
        public static IZappyTaskExtensionPackageManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ZappyTaskExtensionPackageManager();
                }
                return s_Instance;
            }
        }
    }

}