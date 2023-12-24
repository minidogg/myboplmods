using BepInEx;
using System;
using HarmonyLib;
using JetBrains.Annotations;
using System.Reflection;
using BoplFixedMath;
using UnityEngine;
using System.Collections.Generic;

namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "Ability Storm", "1.2.0")]
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

            MethodInfo original2 = AccessTools.Method(typeof(SlimeController), "AddAdditionalAbility");
            MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "AddAdditionalAbility_p");
            harmony.Patch(original2, new HarmonyMethod(patch2));


            //MethodInfo original = AccessTools.Field(typeof());
        }


        public static bool AddAdditionalAbility_p(AbilityMonoBehaviour ability, Sprite indicatorSprite, GameObject abilityPrefab, ref SlimeController __instance, ref Fix[] ___abilityCooldownTimers)
        {
            System.Random rnd = new System.Random();
            if (__instance.abilities.Count == 3)
            {
                int temp = rnd.Next(0, 3);
                __instance.abilities[temp] = ability;
                PlayerHandler.Get().GetPlayer(__instance.playerNumber).CurrentAbilities[2] = abilityPrefab;
                __instance.AbilityReadyIndicators[temp].SetSprite(indicatorSprite, true);
                __instance.AbilityReadyIndicators[temp].ResetAnimation();
                ___abilityCooldownTimers[temp] = (Fix)100000L;
            }
            else if (__instance.abilities.Count > 0 && __instance.AbilityReadyIndicators[0] != null)
            {
                __instance.abilities.Add(ability);
                PlayerHandler.Get().GetPlayer(__instance.playerNumber).CurrentAbilities.Add(abilityPrefab);
                __instance.AbilityReadyIndicators[__instance.abilities.Count - 1].SetSprite(indicatorSprite, true);
                __instance.AbilityReadyIndicators[__instance.abilities.Count - 1].ResetAnimation();
                ___abilityCooldownTimers[__instance.abilities.Count - 1] = (Fix)100000L;
                for (int i = 0; i < __instance.abilities.Count; i++)
                {
                    if (__instance.abilities[i] == null || __instance.abilities[i].IsDestroyed)
                    {
                        return false;
                    }
                    __instance.AbilityReadyIndicators[i].gameObject.SetActive(true);
                    __instance.AbilityReadyIndicators[i].InstantlySyncTransform();
                }
            }
            AudioManager.Get().Play("abilityPickup");
            return false;
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
