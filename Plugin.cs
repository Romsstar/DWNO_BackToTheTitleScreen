using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [HarmonyPatch(typeof(AppMainScript._HeadsUpDisp_d__100), "MoveNext")]
    public static class Splash
    {
        public static bool Prefix(AppMainScript._HeadsUpDisp_d__100 __instance)
        {
            __instance._waitStartTime_5__2 = 0f;
           return true;
        }

        [HarmonyPatch(typeof(AppMainScript.__CheckDlc_d__96), "MoveNext")]
        public static bool Prefix(AppMainScript.__CheckDlc_d__96 __instance)
        {
            __instance._startTime_5__2 = 0f;
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

        // Harmony patch class
        [HarmonyPatch(typeof(MainTitle))]
        class MainTitlePatch_Update_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            static bool Prefix(MainTitle __instance)
            {
                GameObject logo = GameObject.Find("Logo");
                UnityEngine.Object.DestroyImmediate(logo);

                if (__instance.m_movie != null && __instance.m_movie.IsPlaying())
                {
      
                    __instance.m_movie.Stop();
                    UnityEngine.Object.Destroy(__instance.m_movie.transform.parent.gameObject, 0);
                    __instance.m_movie = null;
                    Resources.UnloadUnusedAssets();
                    GC.Collect();
                    return false; 
                }

                // Otherwise, allow the original Update method to execute as usual
                return true;
            }
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