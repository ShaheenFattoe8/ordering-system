using System;
using System.ComponentModel.DataAnnotations;

namespace Fattoe_Shaheen_HW5.Models
{
    public class Order
    {
        private const decimal TAX_RATE = 0.0825m;

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Order Number")]
        public int OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Notes")]
        public string OrderNotes { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal
        {
            get { return OrderDetails.Sum(od => od.ExtendedPrice); }
        }

        [Display(Name = "Sales Tax")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal SalesTax
        {
            get { return TAX_RATE * Subtotal; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal OrderTotal
        {
            get { return Subtotal + SalesTax; }
        }

        // Navigation property to AppUser (customer)
        public AppUser AppUser { get; set; }

        // Navigation property to OrderDetails
        public List<OrderDetail> OrderDetails { get; set; }

        // Constructor initializes OrderDetails list to avoid null references
        public Order()
        {
            OrderDetails = OrderDetails ?? new List<OrderDetail>();
        }
    }
}