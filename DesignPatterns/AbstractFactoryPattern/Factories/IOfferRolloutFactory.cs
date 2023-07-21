using DesignPatterns.AbstractFactoryPattern.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern.Factories
{
    interface IOfferRolloutFactory
    {
        IPremiumCalculator FetchPremiumCalculator();

        IOfferGenerator FetchOfferGenerator();
    }
}
