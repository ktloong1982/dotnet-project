namespace Api.Utils;

using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Api.Models;
using log4net;

public class PartnerService
{
  private List<Partner> _partners;
  private ILog _logger;
  public PartnerService()
  {
    _logger = LogManager.GetLogger(typeof(PartnerService));
    _partners = new List<Partner>();
    LoadMockDataAsync().Wait();
  }

  private async Task LoadMockDataAsync()
  {
    try
    {
      var filePath = Path.Combine(Directory.GetCurrentDirectory(), "MockData", "partners.json");
      var jsonData = await File.ReadAllTextAsync(filePath);
      _partners = JsonSerializer.Deserialize<List<Partner>>(jsonData) ?? new List<Partner>();
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }

    //Console.WriteLine(_partners[0].allowedPartner);
  }

  public bool ValidatePartner(string partnerkey, string partnerPassword)
  {
    bool blnStatus = false;
    try
    {
      blnStatus = _partners.Any(p => p.allowedPartner == partnerkey && p.partnerPassword == partnerPassword);
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return blnStatus;
  }

  public Partner GetPartnerObj(string partnerKey)
  {
    Partner partner = new Partner();
    try
    {
      partner = _partners.Where(p => p.allowedPartner == partnerKey).FirstOrDefault()
                   ?? throw new InvalidOperationException("Partner object not found");

    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return partner;
  }
}