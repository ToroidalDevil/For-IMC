
namespace IMC_CodeTest_WebAPI.Models.API
{
    public class TaxResponse_Breakdown
    {
        public decimal special_district_tax_collectable{get;set;}

        public decimal special_district_taxable_amount{get;set;}

        public decimal special_tax_rate{get;set;}

        public decimal state_tax_collectable{get;set;}

        public decimal state_tax_rate{get;set;}

        public decimal state_taxable_amount{get;set;}

        public decimal tax_collectable{get;set;}

        public decimal taxable_amount{get;set;}

        public TaxResponse_Shipping shipping{get;set;}

        public TaxResponse_LineItem[] line_items{get;set;}
    }
}
