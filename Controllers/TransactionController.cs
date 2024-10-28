using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Utils;
using log4net;
using System.Text.Json;

[Route("api/submittrxmessage")]
[ApiController]
public class TransactionController : ControllerBase
{

  private readonly PartnerService _partnerService;
  private readonly CryptographyService _cryptographyService;
  private readonly ValidatorService _validatorService;
  private readonly ILog _logger;

  public TransactionController()
  {
    _logger = LogManager.GetLogger(typeof(TransactionController));
    _partnerService = new PartnerService();
    _cryptographyService = new CryptographyService();
    _validatorService = new ValidatorService();
  }

  [HttpPost]
  public IActionResult submittrxmessage([FromBody] TransactionRequest request)
  {
    _logger.Info("[Request]" + JsonSerializer.Serialize(request));
    Partner partner = new Partner();
    ResponseResult response = new ResponseResult()
    {
      result = 1,
      resultMessage = ""
    };

    BaseResponse baseResponse = new BaseResponse()
    {
      result = 1,
      resultMessage = ""
    };
    try
    {
      if (request == null)
      {
        baseResponse.result = 0;
        baseResponse.resultMessage = "Invalid request data";
        _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
        return new JsonResult(baseResponse) { StatusCode = 501 };
      }

      string requestValidation = request.IsValid();
      if (!string.IsNullOrEmpty(requestValidation))
      {
        baseResponse.result = 0;
        baseResponse.resultMessage = requestValidation;
        _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
        return new JsonResult(baseResponse) { StatusCode = 501 };
      }

      string clearPartnerPassword = _cryptographyService.DecodeBased64String(request.partnerPassword ?? "");
      if (!_partnerService.ValidatePartner(request.partnerKey ?? "", clearPartnerPassword))
      {
        baseResponse.result = 0;
        baseResponse.resultMessage = "Access Denied!";
        _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
        return new JsonResult(baseResponse) { StatusCode = 502 };
      }
      else
      {
        partner = _partnerService.GetPartnerObj(request.partnerKey ?? "");
      }

      if (request.items != null && request.items.Length > 0)
      {
        bool bCheckValidItem = true;
        int idx = 1;
        foreach (ItemDetailRequest itemDetail in request.items)
        {
          string errMsg = itemDetail.IsValid();
          if (!string.IsNullOrEmpty(errMsg))
          {
            bCheckValidItem = false;
            baseResponse.result = 0;
            baseResponse.resultMessage = "Item #" + idx + ":" + errMsg;
            break;
          }
          idx++;
        }
        if (!bCheckValidItem)
        {
          _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
          return new JsonResult(baseResponse) { StatusCode = 501 };
        }

        long totalChecked = 0;
        foreach (var item in request.items)
        {
          totalChecked += (item.qty * item.unitPrice);
        }

        if (request.totalAmount != totalChecked)
        {
          baseResponse.result = 0;
          baseResponse.resultMessage = "Invalid Total Amount";
          _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
          return new JsonResult(baseResponse) { StatusCode = 504 };
        }
      }

      string newClearSig = DateTime.Parse(request.timestamp ?? "1970-01-01").ToString("yyyyMMddHHmmss") + partner.allowedPartner + partner.partnerNo + request.totalAmount + _cryptographyService.ConvertBased64String(partner.partnerPassword ?? "");
      string newSig = _cryptographyService.formMessageSignatureSha256(newClearSig);

      Console.WriteLine("newSig = " + newSig);
      DateTime timestamp = DateTime.Parse(request.timestamp ?? "1970-01-01", null, System.Globalization.DateTimeStyles.RoundtripKind);
      DateTime currentTimestamp = DateTime.UtcNow;
      TimeSpan timedifferent = currentTimestamp - timestamp;
      _logger.Info("[NewSignature] => " + JsonSerializer.Serialize(newSig));
      if (Math.Abs(timedifferent.TotalMinutes) > 5)
      {
        baseResponse.result = 0;
        baseResponse.resultMessage = "Expired ";
        _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
        return new JsonResult(baseResponse) { StatusCode = 505 };
      }

      if (request.sig != newSig)
      {
        baseResponse.result = 0;
        baseResponse.resultMessage = "Access Denied !";
        return new JsonResult(baseResponse) { StatusCode = 503 };
      }

      long discountAmount = CalculateTotalDiscount(request.totalAmount);
      long finalAmount = request.totalAmount - discountAmount;

      if (response.result == 1)
      {
        response.totalamount = request.totalAmount;
        response.totaldiscount = discountAmount;
        response.finalamount = finalAmount;
      }
      _logger.Info("[Response] => " + JsonSerializer.Serialize(baseResponse));
      return new JsonResult(response) { StatusCode = 200 };

    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
      baseResponse.result = 0;
      baseResponse.resultMessage = ex.Message;
      return new JsonResult(response) { StatusCode = 500 };

    }

  }

  private long CalculateTotalDiscount(long totalAmount)
  {
    long totalDiscount = 0;
    try
    {
      if (totalAmount >= 20000 && totalAmount <= 50000)
      {
        totalDiscount = totalAmount * 5 / 100;
      }
      else if (totalAmount >= 50100 && totalAmount <= 80000)
      {
        totalDiscount = totalAmount * 7 / 100;
      }
      else if (totalAmount >= 80100 && totalAmount <= 120000)
      {
        totalDiscount = totalAmount * 10 / 100;
      }
      else if (totalAmount > 120000)
      {
        totalDiscount = totalAmount * 15 / 100;
      }

      if (totalAmount > 50000 && _validatorService.IsPrimeNumber(Convert.ToInt32(totalAmount / 100)))
      {
        totalDiscount += totalAmount * 8 / 100;
      }

      if (totalAmount > 90000 && _validatorService.isLastDigitFive(Convert.ToInt32(totalAmount / 100)))
      {
        totalDiscount += totalAmount * 10 / 100;
      }

      if ((totalAmount * 20 / 100) < totalDiscount)
      {
        totalDiscount = totalAmount * 20 / 100;
      }
    }
    catch (Exception ex)
    {
      _logger.Error(ex.ToString());
    }
    return totalDiscount;
  }

}