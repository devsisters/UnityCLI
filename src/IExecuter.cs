namespace CLI
{
    public abstract class IExecuter
    {
        public Result Execute(Command cmd) { return ExecuteFrom(cmd, 0); }
        public abstract Result ExecuteFrom(Command cmd, int argFrom);
    }
}
