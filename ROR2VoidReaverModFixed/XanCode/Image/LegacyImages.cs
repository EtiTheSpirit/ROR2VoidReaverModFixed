using FubukiMods.Modules;
using UnityEngine;

namespace ROR2VoidReaverModFixed.XanCode.Image {

	/// <summary>
	/// All data here was created by LuaFubuki, and generates an image using a custom indexed format for pixel art.<para/>
	/// 
	/// This has been kept for the sake of user preferences, so that the replacements in <see cref="ModernImages"/> can be disabled
	/// for those who choose to make such a change.
	/// </summary>
	public static class LegacyImages {
		private static readonly Color[] PALETTE = new Color[] {
			new Color(0f, 0f, 0f),
			new Color(0.3f, 0f, 0.3f),
			new Color(0.5f, 0f, 0.5f),
			new Color(0.3f, 0f, 0.5f),
			new Color(0.6f, 0.25f, 1f),
			new Color(0.8f, 0.75f, 1f),
			new Color(1f, 1f, 1f),
			new Color(0f, 0.1f, 0.2f),
			new Color(0.6f, 0.7f, 1f),
			new Color(0.5f, 0.5f, 0.7f),
			new Color(0.3f, 0.4f, 0.5f),
			default,
			default,
			default,
			default,
			new Color(0f, 0f, 0f, 0f)
		};

		public static Sprite Portrait {
			get {
				if (_portrait == null) {
					_portrait = Tools.SpriteFromString("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8888888888888888fffffffffffff8882222444444442222888ffffffff88222221110000001112222288fffff8222159549999999999459512228fff822299994499999999994499992228f82255555219999999999991255555228559999991199999999999911999999559956559998999998899999899955659915566655998888899888889955666551215566665999999999999995666655122155566665599999999995566665551221555566666555555555566666555512221655566666655555566666655561222216666556666666666666655666612282216666666666666666666666661228f882111008886666666688800111288ffff888888fff86666668fff888888ffffffffffffff8000000008fffffffffffffffffff8881110000111888fffffffffffff8881111111111111111888fffffffff811111122222222221111118fffffff89922222222222222222222998fffff89999224222222222222422999a8fff899999922542222222245229999aa8ff899995992245422224542299999aa8fff8999566992442222442999999aa8fffff89956659922422422999999aa8fffff889995665999222299999999aa88fff822199955999999999999999aa1228f824221999999999999999999aa122428", PALETTE, true);
				}
				return _portrait;
			}
		}
		private static Sprite _portrait = null;

		public static Sprite PrimaryTripleShotIcon {
			get {
				if (_primaryStringShotIcon == null) {
					_primaryStringShotIcon = Tools.SpriteFromString("7777777070000000777777777777770007777777777770007700000000777777777700112207777000111112262777771000011122000777000000001112227701101101112266270000011111226627011220000112227711226207000007700012207000777777000007777777700000777777777777007777777770700000", PALETTE, false);
				}
				return _primaryStringShotIcon;
			}
		}
		private static Sprite _primaryStringShotIcon = null;

		public static Sprite PrimarySpreadShotIcon {
			get {
				if (_primarySpreadShotIcon == null) {
					_primarySpreadShotIcon = Tools.SpriteFromString("7777770112477000777001122227777000112111000025771100000002222277000111122000077711100077700022210000000001222261111111212111222100000000000001171110000000777777000111111112217777700012222462770000077011266277111122227712217012224664277777000124666627770000", PALETTE, false);
				}
				return _primarySpreadShotIcon;
			}
		}
		private static Sprite _primarySpreadShotIcon = null;

		public static Sprite SecondaryIcon {
			get {
				if (_secondaryIcon == null) {
					_secondaryIcon = Tools.SpriteFromString("7770307700770007777030770077000777704077007703077770407733770307777040705507030777705074554704077770507744770407777050777770050070056500777356530356665307743434305666503777444740356530477777007453435477000044777555770033445577777770030445567777770040445566", PALETTE, false);
				}
				return _secondaryIcon;
			}
		}
		private static Sprite _secondaryIcon = null;

		public static Sprite UtilityIcon {
			get {
				if (_utilityIcon == null) {
					_utilityIcon = Tools.SpriteFromString("0000000000333300700703303355543007777073554445437777073544566453777777344566645377777344455545307463735454444300754773444455300043773444453300003773344533700000773344337707300073343377004530003443770777340000433777773370000033777777700770003777777777770000", PALETTE, false);
				}
				return _utilityIcon;
			}
		}
		private static Sprite _utilityIcon = null;

		public static Sprite SpecialWeakIcon {
			get {
				if (_specialWeakIcon == null) {
					_specialWeakIcon = Tools.SpriteFromString("2245656625654642242552652465265445254254225425444524221000442544452400100012242454020510015201255244001000124425500100000011010554010000000101245422100000021245542201111112024545240020101224542525242020242554254524524245255414564562625626521246456564564642", PALETTE, false);
				}
				return _specialWeakIcon;
			}
		}
		private static Sprite _specialWeakIcon = null;

		public static Sprite SpecialSuicideIcon => SpecialWeakIcon;

	}
}
