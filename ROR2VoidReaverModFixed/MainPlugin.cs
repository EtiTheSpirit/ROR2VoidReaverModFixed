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
using ROR2VoidReaverModFixed.XanCode.ILPatches;

namespace FubukiMods {
	[
		BepInDependency(R2API.R2API.PluginGUID),
		R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(DamageAPI)),
		NetworkCompatibility
	]
	[BepInPlugin(PLUGIN_GUID, DISPLAY_NAME, VERSION)]
	public class MainPlugin : BaseUnityPlugin {

		public const string PLUGIN_GUID = "Xan.VoidReaverPlayerCharacter";
		public const string DISPLAY_NAME = "Void Reaver Survivor (Xan's Edit)";
		public const string VERSION = "2.0.5";

		void Awake() {
			Log.Init(Logger);
			Log.LogMessage("Xan's Void Reaver Mod has started its initialization cycle.");
			Configuration.Init(Config);
			XanConstants.Init();
			Lang.Init();
			// VoidShieldColorizer.Init();

			// On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };

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

			ReaveProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion(), "VoidSpecialAttack", true);
			ProjectileDamage reaveDamage = ReaveProjectile.GetComponent<ProjectileDamage>();
			ProjectileExplosion reaveBaseExplosion = ReaveProjectile.GetComponent<ProjectileExplosion>();
			ProjectileImpactExplosion reaveImpactExplosion = ReaveProjectile.GetComponent<ProjectileImpactExplosion>();
			reaveBaseExplosion.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
			reaveBaseExplosion.blastDamageCoefficient = 1f;
			reaveBaseExplosion.blastProcCoefficient = 1f;
			reaveBaseExplosion.totalDamageMultiplier = 1f;
			reaveBaseExplosion.falloffModel = BlastAttack.FalloffModel.None;
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				reaveBaseExplosion.blastRadius *= 1.25f;
			}
			reaveDamage.damage = 1f;
			reaveDamage.damageColorIndex = DamageColorIndex.Void;
			reaveDamage.damageType = DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection; // Do NOT include VoidDeath on reave!!!
			reaveImpactExplosion.lifetime = XanConstants.REAVER_DEATH_DURATION;
			reaveImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
			ReaveProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Add(XanConstants.ReaveOrCollapse);
			ContentAddition.AddProjectile(ReaveProjectile);
			Log.LogTrace("Registered Reave death projectile and custom its damage type.");

			InstakillReaveProjectile = PrefabAPI.InstantiateClone(ReaveProjectile, "VoidSpecialAttackInstakill", true);
			// DO include VoidDeath on this one though
			ProjectileDamage collapseDamage = InstakillReaveProjectile.GetComponent<ProjectileDamage>();
			DamageAPI.ModdedDamageTypeHolderComponent collapseModDamageTypeHolder = InstakillReaveProjectile.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
			collapseDamage.damageType |= DamageType.VoidDeath;
			collapseModDamageTypeHolder.Add(XanConstants.VoidCollapse);
			ContentAddition.AddProjectile(InstakillReaveProjectile);
			Log.LogTrace("Registered Reave death projectile and custom its damage type (except this time its the one that really hurts).");

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
		public static GameObject ReaveProjectile { get; private set; }

		/// <summary>
		/// A projectile that spawns the reaver death effect, except this one hurts really bad.
		/// </summary>
		public static GameObject InstakillReaveProjectile { get; private set; }

	}
}
