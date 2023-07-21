using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern
{
    abstract class TopupCoverage : BaseCoverage
    {
        public BaseCoverage BaseCoverageForTopup { get; protected set; }
        protected TopupCoverage(BaseCoverage baseCoverage)
        {
            this.BaseCoverageForTopup = baseCoverage;
        }

        public override double CalculatePremium()
        {
            return this.BaseCoverageForTopup.CalculatePremium();
        }
        public override string FetchTopupDescription()
        {
            return this.BaseCoverageForTopup.FetchTopupDescription();
        }
    }
}
