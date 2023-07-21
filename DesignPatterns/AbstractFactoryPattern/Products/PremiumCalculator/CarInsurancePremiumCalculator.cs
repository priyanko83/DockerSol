using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern.Products.PremiumCalculator
{
    public class CarInsurancePremiumCalculator : IPremiumCalculator
    {
        public string CalculatePremium()
        {
            return "Car Insurance Premium calculated.";
        }
    }
}
