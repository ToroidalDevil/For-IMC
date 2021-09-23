using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using System.Collections.Generic;

namespace IMC_CodeTest_WebAPI.Tests.Mocks
{
    public class MockCalculatorSelector : ITaxCalculatorSelector
    {
        protected Dictionary<TaxCalculatorDesignation, ITaxCalculator> calculators = new Dictionary<TaxCalculatorDesignation, ITaxCalculator>();

        public void SetCalculator(TaxCalculatorDesignation id, ITaxCalculator calculator)
        {
            calculators.Add(id, calculator);
        }

        public void ReplaceCalculator(TaxCalculatorDesignation id, ITaxCalculator calculator)
        { 
            calculators[id] = calculator;
        }

        public ITaxCalculator SelectCalculator(TaxCalculatorDesignation designation)
        {
            if(calculators.ContainsKey(designation))
                return calculators[designation];

            return null;
        }
    }
}
