using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace gacify
{
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        static readonly Type s_ProjectInSolution;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;

        static SolutionProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_ProjectInSolution != null)
            {
                s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string ProjectType { get; private set; }
        public Dictionary<string, string> ConfigPathDict { get; private set; }
        public bool IsSigned { get; private set; }
        public string SolutionFolder { get; private set; }
        public bool IsLibrary { get; private set; }

        public SolutionProject(string solutionFolder, object solutionProject)
        {
            ProjectName = s_ProjectInSolution_ProjectName.GetValue(solutionProject, null) as string;
            RelativePath = s_ProjectInSolution_RelativePath.GetValue(solutionProject, null) as string;
            ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(solutionProject, null) as string;
            ProjectType = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null).ToString();
            SolutionFolder = solutionFolder ?? throw new ArgumentNullException(nameof(solutionFolder));
            ConfigPathDict = GetConfigurations(solutionFolder, ProjectName);

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Path.Combine(SolutionFolder, RelativePath));
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot) // TODO
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "SignAssembly")
                        IsSigned = XmlConvert.ToBoolean(childnode.InnerText);

                    if (childnode.Name == "OutputType")
                        IsLibrary = childnode.InnerText == "Library";
                }
            }
        }

        private Dictionary<string,string> GetConfigurations(string solutionFolder, string projectName)
        {
            var firstPartFolder = Path.Combine(solutionFolder, projectName, "bin");
            var dllFiles = Directory.GetFiles(firstPartFolder, projectName + ".dll", SearchOption.AllDirectories);

            return dllFiles.Select(filePath =>
                {
                    var configName = filePath.Substring(firstPartFolder.Length)
                        .TrimStart(Path.DirectorySeparatorChar);
                    configName = configName.Substring(0, configName.Length - (projectName + ".dll").Length)
                        .TrimEnd(Path.DirectorySeparatorChar);
                    return new KeyValuePair<string, string>(configName, filePath);
                }
            ).ToDictionary(k => k.Key, k => k.Value);
        }
    }
}
