using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.TaxCalculators.Interfaces
{
    /// <summary>
    /// Provides tax rates and computes tax for given orders.
    /// </summary>
    public interface ITaxCalculator
    {
        //The assumption in returning these API specifc DTOs is that regardless of the calculation service used. The response structure needs to remain the same.
        //If that's not the case, then I would have changed these to object, and for successful requests, just passed the response from the service without performing DTO conversions.

        /// <summary>
        /// Returns the tax rates for the specified locale.
        /// </summary>
        /// <param name="zip">US zip code or equivalent</param>
        /// <param name="country">Country</param>
        /// <param name="city">City</param>
        /// <returns>Applicable tax rates.</returns>
        TaxRateResponse GetTaxRates(string zip, string country, string city);

        /// <summary>
        /// Calculates the tax for an order.
        /// </summary>
        /// <param name="order">An order to calculate tax for. Contains shipping, items, and locale information</param>
        /// <returns>All applicable taxes for the order, including basis information.</returns>
        TaxOnOrderResponse CalculateTaxesForOrder(Order order);
    }
}
