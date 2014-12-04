using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;

namespace ProjectDONE.Data
{
    public interface IAppRepoFactory<T> where T : IBaseAppModel 
    {
        IList<T> Get(int? skip, int? take);
        T GetSingle(long id);
        void Add(T item);
        void Remove(T item);
        void RemoveAll(IList<T> items);
        void Update(T item);
    }

    public interface IFactory_IJobRepo : IAppRepoFactory<IJob>
    {
        IList<IJob> GetByOwner(long ownerID, int? skip, int? take);
        
    }

    public interface IFactory_IBidRepo : IAppRepoFactory<IBid>
    {
        IList<IBid> GetByJob(long jobId, int skip, int take);
        IList<IBid> GetByOwner(long id, int skip, int take);

    }

}