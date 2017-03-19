using UnityEngine;

public class MultipleChannels : MonoBehaviour
{
    public CLI.Bridge Channel1;
    public CLI.Bridge Channel2;
    public CLI.Bridge Channel3;

    private void OnEnable()
    {
        Channel1.Executer = new CLI.CustomExecuter(ExecuteChannel1);
        Channel2.Executer = new CLI.CustomExecuter(ExecuteChannel2);
        Channel3.Executer = new CLI.CustomExecuter(ExecuteChannel3);
    }

    private CLI.Result ExecuteChannel1(CLI.Command cmd, int argsFrom)
    {
        return CLI.Result.Success("[Ch1] " + cmd.Raw);
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
