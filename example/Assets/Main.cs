using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;
    }

    private void OnEnable()
    {
        var instance = CLI.Bridge.TryInstall(1234, welcomeMessage: "hello");
        instance.ExecuteCmd = ExecuteCmd;
    }

    private void OnDestroy()
    {
        CLI.Bridge.Uninstall();
    }

    private CLI.Result ExecuteCmd(CLI.Command cmd)
    {
        return CLI.Result.Success(cmd.Raw);
    }
}
