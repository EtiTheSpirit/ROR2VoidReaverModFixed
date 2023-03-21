using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace VoidReaverMod.Initialization.Sprites {
	public static class Images {

		public static Sprite GenericWarning {
			get {
				if (_genericWarning == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Generic_Warning)) {
						_genericWarning = ImageHelper.CreateSprite(stream);
					}
				}
				return _genericWarning;
			}
		}
		private static Sprite _genericWarning = null;

		public static Sprite Portrait {
			get {
				if (_portrait == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Portrait)) {
						_portrait = ImageHelper.CreateSprite(stream);
					}
				}
				return _portrait;
			}
		}
		private static Sprite _portrait = null;

		public static Sprite VoidImpulseIcon {
			get {
				if (_voidImpulseIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.VoidImpulse)) {
						_voidImpulseIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _voidImpulseIcon;
			}
		}
		private static Sprite _voidImpulseIcon = null;


		public static Sprite VoidSpreadIcon {
			get {
				if (_voidSpreadIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.VoidSpread)) {
						_voidSpreadIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _voidSpreadIcon;
			}
		}
		private static Sprite _voidSpreadIcon = null;

		public static Sprite UndertowIcon {
			get {
				if (_undertowIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Undertow)) {
						_undertowIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _undertowIcon;
			}
		}
		private static Sprite _undertowIcon = null;

		public static Sprite DiveIcon {
			get {
				if (_diveIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Dive2)) {
						// TODO: Dive 1 or 2?
						_diveIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _diveIcon;
			}
		}
		private static Sprite _diveIcon = null;

		public static Sprite ReaveIcon {
			get {
				if (_reaveIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Reave)) {
						_reaveIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _reaveIcon;
			}
		}
		private static Sprite _reaveIcon = null;


		public static Sprite DetainIcon {
			get {
				if (_detainIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Detain)) {
						_detainIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _detainIcon;
			}
		}
		private static Sprite _detainIcon = null;


		public static Sprite VoidRiftShockIcon { 
			get {
				if (_voidRiftShockIcon == null) {
					using (MemoryStream stream = new MemoryStream(Properties.Resources.Debuff)) {
						_voidRiftShockIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _voidRiftShockIcon;
			}
		}
		private static Sprite _voidRiftShockIcon = null;

	}
}
