using System;
using System.IO;
using Core.Data.Entities;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Core.Data;
using System.Reflection;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace Core.Data
{
    public class NHibernateHelper
    {

        private static ISessionFactory _sessionFactory;
        private static ISessionFactory SessionFactory
        {
            get
            {

                if (_sessionFactory == null)

                    InitializeSessionFactory();


                return _sessionFactory;

            }

        }

        private static void InitializeSessionFactory()
        {

            _sessionFactory = Fluently.Configure()
                  .Database(MsSqlConfiguration.MsSql2008
                  .ConnectionString(
                                 @"Data Source=BEST\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True")
                         .ShowSql()
                         )

                  .Mappings(m =>
                      m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()).ExportTo(@"C:\"))

                  .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                  .BuildSessionFactory();         

        }
       
        public static ISession OpenSession()
        {

            return SessionFactory.OpenSession();

        }

    }

}

