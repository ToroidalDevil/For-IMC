
namespace IMC_CodeTest_WebAPI.Models.API
{
    public class TaxResponse_Shipping
    {
        public decimal city_amount{get;set;}

        public decimal city_tax_rate{get;set;}

        public decimal city_taxable_amount{get;set;}

        public decimal combined_tax_rate{get;set;}

        public decimal county_amount{get;set;}

        public decimal county_tax_rate{get;set;}

        public decimal county_taxable_amount{get;set;}

        public decimal special_district_amount{get;set;}

        public decimal special_tax_rate{get;set;}

        public decimal special_taxable_amount{get;set;}

        public decimal state_amount{get;set;}

        public decimal state_sales_tax_rate{get;set;}

        public decimal state_taxable_amount{get;set;}

        public decimal tax_collectable{get;set;}

        public decimal taxable_amount{get;set;}
    }
}
