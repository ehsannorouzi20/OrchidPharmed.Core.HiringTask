namespace OrchidPharmed.Core.HiringTask.API.Repositories
{
    public interface IUnitOfWork
    {
        IApplicationRepository ApplicationRepository { get; }
        Task CommitAsync();
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollBackTransaction();
    }
}
