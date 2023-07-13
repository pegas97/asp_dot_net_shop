﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WorldMarket.Data;
using WorldMarket.DataAccess.Repository.IRepository;

namespace WorldMarket.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
           _db = db;
            this.dbSet =_db.Set<T>();
        }
        public void Add(T entity)
        {
           dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(string? includeProperties = null )
        {
            IQueryable<T> query = dbSet;
            if(includeProperties != null)
            {
                foreach(var includProp in includeProperties.Split(new char[]{ ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includProp);
                }
                
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if(includeProperties != null)
            {
                foreach(var includProp in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includProp);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
           dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
