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

public class ClassBinding : MonoBehaviour
{
    private void OnEnable()
    {
        // start TCP/IP communication
        const int port = 6670;
        const string welcomeMessage = "Hello, ClassBinding!";
        var cli = CLI.Bridge.TryInstall(port, welcomeMessage: welcomeMessage);

        // bind class
        cli.Executer = new CLI.Executer()
            .Bind(typeof(Binding1))
            .Bind(typeof(Binding2));
    }
}

