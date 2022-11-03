#pragma warning disable Publicizer001
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using XanVoidReaverEdit;
using static R2API.DamageAPI;

namespace ROR2VoidReaverModFixed.XanCode.Data {
	public static class XanConstants {

		/// <summary>
		/// This damage type represents the specific "Void Collapse" ability.
		/// </summary>
		public static ModdedDamageType VoidCollapse { get; private set; }

		/// <summary>
		/// This damage type signifies that it was reave or collapse that is responsible for the damage,
		/// and causes the flashy void crit glasses effect to spawn when killing something (if the user has that effect config enabled).
		/// </summary>
		public static ModdedDamageType ReaveOrCollapse { get; private set; }

		/// <summary>
		/// A variation of the crit goggles kill effect that does not emit a sound. This is not immediately set and may be null if called in a mod's init cycle.
		/// </summary>
		public static GameObject SilentVoidCritDeathEffect { get; private set; }

		/// <summary>
		/// The duration that the void reaver death effect lasts for. This is something the game itself defines.
		/// </summary>
		public const float REAVER_DEATH_DURATION = 2.5f;

		public static void Init() {
			VoidCollapse = ReserveDamageType();
			ReaveOrCollapse = ReserveDamageType();
			Log.LogTrace("Void Collapse damage type registered.");
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
			Log.LogTrace("Instantiated prefab for silent void crit death effect.");
		}
	}
}
