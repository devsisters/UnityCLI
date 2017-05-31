using Args = System.Collections.Generic.List<string>;

namespace CLI
{
    public sealed class Command
    {
        public string this[int idx] { get { return Args[idx]; } }

        public readonly string Raw;
        public readonly Args Args;

        private Command(string raw, Args args)
        {
            Raw = raw;
            Args = args;
        }

        private static char[] _trimTokens = new[] { ' ', '\r', '\n', };

        public static Command Parse(string raw)
        {
            var tokens = raw.Split(_trimTokens,
                System.StringSplitOptions.RemoveEmptyEntries);
            var args = new Args(tokens.Length);
            for (var i = 0; i < tokens.Length; ++i)
                args.Add(tokens[i].Trim(_trimTokens));
            return new Command(raw, args);
        }
    }
}