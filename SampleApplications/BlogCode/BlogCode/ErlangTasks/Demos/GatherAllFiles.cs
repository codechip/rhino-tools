namespace pipelines.ErlangTasks.Demos
{
    using System.Collections.Generic;
    using System.IO;

    public class GatherAllFiles : AbstractTask
    {
        private readonly string root;

        public GatherAllFiles(string root)
        {
            this.root = root;
        }

        protected override IEnumerable<Condition> Execute()
        {
            List<string> results = new List<string>();
            results.Add(root);
            
            List<Future> futures = new List<Future>();
            foreach (string directory in Directory.GetDirectories(root))
            {
                Future spawn = Spawn(new GatherAllFiles(directory));
                futures.Add(spawn);
            }
            string[] files = Directory.GetFiles(root);

            yield return Done(futures);

            foreach (Future future in futures)
            {
                results.AddRange(future.GetValue<IEnumerable<string>>());
            }
            results.AddRange(files);
            SetResult(results);
        }
    }
}