using IMC_CodeTest_WebAPI.Controllers;
using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using System.Net;

namespace IMC_CodeTest_WebAPI.Tests.Controllers
{
    [TestClass]
    public class Test_TaxOnOrderController
    {
        /// <summary>
        /// Sets up a Mock TaxCalculatorSelector
        /// </summary>
        /// <param name="response">The response for taxcalculator that will be returned from thw selector</param>
        protected static ITaxCalculatorSelector Setup(TaxOnOrderResponse response)
        { 
            Mock<ITaxCalculator> calculator = new Mock<ITaxCalculator>();
            calculator.
                Setup((c) => c.CalculateTaxesForOrder(It.IsAny<Order>())).
                Returns(response);

            Mock<ITaxCalculatorSelector> selector = new Mock<ITaxCalculatorSelector>();
            selector.Setup(s => s.SelectCalculator(It.IsAny<TaxCalculatorDesignation>())).Returns(calculator.Object);

            return selector.Object;
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
            ITaxCalculatorSelector calcSelect = Setup(new TaxOnOrderResponse()
                {
                    Error = null, 
                    TaxResponseValues= new TaxResponse(){ amount_to_collect=10.99M }  
                }
            );

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
            ITaxCalculatorSelector calcSelect = Setup(null);

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
            Mock<ITaxCalculator> calculator = new Mock<ITaxCalculator>();
            calculator.
                Setup((c) => c.CalculateTaxesForOrder(It.IsAny<Order>())).
                Throws(new System.Exception("CalculateTaxesForOrder - intentional Exception"));

            Mock<ITaxCalculatorSelector> selector = new Mock<ITaxCalculatorSelector>();
            selector.Setup(s => s.SelectCalculator(It.IsAny<TaxCalculatorDesignation>())).Returns(calculator.Object);

            TaxOnOrderController controller = new TaxOnOrderController(selector.Object);

            //Run
            IActionResult result = controller.Post(MockOrder);

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.FailedDependency, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxOnOrderController.TaxServiceInternalErrorText, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual("CalculateTaxesForOrder - intentional Exception", ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
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
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(new TaxOnOrderResponse()
                {
                    Error = "Mock Taxes Error", 
                    TaxResponseValues= null  
                }
            );

            TaxOnOrderController controller = new TaxOnOrderController(calcSelect);

            //Run
            IActionResult result = controller.Post(MockOrder);

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxOnOrderController.TaxServiceCouldNotProcessRequest, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual("Mock Taxes Error", ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }
    }
}
