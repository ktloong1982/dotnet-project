namespace Api.Models
{
  public class TransactionRequest
  {
    public string? partnerKey { get; set; }

    public string? partnerRefno { get; set; }

    public string? partnerPassword { get; set; }

    public long totalAmount { get; set; }

    public string? timestamp { get; set; }

    public string? sig { get; set; }
    public ItemDetailRequest[]? items { get; set; }

    public string IsValid()
    {
      string errMsg = "";
      if (string.IsNullOrEmpty(partnerKey))
      {
        errMsg = "Partner key is required";
      }
      else if (string.IsNullOrEmpty(partnerRefno))
      {
        errMsg = "Partner Ref No is required";
      }
      else if (string.IsNullOrEmpty(partnerPassword))
      {
        errMsg = "Partner's password is required";
      }
      else if (totalAmount <= 0)
      {
        errMsg = "Total amount must be greater than zero";
      }
      else if (string.IsNullOrEmpty(timestamp))
      {
        errMsg = "Timestamp is required";
      }
      else if (string.IsNullOrEmpty(sig))
      {
        errMsg = "Signature is required";
      }
      return errMsg;
    }
  }

  public class ItemDetailRequest
  {
    public string? partnerItemRef { get; set; }
    public string? name { get; set; }
    public int qty { get; set; }
    public long unitPrice { get; set; }

    public string IsValid()
    {
      string errMsg = string.Empty;
      if (string.IsNullOrEmpty(partnerItemRef))
      {
        errMsg = "Partner Item Ref is required";
      }
      else if (string.IsNullOrEmpty(name))
      {
        errMsg = "Name is required";
      }
      else if (qty < 1 || qty >= 5)
      {
        errMsg = "Quantity is out of range";
      }
      return errMsg;
    }
  }
}