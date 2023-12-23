using BepInEx;
using System;
using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using BoplFixedMath;
using UnityEngine;

namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "Ability Storm", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.abilitystorm";
        public static IInputSystem Current { get; }

        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");//feel free to remove this
            Harmony harmony = new Harmony(pluginGuid);
            MethodInfo original = AccessTools.Method(typeof(AbilitySpawner), "UpdateSim");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "UpdateSim_p");
            harmony.Patch(original, new HarmonyMethod(patch));


            //MethodInfo original = AccessTools.Field(typeof());
        }
        public static bool UpdateSim_p(Fix simDeltaTime, ref Fix ___time, ref Fix ___SpawnDelay, ref int ___spawns, ref int ___MaxSpawns, ref AbilitySpawner __instance, ref FixTransform ___fixTrans)
        {
            ___time += (GameTime.IsTimeStopped() ? Fix.Zero : simDeltaTime);
            if (___time > (Fix)0.5f)
            {
                ___time = Fix.Zero;
                //Spawn()
                System.Random rnd = new System.Random();
                Vec2 newPos = ___fixTrans.position;
                newPos.x += (Fix)rnd.Next(-50, 50);
                newPos.y += (Fix)rnd.Next(-50, 50);
                DynamicAbilityPickup dynamicAbilityPickup = FixTransform.InstantiateFixed<DynamicAbilityPickup>(__instance.pickupPrefab, newPos);
                dynamicAbilityPickup.InitPickup(null, null, Updater.RandomUnitVector());
                dynamicAbilityPickup.SwapToRandomAbility();
                //
                ___spawns++;
                if (___spawns >= ___MaxSpawns)
                {
                    //__instance.enabled = false;
                }
            }
            return false;
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
