using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern.Products.OfferGenerator
{
    class CarInsuranceOfferGenerator : IOfferGenerator
    {
        public string GenerateOffer()
        {
            return "Car insurance offer generated.";
        }
    }
}
