using System.Data;
using System.Data.Objects;
using G1mist.CMS.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using G1mist.CMS.Modal;

namespace G1mist.CMS.DAL
{
    public class SqlServerDal<T> : IBaseDal<T> where T : class
    {
        public ObjectContext Context { get; set; }

        public SqlServerDal()
        {
            Context = ContextFactory.GetCurrentDbContext();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体类型</param>
        /// <returns></returns>
        public bool Insert(T entity)
        {
            Context.CreateObjectSet<T>().AddObject(entity);

            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体类型</param>
        /// <returns></returns>
        public bool Update(T entity)
        {
            if (!IsAttached(entity))
            {
                Context.CreateObjectSet<T>().Attach(entity);
            }

            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体类型</param>
        /// <returns></returns>
        public bool Delete(T entity)
        {
            if (!IsAttached(entity))
            {
                Context.CreateObjectSet<T>().Attach(entity);
            }

            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);

            return Context.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="where">where条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="selector">select条件</param>
        /// <param name="isAesc">是否正序</param>
        /// <returns></returns>
        public List<T> GetList<TS>(Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector, bool isAesc = true)
        {
            var list = isAesc ? Context.CreateObjectSet<T>().Where(where).OrderBy(orderBy).Select(selector).ToList() : Context.CreateObjectSet<T>().Where(where).OrderByDescending(orderBy).Select(selector).ToList();

            return list;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="where">where条件</param>
        /// <returns></returns>
        public List<T> GetList(Func<T, bool> @where)
        {
            var list = Context.CreateObjectSet<T>().Where(where).ToList();

            return list;
        }

        /// <summary>
        /// 分页获取实体列表
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="skip">忽略数量</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where查询条件</param>
        /// <param name="orderBy">order排序条件</param>
        /// <param name="selector">select条件</param>
        /// <param name="totalCount">总数</param>
        /// <param name="isAesc">是否升序(默认为升序)</param>
        /// <returns></returns>
        public List<T> GetListByPage<TS>(int skip, int pageSize, Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector,
    out int totalCount, bool isAesc = true)
        {
            //总数
            totalCount = Context.CreateObjectSet<T>().Where(where).Count();

            //结果集
            List<T> listResult;

            if (isAesc) //升序
            {
                listResult = Context.CreateObjectSet<T>().Where(where).Select(a => a)
                   .OrderBy(orderBy).Skip(skip)
                   .Take(pageSize).Select(selector).ToList();
            }
            else
            {
                //降序
                listResult = Context.CreateObjectSet<T>().Where(where).Select(a => a)
                    .OrderByDescending(orderBy).Skip(skip)
                    .Take(pageSize).Select(selector).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// 分页获取实体列表
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where查询条件</param>
        /// <param name="orderBy">order排序条件</param>
        /// <param name="selector">select条件</param>
        /// <param name="totalCount">总数</param>
        /// <param name="isAesc">是否升序(默认为升序)</param>
        /// <returns></returns>
        //public List<T> GetListByPage<TS>(int pageIndex, int pageSize, Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector,
        //    out int totalCount, bool isAesc = true)
        //{
        //    //总数
        //    totalCount = Context.CreateObjectSet<T>().Count();

        //    //结果集
        //    List<T> listResult;

        //    if (isAesc) //升序
        //    {
        //        listResult = Context.CreateObjectSet<T>().Where(where).Select(a => a)
        //           .OrderBy(orderBy).Skip((pageIndex - 1) * pageSize)
        //           .Take(pageSize).Select(selector).ToList();
        //    }
        //    else
        //    {
        //        //降序
        //        listResult = Context.CreateObjectSet<T>().Where(where).Select(a => a)
        //            .OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize)
        //            .Take(pageSize).Select(selector).ToList();
        //    }
        //    return listResult;
        //}

        /// <summary>
        /// 判断实体是否已经被EF附加(跟踪)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsAttached(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            ObjectStateEntry entry;

            if (Context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                return (entry.State != EntityState.Detached);
            }

            return false;
        }
    }
}