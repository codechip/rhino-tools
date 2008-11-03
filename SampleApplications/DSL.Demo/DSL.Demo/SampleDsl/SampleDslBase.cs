using System;
using System.Collections.Generic;
using DSL.Demo.Model;

namespace DSL.Demo.SampleDsl
{
    public abstract class SampleDslBase
    {
        #region Delegates

        protected object Entity;

        public delegate void Action();
        private readonly Dictionary<Type, Action> actions = 
            new Dictionary<Type, Action>();
        #endregion

        public void OnCreate(Type entity, Action action)
        {
            Console.WriteLine("Action registered for " + entity);
            actions[entity] = action;
        }

        public void BeginManualApprovalFor(Order order)
        {
            Console.WriteLine("Starting manual approval for " + order.Id);
        }

        public abstract void Prepare();

        public void Creating<T>(T entity)
        {
            Entity = entity;
            Action action = actions[typeof (T)];
            action();
        }
    }

    public class MyDsl : SampleDslBase
    {
        public override void Prepare()
        {
            OnCreate(typeof(Account),delegate
            {
                Account account = (Account) Entity;
                account.AccountNumber = DateTime.Now.Ticks;
            });

            OnCreate(typeof(Order), delegate
            {
                Order o = (Order) Entity;
                if(o.Account.MaxOrderTotal > o.Total)
                    BeginManualApprovalFor(o);
            });
        }
    }
}