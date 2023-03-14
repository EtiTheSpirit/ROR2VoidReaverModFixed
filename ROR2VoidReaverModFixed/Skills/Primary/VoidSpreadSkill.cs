using EntityStates;
using EntityStates.GlobalSkills.LunarNeedle;
using Mono.Security.Authenticode;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Initialization;

namespace VoidReaverMod.Skills.Primary {
	public class VoidSpreadSkill : CommonVoidPrimaryBase {

		private Vector2 _spread;

		public override void OnEnter() {
			base.OnEnter();
			Duration = BaseDuration / attackSpeedStat;
			_spread = Configuration.PrimarySpreadShotSpread;
			Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 0.75f);
			Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 1f);
			Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 1.25f);
			if (isAuthority) {
				float arcLen = Configuration.SpreadShotArcLengthDegs;
				int numBullets = Configuration.BulletsPerSpreadShot;
				float increment = arcLen / numBullets;
				float horzSpreadFactor = (-arcLen / 2) + (increment / 2);
				for (int i = 0; i < numBullets; i++) {
					Ray aimRay = GetAimRay();
					aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 1f, 1f, horzSpreadFactor, 0f);
					horzSpreadFactor += increment;
					ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
					AddRecoil(-0.1f, -0.2f, -0.075f, 0.075f);
				}
			}
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
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
