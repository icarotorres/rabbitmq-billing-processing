// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Library.Configurations
{
    [Serializable]
    public class ExchangeDictionary : Dictionary<string, ExchangeSettings>
    {
        public ExchangeSettings GetSettings(string key) => TryGetValue(key, out var settings)
            ? settings
            : throw new ArgumentException($"ExchangeSettings not found. Ensure your appsettings has a entry for given key {key}");
    }
}
