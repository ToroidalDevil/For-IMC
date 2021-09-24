using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.Models.TaxJar;
using IMC_CodeTest_WebAPI.TaxCalculators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using RestSharp;

using System;
using System.Net;

namespace IMC_CodeTest_WebAPI.Tests.TaxCalculators
{
    [TestClass]
    public class Test_TaxJar
    {
        #region TaxRates Tests
        /// <summary>
        /// Provides a mocked client for testing calls for tax rates
        /// </summary>
        /// <param name="content">Content of the response the client returns.</param>
        /// <param name="status">Status code the client returns.</param>
        /// <returns>The mocked client</returns>
        protected static Mock<IRestClient> SetupClientForRates(string content, HttpStatusCode status, Exception exception = null, string errorMessage = null)
        { 
            Mock<IRestClient> client = new Mock<IRestClient>();
            client.
                Setup(mc => mc.Execute<TaxJarRateResponse>(It.IsAny<IRestRequest>())).
                Returns(new RestResponse<TaxJarRateResponse>
                { 
                    StatusCode = status,
                    Content = content,
                    ErrorException = exception,
                    ErrorMessage = errorMessage
                });
            return client;
        }

        /// <summary>
        /// When:
        ///     A request for tax rates is made.
        ///     The request is proper
        /// Then:
        ///     The taxrates should be returned.
        /// </summary>
        [TestMethod]
        public void GetRates_Success()
        { 
            //Setup
            string content = 
@"{
  ""rate"": {
    ""zip"": ""90404"",
    ""state"": ""CA"",
    ""state_rate"": ""0.0625"",
    ""county"": ""LOS ANGELES"",
    ""county_rate"": ""0.01"",
    ""city"": ""SANTA MONICA"",
    ""city_rate"": ""0.0"",
    ""combined_district_rate"": ""0.025"",
    ""combined_rate"": ""0.0975"",
    ""freight_taxable"": false
  }
}";
            Mock<IRestClient> mockedClient =  SetupClientForRates(content, HttpStatusCode.OK);

            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxRateResponse trResponse = taxjar.GetTaxRates("T-zip", "t-county", "t-city");

            //Verify
            Assert.IsNotNull(trResponse);
            Assert.IsNotNull(trResponse.RateResponseValues);
            Assert.IsNull(trResponse.Error);
            Assert.AreEqual("90404", trResponse.RateResponseValues.zip);
            Assert.AreEqual("CA", trResponse.RateResponseValues.state);
            Assert.AreEqual(0.0625M, trResponse.RateResponseValues.state_rate);
            Assert.AreEqual("LOS ANGELES", trResponse.RateResponseValues.county);
            Assert.AreEqual(0.01M, trResponse.RateResponseValues.county_rate);
            Assert.AreEqual("SANTA MONICA", trResponse.RateResponseValues.city);
            Assert.AreEqual(0.0M, trResponse.RateResponseValues.city_rate);
            Assert.AreEqual(0.025M, trResponse.RateResponseValues.combined_district_rate);
            Assert.AreEqual(0.0975M, trResponse.RateResponseValues.combined_rate);
            Assert.AreEqual(false, trResponse.RateResponseValues.freight_taxable);
        }

        /// <summary>
        /// When:
        ///     A request for tax rates is made
        ///     An HttpStatusCode of BadRequest or higher is returned.
        /// Then:
        ///     Rates should not be returned
        ///     An appropriate error should be returned.
        /// </summary>
        [TestMethod]
        public void GetRates_Fail_BadRequest()
        { 
            //Setup
            string content = @"{""status"":""406"",""error"":""Not Acceptable"",""detail"":""country must be a two-letter ISO code.""}";
            Mock<IRestClient> mockedClient =  SetupClientForRates(content, HttpStatusCode.NotAcceptable);

            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxRateResponse trResponse = taxjar.GetTaxRates("T-zip", "t-county", "t-city");

            //Verify
            Assert.IsNotNull(trResponse);
            Assert.IsNull(trResponse.RateResponseValues);
            Assert.IsNotNull(trResponse.Error);
            Assert.AreEqual("406 - Not Acceptable - country must be a two-letter ISO code.", trResponse.Error);
        }


        /// <summary>
        /// When:
        ///     A request for tax rates is made
        ///     An Exception is returned
        /// Then:
        ///     Rates should not be returned
        ///     An appropriate error should be returned.
        /// </summary>
        [TestMethod]
        public void GetRates_Fail_Exception()
        { 
            //Setup
            string content = null;
            Mock<IRestClient> mockedClient =  SetupClientForRates(content, HttpStatusCode.OK, new Exception("Ketchup!"), "Momma tomato, Daddy tomato, and Baby tomato are walking, and baby tomato is falling behind. Daddy tomato turns around and stomps on baby tomato. What does he say?");
            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxRateResponse trResponse = taxjar.GetTaxRates("T-zip", "t-county", "t-city");

            //Verify
            Assert.IsNotNull(trResponse);
            Assert.IsNull(trResponse.RateResponseValues);
            Assert.IsNotNull(trResponse.Error);
            Assert.AreEqual("OK - Momma tomato, Daddy tomato, and Baby tomato are walking, and baby tomato is falling behind. Daddy tomato turns around and stomps on baby tomato. What does he say? - Ketchup!", trResponse.Error);
        }
        #endregion


        #region TaxOnOrder Tests
        /// <summary>
        /// Provides a mocked client for testing calls for tax rates
        /// </summary>
        /// <param name="content">Content of the response the client returns.</param>
        /// <param name="status">Status code the client returns.</param>
        /// <returns>The mocked client</returns>
        public static Mock<IRestClient> SetupClientForTaxOnOrder(string content, HttpStatusCode status, Exception exception = null, string errorMessage = null)
        { 
            Mock<IRestClient> client = new Mock<IRestClient>();
            client.
                Setup(mc => mc.Execute<TaxJarOrderCalculationResponse>(It.IsAny<IRestRequest>())).
                Returns(new RestResponse<TaxJarOrderCalculationResponse>
                { 
                    StatusCode = status,
                    Content = content,
                    ErrorException = exception,
                    ErrorMessage = errorMessage
                });
            return client;
        }

        /// <summary>
        /// When:
        ///     A request for taxOnOrder is made
        ///     The request is proper
        /// Then:
        ///     Taxes for the order should be returned.
        /// </summary>
        [TestMethod]
        public void GetTaxes_Success()
        { 
            //Setup
            string content = 
@"{
	""tax"": {
          ""amount_to_collect"": 1.71,
          ""freight_taxable"": true,
          ""has_nexus"": true,
          ""order_total_amount"": 29.95,
          ""rate"": 0.05711,
          ""shipping"": 10,
          ""tax_source"": ""destination"",
          ""taxable_amount"": 29.95,
          ""jurisdictions"": {
            ""city"": ""MAHOPAC"",
            ""country"": ""US"",
            ""county"": ""PUTNAM"",
            ""state"": ""NY""
          },
          ""breakdown"": {
            ""special_district_tax_collectable"": 0,
            ""special_district_taxable_amount"": 0,
            ""special_tax_rate"": 0,
            ""state_tax_collectable"": 0.4,
            ""state_tax_rate"": 0.04,
            ""state_taxable_amount"": 10,
            ""tax_collectable"": 1.71,
            ""taxable_amount"": 29.95,
            ""shipping"": {
              ""city_amount"": 0,
              ""city_tax_rate"": 0,
              ""city_taxable_amount"": 0,
              ""combined_tax_rate"": 0.08375,
              ""county_amount"": 0.44,
              ""county_tax_rate"": 0.04375,
              ""county_taxable_amount"": 10,
              ""special_district_amount"": 0,
              ""special_tax_rate"": 0,
              ""special_taxable_amount"": 0,
              ""state_amount"": 0.4,
              ""state_sales_tax_rate"": 0.04,
              ""state_taxable_amount"": 10,
              ""tax_collectable"": 0.84,
              ""taxable_amount"": 10
            },
            ""line_items"": [
              {
                ""city_amount"": 0,
                ""city_tax_rate"": 0,
                ""city_taxable_amount"": 0,
                ""combined_tax_rate"": 0.04375,
                ""county_amount"": 0.87,
                ""county_tax_rate"": 0.04375,
                ""county_taxable_amount"": 19.95,
                ""id"": ""1"",
                ""special_district_amount"": 0,
                ""special_district_taxable_amount"": 0,
                ""special_tax_rate"": 0,
                ""state_amount"": 0,
                ""state_sales_tax_rate"": 0,
                ""state_taxable_amount"": 0,
                ""tax_collectable"": 0.87,
                ""taxable_amount"": 19.95
              }
            ]
          }
        }
}";
            Mock<IRestClient> mockedClient =  SetupClientForTaxOnOrder(content, HttpStatusCode.OK);
            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxOnOrderResponse tooResponse = taxjar.CalculateTaxesForOrder(new Order());

            //Verify
            Assert.IsNotNull(tooResponse);
            Assert.IsNotNull(tooResponse.TaxResponseValues);
            Assert.IsNull(tooResponse.Error);

            //Response
            Assert.AreEqual(1.71M, tooResponse.TaxResponseValues.amount_to_collect);
            Assert.AreEqual(true, tooResponse.TaxResponseValues.freight_taxable);
            Assert.AreEqual(true, tooResponse.TaxResponseValues.has_nexus);
            Assert.AreEqual(29.95M, tooResponse.TaxResponseValues.order_total_amount);
            Assert.AreEqual(0.05711M, tooResponse.TaxResponseValues.rate);
            Assert.AreEqual(10M, tooResponse.TaxResponseValues.shipping);
            Assert.AreEqual("destination", tooResponse.TaxResponseValues.tax_source);
            Assert.AreEqual(29.95M, tooResponse.TaxResponseValues.taxable_amount);

            //Response.Jurisdictions
            Assert.AreEqual("MAHOPAC", tooResponse.TaxResponseValues.jurisdictions.city);
            Assert.AreEqual("US", tooResponse.TaxResponseValues.jurisdictions.country);
            Assert.AreEqual("PUTNAM", tooResponse.TaxResponseValues.jurisdictions.county);
            Assert.AreEqual("NY", tooResponse.TaxResponseValues.jurisdictions.state);

            //Response.Breakdown
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.special_district_tax_collectable);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.special_district_taxable_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.special_tax_rate);
            Assert.AreEqual(0.4M, tooResponse.TaxResponseValues.breakdown.state_tax_collectable);
            Assert.AreEqual(0.04M, tooResponse.TaxResponseValues.breakdown.state_tax_rate);
            Assert.AreEqual(10M, tooResponse.TaxResponseValues.breakdown.state_taxable_amount);
            Assert.AreEqual(1.71M, tooResponse.TaxResponseValues.breakdown.tax_collectable);
            Assert.AreEqual(29.95M, tooResponse.TaxResponseValues.breakdown.taxable_amount);

            //Response.breakdown.shipping
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.city_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.city_tax_rate);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.city_taxable_amount);
            Assert.AreEqual(0.08375M, tooResponse.TaxResponseValues.breakdown.shipping.combined_tax_rate);
            Assert.AreEqual(0.44M, tooResponse.TaxResponseValues.breakdown.shipping.county_amount);
            Assert.AreEqual(0.04375M, tooResponse.TaxResponseValues.breakdown.shipping.county_tax_rate);
            Assert.AreEqual(10M, tooResponse.TaxResponseValues.breakdown.shipping.county_taxable_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.special_district_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.special_tax_rate);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.shipping.special_taxable_amount);
            Assert.AreEqual(0.4M, tooResponse.TaxResponseValues.breakdown.shipping.state_amount);
            Assert.AreEqual(0.04M, tooResponse.TaxResponseValues.breakdown.shipping.state_sales_tax_rate);
            Assert.AreEqual(10M, tooResponse.TaxResponseValues.breakdown.shipping.state_taxable_amount);
            Assert.AreEqual(0.84M, tooResponse.TaxResponseValues.breakdown.shipping.tax_collectable);
            Assert.AreEqual(10M, tooResponse.TaxResponseValues.breakdown.shipping.taxable_amount);

            //Response.breakdown.lineitem[0]
            Assert.AreEqual(1, tooResponse.TaxResponseValues.breakdown.line_items.Length);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].city_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].city_tax_rate);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].city_taxable_amount);
            Assert.AreEqual(0.04375M, tooResponse.TaxResponseValues.breakdown.line_items[0].combined_tax_rate);
            Assert.AreEqual(0.87M, tooResponse.TaxResponseValues.breakdown.line_items[0].county_amount);
            Assert.AreEqual(0.04375M, tooResponse.TaxResponseValues.breakdown.line_items[0].county_tax_rate);
            Assert.AreEqual(19.95M, tooResponse.TaxResponseValues.breakdown.line_items[0].county_taxable_amount);
            Assert.AreEqual("1", tooResponse.TaxResponseValues.breakdown.line_items[0].id);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].special_district_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].special_district_taxable_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].special_tax_rate);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].state_amount);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].state_sales_tax_rate);
            Assert.AreEqual(0M, tooResponse.TaxResponseValues.breakdown.line_items[0].state_taxable_amount);
            Assert.AreEqual(0.87M, tooResponse.TaxResponseValues.breakdown.line_items[0].tax_collectable);
            Assert.AreEqual(19.95M, tooResponse.TaxResponseValues.breakdown.line_items[0].taxable_amount);
        }

        /// <summary>
        /// When:
        ///     A request for taxOnOrder is made
        ///     An HttpStatusCode of BadRequest or higher is returned.
        /// Then:
        ///     Taxes should not be returned
        ///     An appropriate error should be returned.
        /// </summary>
        [TestMethod]
        public void GetTaxes_Fail_BadRequest()
        { 
            //Setup
            string content = @"{""status"":""406"",""error"":""Not Acceptable"",""detail"":""No line items present""}";
            Mock<IRestClient> mockedClient =  SetupClientForTaxOnOrder(content, HttpStatusCode.NotAcceptable);

            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxOnOrderResponse tooResponse = taxjar.CalculateTaxesForOrder(new Order());

            //Verify
            Assert.IsNotNull(tooResponse);
            Assert.IsNull(tooResponse.TaxResponseValues);
            Assert.IsNotNull(tooResponse.Error);
            Assert.AreEqual("406 - Not Acceptable - No line items present", tooResponse.Error);
        }


        /// <summary>
        /// When:
        ///     A request for taxOnOrder is made
        ///     An Exception is returned
        /// Then:
        ///     Rates should not be returned
        ///     An appropriate error should be returned.
        /// </summary>
        [TestMethod]
        public void GetTaxes_Fail_Exception()
        { 
            //Setup
            string content = null;
            Mock<IRestClient> mockedClient =  SetupClientForTaxOnOrder(content, HttpStatusCode.OK, new Exception("This should fail"), "Failure because");
            TaxJar taxjar = new TaxJar(mockedClient.Object);

            //Run
            TaxOnOrderResponse tooResponse = taxjar.CalculateTaxesForOrder(new Order());

            //Verify
            Assert.IsNotNull(tooResponse);
            Assert.IsNull(tooResponse.TaxResponseValues);
            Assert.IsNotNull(tooResponse.Error);
            Assert.AreEqual("OK - Failure because - This should fail", tooResponse.Error);
        }
        #endregion
    }
}
