using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Net;

namespace IMC_CodeTest_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxRateController : ControllerBase
    {
        #region Constructor
        public TaxRateController(ITaxCalculatorSelector calcSelector)
        { 
            _taxCalcSelector = calcSelector;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Expected error if zip is not provided
        /// </summary>
        public static string ParmFailureError_Zip{get{return "Query parameter [zip] is required.";} }

        /// <summary>
        /// Expected error if country is not provided
        /// </summary>
        public static string ParmFailureError_Country{get{return "Query parameter [country] is required.";} }

        /// <summary>
        /// Expected error if city is not provided
        /// </summary>
        public static string ParmFailureError_City{get{return "Query parameter [city] is required.";} }

        /// <summary>
        /// Expected error if tax service had an internal error
        /// </summary>
        public static string TaxServiceInternalErrorText{get{return "Tax calculator had an internal error";} }

        /// <summary>
        /// Expected error if tax service could not process the request.
        /// </summary>
        public static string TaxServiceCouldNotProcessRequest{get{return "Tax calculator failed.";} }
        #endregion


        #region Data
        /// <summary>
        /// Selects a TaxCalculator based on a designation.
        /// </summary>
        protected ITaxCalculatorSelector _taxCalcSelector = null;
        #endregion


        #region Public functions
        /// <summary>
        /// Gets the tax rates using the query parameters provided.
        /// </summary>
        /// <param name="zip">zipcode for lookup</param>
        /// <param name="country">country for lookup</param>
        /// <param name="city">city for lookup</param>
        /// <returns>Success: TaxRateResponse object json. Fail: ErrorResponse object.</returns>
        [HttpGet]
        public IActionResult Get(string zip, string country, string city)
        {
            //Validate that all inputs were received.
            IActionResult result = null;
            if(null != (result = ValidateInputs(zip, country, city)))
                return result;

            ITaxCalculator taxCalc = _taxCalcSelector.SelectCalculator(TaxCalculatorDesignation.TaxJar);

            return GetTaxRates(taxCalc, zip, country, city);
        }
        #endregion


        #region Protected functions
        /// <summary>
        /// Validate the presence of the basic request inputs.
        /// 
        /// This method is provided separately so that if a new method is created that accepts a client id. This method can be used to validate these inputs in that call.
        /// </summary>
        /// <param name="zip">zipcode for lookup</param>
        /// <param name="country">country for lookup</param>
        /// <param name="city">city for lookup</param>
        /// <returns>An error response if the inputs fail validation. Otherwise null</returns>
        protected IActionResult ValidateInputs(string zip, string country, string city)
        { 
            //Validate that all inputs were received.
            if(string.IsNullOrWhiteSpace(zip))
                return new BadRequestObjectResult(new ErrorResponse(){status_code= HttpStatusCode.BadRequest, description= ParmFailureError_Zip});

            if(string.IsNullOrWhiteSpace(country))
                return new BadRequestObjectResult(new ErrorResponse(){status_code= HttpStatusCode.BadRequest, description= ParmFailureError_Country});

            if(string.IsNullOrWhiteSpace(city))
                return new BadRequestObjectResult(new ErrorResponse(){status_code= HttpStatusCode.BadRequest, description= ParmFailureError_City});

            return null;
        }

        /// <summary>
        /// Uses the tax calculator and the inputs to request the rates.
        /// 
        /// This method is provided separately so that if a new method is created that accepts a client id. This method can be used to retreive the rates, and handle any 
        /// error generation related to tax calculator failure.
        /// </summary>
        /// <param name="taxCalc">Tax calculator to use to get the rates.</param>
        /// <param name="zip">zipcode for lookup</param>
        /// <param name="country">country for lookup</param>
        /// <param name="city">city for lookup</param>
        /// <returns>An error respone if there is a problem with the call. Otherwise the json data contianing the result.</returns>
        protected IActionResult GetTaxRates(ITaxCalculator taxCalc, string zip, string country, string city)
        { 
            TaxRateResponse rateResponse = null;
            try
            { 
                rateResponse = taxCalc.GetTaxRates(zip, country, city);
            }
            catch(Exception e)
            {
                return new ObjectResult(new ErrorResponse{status_code= HttpStatusCode.FailedDependency, description= TaxServiceInternalErrorText, error_detail= e.Message});
            }

            if(null != rateResponse.Error)
                return new ObjectResult(new ErrorResponse{status_code= HttpStatusCode.BadRequest, description= TaxServiceCouldNotProcessRequest, error_detail= rateResponse.Error});

            return new JsonResult(rateResponse.RateResponseValues);
        }
        #endregion
    }
}
