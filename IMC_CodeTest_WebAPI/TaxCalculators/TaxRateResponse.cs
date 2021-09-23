using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.TaxCalculators
{
    /// <summary>
    /// Services expected return type from any tax service called to retrieve rates.
    /// </summary>
    public class TaxRateResponse
    { 
        /// <summary>
        /// The tax rates response that the service will return to the requester.
        /// </summary>
        public RateResponse RateResponseValues{get;set;}

        /// <summary>
        /// Error information to be returned, in the event that RateResponse could not be properly provided.
        /// </summary>
        public string Error{get;set;}
    }
}
