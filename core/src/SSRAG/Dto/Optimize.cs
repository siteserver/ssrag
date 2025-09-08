namespace SSRAG.Dto
{
    public class OptimizeRelatedScenario
    {
        public string ScenarioName { get; set; }
        public string Description { get; set; }
    }

    public class OptimizeApplicationCase
    {
        public string CaseName { get; set; }
        public string Details { get; set; }
    }

    public class OptimizeMin
    {
        public string Input { get; set; }
        public string OptimizedContent { get; set; }
    }

    public class OptimizeMax
    {
        public string Input { get; set; }
        public string OptimizedContent { get; set; }
        public string BackgroundInfo { get; set; }
        public OptimizeRelatedScenario[] RelatedScenarios { get; set; }
        public OptimizeApplicationCase[] ApplicationCases { get; set; }
    }
}
