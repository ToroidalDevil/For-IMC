using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;

using System;

namespace IMC_CodeTest_WebAPI.Tests.Mocks
{
    /// <summary>
    /// Mock calcualtor for testing. Returns specific errors for exceptions thrown from the tax calculator
    public class MockCalculator_Exception : MockCalculator
    {
        public static string OrderExceptionText{get{return "CalculateTaxesForOrder - intentional Exception";}}
        public static string RateExceptionText{get{return "GetTaxRates - intentional Exception";} }

        public override TaxOnOrderResponse CalculateTaxesForOrder(Order order)
        {
            throw new Exception(OrderExceptionText);
        }

        public override TaxRateResponse GetTaxRates(string zip, string country, string city)
        {
            throw new Exception(RateExceptionText);
        }
    }
}
