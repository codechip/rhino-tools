using System;
using System.Reflection;

namespace Rhino.Igloo
{
    /// <summary>
    ///  Utility methods that simplifies access to attribute.
    /// </summary>
    public static class AttributeUtil
    {
        /// <summary>
        /// Gets the <see cref="InjectAttribute"/> on properties.
        /// </summary>
        /// <param myName="memberInfo">The <see cref="MemberInfo"/>.</param>
        /// <returns>The attribute</returns>
        public static InjectAttribute GetInjectAttribute(MemberInfo memberInfo)
        {
            InjectAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectAttribute), true) as InjectAttribute;
            if (attribute != null)
            {
                attribute.Name = attribute.Name ?? memberInfo.Name;
            }
            return attribute;
        }

		/// <summary>
		/// Determines whether the specified type is a view.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is view; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsView(Type type)
    	{
    		return type.GetCustomAttributes(typeof (ViewAttribute), true).Length > 0;
    	}

        /// <summary>
        /// Gets the inject entity attribute.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static InjectEntityAttribute GetInjectEntityAttribute(MemberInfo memberInfo)
        {
            InjectEntityAttribute attribute = Attribute.GetCustomAttribute(memberInfo, typeof(InjectEntityAttribute), true) as InjectEntityAttribute;
            if (attribute != null)
            {
                attribute.Name = attribute.Name ?? memberInfo.Name;
            }
            return attribute;
        }

        /// <summary>
        /// Shoulds we the skip automatic registration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool ShouldSkipAutomaticRegistration(Type type)
        {
            return type.GetCustomAttributes(typeof (NoAutomaticRegistrationInContainer), true).Length != 0;
        }
    }
}