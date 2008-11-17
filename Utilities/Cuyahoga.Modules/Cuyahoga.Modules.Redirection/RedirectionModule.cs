using System;
using System.Data;
using Castle.Facilities.NHibernateIntegration;
using Cuyahoga.Core;
using Cuyahoga.Core.Domain;
using Cuyahoga.Modules.Redirection.Domain;
using NHibernate.Expression;
using System.Collections.Generic;

namespace Cuyahoga.Modules.Redirection
{
    public class RedirectionModule : ModuleBase, INHibernateModule
    {
        private readonly ISessionManager sessionManager;
        public bool ShouldRedirect
        {
            get{ return CurrentUrlId != null;}
        }

        public RedirectionModule(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        protected override void ParsePathInfo()
        {
            base.ParsePathInfo();
            if (ModuleParams != null)
            {
                if (ModuleParams.Length == 2)
                {
                    try
                    {
                        if ("redirect".Equals(ModuleParams[0],StringComparison.InvariantCultureIgnoreCase))
                        {
                            int result;
                            if (int.TryParse(ModuleParams[1], out result))
                                CurrentUrlId = result;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error when parsing module parameters: " + ModulePathInfo, ex);
                    }
                }
            }
        }

        public int? CurrentUrlId { get; set; }

        public IEnumerable<RedirectionUrl> GetAllUrls()
        {
            using(var session = sessionManager.OpenSession())
            using(var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var list = session.CreateCriteria(typeof (RedirectionUrl))
                    .Add(Expression.Eq("Section", Section))
                    .AddOrder(Order.Desc("DatePublished"))
                    .List();
                tx.Commit();
                var results = new List<RedirectionUrl>();
                foreach (RedirectionUrl url in list)
                {
                    results.Add(url);
                }
                return results;
            }
        }

        public string RedirectToCurrentFile()
        {
            using (var session = sessionManager.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var redirectionUrl = (RedirectionUrl)session.Load(typeof(RedirectionUrl), CurrentUrlId.Value);
                redirectionUrl.NumberOfDownloads += 1;
                tx.Commit();
                return redirectionUrl.Url;
            }
        }

        public RedirectionUrl GetbyId(int id)
        {
            using (var session = sessionManager.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                var url = (RedirectionUrl)session.Get(typeof(RedirectionUrl),id);
                tx.Commit();
                return url;
            } 
        }

        public void Save(RedirectionUrl url)
        {
            using (var session = sessionManager.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                session.SaveOrUpdate(url);  
                tx.Commit();
            } 
        }

        public void Delete(RedirectionUrl url)
        {
            using (var session = sessionManager.OpenSession())
            using (var tx = session.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                session.Delete(url);
                tx.Commit();
            } 
        }
    }
}