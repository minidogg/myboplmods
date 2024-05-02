using BepInEx;
using System;
using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using BoplFixedMath;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using BepInEx.Logging;
using UnityEngine.UI;
using UnityEngine.UIElements;
using BepInEx.Configuration;

namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "LessLimit", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.lesslimit";
        public static string cheese = "Cheese?";
        public static Canvas canvas;
        public static bool menu = false;

        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");//feel free to remove this
            Harmony harmony = new Harmony(pluginGuid);
            //spike
            MethodInfo original = AccessTools.Method(typeof(Spike), "CastSpike");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "CastSpike_p");
            harmony.Patch(original, new HarmonyMethod(patch));

            //blackhole and other
            /*            MethodInfo original2 = AccessTools.Method(typeof(SlimeController), "isAbilityCastable");
                        MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "temp_p");
                        harmony.Patch(original2, new HarmonyMethod(patch2));*/


            //MethodInfo original = AccessTools.Field(typeof());
        }


        public static bool temp_p()
        {

            return true;
        }
        public static bool CastSpike_p(ref SpikeAttack ___currentSpike)
        {
            ___currentSpike = null;
            return true;
        }

        private Rect windowRect = new Rect(0, 50, 500, (Screen.width) * 0.2f);

        public string GUIName = "Config Manager";
        private UnityEngine.Color guiColor = UnityEngine.Color.cyan;


        private void Update() {
            menu = SceneManager.GetActiveScene().name == "MainMenu";
        }
        private void OnGUI()
        {
            if (menu == false)
            {
                return;
            }
            windowRect.width = (Screen.width) * 0.35f;
            windowRect.y = (Screen.width) * 0.05f;
            windowRect.height = (Screen.height) * 0.8f;
            GUI.Window(10000, windowRect, WindowGUI, GUIName);

            void WindowGUI(int windowID)
            {

            }
        }



        /*        public static bool UpdateSim_p(Fix simDeltaTime, ref Fix ___time, ref Fix ___SpawnDelay, ref int ___spawns, ref int ___MaxSpawns,ref AbilitySpawner __instance, ref FixTransform ___fixTrans)
                {
                    ___time += (GameTime.IsTimeStopped() ? Fix.Zero : simDeltaTime);
                    if (___time > ___SpawnDelay)
                    {
                        ___time = Fix.Zero;
                        //spawn
                        DynamicAbilityPickup dynamicAbilityPickup = FixTransform.InstantiateFixed<DynamicAbilityPickup>(__instance.pickupPrefab, ___fixTrans.position);
                        dynamicAbilityPickup.InitPickup(null, null, Updater.RandomUnitVector());
                        dynamicAbilityPickup.SwapToRandomAbility();
                        //
                        ___spawns++;
                        if (___spawns >= ___MaxSpawns)
                        {
                            __instance.enabled = false;
                        }
                    }
                    return false;
                }*/

    }
}
