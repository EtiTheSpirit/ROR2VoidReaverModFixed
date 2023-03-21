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
			// IL.RoR2.HealthComponent.TakeDamage += InjectTakeDamage;
			On.RoR2.HealthComponent.TakeDamage += TestForCustomNullify;
		}

		private static void TestForCustomNullify(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			bool hadNullify = @this.body.HasBuff(RoR2Content.Buffs.Nullified);
			originalMethod(@this, damageInfo);
			bool hasNullify = @this.body.HasBuff(RoR2Content.Buffs.Nullified);

			if (!hadNullify && hasNullify) {
				Log.LogTrace("Something just got Nullified...");
				if (damageInfo.HasModdedDamageType(Projectiles.CustomDurationNullify)) {
					Log.LogTrace("The damage has the Custom Duration Nullify tag!");
					// Only modify the duration if they actually have it there.
					if (@this.body.isBoss) {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationBosses);
						Log.LogTrace("Modified duration for bosses.");
					} else {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationMonsters);
						Log.LogTrace("Modified duration for monsters.");
					}
				} else {
					Log.LogTrace("A standard reaver seems to have done this damage.");
				}
			}
		}
		/*
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
				Log.LogTrace("The damage type is proper and has Custom Duration Nullify.");
				if (@this.body.HasBuff(RoR2Content.Buffs.Nullified)) {
					// Only modify the duration if they actually have it there.
					if (@this.body.isBoss) {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationBosses);
						Log.LogTrace("Modified duration for bosses.");
					} else {
						@this.body.AddTimedBuff(RoR2Content.Buffs.Nullified, Configuration.NullifyDurationMonsters);
						Log.LogTrace("Modified duration for monsters.");
					}
				} else {
					Log.LogTrace("Nope. Doesn't have nullify, skipping.");
				}
			} else {
				Log.LogTrace("A standard reaver seems to have done this damage.");
			}
		}
		*/
	}
}
