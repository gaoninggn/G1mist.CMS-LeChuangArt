using System.Data;
using System.Data.Objects;
using G1mist.CMS.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using G1mist.CMS.Modal;

namespace G1mist.CMS.DAL
{
    public class MysqlDal<T> : IBaseDal<T> where T : class
    {
        public bool Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public List<T> GetList<TS>(Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector, bool isAesc = true)
        {
            throw new NotImplementedException();
        }

        public List<T> GetList(Func<T, bool> where)
        {
            throw new NotImplementedException();
        }

        public List<T> GetListByPage<TS>(int pageIndex, int pageSize, Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector, out int totalCount, bool isAsc = true)
        {
            throw new NotImplementedException();
        }
    }
}