using System;
using System.Collections.Generic;

namespace Zappy.Decode.Helper
{
    internal abstract class ZappyTaskExtensionPackageBase : ZappyTaskExtensionPackage
    {
        private readonly string description;
        private readonly string name;
        private readonly Dictionary<Type, object> supportedServices;
        private readonly string vendor;
        private readonly Version version;
        private readonly Version vsVersion;

        protected ZappyTaskExtensionPackageBase(string extensionName, string extensionDescription) : this(extensionName, extensionDescription, new Version(1, 0), "Crapy", new Version(10, 0))
        {
        }

        protected ZappyTaskExtensionPackageBase(string extensionName, string extensionDescription, Version extensionVersion, string vendorName, Version vsVersionSupported)
        {
            name = extensionName;
            description = extensionDescription;
            version = extensionVersion;
            vendor = vendorName;
            vsVersion = vsVersionSupported;
            supportedServices = new Dictionary<Type, object>();
        }


        protected void AddService(Type serviceType, object service)
        {
            supportedServices.Add(serviceType, service);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dictionary<object, bool> dictionary = new Dictionary<object, bool>();
                foreach (KeyValuePair<Type, object> pair in supportedServices)
                {
                    if (pair.Value is Array)
                    {
                        Array array = pair.Value as Array;
                        foreach (object obj2 in array)
                        {
                            if (!dictionary.ContainsKey(obj2))
                            {
                                dictionary.Add(obj2, false);
                            }
                        }
                    }
                    else if (!dictionary.ContainsKey(pair.Value))
                    {
                        dictionary.Add(pair.Value, false);
                    }
                }
                supportedServices.Clear();
                foreach (KeyValuePair<object, bool> pair2 in dictionary)
                {
                    IDisposable key = pair2.Key as IDisposable;
                    if (key != null)
                    {
                        key.Dispose();
                    }
                }
            }
        }

        public override object GetService(Type serviceType)
        {
            foreach (KeyValuePair<Type, object> pair in supportedServices)
            {
                if (Equals(pair.Key, serviceType))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public override string PackageDescription =>
            description;

        public override string PackageName =>
            name;

        public override string PackageVendor =>
            vendor;

        public override Version PackageVersion =>
            version;

        public override Version VSVersion =>
            vsVersion;
    }
}