using Args = System.Collections.Generic.List<string>;

namespace CLI
{
    public class Command
    {
        public string this[int idx] { get { return Args[idx]; } }

        public readonly string Raw;
        public readonly string Exe;
        public readonly Args Args;

        private Command(string raw, string exe, Args args)
        {
            Raw = raw;
            Exe = exe;
            Args = args;
        }

        private static char[] _trimTokens = new[] { ' ', '\r', '\n', };

        internal static Command Parse(string raw)
        {
            var tokens = raw.Split(_trimTokens,
                System.StringSplitOptions.RemoveEmptyEntries);

            var exe = tokens[0].Trim(_trimTokens);
            var args = new Args(tokens.Length - 1);
            if (tokens.Length > 1)
            {
                for (var i = 1; i < tokens.Length; ++i)
                    args.Add(tokens[i].Trim(_trimTokens));
            }

            return new Command(raw, exe, args);
        }
    }
}