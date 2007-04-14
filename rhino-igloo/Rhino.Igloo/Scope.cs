using Castle.Components.Validator;
using Rhino.Commons;
using Rhino.Igloo.Properties;

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
                summary.RegisterErrorMessage(Resources.ErrorMessage, value);
                ErrorSummary = summary;
			    Flash[Constants.ErrorMessage] = value;
			}
    	}

		/// <summary>
		/// SuccessMessage.
		/// </summary>
    	public static string SuccessMesssage
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
            IContextProvider contextProvider = IoC.Resolve<IContextProvider>();
            IContext currentContext = contextProvider.Current;
            string result = currentContext.GetInputVariable(key);
            return result;
        }

        // note multiplies of those
        private static string[] GetMultiplyObjectsFromInput(string key)
        {
            IContextProvider contextProvider = IoC.Resolve<IContextProvider>();
            IContext currentContext = contextProvider.Current;
            return currentContext.GetMultiplyInputVariables(key);
        }

        private static object GetFromSession(string key)
        {
            IContextProvider contextProvider = IoC.Resolve<IContextProvider>();
            IContext currentContext = contextProvider.Current;
            return currentContext.GetFromSession(key);
        }

        private static void SetAtSession(string key, object val)
        {
            IContextProvider contextProvider = IoC.Resolve<IContextProvider>();
            IContext currentContext = contextProvider.Current;
            currentContext.SetAtSession(key, val);
        }
    }
}