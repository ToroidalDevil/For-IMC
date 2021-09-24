
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
    public class Test_TaxRateController
    {
        /// <summary>
        /// Generates a Mock TaxCalculatorSelector. Sets up the response for the GetTaxRates call. Returns the response provided.
        /// </summary>
        /// <param name="response">The response to be returned from the TaxCalculators call to GetTaxRates</param>
        /// <returns>Mocked TaxCalculatorSelector</returns>
        protected ITaxCalculatorSelector Setup(TaxRateResponse response)
        { 
            Mock<ITaxCalculator> calculator = new Mock<ITaxCalculator>();
            calculator.
                Setup(c=> c.GetTaxRates(
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)), 
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)),
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)))).
                Returns(response);

            Mock<ITaxCalculatorSelector> selector = new Mock<ITaxCalculatorSelector>();
            selector.
                Setup(s => s.SelectCalculator(It.IsAny<TaxCalculatorDesignation>())).
                Returns(calculator.Object);

            return selector.Object;
        }

        /// <summary>
        /// When:
        ///     Query parameters are valid
        ///     There are no problems with the tax calculator or its response
        /// Then:
        ///     A JsonResult with the rate values should be returned.
        /// </summary>
        [TestMethod]
        public void Success()
        { 
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(new TaxRateResponse()
                {
                    Error=null, 
                    RateResponseValues = new RateResponse(){city="mock"} 
                }
            );

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsInstanceOfType(((JsonResult)result).Value, typeof(RateResponse));
            Assert.AreEqual("mock", ((RateResponse)((JsonResult)result).Value).city);
        }

        /// <summary>
        /// When:
        ///     Query parameter zip is not provided.
        /// Then:
        ///     A BadRequestObjectResult should be returned with relevant description
        /// </summary>
        [TestMethod]
        public void QueryParamIsInValid_City()
        { 
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(null);

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get(string.Empty, "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(((BadRequestObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.ParmFailureError_Zip, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).description);
            Assert.IsNull(((ErrorResponse)(((BadRequestObjectResult)result).Value)).error_detail);
        }

        /// <summary>
        /// When:
        ///     Query parameter state is not provided.
        /// Then:
        ///     A BadRequestObjectResult should be returned with relevant description
        /// </summary>
        [TestMethod]
        public void QueryParamIsInValid_State()
        { 
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(null);

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", string.Empty, "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(((BadRequestObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.ParmFailureError_Country, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).description);
            Assert.IsNull(((ErrorResponse)(((BadRequestObjectResult)result).Value)).error_detail);
        }

        /// <summary>
        /// When:
        ///     Query parameter city is not provided.
        /// Then:
        ///     A BadRequestObjectResult should be returned with relevant description
        /// </summary>
        [TestMethod]
        public void QueryParamIsInValid_Zip()
        { 
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(null);

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", string.Empty);

            //Verify
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(((BadRequestObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.ParmFailureError_City, ((ErrorResponse)(((BadRequestObjectResult)result).Value)).description);
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
                Setup(c=> c.GetTaxRates(
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)), 
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)),
                    It.Is<string>(s => !string.IsNullOrWhiteSpace(s)))).
                Throws(new System.Exception("GetTaxRates - intentional Exception"));

            Mock<ITaxCalculatorSelector> calcSelect = new Mock<ITaxCalculatorSelector>();
            calcSelect.Setup(s => s.SelectCalculator(It.IsAny<TaxCalculatorDesignation>())).Returns(calculator.Object);

            TaxRateController controller = new TaxRateController(calcSelect.Object);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.FailedDependency, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.TaxServiceInternalErrorText, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual("GetTaxRates - intentional Exception", ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }

        /// <summary>
        /// When:
        ///     The tax calcualtor cannot proces the request, but does not have a fatal error.
        /// Then:
        ///     An ObjectResult should be returned with the expected descriptions and status code.
        /// </summary>
        [TestMethod]
        public void TaxCalculatorNoResults()
        { 
            //Setup
            ITaxCalculatorSelector calcSelect = Setup(new TaxRateResponse(){Error= "Mock Rates Error", RateResponseValues= null});

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.TaxServiceCouldNotProcessRequest, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual("Mock Rates Error", ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }
    }
}
