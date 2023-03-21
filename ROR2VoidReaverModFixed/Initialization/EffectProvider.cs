using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace VoidReaverMod.Initialization {
	public static class EffectProvider {

		/// <summary>
		/// The rift that the reaver comes out of.
		/// </summary>
		public static GameObject SpawnEffect { get; private set; }

		public static void Initialize() {

			Log.LogTrace("Creating spawn effect...");
			SpawnEffect = CreateNetworkedCloneFromPath("RoR2/Base/Nullifier/NullifierSpawnEffect.prefab", "NullifierSpawnEffectResizable");
			EffectComponent effect = SpawnEffect.GetComponent<EffectComponent>();
			effect.applyScale = true;
			effect.disregardZScale = false;

		}

		private static GameObject CreateNetworkedCloneFromPath(string path, string newName) {
			Log.LogTrace($"Duplicating {path} as {newName}...");
			GameObject o = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion(), newName);
			Log.LogTrace("Adding a network identity...");
			o.AddComponent<NetworkIdentity>();
			Log.LogTrace("Registering...");
			ContentAddition.AddEffect(o);
			Log.LogTrace("Done.");
			return o;
		}

	}
}
