using BepInEx;
using System;
using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System.Runtime.CompilerServices;

namespace AccurateEnemies
{
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.AccurateEnemies", "Accurate Enemies", "1.0.0")]
    public class AccurateEnemiesPlugin : BaseUnityPlugin
    {
        public static bool InfernoLoaded = false;
        public static bool RiskyArtifactsLoaded = false;
        public static DifficultyDef InfernoDef = null;

        public static float basePredictionAngle = 45f;
        public static bool hardmodeOnly = false;

        public void Awake()
        {
            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            RiskyArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");
            ReadConfig();
            RunHooks();
            RoR2.RoR2Application.onLoad += InfernoCompat;
        }

        public void InfernoCompat()
        {
            if (InfernoLoaded)
            {
                InfernoDef = GetInfernoDef();
            }
        }

        private void ReadConfig()
        {
            hardmodeOnly = Config.Bind("Gameplay", "Restrict to Hard Difficulties", false, "Changes only apply on Monsoon and above.").Value;
        }

        private void RunHooks()
        {
            Hooks.Lemurian.Init();
            Hooks.Vulture.Init();
            Hooks.Bronzong.Init();
            Hooks.GreaterWisp.Init();
            Hooks.BeetleGuard.Init();
            Hooks.ClayGrenadier.Init();
            Hooks.LemurianBruiser.Init();
            Hooks.LunarWisp.Init();
            Hooks.Scavenger.Init();
            Hooks.Vagrant.Init();   //Slight improvement over vanilla, but still lags behind. Probably because projectile is too slow.
            Hooks.VoidJailer.Init();
            Hooks.ClayBoss.Init();
            Hooks.Grovetender.Init();
            Hooks.RoboBallBoss.Init();
            Hooks.FlyingVermin.Init();
            Hooks.MinorConstruct.Init();
            Hooks.LunarExploder.Init();

            //Grandparent rock throw is irrelevant
            //LunarGolem doesnt seem to work
            //MiniMushrum has built-in aiming
            //UrchinTurret has bad aim
            //VoidBarnacle has built-in homing
            //BeetleQueen doesn't seem to do much, has built-in aiming.
            //VoidMegaCrab isn't very reliant on aiming.
            //ImpBoss doesn't seem to do much. Threat from spikes comes from environmental traps, rather than direct hits.
            //Brother lunar shards get aimed into the floor.
        }

        public static float GetProjectileSimpleModifiers(float speed)
        {
            if (InfernoLoaded) speed *= GetInfernoProjectileSpeedMult();
            if (RiskyArtifactsLoaded) speed *= GetRiskyArtifactsWarfareProjectileSpeedMult();
            return speed;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetRiskyArtifactsWarfareProjectileSpeedMult()
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Risky_Artifacts.Artifacts.Warfare.artifact))
            {
                return Risky_Artifacts.Artifacts.Warfare.projSpeed;
            }
            return 1f;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static DifficultyDef GetInfernoDef()
        {
            return Inferno.Main.InfernoDiffDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetInfernoProjectileSpeedMult()
        {
            if (Run.instance && DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == AccurateEnemiesPlugin.InfernoDef)
            {
                return Inferno.Main.ProjectileSpeed.Value;
            }
            return 1f;
        }
    }
}

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute
    {
    }
}
