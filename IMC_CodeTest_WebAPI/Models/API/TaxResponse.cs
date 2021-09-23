
namespace IMC_CodeTest_WebAPI.Models.API
{
    public class TaxResponse
    {
        public decimal amount_to_collect{get;set;}

        public bool freight_taxable{get;set;}

        public bool has_nexus{get;set;}

        public decimal order_total_amount{get;set;}

        public decimal rate{get;set;}

        public decimal shipping{get;set;}

        public string tax_source{get;set;}

        public decimal taxable_amount{get;set;}

        public TaxResponse_Jurisdictions jurisdictions{get;set;}

        public TaxResponse_Breakdown breakdown{get;set;}
    }
}
