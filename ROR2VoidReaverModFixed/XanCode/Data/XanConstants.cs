using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DamageAPI;

namespace ROR2VoidReaverModFixed.XanCode.Data {
	public static class XanConstants {

		/// <summary>
		/// This damage type represents a "Void Collapse", which is the primary terminology used for the Reave and Collapse abilities of the survivor.
		/// </summary>
		public static ModdedDamageType VoidCollapse { get; private set; }

		/// <summary>
		/// The settings indicate to the user that setting the damage boost to a value greater than or equal to this number will
		/// override the void death effect to work just like normal void enemies, instakilling anything in range.<para/>
		/// The value is 1,000,000.
		/// </summary>
		public static float VoidDeletionThreshold { get; } = 1000000f;

		public static void Init() {
			VoidCollapse = ReserveDamageType();
		}

	}
}
