using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MapperForm());
    }
}

internal sealed class MapperForm : Form
{
    private readonly string root;
    private readonly Label title;
    private readonly Label prompt;
    private readonly Label hint;
    private readonly Button startButton;
    private readonly Timer timer;
    private readonly List<Step> steps;
    private readonly Dictionary<string, Binding> bindings = new Dictionary<string, Binding>();

    private int joystickId = -1;
    private int stepIndex = -1;
    private JoyState baseline;
    private bool waitingRelease;
    private DateTime releaseSince;

    public MapperForm()
    {
        root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

        Text = "拳皇 2002 摇杆按键采集";
        ClientSize = new Size(620, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(22, 25, 32);

        title = new Label
        {
            Text = "拳皇 2002 摇杆按键采集",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 18, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 24),
            Size = new Size(580, 42)
        };

        prompt = new Label
        {
            Text = "请先接好摇杆，然后点击开始。",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 20, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 90),
            Size = new Size(580, 58)
        };

        hint = new Label
        {
            Text = "采集顺序：方向、A轻拳、B轻脚、C重拳、D重脚、投币、开始。",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Microsoft YaHei UI", 10),
            ForeColor = Color.Gainsboro,
            Location = new Point(30, 160),
            Size = new Size(560, 48)
        };

        startButton = new Button
        {
            Text = "开始采集",
            Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
            Location = new Point(230, 225),
            Size = new Size(160, 42)
        };
        startButton.Click += StartButton_Click;

        Controls.Add(title);
        Controls.Add(prompt);
        Controls.Add(hint);
        Controls.Add(startButton);

        steps = new List<Step>
        {
            new Step("up", "请推摇杆：上"),
            new Step("down", "请推摇杆：下"),
            new Step("left", "请推摇杆：左"),
            new Step("right", "请推摇杆：右"),
            new Step("gameA", "请按游戏 A：轻拳"),
            new Step("gameB", "请按游戏 B：轻脚"),
            new Step("gameC", "请按游戏 C：重拳"),
            new Step("gameD", "请按游戏 D：重脚"),
            new Step("select", "请按：投币 / Select"),
            new Step("start", "请按：开始 / Start")
        };

        timer = new Timer { Interval = 25 };
        timer.Tick += delegate { Poll(); };
    }

    private void StartMapping()
    {
        joystickId = FindJoystick();
        if (joystickId < 0)
        {
            MessageBox.Show(this, "没有检测到可用摇杆。请重新插入 USB 摇杆后再试。", "未检测到摇杆", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        startButton.Enabled = false;
        stepIndex = 0;
        waitingRelease = false;
        baseline = ReadState();
        ShowCurrentPrompt();
        timer.Start();
    }

    private void Poll()
    {
        JoyState state = ReadState();

        if (waitingRelease)
        {
            if (state.IsNeutral())
            {
                if (releaseSince == DateTime.MinValue)
                {
                    releaseSince = DateTime.Now;
                }
                else if ((DateTime.Now - releaseSince).TotalMilliseconds > 350)
                {
                    stepIndex++;
                    waitingRelease = false;
                    releaseSince = DateTime.MinValue;
                    baseline = ReadState();
                    ShowCurrentPrompt();
                }
            }
            else
            {
                releaseSince = DateTime.MinValue;
            }

            return;
        }

        Binding binding = DetectBinding(baseline, state);
        if (binding == null)
        {
            return;
        }

        Step step = steps[stepIndex];
        bindings[step.Key] = binding;
        prompt.Text = "已记录：" + binding.Value;
        hint.Text = "请松开摇杆和按键...";
        waitingRelease = true;
    }

    private void ShowCurrentPrompt()
    {
        if (stepIndex >= steps.Count)
        {
            timer.Stop();
            SaveConfig();
            prompt.Text = "采集完成";
            hint.Text = "已写入 config\\kof2002-joystick.cfg，可以关闭窗口。";
            startButton.Text = "完成";
            startButton.Enabled = true;
            startButton.Click -= StartButton_Click;
            startButton.Click += CloseButton_Click;
            return;
        }

        prompt.Text = steps[stepIndex].Prompt;
        hint.Text = "第 " + (stepIndex + 1) + " / " + steps.Count + " 项。按住一下即可，记录后再松开。";
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        StartMapping();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
        Close();
    }

    private int FindJoystick()
    {
        int count = Native.joyGetNumDevs();
        for (int i = 0; i < count; i++)
        {
            var info = Native.JOYINFOEX.Create();
            if (Native.joyGetPosEx(i, ref info) == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private JoyState ReadState()
    {
        var info = Native.JOYINFOEX.Create();
        int result = Native.joyGetPosEx(joystickId, ref info);
        if (result != 0)
        {
            return new JoyState();
        }

        return new JoyState
        {
            Buttons = info.dwButtons,
            Pov = info.dwPOV,
            Axes = new[] { info.dwXpos, info.dwYpos, info.dwZpos, info.dwRpos, info.dwUpos, info.dwVpos }
        };
    }

    private Binding DetectBinding(JoyState baseState, JoyState state)
    {
        uint changedButtons = state.Buttons & ~baseState.Buttons;
        if (changedButtons != 0)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((changedButtons & (1u << i)) != 0)
                {
                    return new Binding("btn", i.ToString());
                }
            }
        }

        if (state.Pov != 0xFFFF && state.Pov != baseState.Pov)
        {
            if (state.Pov >= 31500 || state.Pov <= 4500) return new Binding("btn", "h0up");
            if (state.Pov >= 4500 && state.Pov <= 13500) return new Binding("btn", "h0right");
            if (state.Pov >= 13500 && state.Pov <= 22500) return new Binding("btn", "h0down");
            if (state.Pov >= 22500 && state.Pov <= 31500) return new Binding("btn", "h0left");
        }

        for (int axis = 0; axis < state.Axes.Length; axis++)
        {
            int delta = state.Axes[axis] - baseState.Axes[axis];
            if (Math.Abs(delta) > 12000)
            {
                return new Binding("axis", (delta > 0 ? "+" : "-") + axis);
            }
        }

        return null;
    }

    private void SaveConfig()
    {
        string configPath = Path.Combine(root, "config", "kof2002-joystick.cfg");
        Directory.CreateDirectory(Path.GetDirectoryName(configPath));

        var lines = new List<string>
        {
            "# Generated by joystick mapper for KOF 2002.",
            "input_player1_joypad_index = \"0\""
        };

        AddDirection(lines, "up", "input_player1_up");
        AddDirection(lines, "down", "input_player1_down");
        AddDirection(lines, "left", "input_player1_left");
        AddDirection(lines, "right", "input_player1_right");

        AddButton(lines, "gameA", "input_player1_b_btn", "Neo Geo A / light punch");
        AddButton(lines, "gameB", "input_player1_a_btn", "Neo Geo B / light kick");
        AddButton(lines, "gameC", "input_player1_y_btn", "Neo Geo C / heavy punch");
        AddButton(lines, "gameD", "input_player1_x_btn", "Neo Geo D / heavy kick");
        AddButton(lines, "select", "input_player1_select_btn", "coin");
        AddButton(lines, "start", "input_player1_start_btn", "start");

        File.WriteAllText(configPath, string.Join(Environment.NewLine, lines) + Environment.NewLine, Encoding.UTF8);
    }

    private void AddDirection(List<string> lines, string key, string retroKey)
    {
        Binding binding = bindings[key];
        if (binding.Kind == "axis")
        {
            lines.Add(retroKey + "_axis = \"" + binding.Value + "\"");
        }
        else
        {
            lines.Add(retroKey + "_btn = \"" + binding.Value + "\"");
        }
    }

    private void AddButton(List<string> lines, string key, string retroKey, string comment)
    {
        Binding binding = bindings[key];
        lines.Add("# " + comment);
        lines.Add(retroKey + " = \"" + binding.Value + "\"");
    }

    private sealed class Step
    {
        public readonly string Key;
        public readonly string Prompt;
        public Step(string key, string prompt)
        {
            Key = key;
            Prompt = prompt;
        }
    }

    private sealed class Binding
    {
        public readonly string Kind;
        public readonly string Value;
        public Binding(string kind, string value)
        {
            Kind = kind;
            Value = value;
        }
    }

    private sealed class JoyState
    {
        public uint Buttons;
        public int Pov = 0xFFFF;
        public int[] Axes = new[] { 32767, 32767, 32767, 32767, 32767, 32767 };

        public bool IsNeutral()
        {
            return Buttons == 0 && Pov == 0xFFFF;
        }
    }
}

internal static class Native
{
    [DllImport("winmm.dll")]
    public static extern int joyGetNumDevs();

    [DllImport("winmm.dll")]
    public static extern int joyGetPosEx(int uJoyID, ref JOYINFOEX pji);

    [StructLayout(LayoutKind.Sequential)]
    public struct JOYINFOEX
    {
        public int dwSize;
        public int dwFlags;
        public int dwXpos;
        public int dwYpos;
        public int dwZpos;
        public int dwRpos;
        public int dwUpos;
        public int dwVpos;
        public uint dwButtons;
        public int dwButtonNumber;
        public int dwPOV;
        public int dwReserved1;
        public int dwReserved2;

        public static JOYINFOEX Create()
        {
            return new JOYINFOEX
            {
                dwSize = Marshal.SizeOf(typeof(JOYINFOEX)),
                dwFlags = 0x000000FF | 0x00000080 | 0x00000040
            };
        }
    }
}
