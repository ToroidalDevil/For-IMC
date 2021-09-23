using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;

namespace IMC_CodeTest_WebAPI.Tests.Mocks
{
    /// <summary>
    /// Mock calcualtor for testing. Returns specific errors for a non-reponse from the tax calculator
    /// </summary>
    public class MockCalculator_NoResults : MockCalculator
    {
        public static TaxRateResponse RatesResponse
        {
            get{return new TaxRateResponse
                { 
                    Error = "Mock Rates Error",
                    RateResponseValues = null
                };
            } 
        }

        public static TaxOnOrderResponse TaxesResponse
        { 
            get{return new TaxOnOrderResponse
                { 
                    Error = "Mock Taxes Error",
                    TaxResponseValues = null
                };
            }
        }

        public override TaxOnOrderResponse CalculateTaxesForOrder(Order order)
        {
            return TaxesResponse;
        }

        public override TaxRateResponse GetTaxRates(string zip, string country, string city)
        {
            return RatesResponse;
        }
    }
}
