using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using static AppMainScript;
using static SwitchDlcItem;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

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



    [HarmonyPatch(typeof(_HeadsUpDisp_d__100), "MoveNext")]
    public static class Splash
    {
        public static bool Prefix(_HeadsUpDisp_d__100 __instance)
        {
            __instance._waitStartTime_5__2 = 0;
            __instance.__4__this.MessageManager.SetActive(false);
            return true;
        }

        [HarmonyPatch(typeof(__CheckDlc_d__96), "MoveNext")]
        public static bool Prefix(__CheckDlc_d__96 __instance)
        {
            __instance._startTime_5__2 = 0;
            __instance.__4__this.MessageManager.SetActive(false);
            return true;
        }

    }


    [HarmonyPatch(typeof(uOptionPanel), "_StartQuitWindow_b__13_0")]
    public static class TitleScreenPatch
    {
        public static bool Prefix(uOptionPanel __instance, bool b)
        {
            if (b)
            {
                __instance.m_State = uOptionPanel.State.CLOSE;
                __instance.m_messageWindow = null;
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
            __instance.m_IsTitle = true;

            if (state == uOptionPanel.MainSettingState.APPLICATION_QUIT)
            {
                string message = "Return to the Title Screen?";

                Action<bool> callback = __instance._StartQuitWindow_b__13_0;
                AppMainScript.Ref.CommonYesNoWindowUI.Open(message, callback);
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(uOptionTopPanelCommand), "enablePanel")]
        public static void Postfix(uOptionTopPanelCommand __instance)
        {
            GameObject applicationQuitObj = __instance.m_items[3]?.gameObject;
            Text textComponent = applicationQuitObj.GetComponentInChildren<Text>();
            textComponent.text = "Return to the Title Screen";
        }
    }
    }
    





















