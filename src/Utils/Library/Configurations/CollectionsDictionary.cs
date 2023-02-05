// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Library.Configurations
{
    public class CollectionsDictionary : Dictionary<string, string>
    {
        public string GetCollectionName(string key) => TryGetValue(key, out var collectionName)
            ? collectionName
            : throw new ArgumentException(
                $"MongoDB Collection not found. Ensure your setting has an entry for given key {key}");
    }
}
