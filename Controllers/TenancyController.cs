using _3DUpdateTenenciesWithExcel.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace _3DUpdateTenenciesWithExcel.Controllers;

public class TenancyController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TenancyController> _logger;
    private const int CLIENT_ID = 1; //Client ID in the [??].[dbo].[Clients]
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

        List<Tenancy> tenanciesToUpdate = new List<Tenancy>();

        var d = _context.Tenancies.Where(t => t.ClientId == 2).First();


        using (var workbook = new XLWorkbook(file.OpenReadStream()))
        {
            var worksheet = workbook.Worksheet("Type");
            int rowCount = worksheet.LastRowUsed().RowNumber() + 1;

            for (int row = 2; row <= rowCount; row++) // Assuming first row is header
            {
                // Parse Excel columns
                if (!int.TryParse(worksheet.Cell(row, 3).Value.ToString(), out int demo))
                    continue;
                try
                {
                    string address = worksheet.Cell(row, 1).Value.ToString(); // Address column
                    string postalCodeAndCity = worksheet.Cell(row, 2).Value.ToString(); // Postal code and city
                    int companyNo = int.Parse(worksheet.Cell(row, 3).Value.ToString()); // Company
                    int departmentNo = int.Parse(worksheet.Cell(row, 4).Value.ToString()); // Department
                    int tenancyNo = int.Parse(worksheet.Cell(row, 5).Value.ToString()); // Tenancy Number
                    long rooms = long.Parse(worksheet.Cell(row, 6).Value.ToString()); // Rooms
                    double size = double.Parse(worksheet.Cell(row, 7).Value.ToString()); // BBR Size (m2)
                    double grossSize = double.Parse(worksheet.Cell(row, 8).Value.ToString()); // Brutto m2
                    double netRent = double.Parse(worksheet.Cell(row, 9).Value.ToString()); // Rent
                    double deposit = double.Parse(worksheet.Cell(row, 10).Value.ToString()); // Deposit
                    string apartmentTypeId = worksheet.Cell(row, 11).Value.ToString(); // Apartment Type
                    string floor = worksheet.Cell(row, 12).Value.ToString(); // Floor (optional)
                    bool isAvailable = worksheet.Cell(row, 13).Value.ToString().ToLower() == "yes"; // Genudlejet / IsAvailable

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
                }
                catch (Exception ex)
                {

                    _logger.LogError("Exception skiffing the row {msg}", ex.Message);
                }
            }
        }


        if (tenanciesToUpdate.Any())
        {
              _context.Tenancies.UpdateRange(tenanciesToUpdate);
              await _context.SaveChangesAsync();
        }

        return Ok("Tenancies updated successfully.");
    }
}
