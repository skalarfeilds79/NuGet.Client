// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#nullable enable

using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

using EnvDTE;

using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;

using NuGet.VisualStudio.SolutionExplorer.Models;

namespace NuGet.VisualStudio.SolutionExplorer
{
    [Export(typeof(IAttachedCollectionSourceProvider))]
    [Name(nameof(PackageReferenceAttachedCollectionSourceProvider))]
    [Order(Before = HierarchyItemsProviderNames.Contains)]
    internal sealed class PackageReferenceAttachedCollectionSourceProvider : AssetsFileTopLevelDependenciesCollectionSourceProvider<(string Name, string Version), PackageReferenceItem>
    {
        public PackageReferenceAttachedCollectionSourceProvider()
            : base(DependencyTreeFlags.PackageDependency)
        {
        }

        protected override bool TryGetIdentity(Properties properties, out (string Name, string Version) identity)
        {
            if (properties.Item("Name")?.Value is string name &&
                properties.Item("Version")?.Value is string version &&
                !string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(version))
            {
                identity = (name, version);
                return true;
            }

            identity = default;
            return false;
        }

        protected override bool TryGetLibrary(AssetsFileTarget target, (string Name, string Version) identity, [NotNullWhen(returnValue: true)] out AssetsFileTargetLibrary? library)
        {
            return target.TryGetPackage(identity.Name, identity.Version, out library);
        }

        protected override PackageReferenceItem CreateItem(AssetsFileTarget targetData, AssetsFileTargetLibrary library)
        {
            return new PackageReferenceItem(targetData, library);
        }

        protected override bool TryUpdateItem(PackageReferenceItem item, AssetsFileTarget targetData, AssetsFileTargetLibrary library)
        {
            return item.TryUpdateState(targetData, library);
        }
    }
}
