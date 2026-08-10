namespace HanYu.Application.Interfaces.DateTime;

public interface IDateTimeProvider
{
    System.DateTime UtcNow { get; }
}