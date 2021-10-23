using UnityEngine;
using BalsaCore;
using IO;
using UI;
using UI.MMX.Data;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;


internal static class ModuleInitializer
{
    internal static void Run()
    {
        Debug.LogError("Called From ModuleInit");
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
    }

    static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
    {
        Debug.LogError("Called From Assembly Resolve");
        foreach (ModCFG modCfg in ModLoader.Instance.ModList)
        {
            string folderPath = Path.GetDirectoryName(modCfg.CfgFilePath);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(modCfg.CfgFilePath) && File.Exists(assemblyPath))
            {
                // This is essentially the same way Balsa is loading asms in a non-locking fashion
                byte[] rawAssembly = File.ReadAllBytes(assemblyPath);
                string path = assemblyPath.Replace(".dll", ".pdb");
                if (File.Exists(path))
                {
                    byte[] rawSymbolStore = File.ReadAllBytes(path);
                    return Assembly.Load(rawAssembly, rawSymbolStore);
                }
                else
                {
                    return Assembly.Load(rawAssembly);
                }
            }
        }
        return null;
    }
}



namespace CloverTech
{
    public class ModReloader : MonoBehaviour
    {
        readonly FloatingInput.KeyCombo reloadKeys = new FloatingInput.KeyCombo( new KeyCode[2]{KeyCode.LeftControl, KeyCode.F5});
        public void Start()
        {
            DontDestroyOnLoad(this);
            LogE("Test");
            Debug.LogError("Init");
            HarmonyContainer.DoPatches();
            //ModLoader.Instance.ForceReloadAllModData();

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

                //LogI($"Destroying AssemblyLoader in {go.name}");
                //foreach (PluginAssembly loadedAssembly in AssemblyLoader.Instance.LoadedAssemblies)
                //{
                //    loadedAssembly.Finalize(AddonInvokeTime.OnLoaded);
                //}
                //AssemblyLoader.AddonTypes = new List<System.Type>();
                //Destroy(go.GetComponent<AssemblyLoader>());
                //AssemblyLoader.Instance = null;
                //LogI("Reiniting AssemblyLoader");
                //go.AddComponent<AssemblyLoader>();

                LogI("Unloading all ABs");
                foreach (PartInfo part in PartLoader.Instance.LoadedPartsList)
                {
                    LogI($"Unloading {Application.dataPath + "/../" + part.fobPath} from {part.Mod.Title}");
                    ABLoader.UnloadAB(Application.dataPath + "/../" + part.fobPath, true);
                }
                GameObject.Find("LoadedParts")?.DestroyGameObject();
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

        [BalsaAddonInit]
        public static void BalsaInit()
        {
            go = GameObject.Find("CloverTech::ModReloader");
            if (go == null)
            {
                Debug.Log("[ModReloader] Creating GameObject");
                go = new GameObject("CloverTech::ModReloader");
                go.AddComponent<ModReloader>();
            } 
        }

        //Game exit
        [BalsaAddonFinalize]
        public static void BalsaFinalize()
        {
            //go.DestroyGameObject();
        }

    }
}
