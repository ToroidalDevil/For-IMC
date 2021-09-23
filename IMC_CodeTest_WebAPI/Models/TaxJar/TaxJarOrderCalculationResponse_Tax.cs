using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.Models.TaxJar
{
    public class TaxJarOrderCalculationResponse_Tax
    {
        public decimal amount_to_collect{get;set;}

        public bool freight_taxable{get;set;}

        public bool has_nexus{get;set;}

        public decimal order_total_amount{get;set;}

        public decimal rate{get;set;}

        public decimal shipping{get;set;}

        public string tax_source{get;set;}

        public decimal taxable_amount{get;set;}

        public TaxJarTaxJurisdictions jurisdictions{get;set;}

        public TaxJarOrderCalculationResponse_Breakdown breakdown{get;set;}

        public TaxResponse ConvertToServiceModel()
        {
            return new TaxResponse()
            { 
                amount_to_collect = amount_to_collect,
                freight_taxable = freight_taxable,
                has_nexus = has_nexus,
                order_total_amount = order_total_amount,
                rate = rate,
                shipping = shipping,
                tax_source = tax_source,
                taxable_amount = taxable_amount,
                jurisdictions = jurisdictions.ConvertToServiceModel(),
                breakdown = breakdown.ConvertToServiceModel()
            };
        }
    }
}
