using System.ComponentModel.DataAnnotations;

namespace Api.Models;
public class Partner
{
  public string? partnerNo { get; set; }
  public string? allowedPartner { get; set; }
  public string? partnerPassword { get; set; }
}