using Jint;
using System;
using System.IO;

namespace arkglide_relay
{
    public class JSExecute
    {
        private string _scriptPath = "script.js"; // 修复了逗号
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
            // 先检查函数是否存在
            var func = _JSengine.GetValue(hookName);
            if (!func.IsCallable()) return Jint.Native.JsValue.Undefined;

            try
            {
                // 使用 Engine.Invoke，它会自动处理 object[] 到 JS 参数的转换
                // 注意：如果参数里面包含 null，Jint 也能安全处理
                return _JSengine.Invoke(hookName, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Jint Error] 脚本执行 {hookName} 异常: {ex.Message}");
                return Jint.Native.JsValue.Null;
            }
        }
    }
}