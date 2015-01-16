using System;
using System.Data.Entity;
using SandboxPolicy.Entities;

namespace SandboxPolicy.Repository
{
    public class UnitOfWork
    {
        private readonly DbContext _context;

        private BaseRepository<Transaction> _transactionRepository;
        private BaseRepository<Policy> _policyRepository;
 
        public UnitOfWork(DbContext context)
        {
            if (context != null && (context as SandboxPolicyEntities != null))
                _context = context;

            //wtf?
            throw new ArgumentException("Context not set or isn't the right type");
        }

        public BaseRepository<Transaction> TransactionRepository
        {
            get { return _transactionRepository ?? (_transactionRepository = new TransactionRepository(_context)); }
        }

        public BaseRepository<Policy> PolicyRepository
        {
            get { return _policyRepository ?? (_policyRepository = new PolicyRepository(_context)); }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
