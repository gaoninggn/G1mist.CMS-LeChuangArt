using System;
using System.Collections.Generic;

namespace G1mist.CMS.IDAL
{
    /// <summary>
    /// 数据访问层接口(IDAL)
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IBaseDal<T> where T : class
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Insert(T entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Delete(T entity);

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="where">where条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="selector">select条件</param>
        /// <param name="isAesc">是否正序</param>
        /// <returns></returns>
        List<T> GetList<TS>(Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector, bool isAesc = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where">where条件</param>
        /// <returns></returns>
        List<T> GetList(Func<T, bool> where);

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
        /// <param name="isAsc">是否升序,默认升序</param>
        /// <returns></returns>
        List<T> GetListByPage<TS>(int pageIndex, int pageSize, Func<T, bool> where, Func<T, TS> orderBy, Func<T, T> selector,
            out int totalCount, bool isAsc = true);

    }
}