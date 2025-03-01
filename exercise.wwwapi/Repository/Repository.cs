using System;
using System.Linq.Expressions;
using exercise.wwwapi.Data;
using Microsoft.EntityFrameworkCore;

namespace exercise.wwwapi.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private DataContext _db;
    private DbSet<T> _table = null;

    public Repository(DataContext db)
    {
        _db = db;
        _table = _db.Set<T>();
    }

    public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions)
    {
        IQueryable<T> query = _table;
        if (includeExpressions.Any())
        {
            query = includeExpressions
                .Aggregate(query, (current, expression) => current.Include(expression));
        }
        return query.ToList();
    }

    public IEnumerable<T> GetAll()
    {
        return _table.ToList();
    }

    public T GetById(object id)
    {
        return _table.Find(id);
    }

    public void Insert(T obj)
    {
        _table.Add(obj);
    }

    public void Update(T obj)
    {
        _table.Attach(obj);
        _db.Entry(obj).State = EntityState.Modified;
    }

    public void Delete(object id)
    {
        T existing = _table.Find(id);
        _table.Remove(existing);
    }

    public void Save()
    {
        _db.SaveChanges();
    }

    public DbSet<T> Table { get { return _table; } }
}
