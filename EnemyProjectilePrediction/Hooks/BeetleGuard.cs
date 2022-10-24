using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class BeetleGuard
    {
        public static bool enabled = true;
        public static bool loopOnly = false;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.BeetleGuardMonster.FireSunder.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.BeetleGuardMonster.FireSunder, Ray>>((aimRay, self) =>
                    {
                        if ((!loopOnly || (Run.instance && Run.instance.stageClearCount >= 5)) && self.characterBody && !self.characterBody.isPlayerControlled)
                        {
                            aimRay.origin = self.handRTransform.position;//Called in Vanilla method, but  call here beforehand before calculating the new aimray.
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPCC(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.BeetleGuardMonster.FireSunder.projectilePrefab, targetHurtbox);
                            //Feed it the projectile prefab in case a mod is changing the speed.
                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.BeetleGuardMonster.FireSunder.FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
