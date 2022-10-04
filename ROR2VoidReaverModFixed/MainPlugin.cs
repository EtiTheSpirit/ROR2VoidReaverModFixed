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
using static R2API.DamageAPI;

namespace FubukiMods {
	[
		BepInDependency(R2API.R2API.PluginGUID),
		R2APISubmoduleDependency(nameof(PrefabAPI), nameof(LoadoutAPI), nameof(LanguageAPI), nameof(DamageAPI)),
		NetworkCompatibility
	]
	[BepInPlugin("com.Fubuki.VoidReaver.XansEdit", "Void Reaver Survivor (Xan's Edit)", "2.0.0")]
	public class MainPlugin : BaseUnityPlugin {

		void Awake() {
			Log.Init(Logger);
			Log.LogMessage("Initializing Void Reaver Mod Edit");
			Configuration.Init(Config);
			Lang.Init();
			XanConstants.Init();

			PrimaryProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarNeedleProjectile.prefab").WaitForCompletion(), "VoidPrimaryAttack", true);
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				PrimaryProjectile.transform.localScale *= 2f;
			}
			ProjectileController primaryController = PrimaryProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion primaryExplosion = PrimaryProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage primaryDamage = PrimaryProjectile.GetComponent<ProjectileDamage>();
			primaryController.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBlaster1Ghost.prefab").WaitForCompletion();
			primaryExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierBombProjectile.prefab").WaitForCompletion();
			primaryExplosion.lifetimeAfterImpact = 0.2f;
			primaryExplosion.blastDamageCoefficient = 1f;
			primaryDamage.damageColorIndex = RoR2.DamageColorIndex.Void;
			ContentAddition.AddProjectile(PrimaryProjectile);

			SecondaryProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierPreBombProjectile.prefab").WaitForCompletion(), "VoidSecondaryAttack");
			ProjectileController secondaryController = SecondaryProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion secondaryExplosion = SecondaryProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage secondaryDamage = SecondaryProjectile.GetComponent<ProjectileDamage>();
			secondaryExplosion.blastProcCoefficient = 1f;
			secondaryExplosion.blastDamageCoefficient = 1f;
			secondaryExplosion.lifetime = 0.75f;
			secondaryExplosion.lifetimeRandomOffset = 0.25f;
			secondaryController.procCoefficient = 1f;
			secondaryDamage.damageColorIndex = RoR2.DamageColorIndex.Void;
			secondaryDamage.damageType = RoR2.DamageType.Nullify; // This causes the Nullify effect.
			ContentAddition.AddProjectile(SecondaryProjectile);

			ReaveProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierDeathBombProjectile.prefab").WaitForCompletion(), "VoidSpecialAttack", true);
			ProjectileDamage reaveDamage = ReaveProjectile.GetComponent<ProjectileDamage>();
			ProjectileExplosion reaveBaseExplosion = ReaveProjectile.GetComponent<ProjectileExplosion>();
			ProjectileImpactExplosion reaveImpactExplosion = ReaveProjectile.GetComponent<ProjectileImpactExplosion>();
			reaveBaseExplosion.blastAttackerFiltering = RoR2.AttackerFiltering.NeverHitSelf;
			reaveBaseExplosion.blastDamageCoefficient = 1f;
			reaveBaseExplosion.blastProcCoefficient = 1f;
			reaveBaseExplosion.totalDamageMultiplier = 1f;
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				reaveBaseExplosion.blastRadius *= 1.25f;
			}
			reaveDamage.damage = 1f;
			reaveDamage.damageColorIndex = RoR2.DamageColorIndex.Void;
			reaveDamage.damageType = RoR2.DamageType.VoidDeath | RoR2.DamageType.BypassArmor | RoR2.DamageType.BypassBlock | RoR2.DamageType.BypassOneShotProtection;//RoR2.DamageType.LunarSecondaryRootOnHit;
			reaveImpactExplosion.lifetime = Skills.VoidDeath.REAVE_DURATION;

			ModdedDamageTypeHolderComponent damageTypeHolder = ReaveProjectile.AddComponent<ModdedDamageTypeHolderComponent>();
			damageTypeHolder.Add(XanConstants.VoidCollapse);

			ContentAddition.AddProjectile(ReaveProjectile);

			Survivors.Init();
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

	}
}
