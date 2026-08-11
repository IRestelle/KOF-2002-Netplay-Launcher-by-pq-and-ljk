using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LauncherForm());
    }
}

internal sealed class LauncherForm : Form
{
    private readonly string root;
    private readonly Label statusLabel;
    private readonly Button hostButton;
    private readonly Button joinButton;

    public LauncherForm()
    {
        root = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        Text = "拳皇 2002 联机对战";
        ClientSize = new Size(474, 250);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(17, 20, 26);

        var title = new Label
        {
            Text = "拳皇 2002 联机对战",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 25),
            Size = new Size(434, 48)
        };

        hostButton = new Button
        {
            Text = "创建对战",
            Font = new Font("Microsoft YaHei UI", 13, FontStyle.Bold),
            Location = new Point(24, 92),
            Size = new Size(202, 70)
        };
        hostButton.Click += delegate { Launch(true); };

        joinButton = new Button
        {
            Text = "加入对战",
            Font = new Font("Microsoft YaHei UI", 13, FontStyle.Bold),
            Location = new Point(248, 92),
            Size = new Size(202, 70)
        };
        joinButton.Click += delegate { Launch(false); };

        statusLabel = new Label
        {
            Text = "准备就绪",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 9),
            ForeColor = Color.Gainsboro,
            Location = new Point(24, 180),
            Size = new Size(426, 40)
        };

        Controls.Add(title);
        Controls.Add(hostButton);
        Controls.Add(joinButton);
        Controls.Add(statusLabel);
    }

    private void Launch(bool host)
    {
        try
        {
            SetBusy(true, host ? "正在创建对战..." : "正在加入对战...");

            var config = LoadConfig();
            ValidateFiles(config);

            if (!host && string.IsNullOrWhiteSpace(config.hostAddress))
            {
                throw new InvalidOperationException("好友端配置缺少主机 Radmin VPN IP。");
            }

            StopRetroArch();
            string logName = "logs\\retroarch-netplay-" + (host ? "host-" : "join-") + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".log";
            var args = BuildRetroArchArguments(config, host, logName);
            StartRetroArch(config, args);

            statusLabel.Text = host
                ? "已启动主机。看到等待好友加入后，让对方加入。"
                : "已启动加入端。若主机正在等待，你应作为玩家 2 加入。";
            Log("launch mode=" + (host ? "host" : "join") + " target=" + (host ? "none" : config.hostAddress) + " log=" + logName);
        }
        catch (Exception ex)
        {
            statusLabel.Text = "启动失败";
            Log("error=" + OneLine(ex.ToString()));
            MessageBox.Show(this, ex.Message, "无法开始对战", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            SetBusy(false, statusLabel.Text);
        }
    }

    private LauncherConfig LoadConfig()
    {
        string path = Path.Combine(root, "config", "launcher.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("找不到启动器配置。", path);
        }

        var serializer = new JavaScriptSerializer();
        return serializer.Deserialize<LauncherConfig>(File.ReadAllText(path, Encoding.UTF8));
    }

    private void ValidateFiles(LauncherConfig config)
    {
        RequireFile(config.retroArchPath, "RetroArch");
        RequireFile(config.corePath, "FBNeo 核心");
        RequireFile(config.contentPath, "游戏 ROM");
        RequireFile(config.retroArchConfigPath, "RetroArch 配置");
        RequireFile("rom\\kof2002.zip", "kof2002.zip");
        RequireFile("rom\\neogeo.zip", "neogeo.zip");

        CheckHash("rom\\kf2k2mp2.zip", "D6D5588B151A2B1F55AD16DCBC2BD1290D9E93AD4EE43FA49F3ADD092EC16781");
        CheckHash("rom\\kof2002.zip", "870204AC21027C010DE7A70DA00DA2202BB710A16F7C01B9B11AB680FE5C2C51");
        CheckHash("rom\\neogeo.zip", "36A47CF50A585CAFC812E19EF7646AD1556A5DE90440D9DCB06F373926CD68EA");
    }

    private void RequireFile(string relativePath, string label)
    {
        string path = FullPath(relativePath);
        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            throw new FileNotFoundException(label + " 不存在或为空。", path);
        }
    }

    private void CheckHash(string relativePath, string expected)
    {
        string path = FullPath(relativePath);
        using (var sha = SHA256.Create())
        using (var stream = File.OpenRead(path))
        {
            string actual = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "");
            if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(Path.GetFileName(relativePath) + " 哈希不一致。");
            }
        }
    }

    private List<string> BuildRetroArchArguments(LauncherConfig config, bool host, string logName)
    {
        var args = new List<string>
        {
            "-v",
            "--log-file=" + logName,
            "-L",
            NormalizeRelative(config.corePath),
            "--config",
            NormalizeRelative(config.retroArchConfigPath)
        };

        string sessionConfig = BuildSessionConfig();
        if (sessionConfig.Length > 0)
        {
            args.Add("--appendconfig=" + sessionConfig);
        }

        args.Add("--check-frames=0");

        if (host)
        {
            args.Add("--host");
        }
        else
        {
            args.Add("--connect=" + config.hostAddress.Trim());
        }

        args.Add("--port=" + config.netplayPort);
        args.Add("--nick=" + (host ? "host" : "friend"));
        args.Add(NormalizeRelative(config.contentPath));
        return args;
    }

    private string BuildSessionConfig()
    {
        var sources = new[]
        {
            "config\\kof2002-netplay-low-latency.cfg",
            "config\\kof2002-joystick.cfg"
        };

        var builder = new StringBuilder();
        foreach (string relativePath in sources)
        {
            string fullPath = Path.Combine(root, relativePath);
            if (!File.Exists(fullPath))
            {
                continue;
            }

            builder.AppendLine("# Begin " + relativePath);
            builder.AppendLine(File.ReadAllText(fullPath, Encoding.UTF8));
            builder.AppendLine("# End " + relativePath);
        }

        if (builder.Length == 0)
        {
            return "";
        }

        string outputRelativePath = "config\\kof2002-session.generated.cfg";
        string outputFullPath = Path.Combine(root, outputRelativePath);
        File.WriteAllText(outputFullPath, builder.ToString(), Encoding.UTF8);
        return outputRelativePath;
    }

    private void StartRetroArch(LauncherConfig config, List<string> args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = FullPath(config.retroArchPath),
            Arguments = QuoteArguments(args),
            WorkingDirectory = root,
            UseShellExecute = true
        };
        Process process = Process.Start(psi);
        try
        {
            if (process != null)
            {
                process.PriorityClass = ProcessPriorityClass.High;
            }
        }
        catch
        {
            // Priority changes can fail under restricted policies; gameplay still starts normally.
        }
    }

    private void StopRetroArch()
    {
        foreach (Process process in Process.GetProcessesByName("retroarch"))
        {
            try { process.Kill(); }
            catch { }
        }
    }

    private string FullPath(string relativePath)
    {
        return Path.GetFullPath(Path.Combine(root, NormalizeRelative(relativePath)));
    }

    private string ResolvePath(string configuredPath)
    {
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return "";
        }

        string expanded = configuredPath.Replace("%ProgramFiles%", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        expanded = Environment.ExpandEnvironmentVariables(expanded);
        expanded = expanded.Replace('/', Path.DirectorySeparatorChar);
        if (Path.IsPathRooted(expanded))
        {
            return expanded;
        }

        return Path.GetFullPath(Path.Combine(root, expanded));
    }

    private static string NormalizeRelative(string path)
    {
        return (path ?? "").Replace('/', Path.DirectorySeparatorChar);
    }

    private static string QuoteArguments(IEnumerable<string> args)
    {
        var builder = new StringBuilder();
        foreach (string arg in args)
        {
            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append('"').Append((arg ?? "").Replace("\"", "\\\"")).Append('"');
        }

        return builder.ToString();
    }

    private void SetBusy(bool busy, string text)
    {
        hostButton.Enabled = !busy;
        joinButton.Enabled = !busy;
        statusLabel.Text = text;
        statusLabel.Refresh();
    }

    private void Log(string line)
    {
        string logDir = Path.Combine(root, "logs");
        Directory.CreateDirectory(logDir);
        File.AppendAllText(
            Path.Combine(logDir, "fixed-launcher-events.log"),
            "timestamp=" + DateTimeOffset.Now.ToString("o") + " " + line + Environment.NewLine,
            Encoding.UTF8);
    }

    private static string OneLine(string value)
    {
        return (value ?? "").Replace("\r", "\\r").Replace("\n", "\\n");
    }
}

internal sealed class LauncherConfig
{
    public string retroArchPath { get; set; }
    public string corePath { get; set; }
    public string contentPath { get; set; }
    public string retroArchConfigPath { get; set; }
    public string hostAddress { get; set; }
    public int netplayPort { get; set; }
}
