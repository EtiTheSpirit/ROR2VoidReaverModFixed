#pragma warning disable Publicizer001
using R2API;
using RoR2;
using ROR2VoidReaverModFixed.XanCode.Image;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using XanVoidReaverEdit;
using static R2API.DamageAPI;

namespace ROR2VoidReaverModFixed.XanCode.Data {
	public static class XanConstants {

		/// <summary>
		/// This damage type represents the specific "Void Collapse" ability.
		/// </summary>
		public static ModdedDamageType ReaveDamage { get; private set; }

		/// <summary>
		/// This damage type signifies that it was reave or collapse that is responsible for the damage,
		/// and causes the flashy void crit glasses effect to spawn when killing something (if the user has that effect config enabled).
		/// </summary>
		public static ModdedDamageType DetainorReaveDamage { get; private set; }

		/// <summary>
		/// This effect is much like Pulverize, but with configurable severity.
		/// </summary>
		public static BuffDef DetainInstability { get; private set; }

		/// <summary>
		/// A variation of the crit goggles kill effect that does not emit a sound. This is not immediately set and may be null if referenced in a mod's init cycle.
		/// </summary>
		public static GameObject SilentVoidCritDeathEffect { get; private set; }

		/// <summary>
		/// The duration that the void reaver death effect lasts for. This is something the game itself defines, and so changing this will only break things.
		/// </summary>
		public const float REAVER_DEATH_DURATION = 3f;//2.5f;

		public static void Init() {
			ReaveDamage = ReserveDamageType();
			DetainorReaveDamage = ReserveDamageType();
			Log.LogTrace("Void death damage type registered.");

			DetainInstability = ScriptableObject.CreateInstance<BuffDef>();
			DetainInstability.isDebuff = true;
			DetainInstability.buffColor = new Color32(0xDD, 0x7A, 0xC6, 0xFF);
			DetainInstability.iconSprite = CommonImages.SpecialDebuffIcon;
			DetainInstability.isCooldown = false;
			DetainInstability.canStack = false;
			if (!ContentAddition.AddBuffDef(DetainInstability)) {
				Log.LogWarning("VOID_RIFT_SHOCK is being set to null because something didn't go right in init.");
				DetainInstability = null;
			}

			Log.LogInfo("Constants initialized.");

			On.RoR2.HealthComponent.AssetReferences.Resolve += InterceptHealthCmpAssetReferences;
		}

		private static void InterceptHealthCmpAssetReferences(On.RoR2.HealthComponent.AssetReferences.orig_Resolve originalMethod) {
			originalMethod();

			SilentVoidCritDeathEffect = PrefabAPI.InstantiateClone(HealthComponent.AssetReferences.critGlassesVoidExecuteEffectPrefab, "SilentVoidCritDeath");
			SilentVoidCritDeathEffect.AddComponent<NetworkIdentity>();
			EffectComponent fx = SilentVoidCritDeathEffect.GetComponentInChildren<EffectComponent>();
			fx.soundName = null;
			ContentAddition.AddEffect(SilentVoidCritDeathEffect);
			On.RoR2.HealthComponent.AssetReferences.Resolve -= InterceptHealthCmpAssetReferences; // Clean up!
			Log.LogTrace("Instantiated prefab for silent void crit death effect.");
		}
	}
}
