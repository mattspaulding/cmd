using ProjectDONE.Models.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDONE.Data
{
    public interface IAppRepo<T>
    {
        IQueryable<T> Get();
        void Add(T item);
        void Remove(T item);
        void RemoveAll(IList<T> items);
        void Update(T item);
        bool Save();

    }
}