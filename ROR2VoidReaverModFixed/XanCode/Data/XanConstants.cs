using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using XanVoidReaverEdit;
using static R2API.DamageAPI;

namespace ROR2VoidReaverModFixed.XanCode.Data {
	public static class XanConstants {

		/// <summary>
		/// This damage type represents a "Void Collapse", which is the primary terminology used for the Reave and Collapse abilities of the survivor.
		/// </summary>
		public static ModdedDamageType VoidCollapse { get; private set; }

		/// <summary>
		/// The duration that the void reaver death effect lasts for. This is something the game itself defines.
		/// </summary>
		public const float REAVER_DEATH_DURATION = 2.5f;

		public static void Init() {
			VoidCollapse = ReserveDamageType();
			Log.LogTrace("Void Collapse damage type registered.");
			Log.LogInfo("Constants initialized.");
		}

	}
}
