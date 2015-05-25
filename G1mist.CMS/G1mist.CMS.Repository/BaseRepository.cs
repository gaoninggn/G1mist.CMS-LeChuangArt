using System;
using System.Collections.Generic;
using System.Linq;
using G1mist.CMS.IDAL;
using G1mist.CMS.IRepository;
using G1mist.CMS.DAL;

namespace G1mist.CMS.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IBaseDal<T> _dal;

        /// <summary>
        /// 
        /// </summary>
        public BaseRepository()
        {
            _dal = DalFactory<T>.GetCurrentDal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Insert(T entity)
        {
            return _dal.Insert(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            return _dal.Update(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            return _dal.Delete(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T GetModal(Func<T, bool> @where)
        {
            return _dal.GetList(where).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public ICollection<T> GetList(Func<T, bool> @where)
        {
            return _dal.GetList(where).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Exits(Func<T, bool> @where)
        {
            return _dal.GetList(where).Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="selector"></param>
        /// <param name="isAesc"></param>
        /// <returns></returns>
        public ICollection<T> GetList<TS>(Func<T, bool> @where, Func<T, TS> orderBy, Func<T, T> selector, bool isAesc = true)
        {
            return _dal.GetList(where, orderBy, selector, isAesc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="selector"></param>
        /// <param name="totalCount"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public ICollection<T> GetListByPage<TS>(int pageIndex, int pageSize, Func<T, bool> @where, Func<T, TS> orderBy, Func<T, T> selector, out int totalCount,
            bool isAsc = true)
        {
            return _dal.GetListByPage(pageIndex, pageSize, where, orderBy, selector, out totalCount);
        }
    }
}
