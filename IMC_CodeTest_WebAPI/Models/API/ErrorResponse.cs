using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IMC_CodeTest_WebAPI.Models.API
{
    /// <summary>
    /// Describes errors from the api for the caller.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Description of the error tha occurred.
        /// </summary>
        public string description{get;set;}

        /// <summary>
        /// Status code for the error
        /// </summary>
        public HttpStatusCode status_code{get;set;}

        /// <summary>
        /// Addintional detail if appropriate.
        /// </summary>
        public string error_detail{get;set;}
    }
}
