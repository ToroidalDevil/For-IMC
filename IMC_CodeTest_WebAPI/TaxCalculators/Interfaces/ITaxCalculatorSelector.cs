
namespace IMC_CodeTest_WebAPI.TaxCalculators.Interfaces
{
    public interface ITaxCalculatorSelector
    {
        /// <summary>
        /// Returns a calculator based on the id passed in.
        /// </summary>
        /// <param name="id">Id of the calculator to select.</param>
        /// <returns>The tax calculator for the id passed in. Null if the id is not found.</returns>
        ITaxCalculator SelectCalculator(TaxCalculatorDesignation id);
    }
}
