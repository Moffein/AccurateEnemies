using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class Lemurian
    {
        public static bool enabled = true;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.LemurianMonster.FireFireball.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.LemurianMonster.FireFireball, Ray>>((aimRay, self) =>
                    {
                        if (self.characterBody && !self.characterBody.isPlayerControlled)
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.LemurianMonster.FireFireball.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                        return aimRay;
                    });
                }
                else
                {
                    Debug.LogError("AccurateEnemies: EntityStates.LemurianMonster.FireFireball.OnEnter IL Hook failed");
                }
            };
        }
    }
}
