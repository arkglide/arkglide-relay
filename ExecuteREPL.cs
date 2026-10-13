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
                List<string> cmdExe = new();
                for(int i=0;i<Command.Count;i++)
                {
                    if (Command[i].Length == 0) continue;
                    if(i==Command.Count-1)
                    {
                        if(Command.Last()!=">")cmdExe.Add(Command[i]);
                        Run(cmdExe.ToArray());
                        cmdExe.Clear();
                        break;
                    }
                    if (Command[i] == ">")
                    {
                        if (cmdExe.Count == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("error token");
                        }
                        ConnectStream=Run(cmdExe.ToArray());
                        cmdExe.Clear();
                        continue;
                    }
                    cmdExe.Add(Command[i]);
                }
            }
        }
        private string ConnectStream = string.Empty;
        private string Run(string[] cmds)
        {

        }
        private void ExitServer() {
            //to do
        }
    }
}
