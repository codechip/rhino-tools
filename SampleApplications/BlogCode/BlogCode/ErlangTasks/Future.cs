namespace pipelines
{
    public class Future
    {
        private object value;
        private bool hasValue;
        private string error;

        public void SetValue(object value)
        {
            this.value = value;
            hasValue = true;
        }

        public void SetError(string error)
        {
            this.error = error;
        }

        public bool HasError
        {
            get { return error != null; }
        }

        public T GetValue<T>()
        {
            return (T)Value;
        }

        public object Value
        {
            get { return value; }
        }

        public bool HasValue
        {
            get { return hasValue; }
        }

        public string Error
        {
            get { return error; }
        }
    }
}