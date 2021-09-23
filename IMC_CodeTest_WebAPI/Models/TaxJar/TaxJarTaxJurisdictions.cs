using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.Models.TaxJar
{
    public class TaxJarTaxJurisdictions
    {
        public string city{get;set;}

        public string country{get;set;}

        public string county{get;set;}

        public string state{get;set;}

        public TaxResponse_Jurisdictions ConvertToServiceModel()
        { 
            return new TaxResponse_Jurisdictions()
            {
                city = city,
                country = country,
                county = county,
                state = state
            };
        }
    }
}
