namespace Core.Contracts.Response
{
    public class Response<T>
    {
       public T Result { get; set; }
       public ResponseStatus Status { get; set; }
       public ErrorResponse Error { get; set; }
    }
}
