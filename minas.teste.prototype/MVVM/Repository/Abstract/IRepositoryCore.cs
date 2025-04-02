using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace minas.teste.prototype.MVVM.Repository
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T Update(T entity);
        void Delete(int id);
        int SaveChanges();
    }
}