using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Initialization;

namespace VoidReaverMod.Skills.Primary {
	/// <summary>
	/// A common base for the two primary attacks.
	/// </summary>
	public abstract class CommonVoidPrimaryBase : BaseState {

		protected FireProjectileInfo GetVoidPrimaryFireInfo(Ray aimRay, CommonVoidPrimaryBase commonVoidPrimary) {
			FireProjectileInfo voidPrimaryProjectile = default;
			voidPrimaryProjectile.position = aimRay.origin;
			voidPrimaryProjectile.rotation = Quaternion.LookRotation(aimRay.direction);
			voidPrimaryProjectile.crit = commonVoidPrimary.RollCrit();
			voidPrimaryProjectile.damage = commonVoidPrimary.damageStat * commonVoidPrimary.Damage;
			voidPrimaryProjectile.owner = commonVoidPrimary.gameObject;
			voidPrimaryProjectile.procChainMask = default;
			voidPrimaryProjectile.force = 0f;
			voidPrimaryProjectile.useFuseOverride = false;
			voidPrimaryProjectile.useSpeedOverride = false;
			voidPrimaryProjectile.target = null;
			voidPrimaryProjectile.projectilePrefab = commonVoidPrimary.Projectile;
			voidPrimaryProjectile.damageColorIndex = DamageColorIndex.Void;
			return voidPrimaryProjectile;
		}

		/// <summary>
		/// A reference to the primary projectile.
		/// </summary>
		public GameObject Projectile => Projectiles.VoidPearlProjectile;

		/// <summary>
		/// The default cooldown of this ability with no attack speed. This should be the same as that of the cooldown defined in the survivor.
		/// </summary>
		public float BaseDuration { get; protected set; } = 1f;

		/// <summary>
		/// The effective duration of this ability. This may be shorter than the cooldown, and should be affected by attack speed where applicable.
		/// tries to complete in.
		/// </summary>
		public float Duration { get; protected set; }

		/// <summary>
		/// The damage done per tick. It is default multiplied by 0.5 because it does two half ticks of damage.
		/// </summary>
		public float Damage { get; protected set; } = 0.5f * Configuration.BasePrimaryDamage;
	}
}
