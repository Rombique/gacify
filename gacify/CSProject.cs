using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace gacify
{
    public class CSProject
    {
        public string DllDebugPath { get; private set; }
        public bool IsSigned { get; private set; }

        public CSProject(string projectPath)
        {
            
        }
    }
}
