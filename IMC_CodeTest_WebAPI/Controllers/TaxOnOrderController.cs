using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Net;

namespace IMC_CodeTest_WebAPI.Controllers
{
    /// <summary>
    /// Provides a Post that takes an Order, and returns the proper taxes for that order.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaxOnOrderController : ControllerBase
    {
        #region Constructor
        public TaxOnOrderController(ITaxCalculatorSelector calcSelector)
        { 
            _taxCalcSelector = calcSelector;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Expected error if an order was not received/parsed in the request body.
        /// </summary>
        public static string ErrorBodyNotReceived{get{return "Body did not contain an Order";}}

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
        /// Given an order of items returns the taxes to apply.
        /// </summary>
        /// <param name="order">The order to be taxed.</param>
        /// <returns>The taxes for the order.</returns>
        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            IActionResult result = null;
            if(null != (result = ValidateOrder(order)))
                return result;

            ITaxCalculator taxCalc = _taxCalcSelector.SelectCalculator(TaxCalculatorDesignation.TaxJar);

            return GetTaxOnOrder(taxCalc, order);
        }
        #endregion


        #region Protected functions
        /// <summary>
        /// Validates the Order object.
        /// 
        /// This method is provided separately so that if a new method is created that accepts a client id. This method can be used to validate the Order input in that call.
        /// </summary>
        /// <param name="order">The Order to validate.</param>
        /// <returns>An ErrorResponse is the order fails validation. Otherwise null</returns>
        protected IActionResult ValidateOrder(Order order)
        { 
            if(null == order)
                return new BadRequestObjectResult(new ErrorResponse(){status_code= HttpStatusCode.BadRequest, description= ErrorBodyNotReceived});

            return null;
        }

        /// <summary>
        /// Uses the tax calculator and the order to request the tax.
        /// 
        /// This method is provided separately so that if a new method is created that accepts a client id. This method can be used to retreive the rates, and handle any 
        /// error generation related to tax calculator failure.
        /// </summary>
        /// <param name="taxCalc">Tax calculator to use to get the tax.</param>
        /// <param name="order">The order to retrive tax for.</param>
        /// <returns>An ErrorResponse if there is a problem with the call. Otherwise the json data containing the result.</returns>
        protected IActionResult GetTaxOnOrder(ITaxCalculator taxCalc, Order order)
        { 
            TaxOnOrderResponse taxedOrderResponse = null;
            try
            {
                taxedOrderResponse = taxCalc.CalculateTaxesForOrder(order);
            }
            catch(Exception e)
            { 
                return new ObjectResult(new ErrorResponse(){status_code= HttpStatusCode.FailedDependency, description= TaxServiceInternalErrorText, error_detail= e.Message});
            }

            if(null != taxedOrderResponse.Error)
                return new ObjectResult(new ErrorResponse(){status_code= HttpStatusCode.BadRequest, description= TaxServiceCouldNotProcessRequest, error_detail= taxedOrderResponse.Error});

            return new JsonResult(taxedOrderResponse.TaxResponseValues);
        }
        #endregion
    }
}
