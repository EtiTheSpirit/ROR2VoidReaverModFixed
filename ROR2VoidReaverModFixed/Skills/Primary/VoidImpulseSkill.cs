using EntityStates;
using EntityStates.GlobalSkills.LunarNeedle;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Initialization;

namespace VoidReaverMod.Skills.Primary {
	public class VoidImpulseSkill : CommonVoidPrimaryBase {

		/// <summary>
		/// Keeps track of the current bullet being fired.
		/// </summary>
		private int _bulletIndex = 0;

		/// <summary>
		/// The amount of bullets that are present when a cluster of bullets is being fired due to requiring more than one for that shot.
		/// </summary>
		private int _numBulletsPerCluster = 0;

		/// <summary>
		/// The number of clusters is always the same as the user's setting. This is the number of stray bullets that occur after the cluster.
		/// This value will always be less than the user's setting for the number of bullets to fire per shot.
		/// </summary>
		private int _numStrayBullets = 0;

		/// <summary>
		/// The amount of bullets per shot, this is set to the configuration.
		/// </summary>
		private int _bulletsPerStringShot = 0;

		/// <summary>
		/// The total number of shots to fire.
		/// </summary>
		private int _totalNumberOfShots = 0;

		/// <summary>
		/// This is set to the user-defined bullet deviation when the state is entered.
		/// </summary>
		private Vector2 _spread = default;

		public override void OnEnter() {
			base.OnEnter();
			Duration = BaseDuration;
			_bulletsPerStringShot = Configuration.BulletsPerImpulseShot;
			_bulletIndex = 0;
			_spread = Configuration.PrimaryImpulseSpread;
			if (Configuration.UseExperimentalSequenceShotBuff) {
				// This is a bit complicated because these behaviors are so different. The names of the variables should help, as well as their documentation.
				// It may help to think of each shot as a "slot" in a sequence.
				// Alongside the 3 (or whatever config says) slots, up to 2 (n.b. "2" comes from 3-1, its always config-1) can trail after.
				int totalNumberOfBullets = Mathf.RoundToInt(_bulletsPerStringShot * attackSpeedStat);
				_numBulletsPerCluster = Mathf.FloorToInt(totalNumberOfBullets / (float)_bulletsPerStringShot);
				_numStrayBullets = totalNumberOfBullets % _bulletsPerStringShot;
				_totalNumberOfShots = _bulletsPerStringShot + _numStrayBullets;
			} else {
				_totalNumberOfShots = Mathf.RoundToInt(_bulletsPerStringShot * attackSpeedStat);
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			if (_bulletIndex < _totalNumberOfShots) {
				double atkSpeedMod, nextBulletShotAge;
				if (Configuration.UseExperimentalSequenceShotBuff) {
					nextBulletShotAge = Duration * (Configuration.PrimaryImpulseShotTime / _totalNumberOfShots) * _bulletIndex;
				} else {
					atkSpeedMod = (Configuration.PrimaryImpulseShotTime / _bulletsPerStringShot) / attackSpeedStat;
					nextBulletShotAge = Duration * atkSpeedMod * _bulletIndex;
				}
				bool isTimeToFire = (double)fixedAge >= nextBulletShotAge;
				if (isTimeToFire) {
					if (isAuthority) {
						// Using experiment
						bool isMultiShot = Configuration.UseExperimentalSequenceShotBuff && (_bulletIndex < _bulletsPerStringShot);
						Ray aimRay = GetAimRay();
						aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 1, 1, 0f, 0f);

						if (isMultiShot) {
							for (int j = 0; j < _numBulletsPerCluster; j++) {
								ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
								aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 0.5f, 0.5f, 0f, 0f);
								// Note: It is intentional that this affects the already spread direction.
							}
						} else {
							ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
						}
					}
					// TODO: Play multiple sounds if it's a multi-shot? This could quickly devolve into noise hell, so no for now.
					Util.PlaySound(FireLunarNeedle.fireSound, gameObject);
					_bulletIndex++;
				}
			}
			bool timeToExit = isAuthority && fixedAge >= Duration;
			if (timeToExit) {
				outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority() {
			return InterruptPriority.Skill;
		}

	}
}
