// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace NuGet.Protocol
{
    internal class SearchNode
    {
        public readonly SearchNode Parent;
        public readonly SearchNode[] Children;
        public bool IsValueNode => !string.IsNullOrWhiteSpace(NamespaceId);
        public bool IsLeaf { get; set; }
        public bool IsGlobbing { get; set; }
        public string NamespaceId { get; set; }
        public HashSet<string> PackageSources;

        public SearchNode(SearchNode parent)
        {
            Parent = parent;
            Children = new SearchNode[39]; // 'a-z', '0-9', '-', '.' , '-'
            NamespaceId = string.Empty;
            IsLeaf = true;
            IsGlobbing = false;
        }
    }
}