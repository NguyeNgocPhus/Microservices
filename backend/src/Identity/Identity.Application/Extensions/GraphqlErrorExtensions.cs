using HotChocolate;

namespace Identity.Application.Extensions;

public static class GraphqlErrorExtensions
{
    public static IError BuildError(this IError error)
    {
        var e = "InternalServerError".GetError();
        // Handle null exception
        if (error.Exception == null)
            return error
                .WithCode(e.Code)
                .RemoveExtension("stackTrace")
                .RemoveLocations()
                .RemovePath()
                .RemoveExtension("message")
                .SetExtension("MessageKey", e.Message);
           

        // try to build the error with the defined code
        e = error.Exception.GetType().Name.GetError();
        return error
            .WithCode(e.Code)
            .RemoveExtension("stackTrace")
            .RemoveLocations()
            .RemovePath()
            .RemoveExtension("message")
            .SetExtension("TitleKey", e.Message.Title)
            .SetExtension("TextKey", e.Message.Text);
    }
}