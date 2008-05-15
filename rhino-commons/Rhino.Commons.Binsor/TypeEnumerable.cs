using System;
using System.Collections;
using System.Collections.Generic;

namespace Rhino.Commons.Binsor
{
    /// <summary>
    /// Allow to filter on the result of the AllTypes call
    /// </summary>
    public class TypeEnumerable : IEnumerable<Type>
    {
        private readonly IEnumerable<Type> inner;
        readonly List<Predicate<Type>> predicates = new List<Predicate<Type>>();

        /// <summary>
        /// Create new instanse
        /// </summary>
        /// <param name="inner"></param>
        public TypeEnumerable(IEnumerable<Type> inner)
        {
            this.inner = inner;
        }

        /// <summary>
        /// Pass a predicate for selection of matching types
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TypeEnumerable Where(Predicate<Type> predicate)
        {
            predicates.Add(predicate);
            return this;
        }

        /// <summary>
        /// Limit returned types to those in a particular namespace
        /// </summary>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        public TypeEnumerable WhereNamespaceEq(params string[] namespaces)
        {
            predicates.Add(delegate(Type type)
            {
                return Array.IndexOf(namespaces, type.Namespace) != -1;
            });
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Type>)this).GetEnumerator();
        }


        IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        private IEnumerable<Type> Enumerate()
        {
            foreach (Type type in inner)
            {
                bool match = true;
                foreach (Predicate<Type> predicate in predicates)
                {
                    if (predicate(type) == false)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    yield return type;
            };
        }
    }
}
