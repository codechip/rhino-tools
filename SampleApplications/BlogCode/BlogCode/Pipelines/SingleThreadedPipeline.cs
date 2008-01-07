namespace pipelines.Pipelines
{
    using System.Collections.Generic;

    public class SingleThreadedPipeline<T>
    {
        private readonly List<IOperation<T>> operations = new List<IOperation<T>>();

        public SingleThreadedPipeline<T> Register(IOperation<T> operation)
        {
            operations.Add(operation);
            return this;
        }

        public void Execute()
        {
            IEnumerable<T> current = new List<T>();
            foreach (IOperation<T> operation in operations)
            {
                current = operation.Execute(current);
            }
            IEnumerator<T> enumerator = current.GetEnumerator();
            while (enumerator.MoveNext()) ;
        }
    }
}