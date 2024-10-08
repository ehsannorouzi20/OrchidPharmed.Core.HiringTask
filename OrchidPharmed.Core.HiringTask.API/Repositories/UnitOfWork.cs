
using AutoMapper;
using OrchidPharmed.Core.HiringTask.API.Models;

namespace OrchidPharmed.Core.HiringTask.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Models.ApplicationDbContext _db;
        public IApplicationRepository ApplicationRepository { get; }

        public UnitOfWork(Models.ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            ApplicationRepository = new ApplicationRepository(db, mapper);
        }

        public async System.Threading.Tasks.Task CommitAsync()
        {
            await _db.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task BeginTransaction()
        {
            await _db.Database.BeginTransactionAsync();
        }

        public async System.Threading.Tasks.Task CommitTransaction()
        {
            await _db.Database.CommitTransactionAsync();
        }

        public async System.Threading.Tasks.Task RollBackTransaction()
        {
            await _db.Database.RollbackTransactionAsync();
        }
    }
}
