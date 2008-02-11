namespace Chapter5.QuoteGeneration
{
    using Rhino.DSL;

    public static class QuoteGenerator
    {
        private static readonly DslFactory dslFactory;

        static QuoteGenerator()
        {
            dslFactory = new DslFactory();
            dslFactory.Register<QuoteGeneratorRule>(new QuoteGenerationDslEngine());
        }

        public static void Generate()
        {
            QuoteGeneratorRule rule = dslFactory.Create<QuoteGeneratorRule>(@"D:\OSS\rhino-tools\SampleApplications\BDSLiB\Chapter5\Chapter5.Scripts\QuoteGenerator\sample.boo", new RequirementsInformation(200,"Vacation"));
            rule.Evaluate();
        }
    }
}