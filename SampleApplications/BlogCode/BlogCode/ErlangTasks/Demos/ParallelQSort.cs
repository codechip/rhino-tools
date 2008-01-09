using System;
using System.Collections.Generic;
using System.Text;

namespace pipelines.ErlangTasks.Demos
{
    /// <summary>
    /// BogoSort is more efficent, this is really demo code.
    /// </summary>
    public class ParallelQSort : AbstractTask
    {
        private readonly int[] items;

        public ParallelQSort(int[] items)
        {
            this.items = items;
            Console.WriteLine(items.Length);
        }

        protected override IEnumerable<Condition> Execute()
        {
            if (items.Length <= 100)
            {
                Array.Sort(items);
                SetResult(items);
                yield break;
            }
            int pivot = items[items.Length / 2];
           
            Tuple t = SplitItems(pivot);

            Future sortedLess = Spawn(new ParallelQSort(t.Less.ToArray()));
            Future sortedGreater = Spawn(new ParallelQSort(t.Greater.ToArray()));

            yield return delegate { return sortedGreater.HasValue && sortedLess.HasValue; };

            int[] result = MergeItems(t, sortedGreater, sortedLess);
            SetResult(result);
        }

        private int[] MergeItems(Tuple t, Future sortedGreater, Future sortedLess)
        {
            List<int> result = new List<int>();
            result.AddRange(sortedLess.GetValue<int[]>());
            result.AddRange(t.Eq);
            result.AddRange(sortedGreater.GetValue<int[]>());
            return result.ToArray();
        }

        private Tuple SplitItems(int pivot)
        {
            Tuple t = new Tuple();
            foreach (int i in items)
            {
                if (i < pivot)
                    t.Less.Add(i);
                else if (i == pivot)
                    t.Eq.Add(i);
                else 
                    t.Greater.Add(i);
            }
            return t;
        }

        public class Tuple
        {
            public List<int> Less = new List<int>();
            public List<int> Eq = new List<int>();
            public List<int> Greater = new List<int>();
        }
    }
}
