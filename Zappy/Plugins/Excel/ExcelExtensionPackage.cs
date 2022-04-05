



using System;
using Zappy.ActionMap.TaskTechnology;
using Zappy.Decode.Aggregator;
using Zappy.Decode.Helper;
using Zappy.ExecuteTask.TaskExecutor;

namespace Zappy.Plugins.Excel
{
                internal class ExcelExtensionPackage : ZappyTaskExtensionPackageBase    {

        public ExcelExtensionPackage() : base("ExcelExtensionPackage", "ExcelExtensionPackage")
        {
            AddService(typeof(ZappyTaskPropertyProvider), propertyProvider);
            AddService(typeof(UITechnologyManager), technologyManager);
            AddService(typeof(ZappyTaskActionFilter), actionFilter);
        }

                                                                public override object GetService(Type serviceType)
        {
                        if (serviceType == typeof(UITechnologyManager))
            {
                return technologyManager;
            }
            else if (serviceType == typeof(ZappyTaskPropertyProvider))
            {
                return propertyProvider;
            }
            else if (serviceType == typeof(ZappyTaskActionFilter))
            {
                return actionFilter;
            }

            return null;
        }

                                public override void Dispose()
        {
                    }

        #region Simple Properties

                                public override string PackageDescription
        {
            get { return "Plugin for Zappy Record and Playback support on Excel"; }
        }

                                public override string PackageName
        {
            get { return "Zappy Excel Plugin"; }
        }

                                public override string PackageVendor
        {
            get { return "Zappy.ai"; }
        }

                                public override Version PackageVersion
        {
            get { return new Version(1, 0); }
        }

                                public override Version VSVersion
        {
            get { return new Version(10, 0); }
        }

        #endregion

                private ExcelTechnologyManager technologyManager = new ExcelTechnologyManager();
        private ExcelPropertyProvider propertyProvider = new ExcelPropertyProvider();
        private ExcelActionFilter actionFilter = new ExcelActionFilter();
    }
}
