using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.Models.TaxJar
{
    public class TaxJarRateResponseValues
    {
        public string city{get;set;}

        public decimal city_rate{get;set;}

        public decimal combined_district_rate{get;set;}

        public decimal combined_rate{get;set;}

        public string country{get;set;}

        public decimal country_rate{get;set;}

        public string county{get;set;}

        public decimal county_rate{get;set;}

        public bool freight_taxable{get;set;}

        public string state{get;set;}

        public decimal state_rate{get;set;}

        public string zip{get;set;}

        public RateResponse ConvertToServiceModel()
        {
            return new RateResponse()
            { 
                city = city,
                city_rate = city_rate,
                combined_district_rate = combined_district_rate,
                combined_rate = combined_rate,
                country = country,
                country_rate = country_rate,
                county = county,
                county_rate = county_rate,
                freight_taxable = freight_taxable,
                state = state,
                state_rate = state_rate,
                zip = zip
            };
        }
    }
}
