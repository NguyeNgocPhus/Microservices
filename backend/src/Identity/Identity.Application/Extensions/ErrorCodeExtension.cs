

using Identity.Core.ObjectValue;

namespace Identity.Application.Extensions;

public static class ErrorCodeExtension
{
    /// <summary>
    /// Build the error object with the errorStr code
    /// </summary>
    /// <param name="errorStr"></param>
    /// <returns></returns>
    public static Error GetError(this string errorStr)
    {
        var code = "";
        // Set default error as 500 if not found errorStr
        code = "999".PadLeft(6, '0');
        return new Error(code, new ErrorMessage($"t_{code}", $"m_{code}")); 
        
    }
    
}
