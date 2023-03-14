using EntityStates;
using EntityStates.Huntress;
using EntityStates.NullifierMonster;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Initialization;

namespace VoidReaverMod.Skills.Undertow {
	public class UndertowSkill : BaseState {

		private const float MAX_AIM_DISTANCE = 200f;

		private readonly float BASE_RAND_RADIUS = Configuration.SecondarySpread;

		private readonly int BASE_BOMB_COUNT = Configuration.SecondaryCount;

		private readonly float BOMB_DAMAGE = Configuration.BaseSecondaryDamage;

		/// <summary>
		/// The effective radius as influenced by attack speed.
		/// </summary>
		private float _realRandRadius;

		/// <summary>
		/// The effective bomb count as influenced by attack speed.
		/// </summary>
		private int _realBombCount;

		/// <summary>
		/// The aim helper that renders on screen.
		/// </summary>
		private GameObject _areaSphere;

		/// <summary>
		/// True if the ability fired. This is checked in the exit state code. It is only valid on the authority.
		/// </summary>
		private bool _lastFired;

		public override void OnEnter() {
			base.OnEnter();
			_realRandRadius = BASE_RAND_RADIUS * (0.75f + attackSpeedStat * 0.25f);
			_realBombCount = Mathf.RoundToInt(attackSpeedStat * BASE_BOMB_COUNT);
			if (isAuthority) {
				_areaSphere = UnityEngine.Object.Instantiate(ArrowRain.areaIndicatorPrefab);
				_areaSphere.transform.localScale = Vector3.one * _realRandRadius;
				_lastFired = false;
				skillLocator.secondary.DeductStock(1);
				Log.LogTrace("Deducted one stock from Undertow.");
			}
		}

		public override void Update() {
			base.Update();
			if (isAuthority) {
				LayerMask hitMask = LayerIndex.world.mask | LayerIndex.enemyBody.mask;
				bool hit = Physics.Raycast(GetAimRay(), out RaycastHit castResult, MAX_AIM_DISTANCE, hitMask);
				if (hit) {
					_areaSphere.transform.position = castResult.point;
					_areaSphere.transform.up = castResult.normal;
					_areaSphere.transform.localScale = Vector3.one * _realRandRadius;
				} else {
					_areaSphere.transform.localScale = Vector3.zero;
				}
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			bool hasReleasedKey = isAuthority && !inputBank.skill2.down;
			if (hasReleasedKey) {
				Ray aimRay = GetAimRay();
				bool hit = Physics.Raycast(aimRay, out RaycastHit castResult, MAX_AIM_DISTANCE);
				if (isAuthority) {
					if (hit) {
						for (int i = 0; i < _realBombCount; i++) {
							Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _realRandRadius;
							randomDirection.y = 0f;
							bool isFirstBomb = i == 0;
							if (isFirstBomb) {
								randomDirection = Vector3.zero; // This makes at least one of them accurate
							}
							Vector3 randomSpread = castResult.point + Vector3.up * _realRandRadius + randomDirection;
							float randDiameter = _realRandRadius * 2f;
							LayerMask mask = LayerIndex.world.mask | LayerIndex.enemyBody.mask;
							bool hitSpread = Physics.Raycast(randomSpread, Vector3.down, out RaycastHit randomSpreadHit, randDiameter, mask);
							if (hitSpread) {
								randomSpread = randomSpreadHit.point;
							} else {
								randomSpread += Vector3.down * UnityEngine.Random.Range(0f, randDiameter);
							}
							ProjectileManager.instance.FireProjectile(
								Projectiles.UndertowProjectile,
								randomSpread,
								Quaternion.identity,
								gameObject,
								damageStat * BOMB_DAMAGE,
								200f,
								Util.CheckRoll(critStat, characterBody.master),
								DamageColorIndex.Void
							);
						}
					}
					outer.SetNextStateToMain();
				}

				if (hit) {
					_lastFired = _realBombCount > 0;
				} else {
					_lastFired = false;
				}
				Log.LogTrace("Undertow reports _lastFired=" + _lastFired.ToString().ToLower());
			}
		}

		public override void OnExit() {
			if (isAuthority) {
				Destroy(_areaSphere.gameObject);
				if (!_lastFired) {
					skillLocator.secondary.DeductStock(1);
					skillLocator.secondary.rechargeStopwatch = skillLocator.secondary.CalculateFinalRechargeInterval();
					Log.LogTrace("Undertow added one stock back because _lastFired was false.");
				}
			}
			if (_lastFired) {
				Util.PlaySound(sfxLocator.barkSound, gameObject);
				EffectManager.SimpleMuzzleFlash(FirePortalBomb.muzzleflashEffectPrefab, gameObject, FirePortalBomb.muzzleString, true);
				_lastFired = false;
			}
;
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.PrioritySkill;
		}

	}
}
