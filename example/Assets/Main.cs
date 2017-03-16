using UnityEngine;

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
        instance.ExecuteCmd = ExecuteSingleton;
        Channel1.ExecuteCmd = ExecuteChannel1;
        Channel2.ExecuteCmd = ExecuteChannel2;
        Channel3.ExecuteCmd = ExecuteChannel3;
    }

    private void OnDestroy()
    {
        CLI.Bridge.Uninstall();
    }

    private CLI.Result ExecuteSingleton(CLI.Command cmd)
    {
        return CLI.Result.Success(cmd.Raw);
    }

    private CLI.Result ExecuteChannel1(CLI.Command cmd)
    {
        return CLI.Result.Success("[Ch1] " + cmd.Raw);
    }

    private CLI.Result ExecuteChannel2(CLI.Command cmd)
    {
        return CLI.Result.Success("[Ch2] " + cmd.Raw);
    }

    private CLI.Result ExecuteChannel3(CLI.Command cmd)
    {
        return CLI.Result.Success("[Ch3] " + cmd.Raw);
    }
}
