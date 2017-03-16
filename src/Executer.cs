using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLI
{
    public sealed class CustomExecuter : IExecuter
    {
        public Func<Command, int, Result> ExecuteCmd;

        public CustomExecuter()
        {
        }

        public CustomExecuter(Func<Command, int, Result> executeCmd)
        {
            ExecuteCmd = executeCmd;
        }

        public Result Execute(Command cmd, int argFrom)
        {
            if (ExecuteCmd == null) return Result.Error("ExecuteCmd is not set yet.");
            return ExecuteCmd(cmd, argFrom);
        }
    }

    // TODO: AttributeTargets.Field | AttributeTargets.Property | 
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class Bind : Attribute { }

    public sealed class ClassExecuter : IExecuter
    {
        private readonly Dictionary<string, MethodInfo> _delegates
            = new Dictionary<string, MethodInfo>();

        public ClassExecuter(Type t)
        {
            var methods = t.GetMethods();
            foreach (var m in methods)
                BindMethod(m);
        }

        private void BindMethod(MethodInfo m)
        {
            var attrs = m.GetCustomAttributes(typeof(Bind), true);
            if (attrs == null || attrs.Length == 0) return;

            if (!m.IsStatic) throw new Exception("Binding none static method.");
            var returnType = m.ReturnType;
            if (returnType != typeof(void) && returnType != typeof(Result))
                throw new Exception("Wrong return type. You should return void or CLI.Result");

            _delegates.Add(m.Name, m);
        }

        public Result Execute(Command cmd, int argFrom)
        {
            var methodName = cmd.Args[argFrom];
            var m = _delegates[methodName];

            var argInfos = m.GetParameters();
            var args = new object[argInfos.Length];
            for (var i = 0; i != argInfos.Length; ++i)
            {
                var argRaw = cmd[argFrom + i + 1];
                var argType = argInfos[i].ParameterType;
                args[i] = ParseArg(argRaw, argType);
            }

            var ret = m.Invoke(null, args);
            if (m.ReturnType == typeof(void))
                return Result.Success();
            return (Result)ret;
        }

        private object ParseArg(string arg, Type t)
        {
            if (t == typeof(string))
                return arg;
            if (t == typeof(int))
                return int.Parse(arg);
            if (t == typeof(bool))
                return bool.Parse(arg);
            if (t == typeof(float))
                return float.Parse(arg);
            if (t == typeof(double))
                return double.Parse(arg);
            return new Exception("Parsing argument failed: " + arg);
        }
    }

    public sealed class Executer : IExecuter
    {
        private readonly Dictionary<string, IExecuter> _executers
            = new Dictionary<string, IExecuter>();

        public Executer Bind(Type t)
        {
            if (t.IsClass)
            {
                _executers[t.Name] = new ClassExecuter(t);
            }
            else
            {
                throw new Exception("Cannot bind: " + t);
            }

            return this;
        }

        public Result Execute(Command cmd, int argFrom)
        {
            return _executers[cmd.Args[argFrom]].Execute(cmd, argFrom + 1);
        }
    }
}