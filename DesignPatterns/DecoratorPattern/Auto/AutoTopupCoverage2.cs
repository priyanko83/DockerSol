using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern.Auto
{
    class AutoTopupCoverage2 : TopupCoverage
    {        
        public AutoTopupCoverage2(BaseCoverage coverage) : base(coverage)
        {
            
        }

        public override double CalculatePremium()
        {
            return this.BaseCoverageForTopup.CalculatePremium() + 20.0;
        }

        public override string FetchTopupDescription()
        {
            return this.BaseCoverageForTopup.FetchTopupDescription() + ", AutoTopupCoverage2 ";
        }
    }
}
