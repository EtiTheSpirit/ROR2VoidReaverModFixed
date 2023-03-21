using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace VoidReaverMod.Skills.SpecialSkills {
	public class SuicideSkill : BaseState {

		/// <summary>
		/// Press R to drink bleach flavored toaster bath water (gamer girl certified)
		/// </summary>
		public override void OnEnter() {
			HealthComponent healthCmp = characterBody.gameObject.GetComponent<HealthComponent>();
			healthCmp.health = 0;
			healthCmp.shield = 0;
			healthCmp.barrier = 0;
			if (NetworkServer.active) {
				healthCmp.Suicide(gameObject, gameObject, DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection | DamageType.Silent);
				healthCmp.killingDamageType = DamageType.VoidDeath; // For the sake of the message.
			}
			outer.SetNextStateToMain();
		}

	}
}
