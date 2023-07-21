using DesignPatterns.AbstractFactoryPattern.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.AbstractFactoryPattern
{
    class AbstractFactoryDemo
    {
        public static void StartDemo()
        {
            Console.WriteLine("What do you want to purchase? (H)ome insurance coverage or (C)ar insurance coverage?");
            char input = Console.ReadKey().KeyChar;
            IOfferRolloutFactory factory;
            Console.WriteLine("");
            switch (input)
            {
                case 'H':
                case 'h':
                    factory = new HomeInsuranceOfferRolloutFactory();
                    break;
                case 'C':
                case 'c':
                    factory = new CarInsuranceOfferRolloutFactory();
                    break;

                default:
                    throw new NotImplementedException();

            }

            var premiumCalculator = factory.FetchPremiumCalculator();
            var offerGenerator = factory.FetchOfferGenerator();

            Console.WriteLine(premiumCalculator.CalculatePremium());
            Console.WriteLine(offerGenerator.GenerateOffer());

            Console.ReadKey();
        }
    }
}
