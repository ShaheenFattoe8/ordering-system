using System;
using System.ComponentModel.DataAnnotations;

namespace Fattoe_Shaheen_HW5.Models
{
    public class Supplier
    {
        [Display(Name = "Supplier ID")]
        public int SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        [Required(ErrorMessage = "Supplier name is required.")]
        public string SupplierName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber { get; set; }

        // Navigation property for the many-to-many relationship with Products
        [Display(Name = "Products Provided")]
        public List<Product> Products { get; set; }

        // Constructor initializes the Products list to avoid null references
        public Supplier()
        {
            Products = Products ?? new List<Product>();
        }
    }
}