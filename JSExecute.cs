using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class JSExecute
    {
        public string _scriptPath = "script,js";
        private Engine _JSengine = new Engine();
        public Engine Post()
        =>_JSengine.Execute(File.ReadAllText(_scriptPath)??throw new Exception("E-JS:FileTextEmpty"));
        
    }
}
