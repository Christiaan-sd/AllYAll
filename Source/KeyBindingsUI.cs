using System;
using UnityEngine;

namespace AllYAll
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KeyBindingsUI : MonoBehaviour
    {
        private bool visible = false;
        private Rect windowRect = new Rect(200, 200, 360, 300);
        private Vector2 scrollPos = Vector2.zero;

        private string awaitingAction = null; // actionId for which we're capturing next key

        void Start()
        {
            // ensure defaults loaded
            var dummy = KeyBindings.AllBindings();
            Debug.Log("[AllYAll] KeyBindingsUI started");
        }

        void Update()
        {
            if (!FlightGlobals.ready) return;

            // Check for hotkey presses
            if (KeyBindings.IsPressed("deploy-solars"))
                ActionManager.ToggleSolarPanels();
            if (KeyBindings.IsPressed("deploy-radiators"))
                ActionManager.ToggleRadiators();
            if (KeyBindings.IsPressed("deploy-antennas"))
                ActionManager.ToggleAntennas();
            if (KeyBindings.IsPressed("execute-science"))
                ActionManager.ExecuteAllScience();
            if (KeyBindings.IsPressed("deploy-cargo-bays"))
                ActionManager.ToggleCargoBays();
        }

        void OnGUI()
        {
            if (GUILayout.Button("AllYAll Keys", GUILayout.Width(120)))
            {
                visible = !visible;
            }

            if (!visible) return;

            windowRect = GUILayout.Window("AllYAllKeys".GetHashCode(), windowRect, DrawWindow, "AllYAll Key Bindings");
        }

        void DrawWindow(int id)
        {
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(220));

            foreach (var b in KeyBindings.AllBindings())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(b.actionId, GUILayout.Width(160));
                string label = GetBindingLabel(b);
                if (awaitingAction == b.actionId)
                    label = "Press any key... (Esc to cancel)";

                if (GUILayout.Button(label, GUILayout.Width(160)))
                {
                    awaitingAction = b.actionId;
                }

                if (GUILayout.Button("Reset", GUILayout.Width(60)))
                {
                    // reset to defaults by re-registering defaults and overwriting this action
                    ResetToDefault(b.actionId);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                // saving handled on change; but ensure saved
                KeyBindingsUI_ForceSave();
            }
            if (GUILayout.Button("Close"))
            {
                visible = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            // capture key when awaiting
            if (awaitingAction != null)
            {
                Event e = Event.current;
                if (e.isKey && e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Escape)
                    {
                        awaitingAction = null;
                    }
                    else
                    {
                        var orig = KeyBindings.GetBinding(awaitingAction);
                        var nb = new KeyBinding(orig.actionId, e.keyCode, e.control, e.alt, e.shift);
                        KeyBindings.SetBinding(awaitingAction, nb);
                        awaitingAction = null;
                    }
                }
            }
        }

        private void KeyBindingsUI_ForceSave()
        {
            // Trigger save by re-setting each binding
            foreach (var b in KeyBindings.AllBindings())
                KeyBindings.SetBinding(b.actionId, b);
            Debug.Log("[AllYAll] Key bindings saved.");
        }

        private string GetBindingLabel(KeyBinding b)
        {
            string s = "";
            if (b.ctrl) s += "Ctrl+";
            if (b.alt) s += "Alt+";
            if (b.shift) s += "Shift+";
            s += b.key.ToString();
            return s;
        }

        private void ResetToDefault(string actionId)
        {
            // Recreate defaults and pick the one matching actionId
            // Note: defaults are registered only if missing, so force here
            // Simple approach: create a new manager instance of defaults then apply
            // We'll hardcode same defaults as KeyBindings.LoadDefaults
            if (actionId == "toggle-window") KeyBindings.SetBinding(actionId, new KeyBinding(actionId, KeyCode.K));
            if (actionId == "next-target") KeyBindings.SetBinding(actionId, new KeyBinding(actionId, KeyCode.RightBracket));
            if (actionId == "prev-target") KeyBindings.SetBinding(actionId, new KeyBinding(actionId, KeyCode.LeftBracket));
        }
    }
}
