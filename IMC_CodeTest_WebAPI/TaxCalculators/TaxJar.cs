using IMC_CodeTest_WebAPI.Models.API;
using IMC_CodeTest_WebAPI.Models.TaxJar;
using IMC_CodeTest_WebAPI.TaxCalculators.Interfaces;

using Newtonsoft.Json;
using RestSharp;

using System.Collections.Generic;

namespace IMC_CodeTest_WebAPI.TaxCalculators
{
    public class TaxJar : ITaxCalculator
    {
        #region Constructor
        /// <summary>
        /// Service acces to the TaxJar client.
        /// </summary>
        /// <param name="restClient">A specific client to use. If not provided, then a default client is created with the uri as: 'https://api.taxjar.com/v2/' </param>
        /// <param name="apiToken">The api authorization token to use.</param>
        public TaxJar(IRestClient restClient = null, string apiToken = "")
        { 
            if(null == restClient)
                _taxJarRestClient = new RestClient("https://api.taxjar.com/v2/");
            else
                _taxJarRestClient = restClient;

            _apiToken = apiToken;
        }
        #endregion


        #region Data
        /// <summary>
        /// The token used for taxjar authentication
        /// </summary>
        protected string _apiToken = string.Empty;

        /// <summary>
        /// The client used to talk to the TaxJar service
        /// </summary>
        protected IRestClient _taxJarRestClient = null;
        #endregion


        #region Public functions
        public virtual TaxRateResponse GetTaxRates(string zip, string country, string city) 
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("country", country);
            parameters.Add("city", city);

            IRestRequest request = CreateRequest($"rates/{zip}", parameters);
            IRestResponse<TaxJarRateResponse> restResponse = _taxJarRestClient.Execute<TaxJarRateResponse>(request);

            string error = CheckResponseForErrors<TaxJarRateResponse>(restResponse);

            return new TaxRateResponse()
            { 
                Error = error,
                RateResponseValues = (null == error) ? (JsonConvert.DeserializeObject<TaxJarRateResponse>(restResponse.Content)).rate.ConvertToServiceModel() : null
            };
        }

        public virtual TaxOnOrderResponse CalculateTaxesForOrder(Order order)
        { 
            IRestRequest request = CreateRequest($"taxes", null, Method.POST, order);
            IRestResponse<TaxJarOrderCalculationResponse> restResponse = _taxJarRestClient.Execute<TaxJarOrderCalculationResponse>(request);

            string error = CheckResponseForErrors<TaxJarOrderCalculationResponse>(restResponse);

            return new TaxOnOrderResponse()
            { 
                Error = error,
                TaxResponseValues = (null == error) ? (JsonConvert.DeserializeObject<TaxJarOrderCalculationResponse>(restResponse.Content)).tax.ConvertToServiceModel() : null
            };
        }
        #endregion 


        #region Protected functions
        /// <summary>
        /// Checks the restResponse for errors. If errors are present an error string is generated.
        /// </summary>
        /// <typeparam name="T">Type of data expected in the restResponse</typeparam>
        /// <param name="restResponse">Rest respons to check</param>
        /// <returns>An error description if there was an error. Otherwise null</returns>
        protected virtual string CheckResponseForErrors<T>(IRestResponse<T> restResponse)
        { 
            if(restResponse.StatusCode >= System.Net.HttpStatusCode.BadRequest)
            {
                TaxJarError taxJarError = JsonConvert.DeserializeObject<TaxJarError>(restResponse.Content);
                return $"{taxJarError.status} - {taxJarError.error} - {taxJarError.detail}";
            }

            if(null != restResponse.ErrorException)
                return $"{restResponse.StatusCode} - {restResponse.ErrorMessage} - {restResponse.ErrorException.Message}";

            return null;
        }

        /// <summary>
        /// Creates a rest request
        /// </summary>
        /// <param name="action">TaxJar action. Ie. 'rates', 'taxes', etc.</param>
        /// <param name="parameters">Any querystring parameters that should be included with the request.</param>
        /// <param name="method">Whether this is GET, POST, etc.</param>
        /// <param name="body">Any information that should be included in the request body.</param>
        /// <returns>An IRestRequest readyto be sent to the Client</returns>
        protected virtual RestRequest CreateRequest(string action, Dictionary<string,string> parameters, Method method = Method.GET, object body = null)
        {
            RestRequest restRequest = new RestRequest(action, method)
            {
                RequestFormat = DataFormat.Json,
                //Could setup NewtonSoft as the Json serializer to use.
            };

            restRequest.AddHeader("Authorization", string.Concat("Bearer ", _apiToken));
            restRequest.Timeout = 3000;

            if(null != parameters)
            {
                foreach(KeyValuePair<string, string> parm in parameters)
                    restRequest.AddParameter(parm.Key, parm.Value);
            }

            if(body != null)
                restRequest.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            return restRequest;
        }
        #endregion
    }
}
