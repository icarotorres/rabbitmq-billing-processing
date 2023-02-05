// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Processing.Scheduled.Worker.Services
{
    public class CpfCarrierComparer : IComparer<ICpfCarrier>, IEqualityComparer<ICpfCarrier>
    {
        public int Compare([AllowNull] ICpfCarrier x, [AllowNull] ICpfCarrier y)
        {
            if (x == null && y == null) return 0;
            if (x == null || y == null) return -1;
            return x.Cpf.CompareTo(y.Cpf);
        }
        public bool Equals([AllowNull] ICpfCarrier x, [AllowNull] ICpfCarrier y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Cpf == y.Cpf;
        }
        public int GetHashCode([DisallowNull] ICpfCarrier obj) => obj.Cpf.GetHashCode();
    }
}
