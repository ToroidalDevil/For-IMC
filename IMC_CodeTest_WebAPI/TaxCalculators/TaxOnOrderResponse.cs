using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.TaxCalculators
{
    /// <summary>
    /// Services expected return type from any tax service called to retrieve rates.
    /// </summary>
    public class TaxOnOrderResponse
    {
        /// <summary>
        /// The tax on order response that the service will return to the requester.
        /// </summary>
        public TaxResponse TaxResponseValues{get;set;}

        /// <summary>
        /// Error information to be returned, in the event that TaxResponse could not be properly provided.
        /// </summary>
        public string Error{get;set;}
    }
}
