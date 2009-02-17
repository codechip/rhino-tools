#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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