using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern.Auto
{
    class AutoCoverage : BaseCoverage
    {
        public string CoverageDescription { get; protected set; }
        public AutoCoverage(string desc)
        {
            this.CoverageDescription = desc;
        }
        public override double CalculatePremium()
        {
            return 100.0;
        }

        public override string FetchTopupDescription()
        {
            return CoverageDescription;
        }
    }
}
