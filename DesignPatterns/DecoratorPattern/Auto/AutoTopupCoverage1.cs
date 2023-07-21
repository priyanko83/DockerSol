using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern.Auto
{
    class AutoTopupCoverage1 : TopupCoverage
    {        
        public AutoTopupCoverage1(BaseCoverage coverage):base(coverage)
        {            
        }

        public override double CalculatePremium()
        {
            return this.BaseCoverageForTopup.CalculatePremium() + 10.0;
        }

        public override string FetchTopupDescription()
        {
            return this.BaseCoverageForTopup.FetchTopupDescription() + ", AutoTopupCoverage1 ";
        }
    }
}
