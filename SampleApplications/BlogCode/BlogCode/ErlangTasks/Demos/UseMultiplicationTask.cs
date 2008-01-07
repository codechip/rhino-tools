namespace pipelines
{
    using System.Collections.Generic;

    public class UseMultiplicationTask : AbstractTask
    {
        protected override IEnumerable<Condition> Execute()
        {
            Future future1 = Spawn(new MultiplicationTask(15));
            Future future2 = Spawn(new MultiplicationTask(23));

            yield return Done(future2) && Done(future2);

            System.Console.WriteLine(future1.GetValue<int>() + future2.GetValue<int>());
        }
    }
}