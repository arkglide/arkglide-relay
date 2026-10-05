using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class ExecuteREPL
    {
        private string GameName;
        public ExecuteREPL(string gameName) { 
            GameName = gameName;
        }
        public void REPL()
        {
            Console.CancelKeyPress +=(s,e)=> ExitServer();
            string DisplayPrompt()
            {
                Console.WriteLine("Server[" + GameName + "]" + Directory.GetCurrentDirectory());
                Console.Write("└──>");
                return Console.ReadLine()??string.Empty;
            }
            while (true)
            {
                string Input = DisplayPrompt();
                if (string.IsNullOrEmpty(Input)) continue;
                bool inString = false;
                StringBuilder cmdBlock = new();
                List<string> Command= new List<string>();
                foreach (var item in Input)
                {
                    
                    if (char.IsWhiteSpace(item) && !inString)
                    {
                        if (cmdBlock.Length == 0) throw new Exception("Empty command token");
                        Command.Add(cmdBlock.ToString());
                        cmdBlock.Clear();
                        continue;
                    }
                    if (inString && item == '"')
                    {cmdBlock.Append(item);inString = false;continue; }
                    if (inString) { cmdBlock.Append(item); continue; }
                    if(!inString&&item == '"')
                    {
                        Command.Add(cmdBlock.ToString());
                        inString = true;
                        cmdBlock.Clear();
                        cmdBlock.Append(item.ToString());
                        continue;
                    }
                    if (!inString && item != '"' && !string.IsNullOrEmpty(item.ToString()))
                    { cmdBlock.Append(item); continue; }
                }
                Command.Add(cmdBlock.ToString());

            }
        }
        
        private void ExitServer() { }
    }
}
