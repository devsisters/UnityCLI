namespace CLI
{
    public struct Result
    {
        public readonly int Code;
        public readonly string Msg;

        public Result(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }

        public override string ToString()
        {
            return "[" + Code + "] " + Msg;
        }

        public static Result Success(string msg = "Success")
        {
            return new Result(0, msg);
        }

        public static Result Error(string msg)
        {
            return new Result(-1, msg);
        }

        public static Result InvalidCmd(Command cmd)
        {
            return Error(cmd.Raw);
        }

        public static Result UnknownCmd(Command cmd)
        {
            var format = "Unknown command: ";
            return Error(format + cmd.Raw);
        }
    }
}