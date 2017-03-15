using UnityEngine;

namespace CLI
{
    public class CLI : MonoBehaviour
    {
        public static CLI Instance;

        public static CLI TryInstall(int initialPort, string welcomeMessage)
        {
            if (Instance != null) return Instance;
            Instance = FindObjectOfType<CLI>();
            if (Instance != null) return Instance;
            var singleton = new GameObject("CLI (Singleton)");
            Instance = singleton.AddComponent<CLI>();
            Instance.InitialPort = initialPort;
            Instance.WelcomeMessage = welcomeMessage;
            Instance.Init();
            DontDestroyOnLoad(singleton);
            return Instance;
        }

        public static void Unstall()
        {
            if (Instance == null) return;
            Destroy(Instance.gameObject);
        }

        public int InitialPort = 6670;
        public string WelcomeMessage;
        private Communicator _communicator;

        private void Init()
        {
            if (_communicator != null) return;
            byte[] welcomeMsg = null;
            if (WelcomeMessage != null)
                welcomeMsg = System.Text.Encoding.UTF8.GetBytes(WelcomeMessage);
            _communicator = Communicator.Start(
                InitialPort,
                ExecuteCmd,
                welcomeMessage: welcomeMsg);
        }

        private void OnDestroy()
        {
            if (_communicator != null)
                _communicator.Stop();
        }

        private void Update()
        {
            Init();
            _communicator.ProcessJobs();
        }

        private static Result ExecuteCmd(Command cmd)
        {
            // TODO
            return Result.Success();
        }
    }
}
