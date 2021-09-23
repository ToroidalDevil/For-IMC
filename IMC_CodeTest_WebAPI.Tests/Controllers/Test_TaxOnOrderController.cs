using IMC_CodeTest_WebAPI.Controllers;
using IMC_CodeTest_WebAPI.Models;
using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.Tests.Mocks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net;

namespace IMC_CodeTest_WebAPI.Tests.Controllers
{
    [TestClass]
    public class Test_TaxOnOrderController
    {
        /// <summary>
        /// Setup Mocks for a basic success scenario
        /// </summary>
        /// <param name="selector">A selector that returns a calculator for the taxJar calcualtor, that returns a basic "success" result/</param>
        protected void SetupSuccess(out MockCalculatorSelector selector)
        { 
            MockCalculator calculator = new MockCalculator();
            calculator.OrderResponse = new TaxOnOrderResponse()
            { 
                Error=null, 
                TaxResponseValues = new TaxResponse()
                { 
                    amount_to_collect = 10.99M
                } 
            };

            selector = new MockCalculatorSelector();
            selector.SetCalculator(TaxCalculatorDesignation.TaxJar, calculator);
        }

        protected static Order MockOrder = new Order
        { 
            amount = 100.00M
        };

        /// <summary>
        /// When:
        ///     The body contains an order.
        ///     There are no problems with the tax calculator or its response
        /// Then:
        ///     A tax response should be returned.
        /// </summary>
        [TestMethod]
        public void Success()
        {
            //Setup
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            TaxOnOrderController controller = new TaxOnOrderController(calcSelect);

            //Run
            IActionResult result = controller.Post(MockOrder);

            //Verify
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsInstanceOfType(((JsonResult)result).Value, typeof(TaxResponse));
            Assert.AreEqual(10.99M, ((TaxResponse)((JsonResult)result).Value).amount_to_collect);
        }

        /// <summary>
        /// When:
        ///     The body is nto received/parsed
        /// Then:
        ///     A BadRequestObjectResult with the proper text
        /// </summary>
        [TestMethod]
        public void BodyIsNotReceived()
        { 
            //Setup
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            TaxOnOrderController controller = new TaxOnOrderController(calcSelect);

            //Run
            IActionResult result = controller.Post(null);

            //Verify
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(((BadRequestObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxOnOrderController.ErrorBodyNotReceived, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).description);
            Assert.IsNull(((ErrorResponse)(((BadRequestObjectResult)result).Value)).error_detail);
        }

        /// <summary>
        /// When:
        ///     The tax calculator service has a fatal error
        /// Then:
        ///     An ObjectResult should be returned with the expected descriptions and status code.
        /// </summary>
        [TestMethod]
        public void TaxCalculatorInternalError()
        { 
            //Setup
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            calcSelect.ReplaceCalculator(TaxCalculatorDesignation.TaxJar, new MockCalculator_Exception());

            TaxOnOrderController controller = new TaxOnOrderController(calcSelect);

            //Run
            IActionResult result = controller.Post(MockOrder);

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.FailedDependency, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxOnOrderController.TaxServiceInternalErrorText, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual(MockCalculator_Exception.OrderExceptionText, ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }

        /// <summary>
        /// When:
        ///     The tax calculator cannot proces the request, but does not have a fatal error.
        /// Then:
        ///     An ObjectResult should be returned with the expected descriptions and status code.
        /// </summary>
        [TestMethod]
        public void TaxCalculatorNoResults()
        { 
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            calcSelect.ReplaceCalculator(TaxCalculatorDesignation.TaxJar, new MockCalculator_NoResults());

            TaxOnOrderController controller = new TaxOnOrderController(calcSelect);

            //Run
            IActionResult result = controller.Post(MockOrder);

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxOnOrderController.TaxServiceCouldNotProcessRequest, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual(MockCalculator_NoResults.TaxesResponse.Error, ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }
    }
}
