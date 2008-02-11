namespace BDSLiB.QuoteGeneration
{
    using Properties;
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
            string url = Settings.Default.QuoteGenerationScript;
            QuoteGeneratorRule rule = dslFactory.Create<QuoteGeneratorRule>(url, new RequirementsInformation(200,"Vacation"));
            rule.Evaluate();
        }
    }
}