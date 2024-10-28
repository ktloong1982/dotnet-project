namespace Api.Utils
{
  public class ValidatorService
  {
    public ValidatorService() { }

    public bool IsPrimeNumber(int number)
    {
      if (number < 2) return false;
      for (int i = 2; i <= Math.Sqrt(number); i++)
      {
        if (number % 2 == 0)
        {
          return false;
        }
      }
      return true;
    }

    public bool isLastDigitFive(int number)
    {
      return number % 10 == 5;
    }

  }
}