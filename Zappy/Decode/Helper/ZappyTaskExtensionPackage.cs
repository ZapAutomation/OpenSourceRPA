using System;

namespace Zappy.Decode.Helper
{
    public abstract class ZappyTaskExtensionPackage : IServiceProvider, IDisposable
    {
        public abstract void Dispose();
        public abstract object GetService(Type serviceType);

        public abstract string PackageDescription { get; }

        public abstract string PackageName { get; }

        public abstract string PackageVendor { get; }

        public abstract Version PackageVersion { get; }

        public abstract Version VSVersion { get; }
    }
}