using UnityEngine;

public class Singleton : MonoBehaviour
{
    private void OnEnable()
    {
        // start TCP/IP communication
        const int port = 6670;
        const string welcomeMessage = "Hello, Singleton!";
        var cli = CLI.Bridge.TryInstall(port, welcomeMessage: welcomeMessage);

        // bind function
        cli.Executer = new CLI.CustomExecuter(ExecuteSingleton);
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
}
