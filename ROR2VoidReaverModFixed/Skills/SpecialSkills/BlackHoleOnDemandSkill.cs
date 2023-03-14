using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using VoidReaverMod.Initialization;
using EntityStates.NullifierMonster;
using RoR2.Projectile;
using Xan.ROR2VoidPlayerCharacterCommon;
using VoidReaverMod.Buffs;

namespace VoidReaverMod.Skills.SpecialSkills {
	public class BlackHoleOnDemandSkill : BaseState {

		/// <summary>
		/// This is used to late-fire the portal effect in FixedUpdate, which is the little poof that happens after the black hole.
		/// </summary>
		protected bool _hasCreatedVoidPortalEffect = false;

		/// <summary>
		/// This controls the zoom effect when looking at the black hole.
		/// </summary>
		private CameraTargetParams.CameraParamsOverrideRequest _zoomOutParams;

		/// <summary>
		/// The references the zoom effect itself.
		/// </summary>
		private CameraTargetParams.CameraParamsOverrideHandle _zoomOutHandle;


		/// <summary>
		/// Similar to <see cref="GetVoidPrimaryFireInfo(Ray, CommonVoidPrimary)"/>, but designed for getting the reave explosion instead.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="projectile"></param>
		/// <param name="muzzleTransform"></param>
		/// <param name="damageMultiplier"></param>
		/// <param name="canCrit"></param>
		/// <param name="isDeath"></param>
		/// <returns></returns>
		public static FireProjectileInfo GetVoidExplosionFireInfo(BaseState state, GameObject projectile, Transform muzzleTransform, float damageMultiplier) {
			FireProjectileInfo deletionProjectile = default;
			deletionProjectile.projectilePrefab = projectile;
			deletionProjectile.position = muzzleTransform.position;
			deletionProjectile.rotation = Quaternion.identity;
			deletionProjectile.owner = state.characterBody.gameObject; //Configuration.VoidDeathFriendlyFire ? null : state.characterBody.gameObject;
			deletionProjectile.damage = state.damageStat * damageMultiplier;
			deletionProjectile.damageColorIndex = DamageColorIndex.Void;
			return deletionProjectile;
		}

		public override void OnEnter() {
			base.OnEnter();

			damageStat = characterBody.damage;
			_zoomOutParams = default;
			_zoomOutParams.cameraParamsData.idealLocalCameraPos = new Vector3(0f, 1f, -30f);
			_zoomOutParams.cameraParamsData.pivotVerticalOffset = 0f;
			_zoomOutHandle = GetComponent<CameraTargetParams>().AddParamsOverride(_zoomOutParams, 2f);

			Transform muzzleTransform = FindModelChild(DeathState.muzzleName);
			PlayCrossfade("Body", "Death", "Death.playbackRate", 3f, 0.1f);
			if (NetworkServer.active && muzzleTransform != null) {
				ProjectileManager.instance.FireProjectile(GetVoidExplosionFireInfo(
					this,
					Projectiles.NonInstakillVoidDeathProjectile, //VoidImplosionObjects.NullifierImplosion,
					muzzleTransform,
					Configuration.BaseSpecialDamage
				));
			} else if (muzzleTransform == null) {
				Log.LogError("WARNING: Failed to execute Reave ability! The character does not have a muzzle transform. Were you deleted or something? You good? Did the furries read \"muzzle\" and steal it for their diabolical activities (if so then lmao also L)?");
			}
			if (Configuration.DetainImmunity) {
				HurtBoxGroup component = GetModelTransform().GetComponent<HurtBoxGroup>();
				component.hurtBoxesDeactivatorCounter++;
			}
		}


		public override void FixedUpdate() {
			bool isEndOfEffect = fixedAge >= 3f;
			if (isEndOfEffect) {
				if (!_hasCreatedVoidPortalEffect) {
					_hasCreatedVoidPortalEffect = true;
					float selfDamage = healthComponent.combinedHealth * Configuration.DetainCost;
					if (NetworkServer.active && selfDamage > 0) {
						DamageInfo dmg = new DamageInfo {
							//attacker = gameObject,
							crit = false,
							damage = selfDamage,
							damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock,
							inflictor = gameObject,
							position = transform.position,
							procCoefficient = 0f
						};
						healthComponent.TakeDamage(dmg);
					}
					base.PlayAnimation("Gesture, Additive", "Empty");
					base.PlayAnimation("Gesture, Override", "Empty");
					outer.SetNextStateToMain();
					return;
				}
			}
			characterMotor.velocity = Vector3.zero;
			base.FixedUpdate();
		}

		public override void OnExit() {
			GetComponent<CameraTargetParams>().RemoveParamsOverride(_zoomOutHandle, 1f);
			float scale = 30f;
			EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
				origin = characterBody.corePosition,
				scale = scale
			}, false);
			if (Configuration.DetainImmunity) {
				HurtBoxGroup component = GetModelTransform().GetComponent<HurtBoxGroup>();
				component.hurtBoxesDeactivatorCounter--;
			}
			if (Configuration.DetainWeaknessDuration > 0 && Configuration.DetainWeaknessArmorReduction != 0 && NetworkServer.active) {
				characterBody.AddTimedBuff(BuffProvider.VoidRiftShock, Configuration.DetainWeaknessDuration);
			}
			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Death;
		}


	}
}
