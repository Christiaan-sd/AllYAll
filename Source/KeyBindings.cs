using System;
using System.Collections.Generic;
using UnityEngine;

namespace AllYAll
{
    // Simple serializable binding representation
    [Serializable]
    public class KeyBinding
    {
        public string actionId;
        public KeyCode key;
        public bool ctrl;
        public bool alt;
        public bool shift;

        public KeyBinding() { }
        public KeyBinding(string actionId, KeyCode key, bool ctrl = false, bool alt = false, bool shift = false)
        {
            this.actionId = actionId;
            this.key = key;
            this.ctrl = ctrl;
            this.alt = alt;
            this.shift = shift;
        }

        public bool MatchesCurrent()
        {
            // Check modifiers
            if (ctrl != (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) return false;
            if (alt != (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) return false;
            if (shift != (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) return false;

            return Input.GetKeyDown(key);
        }

        public string ToConfigString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}", actionId, key, ctrl ? 1 : 0, alt ? 1 : 0, shift ? 1 : 0);
        }

        public static KeyBinding FromConfigString(string line)
        {
            // actionId:KeyCode:ctrl:alt:shift
            var parts = line.Split(':');
            if (parts.Length < 5) return null;
            try
            {
                var action = parts[0];
                var key = (KeyCode)Enum.Parse(typeof(KeyCode), parts[1]);
                var ctrl = parts[2] == "1";
                var alt = parts[3] == "1";
                var shift = parts[4] == "1";
                return new KeyBinding(action, key, ctrl, alt, shift);
            }
            catch { return null; }
        }
    }

    // Manager: load/save and query bindings
    public static class KeyBindings
    {
        private static Dictionary<string, KeyBinding> bindings = new Dictionary<string, KeyBinding>();
        private static string cfgPath = "GameData/AllYAll/Keys.cfg";

        static KeyBindings()
        {
            LoadDefaults();
            LoadFromConfig();
        }

        public static void RegisterDefault(string actionId, KeyCode key, bool ctrl = false, bool alt = false, bool shift = false)
        {
            if (!bindings.ContainsKey(actionId))
                bindings[actionId] = new KeyBinding(actionId, key, ctrl, alt, shift);
        }

        public static void SetBinding(string actionId, KeyBinding binding)
        {
            bindings[actionId] = binding;
            SaveToConfig();
        }

        public static KeyBinding GetBinding(string actionId)
        {
            if (bindings.TryGetValue(actionId, out var b)) return b;
            return null;
        }

        public static bool IsPressed(string actionId)
        {
            var b = GetBinding(actionId);
            if (b == null) return false;
            return b.MatchesCurrent();
        }

        public static IEnumerable<KeyBinding> AllBindings() => bindings.Values;

        private static void LoadDefaults()
        {
            // Register sensible defaults for this mod's actions
            RegisterDefault("deploy-solars", KeyCode.S);
            RegisterDefault("deploy-radiators", KeyCode.R);
            RegisterDefault("deploy-antennas", KeyCode.A);
            RegisterDefault("execute-science", KeyCode.X);
            RegisterDefault("deploy-cargo-bays", KeyCode.B);
        }

        private static void LoadFromConfig()
        {
            try
            {
                if (!System.IO.File.Exists(cfgPath)) return;
                var lines = System.IO.File.ReadAllLines(cfgPath);
                foreach (var l in lines)
                {
                    var t = KeyBinding.FromConfigString(l);
                    if (t != null) bindings[t.actionId] = t;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[AllYAll] Failed to read key config: " + e.Message);
            }
        }

        private static void SaveToConfig()
        {
            try
            {
                var dir = System.IO.Path.GetDirectoryName(cfgPath);
                if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                var lines = new List<string>();
                foreach (var b in bindings.Values)
                    lines.Add(b.ToConfigString());
                System.IO.File.WriteAllLines(cfgPath, lines.ToArray());
            }
            catch (Exception e)
            {
                Debug.Log("[AllYAll] Failed to save key config: " + e.Message);
            }
        }
    }
}
