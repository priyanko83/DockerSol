using DesignPatterns.DecoratorPattern.Auto;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern
{
    class DecoratorPatternDemo
    {
        public static void StartDemo()
        {
            BaseCoverage coverage = new AutoCoverage("Auto Coverage");

            // Apply coverage 1
            coverage = new AutoTopupCoverage1(coverage);
            // Apply coverage 2
            coverage = new AutoTopupCoverage2(coverage);
            // Apply coverage 3
            coverage = new AutoTopupCoverage3(coverage);
            // Apply coverage 4
            coverage = new AutoTopupCoverage4(coverage);

            Console.WriteLine(coverage.FetchTopupDescription());
            Console.WriteLine(coverage.CalculatePremium());

            Console.ReadKey();
        }
    }
}
