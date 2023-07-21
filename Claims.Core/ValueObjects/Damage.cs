using CQRSFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Claims.Core.ValueObjects
{
    public class Damage : ValueObject<Damage>
    {
        readonly string DamagePartCode;
        readonly DateTime DamagePartPurchaseDate;
        

        public Damage(string partCode, DateTime purchaseDate)
        {
            this.DamagePartCode = partCode;
            this.DamagePartPurchaseDate = purchaseDate;
        }

        public double AllowedClaimAmount()
        {
            return new Random().Next(100, 5000);
        }
    }
}
