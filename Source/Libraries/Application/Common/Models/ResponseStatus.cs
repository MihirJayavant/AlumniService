namespace Application.Common.Models;

public enum ResponseStatus
{
    Success = 200,
    Created = 201,
    BadRequest = 400,
    NotFound = 404,
    Conflict = 409,
    InternalError = 500
}
