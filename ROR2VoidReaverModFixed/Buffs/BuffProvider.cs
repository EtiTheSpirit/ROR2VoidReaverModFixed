using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Buffs.Interop;
using VoidReaverMod.Initialization;
using VoidReaverMod.Initialization.Sprites;

namespace VoidReaverMod.Buffs {
	public static class BuffProvider {


		/// <summary>
		/// This effect is much like Pulverize, but with configurable severity.
		/// </summary>
		public static BuffDef VoidRiftShock { get; private set; }

		internal static void Initialize() {
			Log.LogTrace("Creating buffs...");
			VoidRiftShock = ScriptableObject.CreateInstance<BuffDef>();
			VoidRiftShock.isDebuff = true;
			VoidRiftShock.buffColor = new Color32(0xDD, 0x7A, 0xC6, 0xFF);
			VoidRiftShock.iconSprite = Images.VoidRiftShockIcon;
			VoidRiftShock.isCooldown = false;
			VoidRiftShock.canStack = false;
			if (!ContentAddition.AddBuffDef(VoidRiftShock)) {
				Log.LogWarning("VOID_RIFT_SHOCK is being set to null because something didn't go right in init.");
				VoidRiftShock = null;
			}
			BetterUIInteroperability.RegisterBuffInfo(VoidRiftShock, Localization.VOID_RIFT_SHOCK_NAME, Localization.VOID_RIFT_SHOCK_DESC);
			Log.LogTrace("Buffs initialized.");
			On.RoR2.CharacterBody.RecalculateStats += OnRecalculateStats;
		}
		private static void OnRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats originalMethod, CharacterBody @this) {
			originalMethod(@this);
			if (@this.HasBuff(VoidRiftShock)) {
				@this.armor -= Configuration.DetainWeaknessArmorReduction;
			}
		}
	}
}
