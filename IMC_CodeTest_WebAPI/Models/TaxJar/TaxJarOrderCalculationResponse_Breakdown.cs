using IMC_CodeTest_WebAPI.Models.API;

using System.Collections.Generic;
using System.Linq;

namespace IMC_CodeTest_WebAPI.Models.TaxJar
{
    public class TaxJarOrderCalculationResponse_Breakdown
    {
        public decimal special_district_tax_collectable{get;set;}

        public decimal special_district_taxable_amount{get;set;}

        public decimal special_tax_rate{get;set;}

        public decimal state_tax_collectable{get;set;}

        public decimal state_tax_rate{get;set;}

        public decimal state_taxable_amount{get;set;}

        public decimal tax_collectable{get;set;}

        public decimal taxable_amount{get;set;}

        public TaxJarOrderCalculationResponse_Shipping shipping{get;set;}

        public List<TaxJarOrderCalculationResponse_LineItem> line_items{get;set;}

        public TaxResponse_Breakdown ConvertToServiceModel()
        { 
            return new TaxResponse_Breakdown()
            {
                special_district_tax_collectable = special_district_tax_collectable,
                special_district_taxable_amount = special_district_taxable_amount,
                special_tax_rate = special_tax_rate,
                state_tax_collectable = state_tax_collectable,
                state_tax_rate = state_tax_rate,
                state_taxable_amount = state_taxable_amount,
                tax_collectable = tax_collectable,
                taxable_amount = taxable_amount,

                shipping = shipping.ConvertToServiceModel(),

                line_items = line_items.Select((li) => {return li.ConvertToServiceModel();}).ToArray()
            };

        }
    }
}
