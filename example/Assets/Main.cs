using UnityEngine;

public static class Binding1
{
    [CLI.Bind]
    public static CLI.Result Foo(int i)
    {
        return CLI.Result.Success("Foo called: " + i);
    }
}

public static class Binding2
{
    [CLI.Bind]
    public static CLI.Result Goo(string str)
    {
        return CLI.Result.Success("Goo called: " + str);
    }
}

namespace Game
{
    public static class Wallet
    {
        public static int Money;
    }
}

namespace CLIBindings
{
    public static class Wallet
    {
        [CLI.Bind]
        public static CLI.Result Add(int amount)
        {
            Game.Wallet.Money += amount;
            return CLI.Result.Success("now: " + Game.Wallet.Money);
        }

        [CLI.Bind]
        public static CLI.Result Set(int amount)
        {
            Game.Wallet.Money = amount;
            return CLI.Result.Success("now: " + Game.Wallet.Money);
        }

        [CLI.Bind]
        public static void Clear()
        {
            Game.Wallet.Money = 0;
        }
    }
}

public class Main : MonoBehaviour
{
    public int SingletonPort = 1234;
    public CLI.Bridge Channel1;
    public CLI.Bridge Channel2;
    public CLI.Bridge Channel3;

    private void Awake()
    {
        Application.runInBackground = true;
    }

    private void OnEnable()
    {
        var instance = CLI.Bridge.TryInstall(SingletonPort, welcomeMessage: "I'm singleton");
        instance.Executer = new CLI.CustomExecuter(ExecuteSingleton);
        Channel1.Executer = new CLI.ClassExecuter(typeof(Binding1));
        Channel2.Executer = new CLI.ClassExecuter(typeof(Binding2));
        var exe = new CLI.Executer();
        exe.Bind(typeof(Binding1))
            .Bind(typeof(Binding2))
            .Bind(typeof(CLIBindings.Wallet));
        Channel3.Executer = exe;
    }

    private void OnDestroy()
    {
        CLI.Bridge.Uninstall();
    }

    private CLI.Result ExecuteSingleton(CLI.Command cmd, int argsFrom)
    {
        switch (cmd[argsFrom])
        {
            case "realtimeSinceStartup":
                return CLI.Result.Success("now: " + Time.realtimeSinceStartup);
            case "timescale":
                var newTimescale = float.Parse(cmd.Args[0]);
                Time.timeScale = newTimescale;
                return CLI.Result.Success("set timescale: " + newTimescale);
            default:
                return CLI.Result.InvalidCmd(cmd);
        }
    }

    private CLI.Result ExecuteChannel2(CLI.Command cmd, int argsFrom)
    {
        return CLI.Result.Success("[Ch2] " + cmd.Raw);
    }

    private CLI.Result ExecuteChannel3(CLI.Command cmd, int argsFrom)
    {
        return CLI.Result.Success("[Ch3] " + cmd.Raw);
    }
}
