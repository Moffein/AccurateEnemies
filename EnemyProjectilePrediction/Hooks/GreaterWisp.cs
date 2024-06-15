using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class GreaterWisp
    {
        public static bool enabled = true;
        public static bool loopOnly = false;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.GreaterWispMonster.FireCannons.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.GreaterWispMonster.FireCannons, Ray>>((aimRay, self) =>
                    {
                        if (Util.AllowPrediction(self.characterBody, loopOnly))
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, self.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.GreaterWispMonster.FireCannons.OnEnter IL Hook failed");
                }
            };
        }
    }
}
