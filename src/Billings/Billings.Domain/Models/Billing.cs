// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Billings.Domain.Models
{
    /// <summary>
    /// Domain representation of a business Billing charged for a customer
    /// </summary>
    public class Billing
    {
        [BsonId, BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        /// <summary>
        /// Unique personal identification in force in Brazil
        /// </summary>
        [BsonElement("cpf")]
        public ulong Cpf { get; set; }

        [BsonElement("amount"), BsonRepresentation(BsonType.Double)]
        public double Amount { get; set; }

        [BsonElement("due_date")]
        public Date DueDate { get; set; }

        [BsonElement("processed_at"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? ProcessedAt { get; set; }
    }
}
