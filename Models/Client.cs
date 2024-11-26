using System.ComponentModel.DataAnnotations;

namespace _3DUpdateTenenciesWithExcel.Models;

public class Client
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string ClientKey { get; set; }
}