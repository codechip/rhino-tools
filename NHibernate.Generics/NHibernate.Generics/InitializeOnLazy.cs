using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Generics
{
    /// <summary>
    /// Determains whatever a set would initialize itself when
    /// an add / remove operation is called on it.
    /// Many-To-Many sets require this to be set.
    /// </summary>
    public enum InitializeOnLazy
    {
        Never,
        Always
    }

    /// <summary>
    /// Determains whatever a duplicate value can be add to the list.
    /// </summary>
    public enum OnDuplicate
    {
        DoNotAdd,
        Add
    }
}
