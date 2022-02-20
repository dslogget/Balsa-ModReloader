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
#if DEBUG
            Harmony.DEBUG = true;
#endif
            harmony.PatchAll();
        }
    }
}