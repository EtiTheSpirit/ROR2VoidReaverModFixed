using HarmonyLib;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using XanVoidReaverEdit;
using System.Reflection;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;

namespace ROR2VoidReaverModFixed.XanCode.ILPatches {

	/// <summary>
	/// Handles colorizing the void shield when the player is using the reaver playermodel.
	/// </summary>
	public static class VoidShieldColorizer {

		[Obsolete("This is now managed by the HP Bar Color API", true)]
		public static void Init() {
			On.RoR2.HealthComponent.GetHealthBarValues += OverrideHealthBarValues;
			IL.RoR2.CharacterModel.UpdateOverlays += OverrideOverlays;
			Log.LogInfo("Initialized Void Reaver shield render override (always render as if a Plasma Shrimp is present).");
		}

		private static void OverrideOverlays(ILContext il) {
			ILCursor cursor = new ILCursor(il);
			ILLabel loadVoidMtl = null;

			// Locate block of code that conditionally loads void shield or stock shield based on the presence of plasma shrimp
			try {
				cursor.GotoNext(
					MoveType.Before,
					instruction => instruction.MatchLdloc(8),
					instruction => instruction.MatchBrtrue(out loadVoidMtl),
					instruction => instruction.MatchLdsfld(typeof(CharacterModel).GetField(nameof(CharacterModel.energyShieldMaterial), BindingFlags.Public | BindingFlags.Static)),
					instruction => instruction.MatchBr(out _),
					instruction => instruction.MatchLdsfld(typeof(CharacterModel).GetField(nameof(CharacterModel.voidShieldMaterial), BindingFlags.Public | BindingFlags.Static)),
					instruction => instruction.MatchLdarg(0)
				);

				// before that code, emit an arg that pushes [this] onto the stack
				// use that to call IsReaverPlayer, and if that is true, jump to the part that loads the void material
				cursor.Emit(OpCodes.Ldarg_0);
				cursor.EmitDelegate(IsReaverPlayer);
				cursor.Emit(OpCodes.Brtrue_S, loadVoidMtl);
			} catch (Exception exc) {
				Log.LogWarning("NOTE: This error will NOT cause anything to break! You will just be able to render the default shield when you should be rendering the void shield.");
				Log.LogError("Failed to patch in the shield overlay override! The Void Reaver player will not have a violet shield overlay by default.");
				Log.LogError(exc.ToString());
				Log.LogError(exc.StackTrace);
				Log.LogWarning("NOTE: This error will NOT cause anything to break! You will just be able to render the default shield when you should be rendering the void shield.");
			}
		}

		private static HealthComponent.HealthBarValues OverrideHealthBarValues(On.RoR2.HealthComponent.orig_GetHealthBarValues orig, HealthComponent @this) {
			if (@this.body.baseNameToken == Lang.SURVIVOR_NAME) {
				HealthComponent.HealthBarValues def = orig(@this);
				def.hasVoidShields = true;
				return def;
			}
			return orig(@this);
		}

		private static bool IsReaverPlayer(CharacterModel @this) => @this.body.baseNameToken == Lang.SURVIVOR_NAME;

	}
}
