# UnityCLI
![][CLIImage]

Unity TCP CLI communication for debugging

# About This Plugin
Making debugging menu is always tedious.
It's also too complex to present every debugging UIs for all the features.
With UnityCLI, you can send commands any debugging logics to the target device without UI.

# Platform
This plugin is implemented by using .Net's System.Net, so it's platform-independent.
You can send commands from PC to mobile device.
If you have TCP terminal on your mobile device, then it's also possible to send commands from that device.

# How To Use
1. Copy bin/UnityCLI.dll to your project.

2. To open communication, call ```CLI.Bridge.TryInstall```.
```cs
CLI.Bridge.TryInstall({SOME_PORT}, {SOME_WELCOME_MESSAGE})
```
This will create singleton, which opens TCP connection at {SOME_PORT}.
If you want to communicate upon more than one port, just attach ```CLI.Bridge``` MonoBehaviour on any of your GameObject.

3. Bind your own CLI logic to ```CLI.Bridge``` like below.
```cs
void BindExecuteCmd()
{
  var inst = CLI.Bridge.Instance;
  inst.ExecuteCmd = ExecuteCmd;
}

CLI.Result ExecuteCmd(CLI.Command cmd)
{
  switch (cmd.Exe)
  {
    case "realtimeSinceStartup":
      return CLI.Result.Success("now: " + Time.realtimeSinceStartup);
    case "timescale":
      var newTimescale = float.Parse(cmd.Args[0]);
      Time.timeScale = newTimescale;
      return CLI.Result.Success("set timescale: " + newTimescale);
    default:
      return CLI.Result.InvalidCmd(cmd);
  }
}
```

4. Build and play your game on target device.

5. To make connection from PC (OSX) to target device, you can use any TCP connection tools.

If you are on OSX, then this can be easily done with ```nc```.
Open your terminal and enter below.
```
nc {TARGET_DEVICE_IP} {TARGET_DEVICE_PORT}
```
Make sure that PC and target device are reachable each other.
To check reachable, on terminal, enter ```ping {TARGET_DEVICE_IP}```.

# Contribution
To build DLL, enter ```make``` on your terminal.
The output DLL will be placed in bin/UnityCLI.dll

[CLIImage]: https://raw.githubusercontent.com/devsisters/UnityCLI/master/doc/CLI.png?token=ACr4DNTruH4ygqDJ4T6wdOT7ORichPBXks5Y02GcwA%3D%3D
