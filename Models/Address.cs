using System.ComponentModel.DataAnnotations;

namespace _3DUpdateTenenciesWithExcel.Models;

public class Address
{
    [Key]
    public int Id { get; set; }
    public int? AddressId { get; set; }
    public string StreetAndLocation { get; set; }
    public string ZipCity { get; set; }
    public string? LocalTown { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string UserId { get; set; }
    public int ClientId { get; set; }
}
