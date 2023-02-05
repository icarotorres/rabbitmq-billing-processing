// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Library.Configurations
{
    public class MongoDBSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public CollectionsDictionary Collections { get; set; }
    }
}
