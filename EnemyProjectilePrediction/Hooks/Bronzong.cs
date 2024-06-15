using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class Bronzong
    {
        public static bool enabled = true;
        public static bool loopOnly = false;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.Bell.BellWeapon.ChargeTrioBomb.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.Bell.BellWeapon.ChargeTrioBomb, Ray>>((aimRay, self) =>
                    {
                        if (Util.AllowPrediction(self.characterBody, loopOnly))
                        {
                            //Uncomment this to improve accuracy further.
                            /*Transform t = self.FindTargetChildTransformFromBombIndex();
                            if (t)
                            {
                                aimRay.origin = t.position;
                            }*/
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.Bell.BellWeapon.ChargeTrioBomb.bombProjectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.Bell.BellWeapon.ChargeTrioBomb.FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
