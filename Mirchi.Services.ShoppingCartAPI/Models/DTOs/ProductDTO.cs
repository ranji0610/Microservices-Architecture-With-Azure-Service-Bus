using System.ComponentModel.DataAnnotations;

namespace Mirchi.Services.ShoppingCartAPI.Models.DTOs
{
    public class ProductDTO
    {        
        public int ProductId { get; set; }   
        public string Name { get; set; }
     
        public double Price { get; set; }

        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
