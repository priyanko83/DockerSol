using System;
using System.Collections.Generic;
using System.Text;
using DesignPatterns.AbstractFactoryPattern.Products;
using DesignPatterns.AbstractFactoryPattern.Products.OfferGenerator;

namespace DesignPatterns.AbstractFactoryPattern.Factories
{
    class HomeInsuranceOfferRolloutFactory : IOfferRolloutFactory
    {
        public IOfferGenerator FetchOfferGenerator()
        {
            return new HomeInsuranceOfferGenerator();
        }

        public IPremiumCalculator FetchPremiumCalculator()
        {
            return new HomeInsurancePremiumCalculator();
        }
    }
}
