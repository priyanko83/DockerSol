using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern.Auto
{
    class AutoTopupCoverage4:TopupCoverage
    {
        public AutoTopupCoverage4(BaseCoverage coverage) : base(coverage)
        {

        }

        public override double CalculatePremium()
        {
            return this.BaseCoverageForTopup.CalculatePremium() + 40.0;
        }

        public override string FetchTopupDescription()
        {
            return this.BaseCoverageForTopup.FetchTopupDescription() + ", AutoTopupCoverage4 ";
        }
    }
}
