using BalsaCore;
using HarmonyLib;
using IO;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace CloverTech
{
    internal static class ModuleInitializer
    {
        internal static void Run()
        {
            Debug.LogError("Init");
            HarmonyContainer.DoPatches();
        }
    }

    class HarmonyContainer
    {
        public static Harmony harmony = new Harmony("Balsa.CloverTech.ModReloader");

        public static void DoPatches()
        {
            Harmony.DEBUG = true;
            harmony.PatchAll();
        }

    }


    //[HarmonyPatch(typeof(AssemblyLoader), "LoadAssemblies")]
    [HarmonyPatch(typeof(ModCFG))]
    [HarmonyPatch("Load")]
    public static class ModCfgPatch
    {
        static public bool Prefix(ConfigNode node)
        {
            Debug.LogError("Prefix");
            if (node.HasNode("HarmonyPlugin"))
            {
                Debug.LogError("Changing HarmonyPlugin to Plugin");
                ConfigNode[] nodes = node.GetNodes("HarmonyPlugin");
                //___pluginInfos = new PluginInfo[nodes.Length];
                for (int index = 0; index < nodes.Length; ++index)
                    //___pluginInfos[index] = new PluginInfo(nodes[index], __instance);
                    nodes[index].name = "Plugin";
            }
            return true;
        }

        static public void Postfix(ConfigNode node)
        {
            Debug.LogError("Postfix");
            if (node.HasNode("HarmonyPlugin"))
            {
                Debug.LogError("This never got updated so");
            }
        }

    }
}
