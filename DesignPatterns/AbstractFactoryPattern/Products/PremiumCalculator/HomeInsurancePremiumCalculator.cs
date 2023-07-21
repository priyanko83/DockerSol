using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern.Products
{
    class HomeInsurancePremiumCalculator : IPremiumCalculator
    {
        public string CalculatePremium()
        {
            return "Home insurance premium calculated.";
        }
    }
}
