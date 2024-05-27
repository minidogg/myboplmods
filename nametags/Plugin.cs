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
using TMPro;
using Steamworks;


namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "NameTags", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.nametags";
        public static Canvas canvas;

        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");//feel free to remove this
            Harmony harmony = new Harmony(pluginGuid);
            //spike
            MethodInfo original = AccessTools.Method(typeof(Spike), "CastSpike");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "CastSpike_p");
            harmony.Patch(original, new HarmonyMethod(patch));

            MethodInfo original2 = AccessTools.Method(typeof(SlimeController), "Awake");
            MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "ItsKronkingTime");
            harmony.Patch(original2, null, new HarmonyMethod(patch2));


            
            //blackhole and other
            /*            MethodInfo original2 = AccessTools.Method(typeof(SlimeController), "isAbilityCastable"); 
                        MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "temp_p");
                        harmony.Patch(original2, new HarmonyMethod(patch2));*/


            //MethodInfo original = AccessTools.Field(typeof());
        }



        public static List<GameObject> textObjs;
        public static List<TextMeshProUGUI> textComps;
        public static List<RectTransform> locations;
        public static List<GameObject> playerGameObjs;
        public static void EmptyOutLists()
        {
            textObjs.Clear();
            textComps.Clear();
            locations.Clear();
        }
        public static void SpawnPlayers_p(ref SlimeController[] ___slimeControllers) { 
            EmptyOutLists();
    
        }
        public static void ItsKronkingTime(ref SlimeController __instance)
        {

            // I don't understand why this is the correct Canvas, but it is
            canvas = GameObject.Find("AbilitySelectCanvas").GetComponent<Canvas>();

            // If canvas doesn't exist yet
            if (canvas == null) throw new MissingReferenceException("Game canvas doesn't exist yet!");

            GameObject textObj;
            TextMeshProUGUI textComp;

            // Create text object
            textObj = new GameObject("NameTag", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(canvas.transform);

            // Create text component
            textComp = textObj.GetComponent<TextMeshProUGUI>();

            // Dunno what this does
            //textComp.raycastTarget = false;

            textComp.fontSize = 20f;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.font = LocalizedText.localizationTable.GetFont(Settings.Get().Language, false);

            // Sets the textbox to the top of the screen
            RectTransform location = textObj.GetComponent<RectTransform>();

            GameObject playerGameObj = __instance.gameObject;
            location.anchoredPosition = GetPosOnScreen((Vector2)playerGameObj.transform.position,canvas);

            textObj.SetActive(true);

            var players = PlayerHandler.Get().PlayerList();
            Friend player = new Friend(players[__instance.GetPlayerId()-1].steamId);
            textComp.text = "insert name here";

            textObjs.Add(textObj);
            textComps.Add(textComp);
            locations.Add(location);
            playerGameObjs.Add(playerGameObj);
        }
        public void Update()
        {
            if (playerGameObjs.Count == 0) return;
            for(int i = 0; i < playerGameObjs.Count; i++)
            {
                GameObject playerGameObj = playerGameObjs[i];
                locations[i].anchoredPosition = GetPosOnScreen((Vector2)playerGameObj.transform.position, canvas);
            }
        }
        public static Vector2 GetPosOnScreen(Vector2 position,Canvas canvas)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
                float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;


                Vector2 worldPosition = position;

                Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                screenPosition = new Vector2(screenPosition.x-canvasWidth/4, screenPosition.y - canvasHeight/4);


                return screenPosition;
            }
            else
            {
                Debug.LogError("Main camera is not found.");
            }
            return Vector2.zero;
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
