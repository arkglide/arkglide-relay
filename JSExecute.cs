using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public static class JSExecute
    {
        public static string _scriptPath = "script,js";
        private static Engine _JSengine = new Engine();
        public static Engine Post()
        =>_JSengine.Execute(File.ReadAllText(_scriptPath)??throw new Exception("E-JS:FileTextEmpty"));
        
    }
}
