using EntityStates;
using Mono.Security.Authenticode;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using Xan.ROR2VoidPlayerCharacterCommon.EntityStates;
using RoR2.Projectile;
using VoidReaverMod.Skills.SpecialSkills;
using Xan.ROR2VoidPlayerCharacterCommon;

namespace VoidReaverMod.Skills.Death {
	public class DeathState : VoidDeathStateBase {

		private bool _isPlayerDeath;
		private Transform _cachedModelTransform;
		private GameObject _cachedGameObject;
		private bool _hasDeleted = false;

		private string muzzleName => EntityStates.NullifierMonster.DeathState.muzzleName;

		/// <summary>
		/// This controls the zoom effect when looking at the black hole.
		/// </summary>
		private CameraTargetParams.CameraParamsOverrideRequest _zoomOutParams;

		/// <summary>
		/// The references the zoom effect itself.
		/// </summary>
		private CameraTargetParams.CameraParamsOverrideHandle _zoomOutHandle;

		public override void OnEnter() {
			base.OnEnter();
			damageStat = characterBody.damage;
			_isPlayerDeath = characterBody.master != null && characterBody.master.GetComponent<PlayerCharacterMasterController>() != null;
			_cachedModelTransform = characterBody.gameObject.GetComponent<ModelLocator>().modelTransform;
			_cachedGameObject = gameObject;

			_zoomOutParams = default;
			_zoomOutParams.cameraParamsData.idealLocalCameraPos = new Vector3(0f, 1f, -30f);
			_zoomOutParams.cameraParamsData.pivotVerticalOffset = 0f;
			_zoomOutHandle = GetComponent<CameraTargetParams>().AddParamsOverride(_zoomOutParams, 2f);
			PlayCrossfade("Body", "Death", "Death.playbackRate", 3f, 0.1f);
			if (isAuthority) {
				Transform muzzleTransform = FindModelChild(muzzleName);
				if (muzzleTransform == null) { muzzleTransform = gameObject.transform; }
				if (muzzleTransform != null) {
					ProjectileManager.instance.FireProjectile(BlackHoleOnDemandSkill.GetVoidExplosionFireInfo(
						this,
						VoidImplosionObjects.NullifierImplosion,
						muzzleTransform,
						1f
					));
				} else {
					Log.LogError("WARNING: Failed to execute death explosion! The character does not have a muzzle transform. Were you deleted or something? You good? Did the furries read \"muzzle\" and steal it for their diabolical activities (if so then lmao also L)?");
				}

				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = characterBody.bestFitRadius
				}, true);
			}

			if (_isPlayerDeath && characterBody != null) {
				UnityEngine.Object.Instantiate(
					LegacyResourcesAPI.Load<GameObject>("Prefabs/TemporaryVisualEffects/PlayerDeathEffect"),
					characterBody.corePosition,
					Quaternion.identity
				).GetComponent<LocalCameraEffect>().targetCharacter = characterBody.gameObject;
			}
		}

		public override void FixedUpdate() {
			fixedAge += Time.fixedDeltaTime;

			if (fixedAge >= 3f + Time.fixedDeltaTime && !_hasDeleted) {
				CameraTargetParams target = GetComponent<CameraTargetParams>();
				if (target != null) target.RemoveParamsOverride(_zoomOutHandle, 1f);

				_hasDeleted = true;
				DeleteReplicatedModel();
			}
		}

		protected void DeleteReplicatedModel() {
			if (_cachedModelTransform != null) {
				UnityEngine.Object.Destroy(_cachedModelTransform.gameObject);
			}
			if (NetworkServer.active) {
				if (_cachedGameObject != null) {
					UnityEngine.Object.Destroy(_cachedGameObject, minTimeToKeepBodyForNetworkMessages);
				}
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Death;
	}
}
