using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

namespace IMC_CodeTest_WebAPI.Tests.Mocks
{
    public class MockCalculator : ITaxCalculator
    {
        public TaxOnOrderResponse OrderResponse{get;set;}
        public TaxRateResponse RateResponse{get;set;}

        public virtual TaxOnOrderResponse CalculateTaxesForOrder(Order order)
        {
            return OrderResponse;
        }

        public virtual TaxRateResponse GetTaxRates(string zip, string country, string city)
        {
            return RateResponse;
        }
    }
}
