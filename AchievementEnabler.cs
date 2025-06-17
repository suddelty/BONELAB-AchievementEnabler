using MelonLoader;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[assembly: MelonInfo(typeof(AchievementEnabler.AchievementEnablerMod), "Achievement Enabler", "1.0.0", "suddelty")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace AchievementEnabler
{
    public class AchievementEnablerMod : MelonMod
    {
        private bool achievementsForced = false;
        private List<Type> steamTypes = new List<Type>();
        private static bool patchesApplied = false;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Achievement Enabler Mod Starting...");
            MelonLogger.Msg("This mod ensures achievements remain enabled with mods loaded.");
            
            try
            {
                // Set up achievement monitoring and patching
                SetupAchievementMonitoring();
                ApplyAchievementPatches();
                MelonLogger.Msg("Achievement Enabler initialized successfully!");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to initialize Achievement Enabler: {ex}");
            }
        }

        private void SetupAchievementMonitoring()
        {
            try
            {
                MelonLogger.Msg("Setting up achievement monitoring...");
                
                // Look for Steam achievement-related types
                steamTypes = FindSteamAchievementTypes();
                MelonLogger.Msg($"Found {steamTypes.Count} potential Steam achievement types");
                
                // Set flag to enable runtime monitoring
                achievementsForced = true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error in SetupAchievementMonitoring: {ex}");
            }
        }

        private void ApplyAchievementPatches()
        {
            if (patchesApplied) return;
            
            try
            {
                MelonLogger.Msg("Applying achievement patches...");
                
                // Patch Steam achievement validation methods
                foreach (var steamType in steamTypes)
                {
                    PatchSteamAchievementType(steamType);
                }
                
                // Patch general mod detection methods
                PatchModDetectionMethods();
                
                patchesApplied = true;
                MelonLogger.Msg("Achievement patches applied successfully!");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error applying achievement patches: {ex}");
            }
        }

        private List<Type> FindSteamAchievementTypes()
        {
            var types = new List<Type>();
            
            try
            {
                // Search for Steam achievement-related types specifically
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            // Look for Steam-specific achievement types
                            if ((type.Name.Contains("Steam") && (type.Name.Contains("Achievement") || type.Name.Contains("Stats"))) ||
                                type.Name.Contains("SteamManager") ||
                                type.Name.Contains("AchievementManager"))
                            {
                                types.Add(type);
                                MelonLogger.Msg($"Found Steam achievement type: {type.FullName}");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Skip assemblies that can't be reflected over
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Error searching for Steam achievement types: {ex}");
            }
            
            return types;
        }

        private void PatchSteamAchievementType(Type steamType)
        {
            try
            {
                var methods = steamType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                
                foreach (var method in methods)
                {
                    // Target specific methods that might block achievements
                    if (IsAchievementBlockingMethod(method))
                    {
                        try
                        {
                            var patch = new HarmonyMethod(typeof(AchievementPatches), nameof(AchievementPatches.AllowAchievementPrefix));
                            HarmonyInstance.Patch(method, prefix: patch);
                            MelonLogger.Msg($"Patched achievement method: {steamType.Name}.{method.Name}");
                        }
                        catch (Exception patchEx)
                        {
                            MelonLogger.Warning($"Failed to patch {steamType.Name}.{method.Name}: {patchEx.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Error patching Steam type {steamType.Name}: {ex.Message}");
            }
        }

        private void PatchModDetectionMethods()
        {
            try
            {
                // Look for methods that detect mods and disable achievements
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            if (!type.Name.Contains("Steam") && !type.Name.Contains("Achievement"))
                                continue;
                                
                            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                            
                            foreach (var method in methods)
                            {
                                if (IsModDetectionMethod(method))
                                {
                                    try
                                    {
                                        var patch = new HarmonyMethod(typeof(AchievementPatches), nameof(AchievementPatches.NoModsDetectedPrefix));
                                        HarmonyInstance.Patch(method, prefix: patch);
                                        MelonLogger.Msg($"Patched mod detection: {type.Name}.{method.Name}");
                                    }
                                    catch (Exception patchEx)
                                    {
                                        MelonLogger.Warning($"Failed to patch mod detection {type.Name}.{method.Name}: {patchEx.Message}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Error patching mod detection methods: {ex.Message}");
            }
        }

        private bool IsAchievementBlockingMethod(MethodInfo method)
        {
            var name = method.Name.ToLower();
            return name.Contains("canunlock") || 
                   name.Contains("isallowed") || 
                   name.Contains("validate") ||
                   name.Contains("checkmod") ||
                   name.Contains("isenabled") ||
                   (name.Contains("get") && name.Contains("enable"));
        }

        private bool IsModDetectionMethod(MethodInfo method)
        {
            var name = method.Name.ToLower();
            return name.Contains("hasmod") || 
                   name.Contains("ismod") ||
                   name.Contains("checkmod") ||
                   name.Contains("modsenabled") ||
                   name.Contains("detectmod");
        }

        public override void OnUpdate()
        {
            // Lightweight monitoring - only check occasionally
            if (!achievementsForced)
                return;
                
            // Every few seconds, ensure achievement systems are still enabled
            if (Time.frameCount % 300 == 0) // Check every ~5 seconds at 60fps
            {
                EnsureAchievementsEnabled();
            }
        }

        private void EnsureAchievementsEnabled()
        {
            try
            {
                // Runtime monitoring of Steam achievement managers
                foreach (var steamType in steamTypes)
                {
                    try
                    {
                        // Find any instances of Steam achievement managers
                        var instances = FindObjectsOfType(steamType);
                        
                        foreach (var instance in instances)
                        {
                            if (instance != null)
                            {
                                ForceEnableAchievements(instance);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Continue with other types if one fails
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                // Don't spam logs with runtime errors
            }
        }

        private UnityEngine.Object[] FindObjectsOfType(Type type)
        {
            try
            {
                // Use reflection to call UnityEngine.Object.FindObjectsOfType<T>()
                var method = typeof(UnityEngine.Object).GetMethod("FindObjectsOfType", new Type[] { });
                if (method != null)
                {
                    var genericMethod = method.MakeGenericMethod(type);
                    return (UnityEngine.Object[])genericMethod.Invoke(null, null);
                }
            }
            catch (Exception)
            {
                // Fallback: return empty array
            }
            
            return new UnityEngine.Object[0];
        }

        private void ForceEnableAchievements(object achievementManager)
        {
            try
            {
                var type = achievementManager.GetType();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (var field in fields)
                {
                    var fieldName = field.Name.ToLower();
                    
                    // Look for fields that might control achievement enabling
                    if ((fieldName.Contains("enable") || fieldName.Contains("allow") || fieldName.Contains("active")) &&
                        (fieldName.Contains("achievement") || fieldName.Contains("unlock")))
                    {
                        if (field.FieldType == typeof(bool))
                        {
                            var currentValue = (bool)field.GetValue(achievementManager);
                            if (!currentValue)
                            {
                                field.SetValue(achievementManager, true);
                                MelonLogger.Msg($"Enabled achievement field: {type.Name}.{field.Name}");
                            }
                        }
                    }
                    
                    // Also check for mod detection flags
                    if ((fieldName.Contains("mod") && fieldName.Contains("detect")) ||
                        fieldName.Contains("hasmod") ||
                        fieldName.Contains("modded"))
                    {
                        if (field.FieldType == typeof(bool))
                        {
                            var currentValue = (bool)field.GetValue(achievementManager);
                            if (currentValue)
                            {
                                field.SetValue(achievementManager, false);
                                MelonLogger.Msg($"Disabled mod detection field: {type.Name}.{field.Name}");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Continue if field modification fails
            }
        }

                public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            // This runs when scenes load, which is better for achievement initialization
            if (buildIndex == 0) // Typically the main menu or first scene
            {
                MelonLogger.Msg("BONELAB Achievement Enabler is active!");
                MelonLogger.Msg("Achievements should work normally with mods installed.");
                
                // Log some useful information
                MelonLogger.Msg("Game: BONELAB");
                MelonLogger.Msg($"MelonLoader Version: {BuildInfo.Version}");
                MelonLogger.Msg("If achievements still don't work, make sure:");
                MelonLogger.Msg("1. Steam overlay is enabled");
                MelonLogger.Msg("2. You're logged into Steam");
                MelonLogger.Msg("3. The game is launched through Steam");
            }
        }
    }

    // Harmony patch methods for achievement enabling
    public static class AchievementPatches
    {
        // Prefix patch that forces achievement validation to always return true
        public static bool AllowAchievementPrefix(ref bool __result)
        {
            __result = true;
            return false; // Skip the original method
        }

        // Prefix patch that makes mod detection always return false (no mods detected)
        public static bool NoModsDetectedPrefix(ref bool __result)
        {
            __result = false;
            return false; // Skip the original method
        }
    }
} 