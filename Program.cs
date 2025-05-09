using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Runtime.Versioning;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

// Steam-API Integration - nur bei Vertrieb über Steam aktivieren
// using Steamworks;

namespace CursorGuard
{
    public enum Language
    {
        English,
        German
    }

    public static class Localization
    {
        public static Language CurrentLanguage { get; private set; } = Language.English;

        // Dictionary for UI text translations
        private static readonly Dictionary<string, Dictionary<Language, string>> Translations = new Dictionary<string, Dictionary<Language, string>>
        {
            // Button text
            { "LockMouse", new Dictionary<Language, string>
                {
                    { Language.English, "Lock Mouse" },
                    { Language.German, "Maus sperren" }
                }
            },
            { "UnlockMouse", new Dictionary<Language, string>
                {
                    { Language.English, "Unlock Mouse" },
                    { Language.German, "Maus entsperren" }
                }
            },
            
            // Label text
            { "LockHotkey", new Dictionary<Language, string>
                {
                    { Language.English, "Lock Hotkey: ALT +" },
                    { Language.German, "Sperr-Hotkey: ALT +" }
                }
            },
            { "UnlockHotkey", new Dictionary<Language, string>
                {
                    { Language.English, "Unlock Hotkey: ALT +" },
                    { Language.German, "Entsperr-Hotkey: ALT +" }
                }
            },
            { "HotkeyExplanation", new Dictionary<Language, string>
                {
                    { Language.English, "Press ALT + the specified letter to activate the functions" },
                    { Language.German, "Drücken Sie ALT + den angegebenen Buchstaben, um die Funktionen zu aktivieren" }
                }
            },
            { "StatusLocked", new Dictionary<Language, string>
                {
                    { Language.English, "Status: Locked" },
                    { Language.German, "Status: Gesperrt" }
                }
            },
            { "StatusUnlocked", new Dictionary<Language, string>
                {
                    { Language.English, "Status: Unlocked" },
                    { Language.German, "Status: Entsperrt" }
                }
            },
            { "HotkeysRegistered", new Dictionary<Language, string>
                {
                    { Language.English, "Hotkeys registered: ALT+{0} to lock, ALT+{1} to unlock" },
                    { Language.German, "Hotkeys registriert: ALT+{0} zum Sperren, ALT+{1} zum Entsperren" }
                }
            },
            { "HotkeysNotRegistered", new Dictionary<Language, string>
                {
                    { Language.English, "Some hotkeys could not be registered" },
                    { Language.German, "Einige Hotkeys konnten nicht registriert werden" }
                }
            },
            
            // Menu items
            { "Language", new Dictionary<Language, string>
                {
                    { Language.English, "Language" },
                    { Language.German, "Sprache" }
                }
            },
            { "English", new Dictionary<Language, string>
                {
                    { Language.English, "English" },
                    { Language.German, "Englisch" }
                }
            },
            { "German", new Dictionary<Language, string>
                {
                    { Language.English, "German" },
                    { Language.German, "Deutsch" }
                }
            },
            
            // Error messages
            { "NoActiveWindow", new Dictionary<Language, string>
                {
                    { Language.English, "No active window detected." },
                    { Language.German, "Kein aktives Fenster erkannt." }
                }
            },
            { "Error", new Dictionary<Language, string>
                {
                    { Language.English, "Error" },
                    { Language.German, "Fehler" }
                }
            },
            { "WindowMinimized", new Dictionary<Language, string>
                {
                    { Language.English, "The target window is minimized or not visible." },
                    { Language.German, "Das Zielfenster ist minimiert oder nicht sichtbar." }
                }
            },
            { "WindowBoundsFailed", new Dictionary<Language, string>
                {
                    { Language.English, "Failed to get window bounds." },
                    { Language.German, "Fenstergrenzen konnten nicht ermittelt werden." }
                }
            },
            { "HotkeyRegisterFailed", new Dictionary<Language, string>
                {
                    { Language.English, "Failed to register {0} hotkey: Alt+{1}" },
                    { Language.German, "Registrierung des {0}-Hotkeys fehlgeschlagen: Alt+{1}" }
                }
            },
            { "LockHotkeyName", new Dictionary<Language, string>
                {
                    { Language.English, "lock" },
                    { Language.German, "Sperr" }
                }
            },
            { "UnlockHotkeyName", new Dictionary<Language, string>
                {
                    { Language.English, "unlock" },
                    { Language.German, "Entsperr" }
                }
            }
        };

        // Method to get translated text
        public static string GetText(string key, params object[] args)
        {
            if (Translations.ContainsKey(key) && Translations[key].ContainsKey(CurrentLanguage))
            {
                return string.Format(Translations[key][CurrentLanguage], args);
            }
            
            // Fallback to English if translation not found
            if (Translations.ContainsKey(key) && Translations[key].ContainsKey(Language.English))
            {
                return string.Format(Translations[key][Language.English], args);
            }
            
            return key; // Return the key itself as last resort
        }

        // Method to change language
        public static void SetLanguage(Language language)
        {
            CurrentLanguage = language;
        }
    }

    [SupportedOSPlatform("windows")]
    class Program
    {
    // Steam App ID - diese müsstest du von Valve zugewiesen bekommen
    private const uint STEAM_APP_ID = 480; // Beispiel ID, muss geändert werden

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try
        {
            // MessageBox.Show("Starting CursorGuard...", "Debug"); // Optional: Keep for quick visual check
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            string logDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFilePath = Path.Combine(logDirectory, "error_log.txt");
            try 
            {
                File.AppendAllText(logFilePath, DateTime.Now.ToString() + " - Unhandled Exception: " + ex.ToString() + "\n\n");
            }
            catch (Exception logEx)
            {
                // Fallback if logging to file fails
                MessageBox.Show("Failed to write to error log: " + logEx.ToString() + "\n\nOriginal Exception: " + ex.ToString(), "Critical Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("An unhandled exception occurred. Please check error_log.txt for details.\n" + ex.Message, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

    [SupportedOSPlatform("windows")]
    public class MainForm : Form
{
    private Button lockButton;
    private Button unlockButton;
    private Label statusLabel;
    private bool isMouseLocked = false;
    private TextBox lockHotkeyTextBox;
    private TextBox unlockHotkeyTextBox;
    private Keys lockHotkey = Keys.L;
    private Keys unlockHotkey = Keys.U;
    private IntPtr lockedWindow = IntPtr.Zero;
    private System.Windows.Forms.Timer checkTimer;
      // Components for language selection
    private MenuStrip? menuStrip;
    private ToolStripMenuItem? languageMenu;
    private ToolStripMenuItem? englishMenuItem;
    private ToolStripMenuItem? germanMenuItem;
    
    // Erweiterte Funktionen für Steam-Version (nur ein Beispiel)
    // private Dictionary<string, HotkeyProfile> savedProfiles = new Dictionary<string, HotkeyProfile>();
    // private ComboBox profileSelector;
    // private Button saveProfileButton;
    // private Button deleteProfileButton;    
    
    // Klasse, um Hotkey-Profile zu speichern
    // private class HotkeyProfile
    // {
    //    public Keys LockHotkey { get; set; }
    //    public Keys UnlockHotkey { get; set; }
    //    public int MarginSize { get; set; }
    // }

    public MainForm()
    {
        Text = "CursorGuard";
        Width = 400;
        Height = 320; // Increased height to accommodate menu
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle; // Fixed size
        MaximizeBox = false; // Disable maximize button
        MinimizeBox = true;  // Keep minimize button enabled

        // Create menu
        InitializeMenu();

        lockButton = new Button
        {
            Text = Localization.GetText("LockMouse"),
            Width = 100,
            Top = 50, // Moved down to make room for menu
            Left = 50
        };
        lockButton.Click += (sender, e) => LockMouseToWindow();        unlockButton = new Button
        {
            Text = Localization.GetText("UnlockMouse"),
            Width = 100,
            Top = 50, // Match with lockButton top position
            Left = 200
        };
        unlockButton.Click += (sender, e) => UnlockMouse();        lockHotkeyTextBox = new TextBox
        {
            Text = lockHotkey.ToString(),
            Top = 100,
            Left = 50,
            Width = 100
        };

        unlockHotkeyTextBox = new TextBox
        {
            Text = unlockHotkey.ToString(),
            Top = 100,
            Left = 200,
            Width = 100
        };

        Label lockHotkeyLabel = new Label
        {
            Text = Localization.GetText("LockHotkey"),
            Top = 80,
            Left = 50,
            Width = 120
        };

        Label unlockHotkeyLabel = new Label
        {
            Text = Localization.GetText("UnlockHotkey"),
            Top = 80,
            Left = 200,
            Width = 120
        };

        Label hotkeyExplanationLabel = new Label
        {
            Text = Localization.GetText("HotkeyExplanation"),
            Top = 130,
            Left = 50,
            Width = 300,
            Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
        };        statusLabel = new Label
        {
            Text = Localization.GetText("StatusUnlocked"),
            Top = 180,  // Move it down a bit to make room for the explanation
            Left = 50,
            Width = 300,
            Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Bold)
        };

        Controls.Add(lockButton);
        Controls.Add(unlockButton);
        Controls.Add(lockHotkeyLabel);
        Controls.Add(unlockHotkeyLabel);
        Controls.Add(lockHotkeyTextBox);
        Controls.Add(unlockHotkeyTextBox);
        Controls.Add(hotkeyExplanationLabel);
        Controls.Add(statusLabel);        // Initialize timer to check mouse bounds
        checkTimer = new System.Windows.Forms.Timer();
        checkTimer.Interval = 100; // 100ms
        checkTimer.Tick += (s, e) => {
            if (isMouseLocked && lockedWindow != IntPtr.Zero)
            {
                UpdateMouseLockBounds();
            }
        };
        checkTimer.Start();

        RegisterHotKeys();
        InitializeKeybindListeners();
        UpdateUILanguage(); // Add this line to update UI with current language
    }

    private void InitializeMenu()
    {
        // Create menu strip
        menuStrip = new MenuStrip();
        
        // Create language menu
        languageMenu = new ToolStripMenuItem(Localization.GetText("Language"));
        
        // Create menu items
        englishMenuItem = new ToolStripMenuItem(Localization.GetText("English"));
        englishMenuItem.Click += (sender, e) => {
            Localization.SetLanguage(Language.English);
            UpdateUILanguage();
            CheckSelectedLanguage();
        };
        
        germanMenuItem = new ToolStripMenuItem(Localization.GetText("German"));
        germanMenuItem.Click += (sender, e) => {
            Localization.SetLanguage(Language.German);
            UpdateUILanguage();
            CheckSelectedLanguage();
        };
        
        // Add items to menu
        languageMenu.DropDownItems.Add(englishMenuItem);
        languageMenu.DropDownItems.Add(germanMenuItem);
        
        menuStrip.Items.Add(languageMenu);
        
        // Add menu to form
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        
        // Check which language is currently selected
        CheckSelectedLanguage();
    }
      private void CheckSelectedLanguage()
    {
        // Set checkmark on the selected language
        if (englishMenuItem != null)
            englishMenuItem.Checked = (Localization.CurrentLanguage == Language.English);
        if (germanMenuItem != null)
            germanMenuItem.Checked = (Localization.CurrentLanguage == Language.German);
    }
    
    private void UpdateUILanguage()
    {
        // Update all UI elements with the current language
        lockButton.Text = Localization.GetText("LockMouse");
        unlockButton.Text = Localization.GetText("UnlockMouse");
        
        // Update menu items
        if (languageMenu != null)
            languageMenu.Text = Localization.GetText("Language");
        if (englishMenuItem != null)
            englishMenuItem.Text = Localization.GetText("English");
        if (germanMenuItem != null)
            germanMenuItem.Text = Localization.GetText("German");
        
        // Update other UI elements
        foreach (Control control in Controls)
        {
            if (control is Label label)
            {
                if (label == statusLabel)
                {
                    label.Text = isMouseLocked 
                        ? Localization.GetText("StatusLocked")
                        : Localization.GetText("StatusUnlocked");
                }
                else if (label.Text.Contains("Lock Hotkey"))
                {
                    label.Text = Localization.GetText("LockHotkey");
                }
                else if (label.Text.Contains("Unlock Hotkey"))
                {
                    label.Text = Localization.GetText("UnlockHotkey");
                }
                else if (label.Text.Contains("Drücken Sie") || label.Text.Contains("Press ALT"))
                {
                    label.Text = Localization.GetText("HotkeyExplanation");
                }
            }
        }
    }

    private void RegisterHotKeys()
    {
        const int MOD_ALT = 0x0001;

        // Unregister existing hotkeys to avoid conflicts
        UnregisterHotKey(this.Handle, 1);
        UnregisterHotKey(this.Handle, 2);

        // Register new hotkeys - use more common combinations with ALT
        bool lockHotkeyRegistered = RegisterHotKey(this.Handle, 1, MOD_ALT, (int)lockHotkey);
        bool unlockHotkeyRegistered = RegisterHotKey(this.Handle, 2, MOD_ALT, (int)unlockHotkey);
        if (!lockHotkeyRegistered)
        {
            MessageBox.Show(
                Localization.GetText("HotkeyRegisterFailed", Localization.GetText("LockHotkeyName"), lockHotkey.ToString()), 
                Localization.GetText("Error"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }

        if (!unlockHotkeyRegistered)
        {
            MessageBox.Show(
                Localization.GetText("HotkeyRegisterFailed", Localization.GetText("UnlockHotkeyName"), unlockHotkey.ToString()), 
                Localization.GetText("Error"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }

        // Update UI to show the current hotkeys
        lockHotkeyTextBox.Text = $"ALT+{lockHotkey}";
        unlockHotkeyTextBox.Text = $"ALT+{unlockHotkey}";

        this.FormClosing += (sender, e) =>
        {
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
        };
        // Show a message to confirm hotkey registration
        statusLabel.Text = lockHotkeyRegistered && unlockHotkeyRegistered 
            ? Localization.GetText("HotkeysRegistered", lockHotkey.ToString(), unlockHotkey.ToString())
            : Localization.GetText("HotkeysNotRegistered");
    }

    private void InitializeKeybindListeners()
    {
        lockHotkeyTextBox.KeyDown += (sender, e) =>
        {
            lockHotkey = e.KeyCode;
            lockHotkeyTextBox.Text = lockHotkey.ToString();
            e.SuppressKeyPress = true; // Prevent the key from being entered as text
            
            // Update the hotkey registration
            RegisterHotKeys();
        };

        unlockHotkeyTextBox.KeyDown += (sender, e) =>
        {
            unlockHotkey = e.KeyCode;
            unlockHotkeyTextBox.Text = unlockHotkey.ToString();
            e.SuppressKeyPress = true; // Prevent the key from being entered as text
            
            // Update the hotkey registration
            RegisterHotKeys();
        };
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;

        if (m.Msg == WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();
            if (id == 1) // Hotkey ID 1 for Lock
            {
                LockMouseToWindow();
            }
            else if (id == 2) // Hotkey ID 2 for Unlock
            {
                UnlockMouse();
            }
        }

        base.WndProc(ref m);
    }

    private void UpdateMouseLockBounds()
    {
        if (!isMouseLocked || lockedWindow == IntPtr.Zero)
            return;

        if (GetWindowRect(lockedWindow, out RECT rect))
        {
            // Check if the window is minimized or not visible
            if (rect.Left == rect.Right || rect.Top == rect.Bottom)
            {
                UnlockMouse();
                return;
            }

            // Create a smaller rectangle to prevent resizing
            int margin = 10; // margin from the edges in pixels
            rect.Left += margin;
            rect.Top += margin;
            rect.Right -= margin;
            rect.Bottom -= margin;

            // Adjust the rectangle to ensure the mouse stays within bounds
            ClipCursor(ref rect);
        }
        else
        {
            // If we can't get the window rect, unlock the mouse
            UnlockMouse();
        }
    }    private void LockMouseToWindow()
    {
        IntPtr hWnd = GetForegroundWindow();
        if (hWnd == IntPtr.Zero)
        {
            MessageBox.Show(
                Localization.GetText("NoActiveWindow"),
                Localization.GetText("Error"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
            return;
        }

        if (GetWindowRect(hWnd, out RECT rect))
        {
            // Check if the window is minimized or not visible
            if (rect.Left == rect.Right || rect.Top == rect.Bottom)
            {
                MessageBox.Show(
                    Localization.GetText("WindowMinimized"),
                    Localization.GetText("Error"), 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                return;
            }

            // Create a smaller rectangle to prevent resizing
            int margin = 10; // margin from the edges in pixels
            rect.Left += margin;
            rect.Top += margin;
            rect.Right -= margin;
            rect.Bottom -= margin;

            ClipCursor(ref rect);
            isMouseLocked = true;
            lockedWindow = hWnd;
            statusLabel.Text = Localization.GetText("StatusLocked");
        }
        else
        {
            MessageBox.Show(
                Localization.GetText("WindowBoundsFailed"),
                Localization.GetText("Error"), 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error
            );
        }
    }

    private void UnlockMouse()
    {
        ClipCursor(IntPtr.Zero);
        isMouseLocked = false;
        lockedWindow = IntPtr.Zero;
        statusLabel.Text = Localization.GetText("StatusUnlocked");
    }

    [DllImport("user32.dll")]
    private static extern bool ClipCursor(ref RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClipCursor(IntPtr lpRect);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [StructLayout(LayoutKind.Sequential)]    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    }
}
