using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Ranger.RangerPatches
{
    [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.AdjustedRange))]
    public static class AdjustedRange_Patch
    {
        public static void Postfix(Verb ownerVerb, Pawn attacker, ref float __result)
        {
            if (attacker.health.hediffSet.HasHediff(RangerDefOf.Hediff_EagleEye) && attacker.equipment.Primary.def.IsRangedWeapon)
                __result *= 2;
        }
    }
}
