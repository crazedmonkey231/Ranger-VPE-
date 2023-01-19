using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using KCSG;

namespace Ranger.RangerPatches
{

    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class Projectile_Patch
    {
        public static void Postfix(Thing hitThing, bool blockedByShield, Projectile __instance, ref Thing ___launcher, ref ThingDef ___equipmentDef)
        {
            if (hitThing == null || ___launcher == null || ___equipmentDef == null) 
                return;
            if (___launcher is Pawn launcher && __instance.intendedTarget.Thing is Pawn target && launcher.equipment.Primary.def.IsRangedWeapon)
            {
                HediffSet pawnHediffs = launcher.health.hediffSet;
                Map map = hitThing.Map;
                IntVec3 pos = hitThing.Position;

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Serrated_Arrows))
                {
                    DamageInfo dinfo1 = new DamageInfo(DamageDefOf.Cut, __instance.DamageAmount, 200, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true);
                    dinfo1.SetIgnoreArmor(true);
                    hitThing.TakeDamage(dinfo1);
                }

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Thunder_Arrows))
                    hitThing.TakeDamage(new DamageInfo(DamageDefOf.EMP, __instance.DamageAmount, __instance.ArmorPenetration, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true));
                
                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Frost_Arrows))
                    hitThing.TakeDamage(new DamageInfo(DamageDefOf.Frostbite, __instance.DamageAmount, __instance.ArmorPenetration, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true));

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Fire_Arrows))
                    hitThing.TakeDamage(new DamageInfo(DamageDefOf.Burn, __instance.DamageAmount, __instance.ArmorPenetration, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true));

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Chemical_Arrows))
                    HealthUtility.AdjustSeverity(target, HediffDefOf.ToxicBuildup, __instance.DamageAmount / 100.0f);

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Achilles_Strike))
                    hitThing.TakeDamage(new DamageInfo(DamageDefOf.Stun, __instance.DamageAmount, __instance.ArmorPenetration, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true));
                
                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_True_Strike))
                {
                    MoteMaker.ThrowText(pos.ToVector3(), map, "Critical Hit!");
                    DamageInfo dinfo1 = new DamageInfo(DamageDefOf.Arrow, __instance.DamageAmount * 10, 200, __instance.ExactRotation.eulerAngles.y, ___launcher, weapon: ___equipmentDef, intendedTarget: target, instigatorGuilty: true);
                    dinfo1.SetIgnoreArmor(true);
                    dinfo1.SetHitPart(target.health.hediffSet.GetBrain());
                    hitThing.TakeDamage(dinfo1);
                }

                if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Rod_From_God))
                    SkyfallerMaker.SpawnSkyfaller(RangerDefOf.Rod_From_God_Incoming, pos, map);
            }
        }
    }
}
