using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace AccurateEnemies.Hooks
{
    public class Vagrant
    {
        public static bool enabled = true;
        public static bool loopOnly = false;
        private static bool initialized = false;
        public static void Init()
        {
            if (!enabled || initialized) return;
            initialized = true;

            IL.EntityStates.VagrantMonster.Weapon.JellyBarrage.FixedUpdate += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.VagrantMonster.Weapon.JellyBarrage, Ray>>((aimRay, self) =>
                    {
                        if ((!loopOnly || (Run.instance && Run.instance.stageClearCount >= 5)) && self.characterBody && !self.characterBody.isPlayerControlled && (AccurateEnemiesPlugin.alwaysAllowBosses || !AccurateEnemiesPlugin.eliteOnly || self.characterBody.isElite))
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                        return aimRay;
                    });
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<Ray, EntityStates.VagrantMonster.Weapon.JellyBarrage, Ray>>((aimRay, self) =>
                        {
                            if ((!loopOnly || (Run.instance && Run.instance.stageClearCount >= 5)) && self.characterBody && !self.characterBody.isPlayerControlled && (AccurateEnemiesPlugin.alwaysAllowBosses || !AccurateEnemiesPlugin.eliteOnly || self.characterBody.isElite))
                            {
                                HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                                Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), AccurateEnemiesPlugin.basePredictionAngle, EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab, targetHurtbox);
                                return newAimRay;
                            }
                            return aimRay;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    Debug.LogError("AccurateEnemies: EntityStates.VagrantMonster.Weapon.JellyBarrage.FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
