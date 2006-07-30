namespace Rhino.Commons
{
    public class Parameter
    {
        string name;
        object value;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Parameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}