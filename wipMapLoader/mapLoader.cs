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
using System.IO;
using System.Xml.Linq;


namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "Map Loader", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.maploader";
        static GameObject capsule = new GameObject();
        static bool capsuleSet = false;


        private void Awake()
        {
            

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");//feel free to remove this
            Harmony harmony = new Harmony(pluginGuid);
            String[] maps = File.ReadAllLines(".\\maps.txt");
            Logger.LogInfo(loadMap(maps[0]));
            //ability spawn
/*            MethodInfo original = AccessTools.Method(typeof(Updater), "PostLevelLoad");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "loadMapToScene");
            harmony.Patch(original, new HarmonyMethod(patch));*/






            //MethodInfo original = AccessTools.Field(typeof());
        }
        private void Update()
        {
            Console.WriteLine("Pain and suffering.");
        }
        public static string DecodeBase64(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var valueBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(valueBytes);
        }

        public static bool loadMap(String map)
        {
            String mapDecoded = "";

            mapDecoded = DecodeBase64(map);
            Transform transform = new GameObject().transform;
            
            //Instantiate(typeof(MachoThrow2).GetField("bouldPrefab", BindingFlags.NonPublic | BindingFlags.Instance), transform);

            return true;
        }
        public static bool loadMapToScene()
        {
            if (capsuleSet == false) {
                Console.WriteLine("E");
                GameObject[] objectss = UnityEngine.SceneManagement.SceneManager.GetSceneByName("Level1").GetRootGameObjects();
                GameObject level = Array.Find(objectss, elem => elem.name == "Level");
                Console.WriteLine(level.name);
                var objects = level.GetComponentsInChildren<GameObject>(true);
                Console.WriteLine(objects.Length);


/*                for (int i = 0; i < objects.Length; i++)
                {
                    Console.WriteLine(objects[i].name);
*//*                    if (objects[i].name.StartsWith("smallCapsule (4)"))
                    {
                        capsule = GameObject.Instantiate(objects[i]);
                        DontDestroyOnLoad(capsule);
                        Console.WriteLine(capsule.transform.position.x + "EA");

                        capsuleSet = true;
                        break;
                    }*//*
                }*/
            }


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
