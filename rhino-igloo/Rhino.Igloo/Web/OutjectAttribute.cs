using System;

namespace Extranet.Infrastructure.Web
{
    /// <summary>
    /// Specifies that a component should be outjected to
    /// the annotated field or setter properties.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Property)]
    public  sealed class OutjectAttribute : Attribute
    {
        private string _name = string.Empty;
        private bool _isRequired = true;

        /// <summary>
        /// The context variable myName. Defaults to the name of
        /// the annotated field or getter method.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Specifies that the outjected value must not be
        /// null, by default true.
        /// </summary>
        public bool Required
        {
            get { return _isRequired; }
            set { _isRequired = value; }
        }
    }
}