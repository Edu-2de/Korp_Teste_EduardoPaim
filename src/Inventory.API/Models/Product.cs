
using System.ComponentModel.DataAnnotations;

namespace Inventory.API.Models
{
  public class Product
  {
    public Guid Id {get; set;}

    [Required]
    [MaxLength(50)]
    public string Code {get; set;} = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description {get; set;}  = string.Empty;

    [Required]
    public int Balance {get; set;}

    [Timestamp]
    public byte[] RowVersion {get; set;}  = Array.Empty<byte>();
  }
}
