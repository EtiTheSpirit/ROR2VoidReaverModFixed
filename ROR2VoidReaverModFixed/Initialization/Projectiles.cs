using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.DamageBehavior;

namespace VoidReaverMod.Initialization {
	public static class Projectiles {

		public static GameObject VoidPearlProjectile { get; private set; }

		public static GameObject UndertowProjectile { get; private set; }

		public static GameObject NonInstakillVoidDeathProjectile { get; private set; }


		internal static void Init() {
			VoidPearlProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarSkillReplacements/LunarNeedleProjectile.prefab").WaitForCompletion(), "VoidPrimaryAttack", true);
			if (Configuration.UseFullSizeCharacter) {
				// We ARE using full size character
				VoidPearlProjectile.transform.localScale *= 2f;
				Log.LogTrace("Upscaling primary projectile by 2x due to using full size player model...");
			}
			ProjectileController primaryController = VoidPearlProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion primaryExplosion = VoidPearlProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage primaryDamage = VoidPearlProjectile.GetComponent<ProjectileDamage>();
			primaryController.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBlaster1Ghost.prefab").WaitForCompletion();
			primaryExplosion.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierBombProjectile.prefab").WaitForCompletion();
			primaryExplosion.lifetimeAfterImpact = 0.2f;
			primaryExplosion.blastDamageCoefficient = 1f;
			primaryDamage.damageColorIndex = DamageColorIndex.Void;
			ContentAddition.AddProjectile(VoidPearlProjectile);
			Log.LogTrace("Registered primary projectile (\"Void Pearls\")");

			UndertowProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierPreBombProjectile.prefab").WaitForCompletion(), "VoidSecondaryAttack");
			ProjectileController secondaryController = UndertowProjectile.GetComponent<ProjectileController>();
			ProjectileImpactExplosion secondaryExplosion = UndertowProjectile.GetComponent<ProjectileImpactExplosion>();
			ProjectileDamage secondaryDamage = UndertowProjectile.GetComponent<ProjectileDamage>();
			secondaryExplosion.blastProcCoefficient = 1f;
			secondaryExplosion.blastDamageCoefficient = 1f;
			secondaryExplosion.lifetime = 0.75f;
			secondaryExplosion.lifetimeRandomOffset = 0.25f;
			secondaryController.procCoefficient = 1f;
			secondaryDamage.damageColorIndex = DamageColorIndex.Void;
			secondaryDamage.damageType = DamageType.Nullify; // This causes the Nullify effect.
			ContentAddition.AddProjectile(UndertowProjectile);
			Log.LogTrace("Registered secondary projectile");

			NonInstakillVoidDeathProjectile = PrefabAPI.InstantiateClone(VoidImplosionObjects.NullifierImplosion, "NullifierImplosionNoVoidDeath");
			NonInstakillVoidDeathProjectile.GetComponent<ProjectileDamage>().damageType &= ~DamageType.VoidDeath;
			NonInstakillVoidDeathProjectile.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>().Remove(VoidDamageTypes.ConditionalVoidDeath);
			ContentAddition.AddProjectile(NonInstakillVoidDeathProjectile);
			Log.LogTrace("Registered nonlethal black hole");

		}

	}
}
