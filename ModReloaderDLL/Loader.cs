using UnityEngine;
using BalsaCore;
using IO;
using UI;
using UI.MMX.Data;


namespace CloverTech
{
    public class ModReloader : MonoBehaviour
    {
        readonly FloatingInput.KeyCombo reloadKeys = new FloatingInput.KeyCombo( new KeyCode[2]{KeyCode.LeftControl, KeyCode.F5});
        public void Start()
        {
            DontDestroyOnLoad(this);
            LogE("Test");
            ModLoader.Instance.ForceReloadAllModData();
        }

        public void Update()
        {
            if (GameLogic.CurrentScene == GameScenes.MENU && reloadKeys.GetKeyDown()) 
            {
                if (ModLoader.Instance != null)
                {
                    ModLoader.Instance.ForceReloadAllModData();
                    ReloadPartCfgs();
                }
                else
                {
                    LogE("No Modloader present");
                }
            }
        }

        public void ReloadPartCfgs()
        {
            LogI("Reloading mods");
            if (PartLoader.Instance != null)
            {
                GameObject go = ModLoader.Instance.gameObject;
                LogI("Unloading all ABs");
                foreach (PartInfo part in PartLoader.Instance.LoadedPartsList)
                {
                    LogI($"Unloading {part.name} from {part.Mod.Title}");
                    ABLoader.UnloadAB(part.assetPath, true);
                }
                LogI($"Destroying PartLoader in {go.name}");
                Destroy(go.GetComponent<PartLoader>());
                PartLoader.Instance = null;
                LogI("Reiniting PartLoader");
                go.AddComponent<PartLoader>();
            }
            else
            {
                LogW("Attempting to recover from unloaded PartLoader");
                GameObject go = GameObject.Find("loaders");
                if (go == null)
                {
                    LogE("Unable to find loaders gameobject");
                }
                else
                {
                    LogW("Found loaders, reinitialising PartLoader");
                    go.AddComponent<PartLoader>();
                }
            }
        }

        public void LogI(string message)
        {
            Debug.Log($"[ModReloader] {message}");
        }
        public void LogW(string message)
        {
            Debug.LogWarning($"[ModReloader] {message}");
        }
        public void LogE(string message)
        {
            Debug.LogError($"[ModReloader] {message}");
        }
    }
    

    [BalsaAddon]
    public class Loader
    {
        private static GameObject go = null;
        private static bool initialised = false;

        [BalsaAddonInit]
        public static void BalsaInit()
        {
            if (!initialised)
            {
                Debug.Log("[ModReloader] Creating GameObject");
                go = new GameObject("CloverTech::ModReloader");
                go.AddComponent<ModReloader>();
                initialised = true;
            }
        }

        //Game exit
        [BalsaAddonFinalize]
        public static void BalsaFinalize()
        {
            go.DestroyGameObject();
        }

    }
}
