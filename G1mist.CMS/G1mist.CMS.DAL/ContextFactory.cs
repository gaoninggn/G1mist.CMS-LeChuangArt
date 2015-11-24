using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Runtime.Remoting.Messaging;
using G1mist.CMS.IDAL;
using G1mist.CMS.Modal;
using System.Data.Objects;

namespace G1mist.CMS.DAL
{
    public class ContextFactory
    {
        ///<summary>
        ///从工厂中获取EF上下文对象
        ///</summary>
        ///<returns>EF上下文对象</returns>
        public static ObjectContext GetCurrentDbContext()
        {
            //从本地线程池中获取上下文对象
            var db = CallContext.GetData("DBContext") as ObjectContext;

            //如果本地线程池中没有数据，则实例化一个上下文对象，并且把上下文对象放入本地线程池中
            if (db == null)
            {
                //TODO:实例化一个EFContext上下文对象，然后加入本地线程池中。
                db = new DB_CMSEntities();
                CallContext.SetData("DBContext", db);
            }

            return db;
        }
    }

    public class DalFactory<T> where T : class
    {
        ///<summary>
        ///从工厂中获取DAL对象
        ///</summary>
        ///<returns>DAL对象</returns>
        public static IBaseDal<T> GetCurrentDal()
        {
            return new SqlServerDal<T>();
            //return new MysqlDal<T>();
        }
    }
}
