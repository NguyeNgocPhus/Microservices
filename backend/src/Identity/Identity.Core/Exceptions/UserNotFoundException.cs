namespace Identity.Core.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string context, string key, string message):base(message)
    {
        Data["Context"] = context;
        Data["Key"] = key;
    }
}