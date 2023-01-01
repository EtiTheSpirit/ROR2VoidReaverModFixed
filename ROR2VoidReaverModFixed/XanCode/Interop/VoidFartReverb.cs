using BepInEx.Bootstrap;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using XanVoidReaverEdit;

namespace ROR2VoidReaverModFixed.XanCode.Interop {
	public static class VoidFartReverb {

		/// <summary>
		/// (somehow, the fart with reverb sound plays when you read this comment. it is very funny to you. you laugh, and then continue doing whatever it is you were doing looking at this source code.)
		/// </summary>
		public static bool VoidFartReverbInstalled {
			get {
				_voidFartReverbInstalled ??= Chainloader.PluginInfos.ContainsKey("Fokoloti.VoidFartReverb");
				return _voidFartReverbInstalled.Value;
			}
		}
		private static bool? _voidFartReverbInstalled = null;

		public static void FartWithReverb(CharacterBody characterBody) {
			if (!VoidFartReverbInstalled) return;
			// This is copied from VoidFartReverb.
			AkSoundEngine.PostEvent(693141013u, characterBody.gameObject);
		}

	}
}
