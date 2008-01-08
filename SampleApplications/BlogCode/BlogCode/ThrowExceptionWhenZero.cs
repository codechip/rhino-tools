namespace pipelines
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class ThrowExceptionWhenZero : AbstractTask
    {
        private readonly int countDown;

        public ThrowExceptionWhenZero(int countDown)
        {
            this.countDown = countDown;
        }

        protected override IEnumerable<Condition> Execute()
        {
            if(countDown==0)
                throw new InvalidDataException("BOOM!");
            Future spawn = Spawn(new ThrowExceptionWhenZero(countDown - 1));
            yield return spawn;
            SetResult(spawn.Value);
        }
    }
}