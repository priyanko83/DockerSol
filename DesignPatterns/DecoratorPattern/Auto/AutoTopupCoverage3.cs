using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern.Auto
{
    class AutoTopupCoverage3:TopupCoverage
    {
        public AutoTopupCoverage3(BaseCoverage coverage) : base(coverage)
        {

        }

        public override double CalculatePremium()
        {
            return this.BaseCoverageForTopup.CalculatePremium() + 30.0;
        }

        public override string FetchTopupDescription()
        {
            return this.BaseCoverageForTopup.FetchTopupDescription() + ", AutoTopupCoverage3 ";
        }
    }
}
