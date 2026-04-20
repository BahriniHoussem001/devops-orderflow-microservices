using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Api.Models
{
    public class Order
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Product { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}