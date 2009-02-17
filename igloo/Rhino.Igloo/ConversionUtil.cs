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
    /// Apply heuristics when converting types.
    /// </summary>
    public static class ConversionUtil
    {
        /// <summary>
        /// Converts array of one type to another, applying heuristics as neccecary
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="inject">The inject.</param>
        /// <exception cref="System.InvalidOperationException">thrown if Type.HasElementType is false (is not an array)</exception>
        /// <returns></returns>
        public static object ConvertTo(Type type, string[] inject)
        {
            if (!type.HasElementType)
            {
                throw new InvalidOperationException("Type " + type.FullName + " is not an array, and cannot be converted as such");
            }
            if (inject == null)
            {
                return !type.IsValueType ? null : Activator.CreateInstance(type, 0);
            }
            if (type.IsInstanceOfType(inject))
            {
                return inject;
            }

            Array array = (Array)Activator.CreateInstance(type, inject.Length);
            Type elementType = type.GetElementType();
            for(int i = 0; i < inject.Length; i++)
            {
                array.SetValue(ConvertTo(elementType, inject[i]), i);
            }
            return array;
        }

        /// <summary>
        /// Converts one type to another, applying heuristics as neccecary
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="inject">The inject.</param>
        /// <returns></returns>
        public static object ConvertTo(Type type, string inject)
        {
            if (inject == null)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            if (type.IsInstanceOfType(inject))
            {
                return inject;
            }
            else if (type == typeof(int))
            {
                int temp;
                if (int.TryParse(inject, out temp))
                    return temp;
                return null;
            }
            else if (typeof(IConvertible).IsAssignableFrom(type))
            {
                return Convert.ChangeType(inject, type);
            }

            //Maybe we have a constructor that accept the type?
            ConstructorInfo ctor = type.GetConstructor(new Type[] { inject.GetType() });
            if (ctor != null)
            {
                return Activator.CreateInstance(type, inject);
            }

            //Maybe we have a Parse method ??
            MethodInfo parseMethod = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
            if (parseMethod != null)
            {
                return parseMethod.Invoke(null, new object[] { inject });
            }

            throw new ArgumentException(string.Format(
                "Cannot convert value '{0}' of type '{1}' to request type '{2}'", 
                inject,
                inject.GetType(), 
                type));
        }
    }
}