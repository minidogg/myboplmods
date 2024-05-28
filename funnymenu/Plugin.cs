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
using Steamworks.Data;
using Steamworks;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Net.Sockets;

namespace BoplBattleTemplate
{
    [BepInPlugin(pluginGuid, "LessLimit", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "com.unluckycrafter.lesslimit";
        public static string cheese = "Cheese?";
        public static Harmony harmony = new Harmony(pluginGuid);

        public static Networker networker;

        public static Module infiniteSpikes = new Module("Infinite Spikes", false);
        public static Module noCooldowns = new Module("NoCooldowns", false);
        public static Module infiniteBlackHole = new Module("InfiniteBlackHole", false);
        /*        public static Module testModule = new Module("Test", "Test");*/

        private void Awake()
        {
            //Loaded plugin
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded, but hasn't finished its Awake method yet. Finishing...");//feel free to remove this

            //register networker
            networker = new Networker(harmony);

            //patches
            MethodInfo original = AccessTools.Method(typeof(Spike), "CastSpike");
            MethodInfo patch = AccessTools.Method(typeof(Plugin), "CastSpike_p");
            harmony.Patch(original, new HarmonyMethod(patch));

            MethodInfo original2 = AccessTools.Method(typeof(Ability), "GetCooldown");
            MethodInfo patch2 = AccessTools.Method(typeof(Plugin), "NoCooldown");
            harmony.Patch(original2, null, new HarmonyMethod(patch2));
            MethodInfo original3 = AccessTools.Method(typeof(InstantAbility), "GetCooldown");
            harmony.Patch(original3, null, new HarmonyMethod(patch2));

            MethodInfo original4 = AccessTools.Method(typeof(BlackHoleClap), "FireCommon");
            MethodInfo patch4 = AccessTools.Method(typeof(Plugin), "FireCommon_p");
            harmony.Patch(original4, new HarmonyMethod(patch4));

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} finished its awake method!");
        }

        static void NoCooldown(ref Fix __result)
        {
            if((bool)noCooldowns==true)__result = Fix.Zero;
        }
        public static bool CastSpike_p(ref SpikeAttack ___currentSpike)
        {
            if((bool)infiniteSpikes==true)___currentSpike = null;
            return true;
        }
        public static void FireCommon_p(ref Ability ___ability)
        {
            if((bool)infiniteBlackHole==true)___ability.isCastable = true;
        }

        public static Rect windowRect = new Rect(0, 150,Screen.width*0.5f,Screen.height-160);

        public string GUIName = "Super Panel";
        private UnityEngine.Color guiColor = UnityEngine.Color.cyan;
        public bool menuOpen = false;



        private void OnGUI()
        {

            if (GUI.Button(new Rect(0, 100, 200, 20), "Super Panel Open: " + UiUtils.boolToOnOffTag(menuOpen)))
            {
                menuOpen = !menuOpen;
            }
            if (menuOpen==true)GUI.Window(10000, windowRect, WindowGUI, GUIName);

            void WindowGUI(int windowID)
            {
                GUI.DragWindow(new Rect(0f, 0f, windowRect.width, 20f));
                ModuleManager.RenderModules(windowID,windowRect);
            }
        }
    }
    public class UiUtils
    {
        public static string boolToOnOffTag(bool boolean)
        {
            return boolean == true ? "<color=green>On</color>" : "<color=red>Off</color>";
        }
    }
    public enum ModuleType
    {
        Number,
        Boolean,
        String,
        Slider
    }
    public class ModuleManager
    {
        public static int moduleIDs = 0;
        public static List<Module> modules = new List<Module>();
        
        public static void AddModule(Module module)
        {
            Console.WriteLine("Registered module to Super Panel: "+module.label);
            modules.Add(module);
        }


        public static void RenderModules(int windowID,Rect windowRect)
        {
            bool isLobbyOwner = SteamManager.instance.currentLobby.IsOwnedBy(SteamClient.SteamId);
            if (modules.Count == 0) return;
            int x = 15;
            int y = 0;
            foreach (Module module in modules)
            {
                y += 25;
                if (y >= windowRect.height)
                {
                    y = 25;
                    x += 200;
                }

                GUI.Label(new Rect(x, y, 115, 20), module.label + ": ");
                Rect valueRect = new Rect(x + 115, y, 85, 20);
                switch (module.moduleType)
                {
                    //boolean type
                    case (ModuleType.Boolean):
                        if (GUI.Button(valueRect, UiUtils.boolToOnOffTag(module.valueBoolean)))
                        {
                            if(isLobbyOwner==true)module.valueBoolean = !module.valueBoolean;
                        }
                        break;
                    case (ModuleType.String):
                        string valueA = GUI.TextField(valueRect, module.valueString);
                        if (isLobbyOwner == true) module.valueString = valueA;
                        break;
                    case (ModuleType.Number):
                        string valueB = GUI.TextField(valueRect, module.valueNumber.ToString());
                        if (isLobbyOwner == true) module.valueNumber = float.Parse(valueB);
                        break;
                }

            }
            if (isLobbyOwner == false) return;
            Networker.instance.SendConfig(modules);
        }
    }
    public class Module
    {
        public ModuleType moduleType;
        public string label = "Unnamed Module";
        public int moduleId = ModuleManager.moduleIDs++;

        public string valueString;
        public float valueNumber;
        public bool valueBoolean;
        public float valueSliderMin;
        public float valueSliderMax;
        public Module(string label,string value, bool addToManager=true)
        {
            this.moduleType = ModuleType.String;
            this.valueString = value;
            this.label = label;
            if (addToManager == true) ModuleManager.AddModule(this);
        }
        public Module(string label, bool value, bool addToManager = true)
        {
            this.moduleType = ModuleType.Boolean;
            this.valueBoolean = value;
            this.label = label;
            if (addToManager == true) ModuleManager.AddModule(this);
        }
        public Module(string label, float value, bool addToManager = true)
        {
            this.moduleType = ModuleType.Number;
            this.valueNumber = value;
            this.label = label;
            if (addToManager == true) ModuleManager.AddModule(this);
        }



        /*Casting*/
        public static explicit operator bool(Module m)
        {
            if(m.moduleType==ModuleType.Boolean)return m.valueBoolean;
            throw new Exception("Tried to explicitly cast to Boolean value from a " + m.moduleType.ToString() + " type module.");
        }
        public static explicit operator float(Module m)
        {
            if (m.moduleType == ModuleType.Number||m.moduleType==ModuleType.Slider) return m.valueNumber;

            throw new Exception("Tried to explicitly cast to float/slider value from a " + m.moduleType.ToString() + " type module.");
        }
    }

    public class metadata
    {
        public static byte[] signature = [189, 128];
    }
    public class Networker
    {
        public static Networker instance;
        public Networker(Harmony harmony)
        {
            instance = this;
            MethodInfo original4 = AccessTools.Method(typeof(SteamSocket), "OnMessage");
            MethodInfo patch4 = AccessTools.Method(typeof(Networker), nameof(OnMessageInterceptor));
            harmony.Patch(original4, new HarmonyMethod(patch4));
        }

        private static bool OnMessageInterceptor(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            // Setup for receiving packets
            byte[] messageBuffer = new byte[size];
            Marshal.Copy(data, messageBuffer, 0, size);
            if (messageBuffer[0] != metadata.signature[0] || messageBuffer[1] != metadata.signature[1]) return true;
            if (!SteamManager.instance.currentLobby.IsOwnedBy(identity.SteamId)) {
                Console.WriteLine(identity.SteamId + " is trying to send super menu config packets without host.");
                return true;
            }

            //handling packets
            Networker.instance.UpdateConfig(messageBuffer);

            return false;
        }
        public void UpdateConfig(byte[] messageBuffer)
        {
            Module module = ModuleManager.modules[messageBuffer[2] + (messageBuffer[3] * 255)];
            byte[] valueData = new byte[messageBuffer.Length - 4];
            if (module.moduleType != ModuleType.Boolean)
            {
                for (int i = 4; i < messageBuffer.Length; i++)
                {
                    valueData[i - 4] = messageBuffer[i];
                }
            }
            switch (module.moduleType)
            {
                case (ModuleType.Boolean):
                    module.valueBoolean = messageBuffer[4] == 1;
                    break;
                case (ModuleType.String):
                    module.valueString = Encoding.ASCII.GetString(valueData);
                    break;
                case (ModuleType.Number):
                    module.valueNumber = float.Parse(Encoding.ASCII.GetString(valueData));
                    break;
            }
        }
        public void SendConfig(List<Module> modules)
        {
            byte[] i = [0,0];
            foreach(Module module in modules)
            {
                List<byte> packet = new List<byte>();
                packet.Add(metadata.signature[0]);
                packet.Add(metadata.signature[1]);
                packet.Add(i[0]);
                packet.Add(i[1]);

                switch (module.moduleType)
                {
                    case (ModuleType.Boolean):
                        packet.Add(module.valueBoolean == true ? (byte)1 : (byte)0);
                        break;
                    case (ModuleType.String):
                        packet.AddRange(Encoding.ASCII.GetBytes(module.valueString));
                        break;
                    case (ModuleType.Number):
                        packet.AddRange(Encoding.ASCII.GetBytes(module.valueNumber.ToString()));
                        break;
                }
                foreach (SteamConnection player in SteamManager.instance.connectedPlayers)
                {
                    player.Connection.SendMessage(packet.ToArray());
                }

                i[0]++;
                if (i[0] > 255)
                {
                    i[0] = 0;
                    i[1]++;
                }
            }
        }
    }
}
