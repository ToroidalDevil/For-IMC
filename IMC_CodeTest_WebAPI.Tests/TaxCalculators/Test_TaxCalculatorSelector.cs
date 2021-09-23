using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IMC_CodeTest_WebAPI.Tests.TaxCalculators
{
    [TestClass]
    public class Test_TaxCalculatorSelector
    {
        /// <summary>
        /// When:
        ///     The designation provided has a calculator assigned
        /// Then:
        ///     The calculator should be returned.
        /// </summary>
        [TestMethod]
        public void CalculatorFound()
        {
            //Setup
            TaxCalculatorSelector selector = new TaxCalculatorSelector(null);

            //Run
            ITaxCalculator calculator = selector.SelectCalculator(TaxCalculatorDesignation.TaxJar);

            //Verify
            Assert.IsNotNull(calculator);
            Assert.IsInstanceOfType(calculator, typeof(TaxJar));
        }

        /// <summary>
        /// When:
        ///     The designation provided does not have a calculator assigned
        /// Then:
        ///     null should be returned.
        /// </summary>
        [TestMethod]
        public void CalculatorNotFound()
        {
            //Setup
            TaxCalculatorSelector selector = new TaxCalculatorSelector(null);

            //Run
            ITaxCalculator calculator = selector.SelectCalculator(TaxCalculatorDesignation.Default);

            //Verify
            Assert.IsNull(calculator);
        }
    }
}
