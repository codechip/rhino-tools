namespace pipelines.Pipelines
{
    using System.Collections.Generic;

    public interface IOperation<T>
    {
        IEnumerable<T> Execute(IEnumerable<T> current);
    }
}