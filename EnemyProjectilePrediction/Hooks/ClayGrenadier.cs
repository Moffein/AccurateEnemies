using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class ClayGrenadier
    {
        public static bool enabled = true;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            On.EntityStates.ClayGrenadier.ThrowBarrel.ModifyProjectileAimRay += (orig, self, aimRay) =>
            {
                HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, self.projectilePrefab, targetHurtbox);
                return orig(self, newAimRay);
            };
        }
    }
}
