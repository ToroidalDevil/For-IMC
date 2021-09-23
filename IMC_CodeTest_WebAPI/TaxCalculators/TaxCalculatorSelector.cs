using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Microsoft.Extensions.Configuration;

using System.Collections.Generic;

namespace IMC_CodeTest_WebAPI.TaxCalculators
{
    /// <summary>
    /// Provides the taxcalculator
    /// </summary>
    public class TaxCalculatorSelector : ITaxCalculatorSelector
    {
        #region Constructor
        public TaxCalculatorSelector(IConfiguration config)
        { 
            string taxJarAuthToken = null;
            
            if(null != config)
                taxJarAuthToken = config.GetValue<string>("TaxJar:AuthToken");

            calculators.Add(TaxCalculatorDesignation.TaxJar, new TaxJar(null, taxJarAuthToken));
        }
        #endregion


        #region Data
        /// <summary>
        /// A dictionary of calculators specified by TaxCalculatorDesignation.
        /// </summary>
        protected Dictionary<TaxCalculatorDesignation, ITaxCalculator> calculators = new Dictionary<TaxCalculatorDesignation, ITaxCalculator>();
        #endregion


        #region Public functions
        public ITaxCalculator SelectCalculator(TaxCalculatorDesignation designation)
        {
            if(calculators.ContainsKey(designation))
                return calculators[designation];

            return null;
        }
        #endregion
    }
}
