using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace BackToTheTitleScreen;

[BepInPlugin(GUID, PluginName, PluginVersion)]
[BepInProcess("Digimon World Next Order.exe")]
public class Plugin : BasePlugin
{
    internal const string GUID = "Romsstar.DWNO.BackToTheTitleScreen";
    internal const string PluginName = "BackToTitle";
    internal const string PluginVersion = "1.1";

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
        [HarmonyPatch(typeof(AppMainScript._HeadsUpDisp_d__100), "MoveNext")]
        [HarmonyPrefix]
        public static bool Prefix(AppMainScript._HeadsUpDisp_d__100 __instance)
        {
            __instance._waitStartTime_5__2 = 0f;
            return true;
        }

        [HarmonyPatch(typeof(AppMainScript.__CheckDlc_d__96), "MoveNext")]
        [HarmonyPrefix]
        public static bool Prefix(AppMainScript.__CheckDlc_d__96 __instance)
        {
            __instance._startTime_5__2 = 0f;
            return true;
        }

        [HarmonyPrefix]
        public static bool Prefix(uOptionPanel __instance, bool b)
        {
            if (b)
            {
                // Set the panel state to CLOSE
                __instance.m_State = uOptionPanel.State.CLOSE;

                SceneManager.Ref.Push(SceneNo.Title);

                if (uDigivicePanel.Ref?.m_TopPanel != null)
                {
                    UnityEngine.Object.Destroy(uDigivicePanel.Ref.m_TopPanel.gameObject);
                }

                if (StorageData.m_uSavePanel != null)
                {
                    UnityEngine.Object.Destroy(StorageData.m_uSavePanel);
                }

                if (SceneManager.Ref != null)
                {
                    SceneManager.Ref.CurrentSceneDestroy();
                }

                StorageData.ClearSaveDataForRetryNewGame();
                StorageData.InitializePartnerDataForRetryNewGame(0);
                StorageData.InitializePartnerDataForRetryNewGame((AppInfo.PARTNER_NO)1);
            }
            else
            {
                return true; // Continue with the original method execution
            }

            return false; // Skip the original method execution as we've handled everything
        }

        [HarmonyPatch(typeof(MainTitle))]
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        static bool Prefix(MainTitle __instance)
        {
            bool gameDataExistsInAnySlot = CheckGameDataExistence();

            if (gameDataExistsInAnySlot)
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
            }

            return true;
        }

        private static bool CheckGameDataExistence()
        {
            if (StorageData.IsExistGameData(0) || StorageData.IsExistGameData(1) || StorageData.IsExistGameData(2))
            {
                // Game data exists in at least one slot
                return true;
            }

            // Game data does not exist in any of the slots
            return false;
        }

        [HarmonyPatch(typeof(uOptionPanel))]
        [HarmonyPrefix]
        [HarmonyPatch("SetMainSettingState")]
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
        [HarmonyPostfix]
        public static void Postfix(uOptionTopPanelCommand __instance)
        {
            GameObject applicationQuitObj = __instance.m_items[3]?.gameObject;
            Text textComponent = applicationQuitObj.GetComponentInChildren<Text>();
            if (textComponent.text == "Quit Game")
            {
                textComponent.text = "Return to the Title Screen";
            }
        }
    }
}    




















































































































































































































































































































