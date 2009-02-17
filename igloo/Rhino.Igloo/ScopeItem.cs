using System;
using System.Collections;
using System.Collections.Generic;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// Utility to for managing items in Scope
    /// </summary>
    /// <typeparam name="StronglyTypedValue">type of value in Scope</typeparam>
    public class ScopeItem<StronglyTypedValue>
    {
        /// <summary>
        /// Create new ScopeItems here
        /// </summary>
        /// <param name="key">name of the key for scope</param>
        /// <returns>new instance of ScopeItem</returns>
        public static ScopeItem<StronglyTypedValue> Create(string key)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(key), "ScopeItem key cannot be null or empty.");
            return new ScopeItem<StronglyTypedValue>(key);
        }

        private readonly string key;

        private ScopeItem(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Convert the item to a string
        /// </summary>
        /// <param name="item">ScopeItem to convert</param>
        /// <returns>value of key</returns>
        public static implicit operator string(ScopeItem<StronglyTypedValue> item)
        {
            return item.key;
        }

        /// <summary>
        /// Get value from Input
        /// </summary>
        public StronglyTypedValue ValueFromInput
        {
            get { return (StronglyTypedValue)ConversionUtil.ConvertTo(typeof(StronglyTypedValue), Scope.Input[this]); }
        }

        /// <summary>
        /// Get or Set value from Flash
        /// </summary>
        public StronglyTypedValue ValueFromFlash
        {
            get { return (StronglyTypedValue)Scope.Flash[this]; }
            set { Scope.Flash[this] = value; }
        }

        /// <summary>
        /// Get or set value from Session
        /// </summary>
        public StronglyTypedValue ValueFromSession
        {
            get { return (StronglyTypedValue)Scope.Session[this]; }
            set { Scope.Session[this] = value; }
        }

        /// <summary>
        /// Get value from anywhere in Scope. Progession: Input &gt; Flash &gt; Session
        /// </summary>
        public StronglyTypedValue ValueFromScope
        {
            get 
            {
                if (Scope.Input[this] != null)
                    return ValueFromInput;
                if (Scope.Flash[this] != null)
                    return ValueFromFlash;
                return ValueFromSession;
            }
        }
    }
}
