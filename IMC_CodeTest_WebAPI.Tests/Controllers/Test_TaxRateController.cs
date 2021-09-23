
using IMC_CodeTest_WebAPI.Controllers;
using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.TaxCalculators;
using IMC_CodeTest_WebAPI.Tests.Mocks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Net;

namespace IMC_CodeTest_WebAPI.Tests.Controllers
{
    [TestClass]
    public class Test_TaxRateController
    {
        /// <summary>
        /// Setup Mocks for a basic success scenario
        /// </summary>
        /// <param name="selector">A selector that returns a calculator for the taxJar calcualtor, that returns a basic "success" result/</param>
        protected void SetupSuccess(out MockCalculatorSelector selector)
        { 
            MockCalculator calculator = new MockCalculator();
            calculator.RateResponse = new TaxRateResponse()
            { 
                Error=null, 
                RateResponseValues= new RateResponse()
                { 
                    city = "mock"
                } 
            };

            selector = new MockCalculatorSelector();
            selector.SetCalculator(TaxCalculatorDesignation.TaxJar, calculator);
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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            calcSelect.ReplaceCalculator(TaxCalculatorDesignation.TaxJar, new MockCalculator_Exception());

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.FailedDependency, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.TaxServiceInternalErrorText, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual(MockCalculator_Exception.RateExceptionText, ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
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
            MockCalculatorSelector calcSelect = new MockCalculatorSelector();
            SetupSuccess(out calcSelect);

            calcSelect.ReplaceCalculator(TaxCalculatorDesignation.TaxJar, new MockCalculator_NoResults());

            TaxRateController controller = new TaxRateController(calcSelect);

            //Run
            IActionResult result = controller.Get("T-zip", "T-country", "T-city");

            //Verify
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.IsInstanceOfType(((ObjectResult)result).Value, typeof(ErrorResponse));
            Assert.AreEqual(HttpStatusCode.BadRequest, ((ErrorResponse)(((ObjectResult)result).Value)).status_code);
            Assert.AreEqual(TaxRateController.TaxServiceCouldNotProcessRequest, ((ErrorResponse)(((ObjectResult)result).Value)).description);
            Assert.AreEqual(MockCalculator_NoResults.RatesResponse.Error, ((ErrorResponse)(((ObjectResult)result).Value)).error_detail);
        }
    }
}
