using DesignPatterns.AbstractFactoryPattern.Products;
using DesignPatterns.AbstractFactoryPattern.Products.OfferGenerator;
using DesignPatterns.AbstractFactoryPattern.Products.PremiumCalculator;

namespace DesignPatterns.AbstractFactoryPattern.Factories
{
    class CarInsuranceOfferRolloutFactory : IOfferRolloutFactory
    {
        public IOfferGenerator FetchOfferGenerator()
        {
            return new CarInsuranceOfferGenerator();
        }

        public IPremiumCalculator FetchPremiumCalculator()
        {
            return new CarInsurancePremiumCalculator();
        }
    }
}
