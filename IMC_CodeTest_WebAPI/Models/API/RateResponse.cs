
namespace IMC_CodeTest_WebAPI.Models.API
{
    public class RateResponse
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
    }
}
