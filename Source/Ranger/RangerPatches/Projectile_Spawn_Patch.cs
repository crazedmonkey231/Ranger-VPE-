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
using Ranger.RangerComps;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Ranger.RangerPatches
{
    internal class Projectile_Spawn_Patch
    {
        public static class GenPatch
        {
            public static Thing SpawnPatch(ThingDef def, IntVec3 loc, Map map, WipeMode wipeMode, Thing caster)
            {
                Projectile newThing = (Projectile) GenSpawn.Spawn(def, loc, map, wipeMode);
                Traverse newThingRoot = Traverse.Create(newThing);
                Traverse field = newThingRoot.Field("comps");
                List<ThingComp> value = (List<ThingComp>) field.GetValue();

                if (value == null)
                    value = new List<ThingComp>();

                List<FleckDef> flecks = new List<FleckDef>();
                List<Vector3> offsets = new List<Vector3>();
                if (caster is Pawn newCaster)
                {
                    HediffSet pawnHediffs = newCaster.health.hediffSet;

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Serrated_Arrows))
                    {
                        flecks.Add(RangerDefOf.Ranger_MicroSparks);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Thunder_Arrows))
                    {
                        flecks.Add(FleckDefOf.LineEMP);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Fire_Arrows))
                    {
                        flecks.Add(RangerDefOf.Ranger_Smoke_Orange);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Frost_Arrows))
                    {
                        flecks.Add(RangerDefOf.Ranger_Smoke_Blue);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Chemical_Arrows))
                    {
                        flecks.Add(RangerDefOf.Ranger_Smoke_Green);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Achilles_Strike))
                    {
                        flecks.Add(RangerDefOf.Ranger_Smoke_Yellow);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_True_Strike))
                    {
                        flecks.Add(RangerDefOf.Ranger_Smoke_Red);
                        offsets.Add(Vector3.zero);
                    }

                    if (pawnHediffs.HasHediff(RangerDefOf.Hediff_Rod_From_God))
                    {
                        flecks.Add(FleckDefOf.FlashHollow);
                        offsets.Add(Vector3.zero);
                    }

                }

                Comp_RangerTrails trails = new Comp_RangerTrails(flecks, offsets, -1)
                {
                    parent = newThing,
                };

                value.Add(trails);

                field.SetValue(value);
                return newThing;
            }
        }

        [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
        internal static class SpawnThingsFromHediffs_Harmony_Patch
        {
            static readonly MethodInfo spawn = typeof(GenSpawn).GetMethod("Spawn", new Type[] { typeof(ThingDef), typeof(IntVec3), typeof(Map), typeof(WipeMode) });
            static readonly MethodInfo patch = typeof(GenPatch).GetMethod("SpawnPatch");

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (CodeInstruction CI in instructions)
                {
                    if (CI.Calls(spawn))
                    {
                        //Pushes this.caster for arg in patch
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(Verb).GetField("caster"));
                        //Patch
                        yield return new CodeInstruction(OpCodes.Call, patch);
                    }
                    else
                    {
                        yield return CI;
                    }
                }
            }
        }
    }
}
