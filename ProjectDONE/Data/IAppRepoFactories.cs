using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDONE.Data
{
    public interface IAppRepo<T> where T : IBaseAppModel 
    {
        IQueryable<T> Get();
        void Add(T item);
        void Remove(T item);
        void RemoveAll(IList<T> items);
        void Update(T item);
        bool Save();

    }

    public interface IJobRepo : IAppRepo<IJob>
    {
        //IList<IJob> GetByOwner(long ownerID, int skip, int take);
        
    }

    public interface IBidRepo : IAppRepo<IBid>
    {
       // IList<IBid> GetByJob(long jobId, int skip, int take);
       // IList<IBid> GetByOwner(long id, int skip, int take);

    }

    public interface IOwnerRepo : IAppRepo<IOwner>
    {

    }

}