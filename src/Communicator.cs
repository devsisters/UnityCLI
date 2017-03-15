using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Encoding = System.Text.Encoding;
using UnityEngine;

namespace CLI
{
    public delegate Result ExecuteCmd(Command cmd);

    internal sealed class Communicator
    {
        private struct Job
        {
            public readonly NetworkStream Stream;
            public readonly string Cmd;

            public Job(NetworkStream stream, string cmd)
            {
                Stream = stream;
                Cmd = cmd;
            }
        }

        private readonly ExecuteCmd _executeCmd;
        private readonly byte[] _welcomeMessage;

        private bool _isRunning;

        private readonly TcpListener _listener;
        private Thread _listenerThread;

        private readonly List<NetworkStream> _streams = new List<NetworkStream>();
        private readonly List<Thread> _streamsThread = new List<Thread>();

        private readonly List<Job> _concurrentJobs = new List<Job>();
        private readonly List<Job> _tempJobs = new List<Job>();


        internal static Communicator Start(int port, ExecuteCmd executeCmd, byte[] welcomeMessage = null)
        {
            var ret = new Communicator(port, executeCmd, welcomeMessage);
            ret.Start();
            return ret;
        }

        private Communicator(int port, ExecuteCmd executeCmd, byte[] welcomeMessage)
        {
            _executeCmd = executeCmd;
            _welcomeMessage = welcomeMessage;
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            if (_isRunning)
            {
                // something went wrong
                return;
            }

            _isRunning = true;
            _listener.Start();
            _listenerThread = new Thread(Loop);
            _listenerThread.Name = "CLI.Communicator.Listerner";
            _listenerThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                // something went wrong
                return;
            }

            _isRunning = false;

            _listener.Stop();
            _listenerThread.Join();
            _listenerThread = null;

            lock (_streams)
            {
                foreach (var stream in _streams)
                {
                    stream.Close();
                    stream.Dispose();
                }

                _streams.Clear();
            }

            lock (_streamsThread)
            {
                foreach (var t in _streamsThread)
                    t.Join();
            }

            lock (_concurrentJobs)
                _concurrentJobs.Clear();
        }

        private static void WriteBufToStream(NetworkStream stream, byte[] buf) { stream.BeginWrite(buf, 0, buf.Length, null, null); }
        private static void WriteStringToStream(NetworkStream stream, string str) { WriteBufToStream(stream, Encoding.ASCII.GetBytes(str)); }
        private static void WriteLineToStream(NetworkStream stream, string str) { WriteStringToStream(stream, str + '\n'); }
        private static void WriteInputBracket(NetworkStream stream) { WriteStringToStream(stream, "> "); }

        private void Loop()
        {
            while (_isRunning)
            {
                var client = LoopAcceptClient();
                if (client == null) break;

                var stream = client.GetStream();
                if (_welcomeMessage != null)
                {
                    WriteBufToStream(stream, _welcomeMessage);
                    WriteStringToStream(stream, "\n");
                }
                WriteInputBracket(stream);

                var streamThread = new Thread(() => AddAndLoopReadStream(stream));
                streamThread.Name = "CLI.Communicator.StreamReader " + client.Client.LocalEndPoint;
                lock (_streamsThread) _streamsThread.Add(streamThread);
                streamThread.Start();
            }
        }

        private TcpClient LoopAcceptClient()
        {
            while (_isRunning)
            {
                try
                {
                    var client = _listener.AcceptTcpClient();
                    if (client != null) return client;
                }
                catch (SocketException)
                {
                    return null;
                }
                catch (ThreadAbortException)
                {
                    return null;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return null;
        }

        private static char[] _trimTokens = new[] { ' ', '\r', '\n', };

        private void AddAndLoopReadStream(NetworkStream stream)
        {
            lock (_streams)
            {
                if (!_isRunning) return;
                _streams.Add(stream);
            }

            while (_isRunning)
            {
                string cmd = null;
                try
                {
                    cmd = ReadStreamAll(stream);
                    if (cmd == null) break;
                }
                catch (Exception)
                {
                    break;
                }

                cmd = cmd.Trim(_trimTokens);
                var job = new Job(stream, cmd);
                lock (_concurrentJobs) _concurrentJobs.Add(job);
            }

            lock (_streams)
                _streams.Remove(stream);
        }

        private static string ReadStreamAll(NetworkStream stream)
        {
            int i = 0;
            var buf = new byte[128];
            string ret = null;

            while ((i = stream.Read(buf, 0, buf.Length)) != 0)
            {
                if (ret == null) ret = string.Empty;
                ret += Encoding.UTF8.GetString(buf, 0, i);
                if (i < buf.Length) break;
            }

            return ret;
        }

        internal void ProcessJobs()
        {
            lock (_concurrentJobs)
            {
                if (_concurrentJobs.Count == 0) return;
                _tempJobs.AddRange(_concurrentJobs);
                _concurrentJobs.Clear();
            }

            foreach (var job in _tempJobs)
            {
                var result = ProcessCmd(job.Cmd);
                WriteLineToStream(job.Stream, result.ToString());
                WriteInputBracket(job.Stream);
            }
            _tempJobs.Clear();
        }

        private Result ProcessCmd(string raw)
        {
            try
            {
                var cmd = Command.Parse(raw);
                return _executeCmd(cmd);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return Result.Error(e.Message);
            }
        }
    }
}