using System.ComponentModel.DataAnnotations;

namespace _3DUpdateTenenciesWithExcel.Models;

public class Tenancy
{
    [Key]
    public int Id { get; set; }
    public int TenancyNo { get; set; }
    public int CompanyNo { get; set; }
    public int DepartmentNo { get; set; }
    public bool IsAvailable { get; set; }
    
    public double NetRent { get; set; }
    public double GrossRent { get; set; }
    public long Rooms { get; set; }
    public double Size { get; set; }
    public double Deposit { get; set; }
    public int TenancyTypeId { get; set; }
    public int ApartmentTypeId { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
    public string UserId { get; set; }
    public int ClientId { get; set; }

    public int? AddressId { get; set; }
    
    // Default constructor
    public Tenancy() { }

    // Parameterized constructor
    public Tenancy(
        int id,
        int tenancyNo,
        int companyNo,
        int departmentNo,
        bool isAvailable,
        int? addressId,
        double netRent,
        double grossRent,
        long rooms,
        double size,
        double deposit,
        int tenancyTypeId,
        int apartmentTypeId,
        DateTime dateCreated,
        DateTime dateUpdated,
        string userId,
        int clientId)
    {
        Id = id;
        TenancyNo = tenancyNo;
        CompanyNo = companyNo;
        DepartmentNo = departmentNo;
        IsAvailable = isAvailable;
        AddressId = addressId;
        NetRent = netRent;
        GrossRent = grossRent;
        Rooms = rooms;
        Size = size;
        Deposit = deposit;
        TenancyTypeId = tenancyTypeId;
        ApartmentTypeId = apartmentTypeId;
        DateCreated = dateCreated;
        DateUpdated = dateUpdated;
        UserId = userId;
        ClientId = clientId;
    }
}
