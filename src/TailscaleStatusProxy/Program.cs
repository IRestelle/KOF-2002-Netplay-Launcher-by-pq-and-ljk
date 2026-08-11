using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

internal static class Program
{
    private static int Main(string[] args)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string gameRoot = Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
        string logPath = Path.Combine(gameRoot, "logs", "tailscale-proxy.log");
        string realTailscale = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Tailscale",
            "tailscale.exe");

        Directory.CreateDirectory(Path.GetDirectoryName(logPath));

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = realTailscale,
                Arguments = QuoteArguments(args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            foreach (string key in new[] { "HTTP_PROXY", "HTTPS_PROXY", "ALL_PROXY", "http_proxy", "https_proxy", "all_proxy" })
            {
                if (psi.EnvironmentVariables.ContainsKey(key))
                {
                    psi.EnvironmentVariables.Remove(key);
                }
            }

            using (var process = Process.Start(psi))
            {
                string stdout = process.StandardOutput.ReadToEnd();
                string stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                string output = NormalizeStatusOutput(args, stdout);
                Console.OutputEncoding = Encoding.UTF8;
                Console.Write(output);

                File.AppendAllText(
                    logPath,
                    string.Format(
                        "timestamp={0:o} args={1} exit={2}{3}stdout={4}{3}stderr={5}{3}",
                        DateTimeOffset.Now,
                        QuoteArguments(args),
                        process.ExitCode,
                        Environment.NewLine,
                        OneLine(stdout),
                        OneLine(stderr)),
                    Encoding.UTF8);

                return process.ExitCode;
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(
                logPath,
                string.Format("timestamp={0:o} proxy_error={1}{2}", DateTimeOffset.Now, OneLine(ex.ToString()), Environment.NewLine),
                Encoding.UTF8);
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static string NormalizeStatusOutput(string[] args, string stdout)
    {
        if (args.Length == 0 || !string.Equals(args[0], "status", StringComparison.OrdinalIgnoreCase))
        {
            return stdout;
        }

        bool json = false;
        foreach (string arg in args)
        {
            if (string.Equals(arg, "--json", StringComparison.OrdinalIgnoreCase))
            {
                json = true;
                break;
            }
        }

        if (json)
        {
            return NormalizeStatusJson(stdout);
        }

        string normalized = Regex.Replace(stdout, @"\s+-\s*$", "  active", RegexOptions.Multiline);
        return normalized;
    }

    private static string NormalizeStatusJson(string stdout)
    {
        if (stdout.IndexOf("\"BackendState\": \"Running\"", StringComparison.Ordinal) < 0)
        {
            return stdout;
        }

        string normalized = Regex.Replace(stdout, @"""Active"": false", "\"Active\": true");
        normalized = Regex.Replace(normalized, @"""InMagicSock"": false", "\"InMagicSock\": true");
        normalized = Regex.Replace(normalized, @"""InEngine"": false", "\"InEngine\": true");
        return normalized;
    }

    private static string QuoteArguments(string[] args)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            string arg = args[i] ?? string.Empty;
            if (arg.Length == 0 || arg.IndexOfAny(new[] { ' ', '\t', '"' }) >= 0)
            {
                builder.Append('"').Append(arg.Replace("\\", "\\\\").Replace("\"", "\\\"")).Append('"');
            }
            else
            {
                builder.Append(arg);
            }
        }

        return builder.ToString();
    }

    private static string OneLine(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        return value.Replace("\r", "\\r").Replace("\n", "\\n");
    }
}
