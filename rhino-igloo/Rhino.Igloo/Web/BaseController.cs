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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Castle.Components.Validator;
using log4net;
using Rhino.Commons;

namespace Rhino.Igloo
{
	/// <summary>
	/// Base class for all the controllers
	/// </summary>
	public class BaseController
	{
		private static IValidatorRegistry validationRegistry = new CachedValidationRegistry();

		private IContext context;
		private ILog log;
		private ValidatorRunner validatorRunner;


		/// <summary>
		/// Initializes a new instance of the <see cref="BaseController"/> class.
		/// </summary>
		/// <param myName="context">The context.</param>
		public BaseController(IContext context)
		{
			this.context = context;
		}


		/// <summary>
		/// Gets or sets the log for this controller.
		/// </summary>
		/// <value>The log.</value>
		public ILog Log
		{
			get { return log; }
			set { log = value; }
		}


		/// <summary>
		/// Gets the context we are currently in.
		/// </summary>
		/// <value>The context.</value>
		public IContext Context
		{
			get { return context; }
		}


		/// <summary>
		/// Return an an empty list of <typeparamref name="T"/>.
		/// Used to clarify the intent of the action only
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The whole point here is to get EmptyList<Type> support")]
		public static IList<T> EmptyList<T>()
		{
			return new List<T>();
		}


		/// <summary>
		/// Tries to parse the inputName as boolean from the user input
		/// </summary>
		/// <param name="inputName">The inputName.</param>
		/// <returns></returns>
		protected static bool? TryParseBoolFromInput(string inputName)
		{
			bool b;
			string userInput = Scope.Input[inputName];
			if (bool.TryParse(userInput, out b))
				return b;
			return null;
		}

		/// <summary>
		/// Tries to parse the inputName as a checkbox.
		/// </summary>
		/// <param name="inputName">The inputName.</param>
		/// <returns></returns>
		protected static bool ParseCheckBox(string inputName)
		{
			string userInput = Scope.Input[inputName];
			if (userInput == "on")
				return true;
			return false;
		}

		/// <summary>
		/// Tries to parse the inputName as int from user input
		/// </summary>
		/// <param name="inputName">The inputName.</param>
		/// <returns></returns>
		protected static int? TryParseInt32FromInput(string inputName)
		{
			int i;
			string userInput = Scope.Input[inputName];
			if (int.TryParse(userInput, out i))
				return i;
			return null;
		}

		/// <summary>
		/// Tries the parse userInput as integer.
		/// </summary>
		/// <param name="userInput">The user input.</param>
		/// <returns></returns>
		protected static int? TryParseInt32(string userInput)
		{
			int i;
			if (int.TryParse(userInput, out i))
				return i;
			return null;
		}


		/// <summary>
		/// Tries to parse the date using dd/MM/yyyy format
		/// </summary>
		/// <param name="inputName">Name of the input.</param>
		/// <returns></returns>
		protected static DateTime? TryParseDateFromInput(string inputName)
		{
			return TryParseDateFromInput(inputName, "dd/MM/yyyy");
		}

        /// <summary>
        /// Tries the parse date and time from input.
        /// </summary>
        /// <param name="inputName">Name of the input.</param>
        /// <returns></returns>
        protected static DateTime? TryParseDateAndTimeFromInput(string inputName)
        {
            return TryParseDateFromInput(inputName, "dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Tries the parse enum from input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputName">Name of the input.</param>
        /// <returns></returns>
        protected static T TryParseEnumFromInput<T>(string inputName)
        {
            string userInput = Scope.Input[inputName];
            int value;
            if (int.TryParse(userInput, out value) == false)
            {
                return default(T);
            }
            return (T)Enum.ToObject(typeof(T), value);
        }

		/// <summary>
		/// Tries to parse the inputName as date.
		/// </summary>
		/// <param name="inputName">The inputName.</param>
		/// <param name="formats">The formats.</param>
		/// <returns></returns>
		protected static DateTime? TryParseDateFromInput(string inputName, params string[] formats)
		{
			DateTime datetime;
			string userInput = Scope.Input[inputName];
			if (DateTime.TryParseExact(userInput, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime))
				return datetime;
			return null;
		}


		/// <summary>
		/// Tries the parse date.
		/// </summary>
		/// <param name="userInput">The user input.</param>
		/// <param name="formats">The formats.</param>
		/// <returns></returns>
		protected static DateTime? TryParseDate(string userInput, params string[] formats)
		{
			DateTime datetime;

			if (DateTime.TryParseExact(userInput, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime))
				return datetime;
			return null;
		}

		/// <summary>
		/// Tries to get the value for the input key.
		/// </summary>
		/// <param name="inputKey">The input key.</param>
		/// <returns></returns>
		protected static T TryGetFromInput<T>(string inputKey)
			where T : class
		{
			string maybeId = Scope.Input[inputKey];
			return TryGetById<T>(maybeId);
		}

		/// <summary>
		/// Tries to get the value for the input key.
		/// </summary>
		/// <param name="inputKey">The input key.</param>
		/// <returns></returns>
		protected static T TryGetFromInputString<T>(string inputKey)
			where T : class
		{
			string maybeId = Scope.Input[inputKey];
			return TryGetByIdString<T>(maybeId);
		}

        /// <summary>
        /// Tries to get the value for the input key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputKey">The input key.</param>
        /// <returns></returns>
        protected static T TryGetFromInputChar<T>(string inputKey)
            where T : class
        {
            string maybeId = Scope.Input[inputKey];
            return TryGetByIdChar<T>(maybeId);
        }

		/// <summary>
		/// Tries the get by id.
		/// </summary>
		/// <param name="maybeId">The maybe id.</param>
		/// <returns></returns>
		protected static T TryGetById<T>(string maybeId)
			where T : class
		{
			int id;
			if (int.TryParse(maybeId, out id) == false)
				return null;
			return Repository<T>.Get(id);
		}

		/// <summary>
		/// Tries the get by id.
		/// </summary>
		/// <param name="maybeId">The maybe id.</param>
		/// <returns></returns>
		protected static T TryGetByIdString<T>(string maybeId)
			where T : class
		{
			if (maybeId == null)
				return null;
			return Repository<T>.Get(maybeId);
		}

        /// <summary>
        /// Tries the get by id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="maybeId">The maybe id.</param>
        /// <returns></returns>
        protected static T TryGetByIdChar<T>(string maybeId)
            where T : class
        {
            if (maybeId == null || maybeId.Length != 1)
                return null;
            return Repository<T>.Get(maybeId.ToCharArray()[0]);
        }

		/// <summary>
		/// Gets the validator runner.
		/// </summary>
		/// <value>The validator runner.</value>
		protected ValidatorRunner ValidatorRunner
		{
			get
			{
				if (validatorRunner == null)
					validatorRunner = new ValidatorRunner(validationRegistry);
				return validatorRunner;
			}
		}

        /// <summary>
        /// Allow a derive class to initialize itself before handing the instance
        /// to the view
        /// </summary>
	    public virtual void Initialize()
	    {
	        
	    }
	}
}