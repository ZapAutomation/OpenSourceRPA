﻿using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Zappy.Helpers.NugetManager
{
    public class PackageSearchItem
    {
        public PackageSearchItem() { }
        public PackageSearchItem(PackageReaderBase reader)
        {
            Identity = reader.GetIdentity();
            Id = Identity.Id;
            Title = reader.NuspecReader.GetTitle();
            if (string.IsNullOrEmpty(Title)) Title = Id;
            //IsInstalled = isPackageIdInProjectDependencies(project, Id);

            InstalledVersion = "";
            // Find installed version !
            var LocalVersion = NuGetPackageManager.Instance.getLocal(Id);
            CanInstall = false;
            if (LocalVersion != null)
            {
                //if (project.dependencies != null)
                //{
                //    foreach (JProperty jp in (JToken)project.dependencies)
                //    {
                //        if(jp.Name == Id)
                //        {
                //            InstalledVersion = (string)jp.Value;
                //            CanInstall = true;
                //        }
                //    }
                //}
                if (string.IsNullOrEmpty(InstalledVersion)) InstalledVersion = LocalVersion.Identity.Version.ToString();
                SelectedVersion = InstalledVersion;
            }
            //NotifyPropertyChanged("InstalledVersion");

            LicenseUrl = reader.NuspecReader.GetLicenseUrl();
            ProjectUrl = reader.NuspecReader.GetProjectUrl();
            Tags = reader.NuspecReader.GetTags();
            Description = reader.NuspecReader.GetDescription();
            IconUrl = reader.NuspecReader.GetIconUrl();
            Authors = reader.NuspecReader.GetAuthors();
            Dependencies = reader.GetPackageDependencies().ToList();
        }
        public PackageSearchItem(IPackageSearchMetadata searchItem)
        {
            Identity = searchItem.Identity;
            Id = Identity.Id;
            Title = searchItem.Title;
            //IsInstalled = isPackageIdInProjectDependencies(project, searchItem.Identity.Id);

            InstalledVersion = "";
            // Find installed version !
            var LocalVersion = NuGetPackageManager.Instance.getLocal(Id);
            CanInstall = true;
            if (LocalVersion != null)
            {
                //if (project.dependencies != null)
                //{
                //    foreach (JProperty jp in (JToken)project.dependencies)
                //    {
                //        if (jp.Name == Id)
                //        {
                //            InstalledVersion = (string)jp.Value;
                //        }
                //    }
                //}
                if (string.IsNullOrEmpty(InstalledVersion)) InstalledVersion = LocalVersion.Identity.Version.ToString();
                SelectedVersion = InstalledVersion;
            }
            //NotifyPropertyChanged("InstalledVersion");

            LicenseUrl = searchItem.LicenseUrl?.AbsoluteUri;
            ProjectUrl = searchItem.ProjectUrl?.AbsoluteUri;
            PublishTime = searchItem.Published?.ToLocalTime().ToString("G");
            Tags = searchItem.Tags;
            Description = searchItem.Description;
            if (searchItem.IconUrl != null) IconUrl = searchItem.IconUrl.ToString();
            Authors = searchItem.Authors;
            Dependencies = searchItem.DependencySets.ToList();
        }
        private bool isPackageIdInProjectDependencies(string package_id)
        {
            //if (project.dependencies == null) return false;
            //foreach (JProperty jp in (JToken)project.dependencies)
            //{
            //    if (jp.Name.ToLower() == package_id.ToLower())
            //    {
            //        return true;
            //    }
            //}
            return false;
        }
        public async Task LoadVersions(bool IncludePrerelease)
        {
            try
            {
                if (VersionList != null) return;
                var _VersionList = new System.Collections.ObjectModel.ObservableCollection<string>();
                List<IPackageSearchMetadata> search_package_versions = await NuGetPackageManager.Instance.SearchPackageVersions(Id, IncludePrerelease);
                search_package_versions.Sort((x, y) => -x.Identity.Version.CompareTo(y.Identity.Version));
                foreach (var ver in search_package_versions)
                {
                    _VersionList.Add(ver.Identity.Version.OriginalVersion);
                }
                if (IsInstalled)
                {

                }
                if (_VersionList != null && _VersionList.Count > 0 && string.IsNullOrEmpty(SelectedVersion)) SelectedVersion = _VersionList.First();
                VersionList = _VersionList;
                //NotifyPropertyChanged("SelectedVersion");
                //NotifyPropertyChanged("InstalledVersion");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
            }
        }
        public PackageIdentity Identity { get { return GetProperty<PackageIdentity>(); } set { SetProperty(value); } }
        public string Id { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Authors { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Description { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string IconUrl { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Title { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public bool IsInstalled { get { return GetProperty<bool>(); } set { SetProperty(value); } }
        public bool CanInstall { get { return GetProperty<bool>(); } set { SetProperty(value); } }
        public string InstalledVersion { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string LicenseUrl { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string ProjectUrl { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string PublishTime { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public string Tags { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public bool RequireLicenseAcceptance { get { return GetProperty<bool>(); } set { SetProperty(value); } }
        public string SelectedVersion { get { return GetProperty<string>(); } set { SetProperty(value); } }
        public List<PackageDependencyGroup> Dependencies { get { return GetProperty<List<PackageDependencyGroup>>(); } set { SetProperty(value); } }
        public System.Collections.ObjectModel.ObservableCollection<string> VersionList { get { return GetProperty<System.Collections.ObjectModel.ObservableCollection<string>>(); } set { SetProperty(value); } }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(InstalledVersion)) return Id;
            return Id + "@" + InstalledVersion;
        }

        public Dictionary<string, object> _backingFieldValues = new Dictionary<string, object>();

        protected T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            try
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }
                object value;
                if (_backingFieldValues.TryGetValue(propertyName, out value))
                {
                    return (T)value;
                }
                return (T)value;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Saves a property value to the internal backing field
        /// </summary>
        protected bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            try
            {
                if (propertyName == null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }
                if (IsEqual(GetProperty<T>(propertyName), newValue)) return false;
                _backingFieldValues[propertyName] = newValue;
                OnPropertyChanged(propertyName);
                Type typeParameterType = typeof(T);
                if (typeParameterType.Name.ToLower().Contains("readonly"))
                {
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Sets a property value to the backing field
        /// </summary>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (IsEqual(field, newValue)) return false;
            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool IsEqual<T>(T field, T newValue)
        {
            // Alternative: EqualityComparer<T>.Default.Equals(field, newValue);
            return Equals(field, newValue);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //NotifyPropertyChanged(propertyName);
        }


    }
}
