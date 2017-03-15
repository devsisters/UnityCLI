using UnityEngine;

namespace CLI
{
    public class Bridge : MonoBehaviour
    {
        public static Bridge Instance;

        public static Bridge TryInstall(int initialPort, string welcomeMessage)
        {
            const string name = "CLI (Singleton)";

            if (Instance != null) return Instance;

            var inst = FindObjectOfType<Bridge>();
            if (inst != null && inst.name == name)
            {
                Instance = inst;
                return Instance;
            }

            var singleton = new GameObject(name);
            Instance = singleton.AddComponent<Bridge>();
            Instance.InitialPort = initialPort;
            Instance.WelcomeMessage = welcomeMessage;
            Instance.Init();
            DontDestroyOnLoad(singleton);
            return Instance;
        }

        public static void Uninstall()
        {
            if (Instance == null) return;
            Destroy(Instance.gameObject);
        }

        public int InitialPort = 6670;
        public string WelcomeMessage;
        public ExecuteCmd ExecuteCmd;
        private Communicator _communicator;

        private void Init()
        {
            if (_communicator != null) return;
            byte[] welcomeMsg = null;
            if (WelcomeMessage != null)
                welcomeMsg = System.Text.Encoding.UTF8.GetBytes(WelcomeMessage);
            _communicator = Communicator.Start(
                InitialPort, DoExecuteCmd,
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

        private Result DoExecuteCmd(Command cmd)
        {
            if (ExecuteCmd == null)
                return Result.Error("ExecuteCmd is null");
            return ExecuteCmd(cmd);
        }
    }
}
