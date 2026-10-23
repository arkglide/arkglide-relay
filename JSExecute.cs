using Jint;
using System;
using System.IO;

namespace arkglide_relay
{
    public class JSExecute
    {
        private string _scriptPath = "script.js"; // 修复了你的逗号
        public Engine _JSengine { get; private set; }

        public JSExecute()
        {
            _JSengine = new Engine(options => options.TimeoutInterval(TimeSpan.FromMilliseconds(100)));
        }

        public Engine Post()
        {
            if (!File.Exists(_scriptPath)) return _JSengine;
            _JSengine.Execute(File.ReadAllText(_scriptPath));
            return _JSengine;
        }

        public Engine ResetScript(string scriptPath)
        {
            _scriptPath = scriptPath;
            return Post();
        }

        // 核心分发器
        public Jint.Native.JsValue Dispatch(string hookName, params object[] args)
        {
            var func = _JSengine.GetValue(hookName);
            if (!func.IsCallable()) return Jint.Native.JsValue.Undefined;

            try
            {
                return func.Call(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Jint Error] 脚本执行 {hookName} 异常: {ex.Message}");
                return Jint.Native.JsValue.Null;
            }
        }
    }
}