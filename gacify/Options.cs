using CommandLine;

namespace gacify
{
    public class Options
    {
        [Option('s', "solution", Required = true, HelpText = "Set solution directory or path. You can use $(SolutionDir) or $(SolutionPath)")]
        public string Solution { get; set; }
        [Option('c', "configuration", Required = false, HelpText = "Set configuration name. Ex: Debug or Release", Default ="Debug")]
        public string Configuration { get; set; }
        [Option('g', "gacutil", Required = false, HelpText = "Set gacutil custom path")]
        public string GacUtil { get; set; }
    }
}
