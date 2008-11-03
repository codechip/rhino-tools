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


using Castle.Components.Validator;
using Rhino.Commons;

namespace Rhino.Igloo
{
    /// <summary>
    /// We use Scope to pass objects betweens layers without breaking encapsulation and 
    /// Seperation of concerns. This allows us to pass data without strong coupling 
    /// between the objects passing the data. 
    /// This also allows to fake the data source for testing purposes.
    /// </summary>
    public static partial class Scope
    {
        private static PropertyIndexer<object, string> request =
            new PropertyIndexer<object, string>(GetObjectFromLocalScope, SetObjectAtLocalScope);

        private static PropertyIndexer<object, string> session =
            new PropertyIndexer<object, string>(GetFromSession, SetAtSession);

        private static PropertyIndexerGetter<string, string> input =
            new PropertyIndexerGetter<string, string>(GetObjectFromUserInput);

        private static PropertyIndexerGetter<string[], string> inputs =
            new PropertyIndexerGetter<string[], string>(GetMultiplyObjectsFromInput);

        /// <summary>
        /// Contact inputted values, be sure to validate the input before using.
        /// </summary>
        public static PropertyIndexerGetter<string, string> Input
        {
            get { return input; }
        }


        /// <summary>
        /// Contact inputted values (mutliplies), be sure to validate the input before using.
        /// </summary>
        public static PropertyIndexerGetter<string[], string> Inputs
        {
            get { return inputs; }
        }


        /// <summary>
        /// Storage for the current request only
        /// </summary>
        public static PropertyIndexer<object, string> Flash
        {
            get { return request; }
        }

        /// <summary>
        /// Storage for the current session
        /// </summary>
        public static PropertyIndexer<object, string> Session
        {
            get { return session; }
        }

        /// <summary>
        /// Gets or sets the error summary, if exists
        /// </summary>
        /// <value>The error summary.</value>
        public static ErrorSummary ErrorSummary
        {
            get { return (ErrorSummary) Flash[Constants.ErrorSummary]; }
            set { Flash[Constants.ErrorSummary] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether there is an error summary in the scope
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the scope has error summary; otherwise, <c>false</c>.
        /// </value>
        public static bool HasErrorSummary
        {
            get { return ErrorSummary != null; }
        }

		/// <summary>
		/// Gets or sets the single error message, if exist
		/// </summary>
		/// <value>The error.</value>
    	public static string ErrorMessage
    	{
			get { return (string) Flash[Constants.ErrorMessage]; }
			set
			{
                ErrorSummary summary = new ErrorSummary();
                summary.RegisterErrorMessage(Constants.ErrorMessage, value);
                ErrorSummary = summary;
			    Flash[Constants.ErrorMessage] = value;
			}
    	}

		/// <summary>
		/// SuccessMessage.
		/// </summary>
    	public static string SuccessMessage
    	{
			get { return (string) Flash[Constants.SuccessMessage]; }
			set { Flash[Constants.SuccessMessage] = value; }
    	}

    	private static void SetObjectAtLocalScope(string key, object obj)
        {
            Local.Data[key] = obj;
        }

        private static object GetObjectFromLocalScope(string key)
        {
            return Local.Data[key];
        }


        private static string GetObjectFromUserInput(string key)
        {
            return Current.GetInputVariable(key);
        }

        // note multiplies of those
        private static string[] GetMultiplyObjectsFromInput(string key)
        {
            return Current.GetMultiplyInputVariables(key);
        }

        private static object GetFromSession(string key)
        {
            return Current.GetFromSession(key);
        }

        private static void SetAtSession(string key, object val)
        {
            Current.SetAtSession(key, val);
        }

        private static IContext Current
        {
            get { return IoC.Resolve<IContextProvider>().Current; }
        }
    }
}
