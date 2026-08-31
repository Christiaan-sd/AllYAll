using System;
using UnityEngine;

namespace AllYAll
{
    /// <summary>
    /// ActionManager - Provides static methods for hotkey-triggered actions
    /// Coordinates all deploy/retract/execute actions across the vessel
    /// </summary>
    public static class ActionManager
    {
        /// <summary>
        /// Deploy/Retract all solar panels (toggle based on current state)
        /// </summary>
        public static void ToggleSolarPanels()
        {
            if (HighLogic.LoadedSceneIsEditor || !FlightGlobals.ready)
            {
                Debug.Log("[AllYAll] Action ignored - not in Flight scene");
                return;
            }

            try
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                if (activeVessel == null)
                    return;

                bool extended = false;
                // Check state of first panel to determine action
                foreach (Part p in activeVessel.Parts)
                {
                    var panel = p.FindModuleImplementing<ModuleDeployableSolarPanel>();
                    if (panel != null && panel.animationName != "")
                    {
                        extended = (panel.deployState == ModuleDeployablePart.DeployState.EXTENDED);
                        break;
                    }
                }

                // Apply to all panels
                foreach (Part p in activeVessel.Parts)
                {
                    var panel = p.FindModuleImplementing<ModuleDeployableSolarPanel>();
                    if (panel != null && panel.animationName != "")
                    {
                        if (extended)
                            panel.Retract();
                        else
                            panel.Extend();
                    }
                }

                Debug.Log($"[AllYAll] Solar panels {(extended ? "retracting" : "extending")}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AllYAll] Error toggling solar panels: {ex.Message}");
            }
        }

        /// <summary>
        /// Deploy/Retract all radiators
        /// </summary>
        public static void ToggleRadiators()
        {
            if (HighLogic.LoadedSceneIsEditor || !FlightGlobals.ready)
            {
                Debug.Log("[AllYAll] Action ignored - not in Flight scene");
                return;
            }

            try
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                if (activeVessel == null)
                    return;

                bool extended = false;
                foreach (Part p in activeVessel.Parts)
                {
                    var radiator = p.FindModuleImplementing<ModuleDeployableRadiator>();
                    if (radiator != null)
                    {
                        extended = (radiator.deployState == ModuleDeployablePart.DeployState.EXTENDED);
                        break;
                    }
                }

                foreach (Part p in activeVessel.Parts)
                {
                    var radiator = p.FindModuleImplementing<ModuleDeployableRadiator>();
                    if (radiator != null)
                    {
                        if (extended)
                            radiator.Retract();
                        else
                            radiator.Extend();
                    }
                }

                Debug.Log($"[AllYAll] Radiators {(extended ? "retracting" : "extending")}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AllYAll] Error toggling radiators: {ex.Message}");
            }
        }

        /// <summary>
        /// Deploy/Retract all antennas
        /// </summary>
        public static void ToggleAntennas()
        {
            if (HighLogic.LoadedSceneIsEditor || !FlightGlobals.ready)
            {
                Debug.Log("[AllYAll] Action ignored - not in Flight scene");
                return;
            }

            try
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                if (activeVessel == null)
                    return;

                bool extended = false;
                foreach (Part p in activeVessel.Parts)
                {
                    var antenna = p.FindModuleImplementing<ModuleDeployableAntenna>();
                    if (antenna != null && antenna.animationName != "")
                    {
                        extended = (antenna.deployState == ModuleDeployablePart.DeployState.EXTENDED);
                        break;
                    }
                }

                foreach (Part p in activeVessel.Parts)
                {
                    var antenna = p.FindModuleImplementing<ModuleDeployableAntenna>();
                    if (antenna != null && antenna.animationName != "")
                    {
                        if (extended)
                            antenna.Retract();
                        else
                            antenna.Extend();
                    }
                }

                Debug.Log($"[AllYAll] Antennas {(extended ? "retracting" : "extending")}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AllYAll] Error toggling antennas: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute all science experiments
        /// </summary>
        public static void ExecuteAllScience()
        {
            if (HighLogic.LoadedSceneIsEditor || !FlightGlobals.ready)
            {
                Debug.Log("[AllYAll] Action ignored - not in Flight scene");
                return;
            }

            try
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                if (activeVessel == null)
                    return;

                int count = 0;
                foreach (Part p in activeVessel.Parts)
                {
                    var science = p.FindModuleImplementing<ModuleScienceExperiment>();
                    if (science != null)
                    {
                        // Check if experiment can be deployed: not inoperable, not already deployed, and has science available
                        if (!science.Inoperable && !science.Deployed)
                        {
                            try
                            {
                                science.DeployExperiment();
                                count++;
                                Debug.Log($"[AllYAll] Deployed science: {p.name}");
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"[AllYAll] Could not deploy {p.name}: {ex.Message}");
                            }
                        }
                    }
                }

                Debug.Log($"[AllYAll] Executed {count} science experiments");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AllYAll] Error executing science: {ex.Message}");
            }
        }

        /// <summary>
        /// Deploy/Retract all cargo bays (via ModuleDockingPort-adjacent actions)
        /// </summary>
        public static void ToggleCargoBays()
        {
            if (HighLogic.LoadedSceneIsEditor || !FlightGlobals.ready)
            {
                Debug.Log("[AllYAll] Action ignored - not in Flight scene");
                return;
            }

            try
            {
                Vessel activeVessel = FlightGlobals.ActiveVessel;
                if (activeVessel == null)
                    return;

                Debug.Log($"[AllYAll] Cargo bay toggle - not yet supported");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AllYAll] Error toggling cargo bays: {ex.Message}");
            }
        }
    }
}
