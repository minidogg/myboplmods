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
using Steamworks;
using Steamworks.Data;


namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "Funny Menu", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.funnymenu";
        public static PlayerHandler instance;
        static public bool fmEnabled = false;

        static public bool antiKick = false;
        static public bool tryStart = false;
        static public bool allowInvis = false;



        private void Awake()
        {

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            Harmony harmony = new Harmony(pluginGuid);
            /*harmony.PatchAll(typeof(getPlayerHandler));*/


            MethodInfo original = AccessTools.Method(typeof(SteamManager), "OnChatMessageCallback");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "OnChatMessageCallback_p");
            harmony.Patch(original, new HarmonyMethod(patch));

            MethodInfo original2 = AccessTools.Method(typeof(Invisibility), "CastInvisibility");
            MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "CastInvisibility_p");
            harmony.Patch(original2, new HarmonyMethod(patch2));

            /*           MethodInfo original2 = AccessTools.Method(typeof(PlayerAverageCamera), "UpdateCamera");
                       MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "UpdateCamera_P");
                       harmony.Patch(original2, new HarmonyMethod(patch2));*/
        }
        public static bool CastInvisibility_p(ref InstantAbility ___ia)
        {
            return allowInvis;
        }
        /*private void UpdateCamera_P(ref PlayerAverageCamera ___instance, ref Camera __camera)
        {
            List<Player> list = PlayerHandler.Get().PlayerList();
            Vector2 vector = default(Vector2);
            float d = (float)list.Count;
            float num = 0f;
            float num2 = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsAlive)
                {
                    Vector2 vector2 = (Vector2)list[i].Position;
                    vector += vector2;
                    num = Mathf.Max(num, vector2.x);
                    num2 = Mathf.Min(num2, vector2.x);
                }
            }
            Vec2[] playerSpawns_readonly = GameSessionHandler.playerSpawns_readonly;
            vector /= d;
            if (___instance.useMinMaxForCameraX)
            {
                vector = new Vector2((num2 + num) * 0.5f, vector.y);
            }
            float num3 = Mathf.Min(___instance.maxDeltaTime, Time.unscaledDeltaTime);
            Vector2 vector3 = (Vector2)((1f - ___instance.weight * num3) * base.transform.localPosition) + (Vector2)(___instance.weight * num3 * vector);
            float num4 = __camera.orthographicSize * __camera.aspect + ___instance.outsideLevelX;
            vector3.x = Mathf.Max((float)SceneBounds.Camera_XMin + num4, vector3.x);
            vector3.x = Mathf.Min((float)SceneBounds.Camera_XMax - num4, vector3.x);
            vector3.x = ___instance.RoundToNearestPixel(vector3.x);
            vector3.y = Mathf.Max((float)SceneBounds.WaterHeight + ___instance.MinHeightAboveFloor, vector3.y);
            vector3.y = Mathf.Min((float)SceneBounds.Camera_YMax, vector3.y);
            vector3.y = ___instance.RoundToNearestPixel(vector3.y);
            if (!___instance.UpdateX)
            {
                vector3.x = base.transform.position.x;
            }
            if (!___instance.UpdateY)
            {
                vector3.y = base.transform.position.y;
            }
            Vector3 position = new Vector3(vector3.x, vector3.y, base.transform.position.z);
            base.transform.position = position;
            float num5 = 0f;
            float num6 = 0f;
            for (int j = 0; j < list.Count; j++)
            {
                Vector2 vector4 = (Vector2)list[j].Position;
                float b = Mathf.Abs(vector4.x - vector.x);
                num5 = Mathf.Max(num5, b);
                b = Mathf.Abs(vector4.y - vector.y);
                num6 = Mathf.Max(num6, b);
            }
            float num7 = (float)(Screen.width / Screen.height);
            num5 *= num7;
            float num8 = Mathf.Max(num5, num6);
            num8 += ___instance.extraZoomRoom;
            if (num8 > ___instance.MAX_ZOOM)
            {
                num8 = ___instance.MAX_ZOOM;
            }
            if (num8 < ___instance.MIN_ZOOM)
            {
                num8 = ___instance.MIN_ZOOM;
            }
            float num9 = Mathf.Clamp((1f - ___instance.zoomWeight * num3) * __camera.orthographicSize + ___instance.zoomWeight * num3 * num8, ___instance.MIN_ZOOM, ___instance.MAX_ZOOM);
            if (__camera.orthographicSize != num9)
            {
                __camera.orthographicSize = num9;
            }
        }*/


        private void Update()
        {
            if (tryStart == true) CharacterSelectHandler_online.TryStartGame();
        }

        public bool returnTrue()
        {
            return true;
        }

        public static bool OnChatMessageCallback_p(Lobby lobby, Friend sender, string msg, ref SteamManager ___instance)
        {
            MonoBehaviour.print("chat message callback: " + msg);

            if (___instance.currentLobby.IsOwnedBy(sender.Id) && SteamClient.Name == msg)
            {
                if (Plugin.antiKick == false) GameSessionHandler.LeaveGame();
            }
            return false;
        }


        //GUI Code
        private Rect windowRect = new Rect(0, 50, 500, 500);
        public string GUIName = "Funny Menu";
        private UnityEngine.Color guiColor = UnityEngine.Color.cyan;

        string boolToOnOffTag(bool boolean)
        {
            return boolean == true ? "<color=green>On</color>" : "<color=red>Off</color>";
        }

        private void OnGUI()
        {
            GUI.backgroundColor = guiColor;
            GUI.color = guiColor;

            if (GUI.Button(new Rect(0, 30, 100, 20), "FM Open: " + boolToOnOffTag(fmEnabled)))
            {
                fmEnabled = !fmEnabled;
            }

            if (fmEnabled == true) windowRect = GUI.Window(10000, windowRect, WindowGUI, GUIName);

            void WindowGUI(int windowID)
            {
                if (GUI.Button(new Rect(60, 80, 170f, 30f), "Try Start Game Loop: " + boolToOnOffTag(tryStart)))
                {
                    tryStart = !tryStart;
                }
                if (GUI.Button(new Rect(60, 110, 170f, 30f), "Anti-Kick: " + boolToOnOffTag(antiKick)))
                {
                    antiKick = !antiKick;
                }
                if (GUI.Button(new Rect(60, 140, 170f, 30f), "Allow Invis: " + boolToOnOffTag(allowInvis)))
                {
                    allowInvis = !allowInvis;
                }


            }
        }



    }

}
