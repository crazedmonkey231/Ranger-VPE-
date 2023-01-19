using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Ranger
{
    public class RangerMod : Mod
    {
        public static RangerSettings settings;
        public static Harmony harmony;

        public RangerMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RangerSettings>();
            harmony = new Harmony(Content.Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }
}
