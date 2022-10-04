using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ROR2VoidReaverModFixed.XanCode.Image {
	public static class CommonImages {

		public static Sprite Portrait => Configuration.UseNewIcons ? ModernImages.Portrait : LegacyImages.Portrait;
		public static Sprite Passive => Portrait;
		public static Sprite PrimaryTripleShotIcon => Configuration.UseNewIcons ? ModernImages.PrimaryTripleShotIcon : LegacyImages.PrimaryTripleShotIcon;
		public static Sprite PrimarySpreadShotIcon => Configuration.UseNewIcons ? ModernImages.PrimarySpreadShotIcon : LegacyImages.PrimarySpreadShotIcon;
		public static Sprite SecondaryIcon => Configuration.UseNewIcons ? ModernImages.SecondaryIcon : LegacyImages.SecondaryIcon;
		public static Sprite UtilityIcon => Configuration.UseNewIcons ? ModernImages.UtilityIcon : LegacyImages.UtilityIcon;
		public static Sprite SpecialWeakIcon => Configuration.UseNewIcons ? ModernImages.SpecialWeakIcon : LegacyImages.SpecialWeakIcon;
		public static Sprite SpecialSuicideIcon => Configuration.UseNewIcons ? ModernImages.SpecialSuicideIcon : LegacyImages.SpecialSuicideIcon;
		public static Sprite DefaultSkinIcon => ModernImages.DefaultSkinIcon;

	}
}
