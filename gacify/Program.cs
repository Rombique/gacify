using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace gacify
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.ASCII;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    string rightSolutionPath = GetRightSolutionPath(o.Solution);
                    if (string.IsNullOrEmpty(rightSolutionPath))
                        throw new Exception("Wrong solution path! Sln file not found!");

                    string gacPath = GetRightGacUtilPath(o.GacUtil);
                    if (string.IsNullOrEmpty(rightSolutionPath))
                        throw new Exception("Something wrong with gacutil path!");

                    var sln = new Solution(rightSolutionPath);
                    foreach (var signedProject in sln.Projects.Where(p => p.IsSigned && p.IsLibrary))
                    {
                        var isConfigExist = signedProject.ConfigPathDict.TryGetValue(o.Configuration, out string configPair);
                        if (!isConfigExist)
                            throw new Exception($"Configuration {o.Configuration} not found in project {signedProject.ProjectName}");
                        RunProcess(gacPath, signedProject.ProjectName, configPair);
                    }
                        
                });
        }

        private static string GetRightSolutionPath(string solution)
        {
            if (!solution.EndsWith(".sln"))
                return Directory.GetFiles(solution, "*.sln", SearchOption.AllDirectories).SingleOrDefault();
            else if (File.Exists(solution))
                return solution;
            return null;
        }

        private static string GetRightGacUtilPath(string gacUtil)
        {
            if (string.IsNullOrEmpty(gacUtil))
                return Directory.GetFiles("C:\\Program Files (x86)\\Microsoft SDKs\\Windows", "gacutil.exe", SearchOption.AllDirectories).FirstOrDefault();
            else if (!gacUtil.EndsWith(".exe"))
                return Directory.GetFiles(gacUtil, "gacutil.exe", SearchOption.TopDirectoryOnly).SingleOrDefault();
            else if (File.Exists(gacUtil))
                return gacUtil;
            return null;
        }

        static string RunProcess(string gacUtilPath, string projectName, string dllPath)
        {
            string output = string.Empty;

            using (Process gacUtil = new Process())
            {
                Console.WriteLine($"Project: {projectName} \nPath: {dllPath}");
                gacUtil.StartInfo.FileName = gacUtilPath;
                gacUtil.StartInfo.UseShellExecute = false;
                gacUtil.StartInfo.RedirectStandardOutput = true;
                gacUtil.StartInfo.CreateNoWindow = false;
                gacUtil.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                gacUtil.StartInfo.Arguments = string.Format("-i \"{0}\"", dllPath);
                gacUtil.Start();

                StreamReader reader = gacUtil.StandardOutput;
                output = reader.ReadToEnd();

                Console.WriteLine(output);

                gacUtil.WaitForExit();
            }

            return output;
        }
    }
}
