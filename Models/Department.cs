namespace _3DUpdateTenenciesWithExcel.Models;

public class Department
{
    public int Id { get; set; }
    public int DepartmentNo { get; set; }
    public int CompanyNo { get; set; }
    public int? AddressId { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public string UserId { get; set; }
    public int ClientId { get; set; }
}

