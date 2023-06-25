﻿using GorillaLocomotion;
using HarmonyLib;
using System.Threading.Tasks;
using UnityEngine;

namespace SlipperyWater.Scripts
{
    [HarmonyPatch]
    public class WatchPatches
    {
        private const float DefaultSlide = 0.0035f;
        private const float EnhancedSlide = 0.02f;
        [HarmonyPatch(typeof(GorillaTagger), "Start"), HarmonyPostfix]
        public static async void TaggerStartPatch()
        {
            await Task.Yield();
            WaterMain.Initalize();
        }

        [HarmonyPatch(typeof(Player), "GetSlidePercentage"), HarmonyPrefix]
        public static bool PlayerSlidePercentagePrefix(Player __instance, ref float __result, RaycastHit raycastHit)
        {
            if (WaterMain.waterSurfaces.Count > 0 && raycastHit.collider != null && WaterMain.waterSurfaces.Contains(raycastHit.collider))
            {
                __result = 1f;
                __instance.slideControl = EnhancedSlide;
                return false;
            }
            __instance.slideControl = DefaultSlide;
            return true;
        }
    }
}
