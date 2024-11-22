using System;
using System.ComponentModel.DataAnnotations;

namespace Fattoe_Shaheen_HW5.Models
{
    public enum ProductType { Hot, Cold, Packaged, Drink, Other }
    public class Product
    {
        // Auto-generated unique identifier for each product
        public int ProductID { get; set; }

        // Required product name
        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        // Optional product description
        [Display(Name = "Product Description (optional)")]
        public string? Description { get; set; }

        // Required price with a display format of $ and 2 decimal places
        [Required(ErrorMessage = "Price is required.")]
        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        // Enumeration for product type
        [Display(Name = "Product Type")]
        public ProductType ProductType { get; set; }

        public List<Supplier> Suppliers { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public Product()
        {
            if (Suppliers == null)
            {
                Suppliers = new List<Supplier>();
            }

            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}