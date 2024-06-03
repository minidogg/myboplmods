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


namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "OneBiggon", "1.0.1")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.onebiggon";
        public static int biggonId = 0;
        public static int oldPlayerCount = -10;
        public static SlimeController[] curSc;

        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");//feel free to remove this
            Harmony harmony = new Harmony(pluginGuid);
            //spike
            /*            MethodInfo original = AccessTools.Method(typeof(Spike), "CastSpike");
                        MethodInfo patch = AccessTools.Method(typeof(Plugin), "CastSpike_p");
                        harmony.Patch(original, new HarmonyMethod(patch));*/
            MethodInfo original = AccessTools.Method(typeof(GameSessionHandler), "SpawnPlayers");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "SpawnPlayers_p");
            harmony.Patch(original, null, new HarmonyMethod(patch));

            MethodInfo original2 = AccessTools.Method(typeof(Ability), "Awake");
            MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "LessCooldown");
            harmony.Patch(original2, null, new HarmonyMethod(patch2));

            /*MethodInfo original3 = AccessTools.Method(typeof(Player), "Kill");
            MethodInfo patch3 = AccessTools.Method(typeof(Plugin), "TransferBiggon");
            harmony.Patch(original3, null, new HarmonyMethod(patch3));

            MethodInfo original4 = AccessTools.Method(typeof(Updater), "Update");
            MethodInfo patch4 = AccessTools.Method(typeof(Plugin), "ScaleBiggon");
            harmony.Patch(original4, null, new HarmonyMethod(patch4));*/


            //blackhole and other
            /*            MethodInfo original2 = AccessTools.Method(typeof(SlimeController), "isAbilityCastable");
                        MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "temp_p");
                        harmony.Patch(original2, new HarmonyMethod(patch2));*/

            //MethodInfo original = AccessTools.Field(typeof());
        }

        public static void ScaleBiggon()
        {
            List<Player> players = PlayerHandler.Get().PlayerList();
            players[biggonId].Scale = (Fix)3L;
        }
        public static void TransferBiggon(int idOfKiller, long timestamp, CauseOfDeath causeOfDeath)
        {
            if (idOfKiller < -1 || 4 < idOfKiller) return;
            biggonId = idOfKiller;
            commitBiggon();
        }
        public static void LessCooldown(ref Ability __instance, ref Fix ___Cooldown)
        {
            if (__instance.GetPlayerInfo().playerId != biggonId) return;
            ___Cooldown = ___Cooldown/(Fix)2;
            return;
        }

        public static void commitBiggon()
        {
            int id = biggonId;
            SlimeController[] slimeControllers = curSc;
            var players = PlayerHandler.Get().PlayerList();

            foreach (SlimeController s in slimeControllers)
            {
                s.GetActivePlayerCollision().isSquishable = true;
            }
            players[id].Scale = (Fix)3L;
            slimeControllers[id].GetActivePlayerCollision().isSquishable = false;

            if (players[id].IsLocalPlayer == true) return;
            foreach (AbilityReadyIndicator a in slimeControllers[id].AbilityReadyIndicators)
            {
                a.SetSprite(slimeControllers[id].abilityIconsFull.GetSprite(1), false);
            }
        }
        public static void SpawnPlayers_p(ref SlimeController[] ___slimeControllers)
        {
            var players = PlayerHandler.Get().PlayerList();

            if (oldPlayerCount != players.Count)
            {
                biggonId = 0;
                oldPlayerCount = players.Count;
            }
            biggonId += 1;
            biggonId = biggonId % (players.Count);

            curSc = ___slimeControllers;

            commitBiggon();

        }


        public static bool temp_p()
        {

            return true;
        }
        /*        public static bool CastSpike_p(ref SpikeAttack ___currentSpike)
                {
                    GameObject square = new GameObject();


                    var texture = new Texture2D(64, 64);
                    SpriteRenderer render = square.AddComponent<SpriteRenderer>();
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
                    render.sprite = sprite;
                    square.transform.localPosition = new Vector2(-3, 8);
                    square.transform.localScale = new Vector2(5, 5);
                    square.transform.transform.rotation = Quaternion.identity;

                    Rigidbody2D rb = square.AddComponent<Rigidbody2D>();
                    BoxCollider2D col = square.AddComponent<BoxCollider2D>();
                    col.size = new Vector2(5, 5);
                    //layer 11

                    Instantiate(square);
                    try
                    {
                        Console.WriteLine(___currentSpike.transform.position.x);
                        Console.WriteLine(___currentSpike.transform.position.y);
                    }
                    catch { }
                    ___currentSpike = null;
                    return true;
                }*/


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
