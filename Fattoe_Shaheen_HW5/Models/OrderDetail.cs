using System;
using System.ComponentModel.DataAnnotations;

namespace Fattoe_Shaheen_HW5.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000.")]
        public int Quantity { get; set; }

        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Required(ErrorMessage = "Product price is required.")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Extended Price")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal ExtendedPrice { get; set; }

        // Navigation property to Order
        public Order Order { get; set; }

        // Navigation property to Product
        public Product Product { get; set; }
    }
}