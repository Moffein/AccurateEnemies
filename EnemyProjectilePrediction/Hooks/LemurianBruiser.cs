using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class LemurianBruiser
    {
        public static bool enabled = true;
        public static bool loopOnly = false;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.LemurianBruiserMonster.FireMegaFireball.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.LemurianBruiserMonster.FireMegaFireball, Ray>>((aimRay, self) =>
                    {
                        if (Util.AllowPrediction(self.characterBody, loopOnly))
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay;

                            float projectileSpeed = EntityStates.LemurianBruiserMonster.FireMegaFireball.projectileSpeed;
                            if (projectileSpeed > 0f)
                            {
                                if (self.GetTeam() != TeamIndex.Player) projectileSpeed = AccurateEnemiesPlugin.GetProjectileSimpleModifiers(projectileSpeed);
                                newAimRay = Util.PredictAimray(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, projectileSpeed, targetHurtbox);
                            }
                            else
                            {
                                newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.LemurianBruiserMonster.FireMegaFireball.projectilePrefab, targetHurtbox);
                            }

                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.LemurianBruiserMonster.FireMegaFireball.FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
