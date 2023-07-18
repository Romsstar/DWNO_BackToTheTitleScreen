using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace BackToTheTitleScreen;

[BepInPlugin(GUID, PluginName, PluginVersion)]
[BepInProcess("Digimon World Next Order.exe")]
public class Plugin : BasePlugin
{
    internal const string GUID = "Romsstar.DWNO.BackToTheTitleScreen";
    internal const string PluginName = "BackToTheTitleScreen";
    internal const string PluginVersion = "1.0.0";

    public override void Load()
    {
        Awake();
    }

    public void Awake()
    {
        Harmony harmony = new Harmony("Patches");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(uOptionPanel), "_StartQuitWindow_b__13_0")]
    public static class TitleScreenPatch
    {
        public static bool Prefix(uOptionPanel __instance, bool b)
        {
            if (b)
            {
                __instance.m_State = uOptionPanel.State.CLOSE;
                UnityEngine.Object.DestroyImmediate(uDigivicePanel.Ref.m_TopPanel.gameObject);
                SceneManager.Ref.CurrentSceneDestroy();
                SceneManager.Ref.Push(SceneNo.Title);
            }
            else
            {
                return true;
            }
            return false;
        }


        [HarmonyPatch(typeof(uOptionPanel), "SetMainSettingState")]
        private static bool Prefix(uOptionPanel __instance, uOptionPanel.MainSettingState state)
        {
            if (state == uOptionPanel.MainSettingState.APPLICATION_QUIT)
            {
                string message = "Return to the Title Screen?"; // Replace with your desired message key

                Action<bool> callback = __instance._StartQuitWindow_b__13_0;
                AppMainScript.Ref.CommonYesNoWindowUI.Open(message, callback);
                return false;
            }
            return true;

        }
    }

}






