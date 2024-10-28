using System.Text;
using System.Security.Cryptography;
using log4net;
namespace Api.Utils;

public class CryptographyService
{
  private ILog _logger;
  public CryptographyService()
  {
    _logger = LogManager.GetLogger(typeof(CryptographyService));
  }

  public string DecodeBased64String(string stringBase64)
  {
    string retVal = "";
    try
    {
      byte[] data = Convert.FromBase64String(stringBase64);
      retVal = Encoding.UTF8.GetString(data);
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return retVal;
  }

  public string ConvertBased64String(string stringBase64)
  {
    string retVal = "";
    try
    {
      byte[] hashData = Encoding.UTF8.GetBytes(stringBase64);
      retVal = Convert.ToBase64String(hashData);
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return retVal;
  }

  public string formMessageSignatureSha256(string clearText)
  {
    string retVal = "";
    try
    {
      using (SHA256 sha256 = SHA256.Create())
      {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(clearText));
        retVal = Convert.ToBase64String(hashBytes);
      }
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return retVal;
  }
}