using _3DUpdateTenenciesWithExcel.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq.Expressions;

namespace _3DUpdateTenenciesWithExcel.Controllers;

public class TenancyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TenancyController> _logger;
    private const int CLIENT_ID = 1; //Client ID in the [??].[dbo].[Clients]

    const int CompanyColumn = 1;
    const int DepartmentColumn = 2;
    const int TenancyNumberColumn = 3;
    const int AddressColumn = 4;
    const int DepositColumn = 5;
    const int RentColumn = 7;
    const int BbrSizeColumn = 8;
    const int GrossSizeColumn = 9;
    const int RoomsColumn = 10;
    const int ApartmentTypeColumn = 11;
    const int PostalCodeAndCityColumn = 99;
    const int FloorColumn = 99;
    const int IsAvailableColumn = 99;

    public TenancyController(ApplicationDbContext context, ILogger<TenancyController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult UploadExcel()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> UploadExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var client = await _context.Clients.Where(t => t.Id == CLIENT_ID).ToListAsync();

        List<Tenancy> tenanciesToUpdate = new List<Tenancy>();

        var d = await _context.Tenancies.Where(t => t.ClientId == CLIENT_ID).FirstOrDefaultAsync();


        using (var workbook = new XLWorkbook(file.OpenReadStream()))
        {
            var worksheet = workbook.Worksheet(1);
            int rowCount = worksheet.LastRowUsed().RowNumber() + 1;

            for (int row = 2; row <= rowCount; row++) // Assuming first row is header
            {
                // Parse Excel columns
                if (!int.TryParse(worksheet.Cell(row, CompanyColumn).Value.ToString(), out int demo))
                    continue;
                try
                {
                    string address = worksheet.Cell(row, AddressColumn).Value.ToString(); // Address column
                    string postalCodeAndCity = worksheet.Cell(row, PostalCodeAndCityColumn).Value.ToString(); // Postal code and city
                    int companyNo = int.Parse(worksheet.Cell(row, CompanyColumn).Value.ToString()); // Company
                    int departmentNo = int.Parse(worksheet.Cell(row, DepartmentColumn).Value.ToString()); // Department
                    int tenancyNo = int.Parse(worksheet.Cell(row, TenancyNumberColumn).Value.ToString()); // Tenancy Number
                    long rooms = long.Parse(worksheet.Cell(row, RoomsColumn).Value.ToString()); // Rooms
                    double size = double.Parse(worksheet.Cell(row, BbrSizeColumn).Value.ToString()); // BBR Size (m2)
                    double grossSize = double.Parse(worksheet.Cell(row, GrossSizeColumn).Value.ToString()); // Brutto m2
                    double netRent = double.Parse(worksheet.Cell(row, RentColumn).Value.ToString()); // Rent
                    double deposit = double.Parse(worksheet.Cell(row, DepositColumn).Value.ToString()); // Deposit
                    string apartmentTypeId = worksheet.Cell(row, ApartmentTypeColumn).Value.ToString(); // Apartment Type
                    string floor = worksheet.Cell(row, FloorColumn).Value.ToString(); // Floor (optional)
                    bool isAvailable = worksheet.Cell(row, IsAvailableColumn).Value.ToString().ToLower() == "yes"; // Genudlejet / IsAvailable
                    // Find Department is not Add
                    var depart = _context.Departments.FirstOrDefault(t => t.DepartmentNo == departmentNo && t.CompanyNo == companyNo);
                    if (depart == null)
                    {
                        depart = new Department
                        {
                            CompanyNo = companyNo,
                            DepartmentNo = departmentNo,
                            Name = "NorthCamp" + departmentNo,
                            Description = "",
                            ClientId = CLIENT_ID,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            UserId = "FromExcelImporter"
                        };
                        _context.Departments.Add(depart);
                        await _context.SaveChangesAsync();

                    }
                    // Find the tenancy in the database based on TenancyNo or another unique field
                    var existingTenancy = _context.Tenancies.FirstOrDefault(t => t.TenancyNo == tenancyNo
                    && t.CompanyNo == companyNo && t.DepartmentNo == departmentNo
                    && t.ClientId == CLIENT_ID);
                    if (existingTenancy != null)
                    {
                        // Update existing tenancy
                        //existingTenancy.CompanyNo = companyNo;
                        //existingTenancy.DepartmentNo = departmentNo;
                        //existingTenancy.IsAvailable = isAvailable;
                        existingTenancy.NetRent = netRent;
                        existingTenancy.GrossRent = netRent;
                        existingTenancy.Rooms = rooms;
                        existingTenancy.Size = size;
                        existingTenancy.Deposit = deposit;
                        _logger.LogInformation("Updating TenancyNo: {TenancyNo} with new values - NetRent: {NetRent}, GrossRent: {GrossRent}, Rooms: {Rooms}, Size: {Size}, Deposit: {Deposit}",
                                tenancyNo, existingTenancy.NetRent, existingTenancy.GrossRent, existingTenancy.Rooms, existingTenancy.Size, existingTenancy.Deposit);

                        //existingTenancy.ApartmentTypeId = apartmentTypeId;
                        existingTenancy.DateUpdated = DateTime.Now; // Update timestamp
                        existingTenancy.UserId = "FromExcelImporter"; // Update user ID
                                                                      // Handle address logic here (you might need to fetch AddressId based on the address fields)
                                                                      // existingTenancy.AddressId = resolveAddressId(address, postalCodeAndCity);

                        tenanciesToUpdate.Add(existingTenancy);

                    }
                    else
                    {
                        var add = new Address {
                            StreetAndLocation = address,
                            ZipCity = "7430 Ikast",//postalCodeAndCity,
                            ClientId = CLIENT_ID,
                            LocalTown = String.Empty,
                            AddressId = 0,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            UserId = "FromExcelImporter" };
                        _context.Addresses.Add(add);
                        
                        await _context.SaveChangesAsync();
                        add.AddressId = add.Id;
                        await _context.SaveChangesAsync();

                        //Add
                        var ten = new Tenancy
                        {
                            TenancyNo = tenancyNo,
                            CompanyNo = companyNo,
                            DepartmentNo = departmentNo,
                            IsAvailable = isAvailable,
                            NetRent = netRent,
                            GrossRent = netRent,
                            Rooms = rooms,
                            Size = size,
                            Deposit = deposit,
                            TenancyTypeId = 1,
                            ApartmentTypeId = 1,

                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            UserId = "FromExcelImporter",
                            AddressId = 0,
                            ClientId = CLIENT_ID
                        };
                        //ten.AddressList.Add(add);
                        
                        ten.AddressId = add.Id;
                        _context.Tenancies.Add(ten);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError("Exception on the row {msg}", ex.Message);
                }
            }
        }


        if (tenanciesToUpdate.Any())
        {
              _context.Tenancies.UpdateRange(tenanciesToUpdate);
              
              
        }
        return Ok("Tenancies updated successfully.");
    }
}
