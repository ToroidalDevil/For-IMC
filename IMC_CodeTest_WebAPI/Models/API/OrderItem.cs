
namespace IMC_CodeTest_WebAPI.Models.API
{
    /// <summary>
    /// Describes an item in an order.
    /// </summary>
    public class OrderItem
    {
        public string id{get;set;}

        public int quantity{get;set;}

        public string product_tax_code{get;set;}

        public decimal unit_price{get;set;}

        public int discount{get;set;}
    }
}
