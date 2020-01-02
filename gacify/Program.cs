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
            if (args.Length == 0)
            {
                Console.WriteLine("Set $(SolutionPath) or $(SolutionDir) as 1st argument");
                return;
            }

            if (!args[0].EndsWith(".sln"))
                args[0] = Directory.GetFiles(args[0], "*.sln", SearchOption.AllDirectories).FirstOrDefault();

            var gacPath = args.Length == 2 
                ? args[1] 
                : Directory.GetFiles("C:\\Program Files (x86)\\Microsoft SDKs\\Windows", "gacutil.exe", SearchOption.AllDirectories).FirstOrDefault();

            var sln = new Solution(args[0]);
            foreach(var signedProject in sln.Projects.Where(p => p.IsSigned && p.IsLibrary))
                RunProcess(gacPath, signedProject);
        }

        static string RunProcess(string gacUtilPath, SolutionProject project)
        {
            string output = string.Empty;

            using (Process gacUtil = new Process())
            {
                Console.WriteLine($"Project: {project.ProjectName} \nPath: {project.DllDebugPath}");
                gacUtil.StartInfo.FileName = gacUtilPath;
                gacUtil.StartInfo.UseShellExecute = false;
                gacUtil.StartInfo.RedirectStandardOutput = true;
                gacUtil.StartInfo.CreateNoWindow = false;
                gacUtil.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                gacUtil.StartInfo.Arguments = string.Format("-i \"{0}\"", project.DllDebugPath);
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
