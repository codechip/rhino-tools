namespace pipelines
{
    using System;

    public class Future
    {
        private object value;
        private bool hasValue;
        private Exception exception;

        public void SetValue(object v)
        {
            this.value = v;
            hasValue = true;
        }

        public void SetError(Exception e)
        {
            this.exception = e;
        }

        public bool HasError
        {
            get { return exception != null; }
        }

        public T GetValue<T>()
        {
            return (T)Value;
        }

        public object Value
        {
            get
            {
                if (exception != null)
                    throw new TaskFailedException("Exception occured when executing task", exception);
                if (hasValue == false)
                    throw new InvalidFutureValueAccessException(
                        "Future value cannot be accessed before it is set. Use the HasValue to verify that it has a value");
                return value;
            }
        }

        public bool HasValue
        {
            get { return hasValue || HasError; }
        }

        public Exception Exception
        {
            get { return exception; }
        }

        public static implicit operator Condition(Future f)
        {
            return delegate { return f.HasValue; };
        }
    }
}
