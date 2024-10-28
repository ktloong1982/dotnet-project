namespace Api.Models
{
  public class ResponseResult : BaseResponse
  {
    public long totalamount { get; set; }
    public long totaldiscount { get; set; }
    public long finalamount { get; set; }
  }
  public class BaseResponse
  {
    public int result { get; set; }

    public string? resultMessage { get; set; }
  }
}