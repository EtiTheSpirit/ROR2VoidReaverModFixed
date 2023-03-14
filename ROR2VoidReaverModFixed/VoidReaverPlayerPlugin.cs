using BepInEx;
using R2API;
using R2API.Utils;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using BepInEx.Bootstrap;
using VoidReaverMod.Initialization;
using VoidReaverMod.Survivor;
using VoidReaverMod.Buffs;

namespace VoidReaverMod
{
    [BepInDependency(R2API.R2API.PluginGUID)]
	[BepInDependency("Fokoloti.VoidFartReverb", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("Xan.VoidPlayerCharacterCommon")]
	[BepInDependency("Xan.ExaggeratedVoidDeaths")]
	[BepInDependency("com.rune580.riskofoptions")]
	[R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(DamageAPI))]
	[BepInPlugin(PLUGIN_GUID, DISPLAY_NAME, VERSION)]
	[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
	public class VoidReaverPlayerPlugin : BaseUnityPlugin {

		public const string PLUGIN_GUID = PLUGIN_AUTHOR + "." + PLUGIN_NAME;
		public const string PLUGIN_AUTHOR = "Xan";
		public const string PLUGIN_NAME = "VoidReaverPlayerCharacter";
		public const string DISPLAY_NAME = "Void Reaver Player Character";
		public const string VERSION = "3.0.0";

		void Awake() {
			Log.Init(Logger);
			Configuration.Init(Config);
			Projectiles.Init();
			Localization.Init();
			BuffProvider.Init();
			VoidReaverSurvivor.Init(this);
		}

	}
}
