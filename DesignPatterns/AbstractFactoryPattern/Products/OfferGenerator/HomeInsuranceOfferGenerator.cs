using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern.Products.OfferGenerator
{
    class HomeInsuranceOfferGenerator : IOfferGenerator
    {
        public string GenerateOffer()
        {
            return "Home insurance offer generated.";
        }
    }
}
