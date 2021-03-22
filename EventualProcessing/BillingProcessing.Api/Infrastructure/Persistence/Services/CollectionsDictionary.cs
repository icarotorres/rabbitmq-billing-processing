﻿using System.Collections.Generic;

namespace BillingProcessing.Api.Infrastructure.Persistence.Services
{
    internal class CollectionsDictionary : Dictionary<string, string>, ICollectionsDictionary
    {
        public string GetCollectionName(string entityName) => TryGetValue(entityName, out string collectionName) ? collectionName : "";
    }
}