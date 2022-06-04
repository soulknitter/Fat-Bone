using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace DankPyon
{
    [StaticConstructorOnStartup]
    public static class HarmonyInstance
    {
        public static Harmony harmony;
        static HarmonyInstance()
        {
            harmony = new Harmony("lalapyhon.rimworld.medievaloverhaul");
            harmony.Patch(AccessTools.Method(typeof(Thing), "ButcherProducts", null, null), null,
                new HarmonyMethod(typeof(HarmonyInstance), "Thing_MakeButcherProducts_FatAndBone_PostFix", null), null);
            harmony.PatchAll();
        }

        private static ThingDef fat = ThingDef.Named("DankPyon_Fat");
        private static ThingDef bone = ThingDef.Named("DankPyon_Bone");
        private static void Thing_MakeButcherProducts_FatAndBone_PostFix(Thing __instance, ref IEnumerable<Thing> __result, Pawn butcher, float efficiency)
        {
            if (__instance is Pawn pawn && pawn.RaceProps.IsFlesh && pawn.RaceProps.meatDef != null)
            {
                bool boneFlag = true;
                bool fatFlag = true;

                var butcherProperties = ButcherProperties.Get(__instance.def);
                if (butcherProperties != null)
                {
                    boneFlag = butcherProperties.hasBone;
                    fatFlag = butcherProperties.hasFat;
                }

                if (boneFlag || fatFlag)
                {
                    int amount = Math.Max(1, (int)(GenMath.RoundRandom(__instance.GetStatValue(StatDefOf.MeatAmount, true) * efficiency) * 0.2f));
                    if (boneFlag)
                    {
                        Thing Bone = ThingMaker.MakeThing(bone, null);
                        Bone.stackCount = amount;
                        __result = __result.AddItem(Bone);
                    }
                    if (fatFlag)
                    {
                        Thing Fat = ThingMaker.MakeThing(fat, null);
                        Fat.stackCount = amount;
                        __result = __result.AddItem(Fat);
                    }
                }
            }
        }
    }
    class ButcherProperties : DefModExtension
    {
        public bool hasBone = true;

        public bool hasFat = true;

        public static ButcherProperties Get(Def def)
        {
            return def.GetModExtension<ButcherProperties>();
        }
    }
}