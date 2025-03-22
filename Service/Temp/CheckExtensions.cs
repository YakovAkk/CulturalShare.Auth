namespace Service.Temp;

public class CheckExtensions
{
    public static bool IsNullOrEmpty<T>(IEnumerable<T> value)
    {
        return value == null || !value.Any();
    }
}
