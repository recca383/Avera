namespace SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime NowInPhilippines { get;  }
    string GetPhilippineTime(DateTime dateToConvert);
}
