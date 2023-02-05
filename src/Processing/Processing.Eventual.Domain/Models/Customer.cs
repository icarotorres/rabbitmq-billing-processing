﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Library.Results;
using MediatR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Processing.Eventual.Domain.Services;

namespace Processing.Eventual.Domain.Models
{
    public class Customer : IRequest<IResult>
    {
        [BsonId, BsonRepresentation(BsonType.Int64), BsonElement("cpf")]
        public virtual ulong Cpf { get; set; }

        public virtual bool AcceptProcessing(Billing billing, IAmountProcessor calculator)
        {
            billing = calculator.Process(this, billing);
            return billing.ProcessedAt != null;
        }

        public static readonly Customer Null = new NullCustomer();
    }
}
