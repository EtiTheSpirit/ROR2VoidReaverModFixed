using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ROR2VoidReaverModFixed.XanCode.Image {

	/// <summary>
	/// Replaces <see cref="LegacyImages"/> with modern rendered images.
	/// </summary>
	/// <remarks>
	/// The overhead of this system is not entirely satisfactory as it requires Unity to allocate a Texture2D, and encode the texture from a PNG file.
	/// It would be ideal if some better format (like DDS) were packed with the application.
	/// </remarks>
	public static class ModernImages {

		private static Stream GetResource(string name) {
			name = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(objName => objName.EndsWith(name));
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
		}

		public static Sprite Portrait {
			get {
				if (_portrait == null) {
					using (Stream stream = GetResource("Portrait.png")) {
						_portrait = ImageHelper.CreateSprite(stream);
					}
				}
				return _portrait;
			}
		}
		private static Sprite _portrait = null;

		public static Sprite PrimaryTripleShotIcon {
			get {
				if (_primaryTripleShotIcon == null) {
					using (Stream stream = GetResource("VoidImpulse.png")) {
						_primaryTripleShotIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _primaryTripleShotIcon;
			}
		}
		private static Sprite _primaryTripleShotIcon = null;

		public static Sprite PrimarySpreadShotIcon {
			get {
				if (_primarySpreadShotIcon == null) {
					using (Stream stream = GetResource("VoidSpread.png")) {
						_primarySpreadShotIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _primarySpreadShotIcon;
			}
		}
		private static Sprite _primarySpreadShotIcon = null;

		public static Sprite SecondaryIcon {
			get {
				if (_secondaryIcon == null) {
					using (Stream stream = GetResource("Undertow.png")) {
						_secondaryIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _secondaryIcon;
			}
		}
		private static Sprite _secondaryIcon = null;

		public static Sprite UtilityIcon {
			get {
				if (_utilityIcon == null) {
					using (Stream stream = GetResource("Dive.png")) {
						_utilityIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _utilityIcon;
			}
		}
		private static Sprite _utilityIcon = null;

		public static Sprite SpecialWeakIcon {
			get {
				if (_specialWeakIcon == null) {
					using (Stream stream = GetResource("Reave.png")) {
						_specialWeakIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _specialWeakIcon;
			}
		}
		private static Sprite _specialWeakIcon = null;

		public static Sprite SpecialSuicideIcon {
			get {
				if (_specialSuicideIcon == null) {
					using (Stream stream = GetResource("Collapse.png")) {
						_specialSuicideIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _specialSuicideIcon;
			}
		}
		private static Sprite _specialSuicideIcon = null;

		public static Sprite DefaultSkinIcon {
			get {
				if (_defaultSkinIcon == null) {
					using (Stream stream = GetResource("DefaultPalette.png")) {
						_defaultSkinIcon = ImageHelper.CreateSprite(stream);
					}
				}
				return _defaultSkinIcon;
			}
		}
		private static Sprite _defaultSkinIcon = null;

	}
}
