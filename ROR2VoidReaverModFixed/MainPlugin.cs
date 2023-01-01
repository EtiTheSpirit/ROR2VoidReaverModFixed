using BepInEx;
using R2API;
using R2API.Utils;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FubukiMods.Modules;
using XanVoidReaverEdit;
using ROR2VoidReaverModFixed.XanCode;
using ROR2VoidReaverModFixed.XanCode.Data;
using RoR2;
using BepInEx.Bootstrap;

namespace FubukiMods {
	[
		BepInDependency(R2API.R2API.PluginGUID),
		BepInDependency("Fokoloti.VoidFartReverb", BepInDependency.DependencyFlags.SoftDependency),
		BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency),
		R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(DamageAPI)),
		NetworkCompatibility
	]
	[BepInPlugin(PLUGIN_GUID, DISPLAY_NAME, VERSION)]
	public class MainPlugin : BaseUnityPlugin {

		public const string PLUGIN_GUID = "Xan.VoidReaverPlayerCharacter";
		public const string DISPLAY_NAME = "Void Reaver Survivor (Xan's Edit)";
		public const string VERSION = "2.0.7";

		/// <summary>
		/// Set this to true to stub the Steam network management event, allowing two clients running on the same computer (and thus Steam account)
		/// to connect to eachother.
		/// </summary>
		private const bool USE_STEAM_CLIENT_STUBBER_LOCALTEST = false;

		void Awake() {
			Log.Init(Logger);
			Log.LogMessage("Xan's Void Reaver Mod has started its initialization cycle.");
			Configuration.Init(Config);
			XanConstants.Init();
			Lang.Init();
			// VoidShieldColorizer.Init();

#pragma warning disable IDE0035, CS0162
			if (USE_STEAM_CLIENT_STUBBER_LOCALTEST) {
				Log.LogError("Steam Network Client Connection Event has been stubbed. This will break multiplayer in production environments! If you are using a release build of this mod and you are seeing this message, report this issue immediately!");
				On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (_, _, _) => { };
			}
#pragma warning restore IDE0035, CS0162

			PrimaryProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarNeedleProjectile.prefab").WaitForCompletion(), "VoidPrimaryAttack", true);
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				PrimaryProjectile.transform.localScale *= 2f;
				Log.LogTrace("Upscaling primary projectile by 2x due to using full size player model...");
			}
			ProjectileController primaryController = PrimaryProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion primaryExplosion = PrimaryProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage primaryDamage = PrimaryProjectile.GetComponent<ProjectileDamage>();
			primaryController.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBlaster1Ghost.prefab").WaitForCompletion();
			primaryExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierBombProjectile.prefab").WaitForCompletion();
			primaryExplosion.lifetimeAfterImpact = 0.2f;
			primaryExplosion.blastDamageCoefficient = 1f;
			primaryDamage.damageColorIndex = DamageColorIndex.Void;
			ContentAddition.AddProjectile(PrimaryProjectile);
			Log.LogTrace("Registered primary projectile (\"Void Pearls\")");

			SecondaryProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierPreBombProjectile.prefab").WaitForCompletion(), "VoidSecondaryAttack");
			ProjectileController secondaryController = SecondaryProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion secondaryExplosion = SecondaryProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage secondaryDamage = SecondaryProjectile.GetComponent<ProjectileDamage>();
			secondaryExplosion.blastProcCoefficient = 1f;
			secondaryExplosion.blastDamageCoefficient = 1f;
			secondaryExplosion.lifetime = 0.75f;
			secondaryExplosion.lifetimeRandomOffset = 0.25f;
			secondaryController.procCoefficient = 1f;
			secondaryDamage.damageColorIndex = DamageColorIndex.Void;
			secondaryDamage.damageType = DamageType.Nullify; // This causes the Nullify effect.
			ContentAddition.AddProjectile(SecondaryProjectile);
			Log.LogTrace("Registered secondary projectile");

			DetainProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion(), "VoidSpecialAttack", true);
			ProjectileDamage detainDamage = DetainProjectile.GetComponent<ProjectileDamage>();
			ProjectileExplosion detainBaseExplosion = DetainProjectile.GetComponent<ProjectileExplosion>();
			ProjectileImpactExplosion detainImpactExplosion = DetainProjectile.GetComponent<ProjectileImpactExplosion>();
			detainBaseExplosion.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
			detainBaseExplosion.blastDamageCoefficient = 1f;
			detainBaseExplosion.blastProcCoefficient = 1f;
			detainBaseExplosion.totalDamageMultiplier = 1f;
			detainBaseExplosion.falloffModel = BlastAttack.FalloffModel.None;
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				detainBaseExplosion.blastRadius *= 1.25f;
			}
			detainDamage.damage = 1f;
			detainDamage.damageColorIndex = DamageColorIndex.Void;
			detainDamage.damageType = DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection; // Do NOT include VoidDeath on this!!!
			detainImpactExplosion.lifetime = XanConstants.REAVER_DEATH_DURATION;
			detainImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
			DetainProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(XanConstants.DetainorReaveDamage);
			ContentAddition.AddProjectile(DetainProjectile);
			Log.LogTrace("Registered Detain projectile and custom its damage type.");

			InstakillReaveProjectile = PrefabAPI.InstantiateClone(DetainProjectile, "VoidSpecialAttackInstakill", true);
			// DO include VoidDeath on this one though
			ProjectileDamage collapseDamage = InstakillReaveProjectile.GetComponent<ProjectileDamage>();
			DamageAPI.ModdedDamageTypeHolderComponent collapseModDamageTypeHolder = InstakillReaveProjectile.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
			collapseDamage.damageType |= DamageType.VoidDeath;
			collapseModDamageTypeHolder.Add(XanConstants.ReaveDamage);
			ContentAddition.AddProjectile(InstakillReaveProjectile);
			Log.LogTrace("Registered Reave projectile and custom its damage type (except this time its the one that really hurts).");

			Survivors.Init(this);
		}

		/// <summary>
		/// The custom primary attack for the reaver
		/// </summary>
		public static GameObject PrimaryProjectile { get; private set; }

		/// <summary>
		/// The "void portal" attack for reavers, which is the NPC's default attack
		/// </summary>
		public static GameObject SecondaryProjectile { get; private set; }

		/// <summary>
		/// A projectile that spawns the reaver death effect.
		/// </summary>
		public static GameObject DetainProjectile { get; private set; }

		/// <summary>
		/// A projectile that spawns the reaver death effect, except this one hurts really bad.
		/// </summary>
		public static GameObject InstakillReaveProjectile { get; private set; }

	}
}
