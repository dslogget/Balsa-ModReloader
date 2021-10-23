using BalsaCore;
using HarmonyLib;
using IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CloverTech
{
    class HarmonyContainer
    {
        public static Harmony harmony = new Harmony("Balsa.CloverTech.ModReloader");

        public static void DoPatches()
        {
            Harmony.DEBUG = true;
            harmony.PatchAll();
        }
    }

    //[HarmonyPatch(typeof(AssemblyLoader))]
    //[HarmonyPatch("LoadAssemblies")]
    //class AssemblyLoaderPatch
    //{
    //    public static void Postfix(AssemblyLoader __instance)
    //    {
    //        // Dedupe AddonTypes by loaded assemblies
    //        int removed = AssemblyLoader.AddonTypes.RemoveAll((Type t) =>
    //       {
    //           return __instance.LoadedAssemblies.Any((PluginAssembly plugAsm) => {
    //               List<Type> foundTypes = plugAsm.asm.GetTypes().ToList().FindAll((Type asmType) => { return asmType.FullName == t.FullName; });
    //               if (foundTypes.Count > 0)
    //               {
    //                   Debug.LogError($"Found: {foundTypes.Count} {t.FullName}s");
    //                   Debug.LogError($"Found: {t.Assembly} {plugAsm.asm}s");
    //               }


    //               bool result = (foundTypes.Count > 0 && t.Assembly != plugAsm.asm);
    //               if (result)
    //               {
    //                   Debug.LogError($"Deduped: {t.FullName} from {plugAsm.asm.GetName().FullName}"); 
    //               }
    //               return result;
    //           });
    //       });
    //    }
    //}


}