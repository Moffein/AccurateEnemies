using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class VoidJailer
    {
        public static bool enabled = true;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            On.EntityStates.VoidJailer.Weapon.Fire.ModifyProjectileAimRay += (orig, self, aimRay) =>
            {
                if (self.characterBody && !self.characterBody.isPlayerControlled)
                {
                    HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                    Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, self.projectilePrefab, targetHurtbox);
                    return orig(self, newAimRay);
                }
                return orig(self, aimRay);
            };
        }
    }
}
