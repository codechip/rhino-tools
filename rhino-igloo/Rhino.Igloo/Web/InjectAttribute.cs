using System;

namespace Rhino.Igloo
{
    /// <summary>
    /// Specifies that a value should be injected to
    /// the annotated field or setter properties from the user's input
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field
                             | AttributeTargets.Property
                             | AttributeTargets.Parameter
                             | AttributeTargets.GenericParameter)]
    public sealed class InjectAttribute : Attribute
    {
        private string _name;

        /// <summary>
        /// The scope variable myName. Defaults to the name of
        /// the annotated field or getter method.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}