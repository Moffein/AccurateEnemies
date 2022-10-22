using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class RoboBallBoss
    {
        public static bool enabled = true;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.RoboBallBoss.Weapon.FireEyeBlast.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.RoboBallBoss.Weapon.FireEyeBlast, Ray>>((aimRay, self) =>
                    {
                        if (self.characterBody && !self.characterBody.isPlayerControlled)
                        {
                            Ray newAimRay;
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            float projectileSpeed = self.projectileSpeed;
                            if (projectileSpeed > 0f)
                            {
                                if (self.GetTeam() != TeamIndex.Player) projectileSpeed = AccurateEnemiesPlugin.GetProjectileSimpleModifiers(projectileSpeed);
                                newAimRay = Util.PredictAimray(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, projectileSpeed, targetHurtbox);
                            }
                            else
                            {
                                newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, self.projectilePrefab, targetHurtbox);
                            }
                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.RoboBallBoss.Weapon.FireEyeBlast.FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
