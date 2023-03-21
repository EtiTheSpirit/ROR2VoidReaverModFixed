using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidReaverMod.Initialization {
	public static class NullifyDamageWithCustomDuration {

		public static void Initialize() {
			IL.RoR2.HealthComponent.TakeDamage += InjectTakeDamage;
		}

		private static void InjectTakeDamage(ILContext il) {
			ILCursor cursor = new ILCursor(il);
			cursor.GotoNext(
				MoveType.After,
				instruction => instruction.MatchLdarg(0),
				instruction => instruction.MatchLdfld(typeof(HealthComponent).GetField("body")),
				instruction => instruction.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("NullifyStack")),
				instruction => instruction.MatchLdcR4(8),
				instruction => instruction.MatchCallvirt(out _)
			);

			cursor.Emit(OpCodes.Ldarg_1);
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate(ManageNullifyIfApplied);
		}

		private static void ManageNullifyIfApplied(HealthComponent @this, DamageInfo dmg) {
			if (dmg.HasModdedDamageType(Projectiles.CustomDurationNullify)) {
				if (@this.body.HasBuff(RoR2Content.Buffs.Nullified)) {
					// Only modify the duration if they actually have it there.
					if (@this.body.isBoss) {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationBosses);
					} else {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationMonsters);
					}
				}
			}
		}
	}
}
