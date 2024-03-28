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
using MonoMod.Cil;


namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "Name Tags", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.nametags";
        public static PlayerHandler instance;

        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            Harmony harmony = new Harmony(pluginGuid);
            /*harmony.PatchAll(typeof(getPlayerHandler));*/

        }
        private void Update()
        {
/*            instance = PlayerHandler.Get();
            Debug.Log(instance.NumberOfPlayers());*/

        }


        //GUI Code
        string boolToOnOffTag(bool boolean)
        {
            return boolean == true ? "<color=green>On</color>" : "<color=red>Off</color>";
        }


        private UnityEngine.Color guiColor = UnityEngine.Color.green;
        private void OnGUI()
        {
            GUI.Box(new Rect(0, 0, 170f, 30f), "Minidogg");

        }

         

    }

    /*  [HarmonyPatch(typeof(PlayerHandler))]
      public class getPlayerHandler
      {
          [HarmonyPatch("Update")]
          [HarmonyPrefix]
          private static void UpdateMethod()
          {
              Debug.Log("Potatoo");
          }

      }*/
}
