using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore;
using Random = System.Random;

namespace Ranger.RangerComps
{
    public class Comp_RangerTrails : ThingComp
    {
        public int ticksSinceLastEmitted = 0;

        public List<FleckDef> flecks;
        public List<Vector3> offsets;
        public int emissionInterval = -1;
        public SoundDef soundOnEmission;
        public string saveKeysPrefix;

        public Comp_RangerTrails(List<FleckDef> flecks, List<Vector3> offsets, int emissionInterval = -1, SoundDef soundOnEmission = null, string saveKeysPrefix = null)
        {
            this.flecks = flecks;
            this.offsets = offsets;
            this.emissionInterval = emissionInterval;
            this.soundOnEmission = soundOnEmission;
            this.saveKeysPrefix = saveKeysPrefix;
        }

        public override void CompTick()
        {
            if (ticksSinceLastEmitted >= emissionInterval)
            {
                Emit();
                ticksSinceLastEmitted = 0;
            }
            else
                ++ticksSinceLastEmitted;
        }

        protected void Emit()
        {
            //FleckMaker.ThrowSmoke(parent.DrawPos + offset, parent.Map, 1);
            Random random = new Random();
            Vector3 randomOffset = new Vector3(random.Next(0, 1), random.Next(0, 1), random.Next(0, 1));
            for(int i = 0; i < flecks.Count; i++)
            {
                FleckMaker.Static(parent.DrawPos + offsets[i] + randomOffset, parent.Map, flecks[i]);
            }
            if (soundOnEmission.NullOrUndefined())
                return;
            soundOnEmission.PlayOneShot(SoundInfo.InMap((TargetInfo)(Thing)parent));
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksSinceLastEmitted, (saveKeysPrefix != null ? saveKeysPrefix + "_" : "") + "ticksSinceLastEmitted");
        }
    }
}
