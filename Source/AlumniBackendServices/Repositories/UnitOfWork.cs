//using Database;

//namespace AlumniBackendServices.Repositories;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly IApplicationDbContext context;
//    public IStudentRepository Student { get; }

//    public UnitOfWork(IApplicationDbContext context)
//    {
//        this.context = context;
//        Student = new StudentRepository(context);
//    }

//    public async Task<int> Complete() => await context.SaveChangesAsync();

//    public void Dispose() => context.Dispose();

//    public ValueTask DisposeAsync() => context.DisposeAsync();
//}
