namespace AlumniBackendServices.Repositories;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IStudentRepository Student { get; }

    Task<int> Complete();
}
