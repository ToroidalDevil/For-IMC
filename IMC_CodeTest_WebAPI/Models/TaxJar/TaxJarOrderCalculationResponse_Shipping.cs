using IMC_CodeTest_WebAPI.Models.API;

namespace IMC_CodeTest_WebAPI.Models.TaxJar
{
    public class TaxJarOrderCalculationResponse_Shipping
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

        public TaxResponse_Shipping ConvertToServiceModel()
        { 
            return new TaxResponse_Shipping()
            { 
                city_amount = city_amount,
                city_tax_rate = city_tax_rate,
                city_taxable_amount = city_taxable_amount,
                combined_tax_rate = combined_tax_rate,
                county_amount = county_amount,
                county_tax_rate = county_tax_rate,
                county_taxable_amount = county_taxable_amount,
                special_district_amount = special_district_amount,
                special_tax_rate = special_tax_rate,
                special_taxable_amount = special_taxable_amount,
                state_amount = state_amount,
                state_sales_tax_rate = state_sales_tax_rate,
                state_taxable_amount = state_taxable_amount,
                tax_collectable = tax_collectable,
                taxable_amount = taxable_amount
            };
        }
    }
}
