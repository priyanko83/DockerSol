using System;
using System.Collections.Generic;
using System.Text;

namespace DesignPatterns.DecoratorPattern
{
    abstract class BaseCoverage
    {
        public abstract double CalculatePremium();
        public abstract string FetchTopupDescription();
    }
}
