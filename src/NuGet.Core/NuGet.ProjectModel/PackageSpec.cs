// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NuGet.LibraryModel;
using NuGet.RuntimeModel;
using NuGet.Shared;
using NuGet.Versioning;

namespace NuGet.ProjectModel
{
    /// <summary>
    /// Represents the specification of a package that can be built.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class PackageSpec
    {
        public static readonly string PackageSpecFileName = "project.json";
        public static readonly NuGetVersion DefaultVersion = new NuGetVersion(1, 0, 0);

        public PackageSpec(IList<TargetFrameworkInformation> frameworks)
        {
            TargetFrameworks = frameworks;
        }

        public PackageSpec() : this(new List<TargetFrameworkInformation>())
        {
        }

        public string FilePath { get; set; }

        public string BaseDirectory => Path.GetDirectoryName(FilePath);

        public string Name { get; set; }

        public string Title { get; set; }

        private NuGetVersion _version = DefaultVersion;
        public NuGetVersion Version
        {
            get => _version;
            set
            {
                _version = value;
            }
        }
        
        public IList<LibraryDependency> Dependencies { get; set; } = new List<LibraryDependency>();

        public IList<TargetFrameworkInformation> TargetFrameworks { get; private set; }

        public RuntimeGraph RuntimeGraph { get; set; } = new RuntimeGraph();

        /// <summary>
        /// Project Settings is used to pass settings like HideWarningsAndErrors down to lower levels.
        /// Currently they do not include any settings that affect the final result of restore.
        /// This should not be part of the Equals and GetHashCode.
        /// Don't write this to the package spec
        /// </summary>
        public ProjectRestoreSettings RestoreSettings { get; set; } = new ProjectRestoreSettings();

        /// <summary>
        /// Additional MSBuild properties.
        /// </summary>
        /// <remarks>Optional. This is normally set for internal use only.</remarks>
        public ProjectRestoreMetadata RestoreMetadata { get; set; }

        public override int GetHashCode()
        {
            var hashCode = new HashCodeCombiner();

            hashCode.AddObject(Title);
            hashCode.AddObject(Version);
            hashCode.AddSequence(Dependencies);
            hashCode.AddSequence(TargetFrameworks);
            hashCode.AddObject(RuntimeGraph);
            hashCode.AddObject(RestoreMetadata);

            return hashCode.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PackageSpec);
        }

        public bool Equals(PackageSpec other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Name and FilePath are not used for comparison since they are not serialized to JSON.

            return Title == other.Title &&
                   EqualityUtility.EqualsWithNullCheck(Version, other.Version) &&
                   EqualityUtility.OrderedEquals(Dependencies, other.Dependencies, dep => dep.Name, StringComparer.OrdinalIgnoreCase) &&
                   EqualityUtility.OrderedEquals(TargetFrameworks, other.TargetFrameworks, tfm => tfm.TargetAlias, StringComparer.OrdinalIgnoreCase) &&
                   EqualityUtility.EqualsWithNullCheck(RuntimeGraph, other.RuntimeGraph) &&
                   EqualityUtility.EqualsWithNullCheck(RestoreMetadata, other.RestoreMetadata);
        }

        /// <summary>
        /// Clone a PackageSpec
        /// </summary>
        public PackageSpec Clone()
        {
            var spec = new PackageSpec();
            spec.Name = Name;
            spec.FilePath = FilePath;
            spec.Title = Title;
            spec.Version = Version;
            spec.Dependencies = Dependencies?.Select(item => item.Clone()).ToList();
            spec.TargetFrameworks = TargetFrameworks?.Select(item => item.Clone()).ToList();
            spec.RuntimeGraph = RuntimeGraph?.Clone();
            spec.RestoreSettings = RestoreSettings?.Clone();
            spec.RestoreMetadata = RestoreMetadata?.Clone();
            return spec;
        }

        private IDictionary<string, IEnumerable<string>> CloneScripts(IDictionary<string, IEnumerable<string>> toBeCloned)
        {
            if (toBeCloned != null)
            {
                var clone = new Dictionary<string, IEnumerable<string>>();
                foreach (var kvp in toBeCloned)
                {
                    clone.Add(kvp.Key, new List<string>(kvp.Value));
                }
                return clone;
            }
            return null;
        }
    }
}
