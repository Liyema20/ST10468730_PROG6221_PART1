using System;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

public class VoiceGreeting
{
    private static readonly BlockingCollection<string> _ttsQueue = new BlockingCollection<string>(new ConcurrentQueue<string>());
    private static readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private static string _preferredVoicePreset = "female"; // default to female for friendlier voice
    private static string _customVoiceName = null; // exact voice name for the platform
    private static bool _enabled = true;
    private static int _rate = 3; // default speed (-10..10)
    private static Process _currentProcess = null;
    private static readonly object _procLock = new object();

    static VoiceGreeting()
    {
        // Start background processor for queued TTS requests
        Task.Run(() => ProcessQueue(_cts.Token));
    }

    public static void PlayGreeting()
    {
        string message = "Hello! Welcome to the Cybersecurity Awareness Bot.";
        Console.WriteLine("🔊 " + message);
        Speak(message);
    }

    // Enqueue text to be spoken. The background processor will serialize playback
    // to avoid overlapping voices.
    public static void Speak(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (!_enabled) return;
        try
        {
            _ttsQueue.Add(message);
        }
        catch
        {
            // ignore enqueue errors
        }
    }

    public static void SetVoicePreset(string preset)
    {
        if (string.IsNullOrWhiteSpace(preset)) preset = "default";
        _preferredVoicePreset = preset.Trim().ToLower();
        _customVoiceName = null;
    }

    public static void SetCustomVoice(string voiceName)
    {
        _customVoiceName = voiceName;
        _preferredVoicePreset = "custom";
    }

    // Control TTS speed. Valid range matches System.Speech (-10..10).
    public static void SetSpeed(int newRate)
    {
        if (newRate < -10 || newRate > 10) return;
        _rate = newRate;
    }

    public static void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    // Interrupt any currently speaking audio and clear queued messages
    public static void Interrupt()
    {
        if (!_enabled) return;

        // Clear queued messages
        try
        {
            while (_ttsQueue.TryTake(out _)) { }
        }
        catch { }

        // Kill current process if running
        lock (_procLock)
        {
            try
            {
                if (_currentProcess != null && !_currentProcess.HasExited)
                {
                    try { _currentProcess.Kill(true); } catch { _currentProcess.Kill(); }
                }
            }
            catch { }
            finally
            {
                try { _currentProcess?.Dispose(); } catch { }
                _currentProcess = null;
            }
        }
    }

    // Interrupt current speech and speak this message immediately
    public static void InterruptAndSpeak(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        if (!_enabled) return;

        Interrupt();

        // Speak immediately on background thread (bypass queue)
        Task.Run(() => {
            try { SpeakNow(message); } catch { }
        });
    }

    // New getters
    public static bool IsEnabled()
    {
        return _enabled;
    }

    public static string GetVoicePreset()
    {
        return _preferredVoicePreset ?? "default";
    }

    public static string GetCustomVoiceName()
    {
        return _customVoiceName;
    }

    private static string ResolveVoiceForOS()
    {
        if (!_enabled) return null;
        // If a custom exact name is provided, use it directly
        if (!string.IsNullOrWhiteSpace(_customVoiceName)) return _customVoiceName;

        var preset = _preferredVoicePreset ?? "default";
        if (preset == "default") return null;

        if (OperatingSystem.IsWindows())
        {
            if (preset == "female") return "Microsoft Zira Desktop";
            if (preset == "male") return "Microsoft David Desktop";
        }
        else if (OperatingSystem.IsMacOS())
        {
            if (preset == "female") return "Victoria";
            if (preset == "male") return "Alex";
        }
        else if (OperatingSystem.IsLinux())
        {
            if (preset == "female") return "en+f3";
            if (preset == "male") return "en+m3";
        }

        return null;
    }

    private static void ProcessQueue(CancellationToken ct)
    {
        foreach (var msg in _ttsQueue.GetConsumingEnumerable(ct))
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                SpeakNow(msg);
            }
            catch
            {
                // ignore individual speak errors and continue with queue
            }
        }
    }

    private static void SpeakNow(string message)
    {
        if (!_enabled) return;

        try
        {
            var voice = ResolveVoiceForOS();

            if (OperatingSystem.IsWindows())
            {
                var escaped = message.Replace("'", "''");
                string selectVoice = string.IsNullOrWhiteSpace(voice) ? string.Empty : $"$ss.SelectVoice('{voice}'); ";
                // Apply speech rate (System.Speech.Synthesis.SpeechSynthesizer.Rate uses -10..10)
                var args = $"-NoProfile -Command \"Add-Type -AssemblyName System.Speech; $ss = New-Object System.Speech.Synthesis.SpeechSynthesizer; {selectVoice}$ss.Rate = {_rate}; $ss.Speak('{escaped}')\"";
                var psi = new ProcessStartInfo("powershell", args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                var p = Process.Start(psi);
                if (p != null)
                {
                    lock (_procLock)
                    {
                        _currentProcess = p;
                    }
                    try
                    {
                        p.WaitForExit();
                    }
                    finally
                    {
                        lock (_procLock)
                        {
                            try { p?.Dispose(); } catch { }
                            if (_currentProcess == p) _currentProcess = null;
                        }
                    }
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                // macOS `say` supports -v voiceName
                ProcessStartInfo psi;
                // Map internal rate (-10..10) to words-per-minute for `say` (approx)
                var wpm = Math.Clamp(175 + _rate * 12, 50, 400);
                if (string.IsNullOrWhiteSpace(voice))
                {
                    var arg = $"-r {wpm} \"{message}\"";
                    psi = new ProcessStartInfo("say", arg) { CreateNoWindow = true, UseShellExecute = false };
                }
                else
                {
                    // quote voice and message
                    var arg = $"-v {voice} -r {wpm} \"{message}\"";
                    psi = new ProcessStartInfo("say", arg) { CreateNoWindow = true, UseShellExecute = false };
                }
                var p = Process.Start(psi);
                if (p != null)
                {
                    lock (_procLock)
                    {
                        _currentProcess = p;
                    }
                    try
                    {
                        p.WaitForExit();
                    }
                    finally
                    {
                        lock (_procLock)
                        {
                            try { p?.Dispose(); } catch { }
                            if (_currentProcess == p) _currentProcess = null;
                        }
                    }
                }
            }
            else if (OperatingSystem.IsLinux())
            {
                try
                {
                    // espeak usage: espeak -v voice "text"
                    ProcessStartInfo psi;
                    // Map internal rate (-10..10) to espeak speed (words per minute)
                    var espeakSpeed = Math.Clamp(175 + _rate * 12, 50, 400);
                    if (string.IsNullOrWhiteSpace(voice))
                    {
                        psi = new ProcessStartInfo("espeak", $"-s {espeakSpeed} \"{message}\"") { CreateNoWindow = true, UseShellExecute = false };
                    }
                    else
                    {
                        psi = new ProcessStartInfo("espeak", $"-v {voice} -s {espeakSpeed} \"{message}\"") { CreateNoWindow = true, UseShellExecute = false };
                    }
                    var p = Process.Start(psi);
                    if (p != null)
                    {
                        lock (_procLock)
                        {
                            _currentProcess = p;
                        }
                        try
                        {
                            p.WaitForExit();
                        }
                        finally
                        {
                            lock (_procLock)
                            {
                                try { p?.Dispose(); } catch { }
                                if (_currentProcess == p) _currentProcess = null;
                            }
                        }
                    }
                }
                catch
                {
                    // ignore if espeak not available
                }
            }
        }
        catch
        {
            // swallow any TTS errors
        }
    }
}