namespace Identity.Core.ObjectValue;

public class Error
{
    public Error()
    {
        
    }
    public Error(string code, ErrorMessage message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; }
    public ErrorMessage Message { get; set; }
}

public class ErrorMessage
{
    public ErrorMessage()
    {
        
    }
    public ErrorMessage(string title, string text)
    {
        Title = title;
        Text = text;
    }

    public string Title { get; set; }
    public string Text { get; set; }
}