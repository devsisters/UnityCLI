namespace CLI
{
    public interface IExecuter
    {
        Result Execute(Command cmd, int argFrom);
    }
}
