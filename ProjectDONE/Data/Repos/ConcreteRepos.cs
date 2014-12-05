using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectDONE.Models.AppModels;

namespace ProjectDONE.Data.Repos
{
    //TODO: Refactor to return IQuerable rather than concrete Lists
    public class BaseRepo<T> : IAppRepo<T> where T : BaseAppModel
    {
        private ApplicationDbContext _context = null;
        public ApplicationDbContext Context
        {
            get { return _context ?? (_context = ApplicationDbContext.Create()); }
        }
        public void Add(T item)
        {
            Context.Set<T>().Add(item);
        }
        public IQueryable<T> Get()
        {
            return Context.Set<T>();
        }

        public void Remove(T item)
        {
            //TODO: Handle Soft Delete
            throw new NotImplementedException();
        }

        public void RemoveAll(IList<T> items)
        {
            //TODO: Handle Soft Delete
            throw new NotImplementedException();
        }

        public void Update(T item)
        {
            Context.Entry<T>(item).State = System.Data.Entity.EntityState.Modified;
        }

        public virtual bool Save()
        {
            return Context.SaveChanges() > 0;
        }
    }

    public class JobRepo : BaseRepo<Job>
    {
    }

    public class BidRepo : BaseRepo<Bid>
    {
        
    }

    public class OwnerRepo : BaseRepo<Owner>
    {

    }

   
}