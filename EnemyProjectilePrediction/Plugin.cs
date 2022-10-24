using BepInEx;
using System;
using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System.Runtime.CompilerServices;
using AccurateEnemies.Hooks;

namespace AccurateEnemies
{
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.AccurateEnemies", "Accurate Enemies", "1.0.4")]
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

        private void InfernoCompat()
        {
            if (InfernoLoaded)
            {
                InfernoDef = GetInfernoDef();
            }
        }

        private void ReadConfig()
        {
            hardmodeOnly = Config.Bind("Gameplay", "Restrict to Hard Difficulties", false, "Changes only apply on Monsoon and above.").Value;

            string enemyString = "Enable projectile prediction.";
            string loopString = "Only use projectile prediction after looping.";

            BeetleGuard.enabled = Config.Bind("Enemies", "Beetle Guard", true, enemyString).Value;
            Bronzong.enabled = Config.Bind("Enemies", "Brass Contraption", true, enemyString).Value;
            ClayBoss.enabled = Config.Bind("Enemies", "Clay Dunestrider", true, enemyString).Value;
            ClayGrenadier.enabled = Config.Bind("Enemies", "Clay Apothecary", true, enemyString).Value;
            FlyingVermin.enabled = Config.Bind("Enemies", "Blind Pest", true, enemyString).Value;
            GreaterWisp.enabled = Config.Bind("Enemies", "Greater Wisp/Archaic Wisp", true, enemyString).Value;
            Grovetender.enabled = Config.Bind("Enemies", "Grovetender", true, enemyString).Value;
            Lemurian.enabled = Config.Bind("Enemies", "Lemurian", true, enemyString).Value;
            LemurianBruiser.enabled = Config.Bind("Enemies", "Elder Lemurian", true, enemyString).Value;
            LunarExploder.enabled = Config.Bind("Enemies", "Lunar Exploder", true, enemyString).Value;
            MinorConstruct.enabled = Config.Bind("Enemies", "Alpha Construct", true, enemyString).Value;
            RoboBallBoss.enabled = Config.Bind("Enemies", "Solus Control Unit/Alloy Worship Unit", true, enemyString).Value;
            Scavenger.enabled = Config.Bind("Enemies", "Scavenger", true, enemyString).Value;
            Vagrant.enabled = Config.Bind("Enemies", "Wandering Vagrant", true, enemyString).Value;
            VoidJailer.enabled = Config.Bind("Enemies", "Void Jailer", true, enemyString).Value;
            Vulture.enabled = Config.Bind("Enemies", "Alloy Vulture", true, enemyString).Value;

            BeetleGuard.loopOnly = Config.Bind("Enemies", "Beetle Guard - Loop Only", false, loopString).Value;
            Bronzong.loopOnly = Config.Bind("Enemies", "Brass Contraption - Loop Only", false, loopString).Value;
            ClayBoss.loopOnly = Config.Bind("Enemies", "Clay Dunestrider - Loop Only", false, loopString).Value;
            ClayGrenadier.loopOnly = Config.Bind("Enemies", "Clay Apothecary - Loop Only", false, loopString).Value;
            FlyingVermin.loopOnly = Config.Bind("Enemies", "Blind Pest - Loop Only", true, loopString).Value;
            GreaterWisp.loopOnly = Config.Bind("Enemies", "Greater Wisp/Archaic Wisp - Loop Only", false, loopString).Value;
            Grovetender.loopOnly = Config.Bind("Enemies", "Grovetender - Loop Only", false, loopString).Value;
            Lemurian.loopOnly = Config.Bind("Enemies", "Lemurian - Loop Only", true, loopString).Value;
            LemurianBruiser.loopOnly = Config.Bind("Enemies", "Elder Lemurian - Loop Only", false, loopString).Value;
            LunarExploder.loopOnly = Config.Bind("Enemies", "Lunar Exploder - Loop Only", false, loopString).Value;
            MinorConstruct.loopOnly = Config.Bind("Enemies", "Alpha Construct - Loop Only", false, loopString).Value;
            RoboBallBoss.loopOnly = Config.Bind("Enemies", "Solus Control Unit/Alloy Worship Unit - Loop Only", false, loopString).Value;
            Scavenger.loopOnly = Config.Bind("Enemies", "Scavenger - Loop Only", false, loopString).Value;
            Vagrant.loopOnly = Config.Bind("Enemies", "Wandering Vagrant - Loop Only", false, loopString).Value;
            VoidJailer.loopOnly = Config.Bind("Enemies", "Void Jailer - Loop Only", false, loopString).Value;
            Vulture.loopOnly = Config.Bind("Enemies", "Alloy Vulture - Loop Only", false, loopString).Value;
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
            Hooks.Scavenger.Init();
            Hooks.Vagrant.Init();   //Slight improvement over vanilla, but still lags behind. Probably because projectile is too slow.
            Hooks.VoidJailer.Init();
            Hooks.ClayBoss.Init();
            Hooks.Grovetender.Init();
            Hooks.RoboBallBoss.Init();
            Hooks.FlyingVermin.Init();
            Hooks.MinorConstruct.Init();
            Hooks.LunarExploder.Init();

            //LunarWisp has built-in homing
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
