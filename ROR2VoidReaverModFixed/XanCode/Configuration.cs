using BepInEx.Configuration;
using FubukiMods.Modules;
using ROR2VoidReaverModFixed.XanCode.ConfigurationUtil;
using ROR2VoidReaverModFixed.XanCode.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XanVoidReaverEdit;

namespace ROR2VoidReaverModFixed.XanCode {

	/// <summary>
	/// A universal hub for referencing all configuration values quickly and easily.
	/// </summary>
	public static class Configuration {

		private static ConfigFile _cfg = null;

		#region Configuration Entries

		#region Public Properties

		#region Base Stats

		public static float BaseMaxHealth => _baseMaxHealth.Value;
		public static float LevelMaxHealth => _levelMaxHealth.Value;

		public static float BaseMoveSpeed => _baseMoveSpeed.Value;
		public static float LevelMoveSpeed => _levelMoveSpeed.Value;

		public static float BaseHPRegen => _baseHPRegen.Value;
		public static float LevelHPRegen => _levelHPRegen.Value;

		public static float BaseArmor => _baseArmor.Value;
		public static float LevelArmor => _levelArmor.Value;

		public static float BaseDamage => _baseDamage.Value;
		public static float LevelDamage => _levelDamage.Value;

		public static float BaseCritChance => _baseCritChance.Value;
		public static float LevelCritChance => _levelCritChance.Value;

		public static float BaseMaxShield => _baseMaxShield.Value;
		public static float LevelMaxShield => _levelMaxShield.Value;

		public static float BaseAcceleration => _baseAcceleration.Value;

		public static int BaseJumpCount => _baseJumpCount.Value;

		public static float BaseJumpPower => _baseJumpPower.Value;
		public static float LevelJumpPower => _levelJumpPower.Value;

		public static float BaseAttackSpeed => _baseAttackSpeed.Value;
		public static float LevelAttackSpeed => _levelAttackSpeed.Value;

		#endregion

		#region Primary Attack
		/// <summary>
		/// The common base damage of Void Impulse and Void Spread
		/// </summary>
		public static float BasePrimaryDamage => _basePrimaryDamage.Value;

		/// <summary>
		/// If true, Void Impulse will prefer to bulk bullets into bursts. The amount of bursts tries to be equal to <see cref="BulletsPerImpulseShot"/>.
		/// </summary>
		public static bool UseExperimentalSequenceShotBuff => _primaryUseExperimentalTripleshotBuff.Value;

		#region Void Impulse
		/// <summary>
		/// The min and max spread of Void Impulse projectiles.
		/// </summary>
		public static Vector2 PrimaryImpulseSpread => _primaryImpulseSpread.Value;

		/// <summary>
		/// The amount of time that Void Impulse tries to fire all projectiles in.
		/// </summary>
		public static float PrimaryImpulseShotTime => _primaryImpulseShotTime.Value;

		/// <summary>
		/// The amount of bursts fired when Void Impulse is used.
		/// </summary>
		public static int BulletsPerImpulseShot => _primaryImpulseBulletCount.Value;
		#endregion

		#region Void Spread
		/// <summary>
		/// The amount of bullets that will be in the fan of shots
		/// </summary>
		public static int BulletsPerSpreadShot => _primarySpreadBulletCount.Value;

		/// <summary>
		/// The min and max deviation for each projectile in the spread shot.
		/// </summary>
		public static Vector2 PrimarySpreadShotSpread => _primarySpreadShotSpread.Value;

		/// <summary>
		/// The width of the arc that shots are evenly distributed within.
		/// </summary>
		public static float SpreadShotArcLengthDegs => _primarySpreadArcDegrees.Value;
		#endregion

		#endregion

		#region Secondary Attack
		/// <summary>
		/// The base damage that Undertow does.
		/// </summary>
		public static float BaseSecondaryDamage => _baseSecondaryDamage.Value;

		/// <summary>
		/// The default radius of Undertow.
		/// </summary>
		public static float SecondarySpread => _secondaryRadius.Value;

		/// <summary>
		/// The amount of black holes that Undertow places.
		/// </summary>
		public static int SecondaryCount => _secondaryCount.Value;

		/// <summary>
		/// The cooldown of Undertow.
		/// </summary>
		public static float SecondaryCooldown => _secondaryCooldown.Value;

		#endregion

		#region Utility
		/// <summary>
		/// The speed at which the player travels whilst using Dive
		/// </summary>
		public static float UtilitySpeed => _utilitySpeed.Value;

		/// <summary>
		/// The amount of health the player regenerates when using Dive
		/// </summary>
		public static float UtilityRegen => _utilityRegen.Value;

		/// <summary>
		/// The amount of time the player travels for in Dive
		/// </summary>
		public static float UtilityDuration => _utilityDuration.Value;
		#endregion

		#region Special

		/// <summary>
		/// The base amount of damage that Reave does. This does not affect Collapse.
		/// </summary>
		public static float BaseSpecialDamage => _baseSpecialDamage.Value;

		/// <summary>
		/// Whether or not Reave/Collapse deals damage to friendly players even when artifact of Chaos is disabled.
		/// </summary>
		[Obsolete("This does not yet function properly.")]
		public static bool VoidDeathFriendlyFire => false;//_collapseFriendlyFire.Value;

		#region Reave

		/// <summary>
		/// The cooldown of Reave in seconds.
		/// </summary>
		public static float SpecialCooldown => _reaveCooldown.Value;

		/// <summary>
		/// The amount of health the player must spend to perform Reave.
		/// </summary>
		public static float ReaveCost => _reaveHealthCostPercent.Value;

		/// <summary>
		/// If true, the player cannot take damage whilst performing their Reave special.
		/// </summary>
		public static bool ReaveImmunity => _reaveImmunity.Value;
		#endregion

		#region Collapse
		/// <summary>
		/// The amount of damage that the implosion on death does.
		/// </summary>
		public static float BaseDeathDamage => _baseDeathDamage.Value;
		/// <summary>
		/// Not a configurable setting. This is identical to <c><see cref="BaseDeathDamage"/> &gt;= <see cref="XanConstants.VoidDeletionThreshold"/></c>
		/// </summary>
		public static bool IsVoidDeathInstakill => BaseDeathDamage >= XanConstants.VoidDeletionThreshold;

		#endregion

		#endregion

		#region Misc.
		/// <summary>
		/// [Not Implemented] Whether to use the old abilities that were effectively duplicates of Lunar abilities.
		/// </summary>
		[Obsolete("This has not been implemented, and might be completely removed.")]
		public static bool UseLegacyLunarMechanics => false;//_useLegacyLunarBase.Value;

		/// <summary>
		/// [Experimental] Allow the scale of the player character to be identical to that of a real reaver.
		/// </summary>
		public static bool UseFullSizeCharacter => _useFullSizeCharacter.Value;

		/// <summary>
		/// Use icons I rendered in Blender instead of the pixel art that LuaFubuki made.
		/// </summary>
		public static bool UseNewIcons => _useNewIcons.Value;

		/// <summary>
		/// If true, the character should be immune to the effects of Void Fog, and the passive damage from void atmospheres (such as the interior of Void Seeds, the ambient atmosphere of the Void Fields and Locus, etc.)
		/// </summary>
		public static bool VoidImmunity => _voidImmunity.Value;
		#endregion

		#endregion

		#region Backing Settings Objects
		#region Base Stats
		private static ConfigEntry<float> _baseMaxHealth;
		private static ConfigEntry<float> _levelMaxHealth;

		private static ConfigEntry<float> _baseMoveSpeed;
		private static ConfigEntry<float> _levelMoveSpeed;

		private static ConfigEntry<float> _baseHPRegen;
		private static ConfigEntry<float> _levelHPRegen;

		private static ConfigEntry<float> _baseArmor;
		private static ConfigEntry<float> _levelArmor;

		private static ConfigEntry<float> _baseDamage;
		private static ConfigEntry<float> _levelDamage;

		private static ConfigEntry<float> _baseCritChance;
		private static ConfigEntry<float> _levelCritChance;

		private static ConfigEntry<float> _baseMaxShield;
		private static ConfigEntry<float> _levelMaxShield;

		private static ConfigEntry<float> _baseAcceleration;

		private static ConfigEntry<int> _baseJumpCount;

		private static ConfigEntry<float> _baseJumpPower;
		private static ConfigEntry<float> _levelJumpPower;

		private static ConfigEntry<float> _baseAttackSpeed;
		private static ConfigEntry<float> _levelAttackSpeed;


		#endregion

		#region Primary Attack
		private static ConfigEntry<float> _basePrimaryDamage;
		//private static ConfigEntry<float> _levelPrimaryDamage;
		private static ConfigEntry<Vector2> _primaryImpulseSpread;
		private static ConfigEntry<Vector2> _primarySpreadShotSpread;
		private static ConfigEntry<float> _primaryImpulseShotTime;
		private static ConfigEntry<int> _primaryImpulseBulletCount;
		private static ConfigEntry<int> _primarySpreadBulletCount;
		private static ConfigEntry<float> _primarySpreadArcDegrees;
		private static ConfigEntry<bool> _primaryUseExperimentalTripleshotBuff;
		#endregion

		#region Secondary Attack
		private static ConfigEntry<float> _baseSecondaryDamage;
		private static ConfigEntry<float> _secondaryRadius;
		private static ConfigEntry<int> _secondaryCount;
		//private static ConfigEntry<float> _levelSecondaryDamage;
		private static ConfigEntry<float> _secondaryCooldown;
		#endregion

		#region Utility
		private static ConfigEntry<float> _utilitySpeed;
		private static ConfigEntry<float> _utilityRegen;
		private static ConfigEntry<float> _utilityDuration;
		#endregion

		#region Special
		private static ConfigEntry<float> _baseSpecialDamage;

		#region Reave
		private static ConfigEntry<float> _reaveCooldown;
		private static ConfigEntry<float> _reaveHealthCostPercent;
		private static ConfigEntry<bool> _reaveImmunity;
		#endregion

		#region Collapse
		private static ConfigEntry<float> _baseDeathDamage;
		private static ConfigEntry<bool> _collapseFriendlyFire;
		#endregion
		#endregion

		#region Misc.
		private static ConfigEntry<bool> _useLegacyLunarBase;
		private static ConfigEntry<bool> _useFullSizeCharacter;
		private static ConfigEntry<bool> _useNewIcons;
		private static ConfigEntry<bool> _voidImmunity;
		#endregion
		#endregion

		#endregion

		#region Backing Code

		/// <summary>
		/// Whether or not the custom config limits are in place.
		/// </summary>
		private static readonly bool USE_CUSTOM_MINMAX = false;

		/// <summary>
		/// Casts <see cref="ConfigEntryBase.DefaultValue"/> into the type represented by a <see cref="ConfigEntry{T}"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cfg"></param>
		/// <returns></returns>
		private static T DefVal<T>(this ConfigEntry<T> cfg) where T : struct => (T)cfg.DefaultValue;

		private const string FMT_DEFAULT = "The base {0} that the character has on a new run.";
		private const string FMT_LEVELED = "For each level the player earns, the base {0} increases by this amount.";

		/// <summary>
		/// An alias to declare a <see cref="ConfigDefinition"/> based on what limit types to include.<para/>
		/// This is a lazy "solution" to custom limits not working very well.
		/// </summary>
		/// <param name="desc">The description of the setting.</param>
		/// <param name="limit">The limit for the setting, which may or may not actually be used.</param>
		/// <returns></returns>
		public static ConfigDescription StaticDeclareConfigDescription(string desc, AcceptableValueBase limit = null) {
			if (USE_CUSTOM_MINMAX) {
				return new ConfigDescription(desc, limit);
			} else {
				if (limit is not AcceptableMinimum && limit is not AcceptableUserDefinedMinMax) {
					// My type is broken so only allow it if it's not mine.
					return new ConfigDescription(desc, limit);
				} else {
					return new ConfigDescription(desc);
				}
			}
		}

		public static void Init(ConfigFile cfg) {
			if (_cfg != null) throw new InvalidOperationException($"{nameof(Configuration)} has already been initialized!");
			_cfg = cfg;

			// TODO: I would *like* to get RiskOfOptions support but there are two critical issues preventing that

			_baseMaxHealth = cfg.Bind("1. Character Vitality", "Base Maximum Health", 200f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "maximum health"), new AcceptableMinimum<float>(1f)));
			_levelMaxHealth = cfg.Bind("1. Character Vitality", "Leveled Maximum Health", 60f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "maximum health"), new AcceptableMinimum<float>()));
			_baseHPRegen = cfg.Bind("1. Character Vitality", "Base Health Regeneration Rate", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "health regeneration"), new AcceptableMinimum<float>()));
			_levelHPRegen = cfg.Bind("1. Character Vitality", "Leveled Health Regeneration Rate", 0.2f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "health regeneration"), new AcceptableMinimum<float>()));
			_baseArmor = cfg.Bind("1. Character Vitality", "Base Armor", 12f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "armor"), new AcceptableMinimum<float>()));
			_levelArmor = cfg.Bind("1. Character Vitality", "Leveled Armor", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "armor"), new AcceptableMinimum<float>()));
			_baseMaxShield = cfg.Bind("1. Character Vitality", "Base Maximum Shield", 0f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "maximum shield"), new AcceptableMinimum<float>()));
			_levelMaxShield = cfg.Bind("1. Character Vitality", "Leveled Maximum Shield", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "maximum shield"), new AcceptableMinimum<float>()));

			_baseMoveSpeed = cfg.Bind("2. Character Agility", "Base Movement Speed", 7f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "walk speed"), new AcceptableMinimum<float>(0f, false, 0.1f)));
			_levelMoveSpeed = cfg.Bind("2. Character Agility", "Leveled Movement Speed", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "walk speed"), new AcceptableMinimum<float>()));
			_baseAcceleration = cfg.Bind("2. Character Agility", "Acceleration Factor", 80f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "movement acceleration"), new AcceptableMinimum<float>()));
			_baseJumpCount = cfg.Bind("2. Character Agility", "Jump Count", 1, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "amount of jumps"), new AcceptableMinimum<int>()));
			_baseJumpPower = cfg.Bind("2. Character Agility", "Base Jump Power", 15f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "amount of upward jump force"), new AcceptableMinimum<float>()));
			_levelJumpPower = cfg.Bind("2. Character Agility", "Leveled Jump Power", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "amount of upward jump force"), new AcceptableMinimum<float>()));

			_basePrimaryDamage = cfg.Bind("3a. Character Primary", "Base Primary Damage", 1f, StaticDeclareConfigDescription("The character's Base Damage (see section 4) is multiplied by this value to determine the damage of a single void pearl (bullet). Since damage is dealt in two ticks, each tick doing half damage, this value is the total of both ticks combined.", new AcceptableMinimum<float>()));
			_primaryImpulseBulletCount = cfg.Bind("3a. Character Primary", "Bullets Per Impulse Shot", 3, StaticDeclareConfigDescription("When using Void Impulse as your primary, this is the amount of bullets fired per shot by default.", new AcceptableMinimum<int>(1)));
			_primaryImpulseSpread = cfg.Bind("3a. Character Primary", "Impulse Spread Factor", new Vector2(0, 1), StaticDeclareConfigDescription("The X component is the minimum spread, and the Y component is the maximum spread, of bullets shot with Void Impulse. Both are measured in degrees.", new AcceptableUserDefinedMinMax()));
			_primarySpreadArcDegrees = cfg.Bind("3a. Character Primary", "Void Spread Total Arc Length", 20f, StaticDeclareConfigDescription("When using Void Spread as your primary, this value, measured in degrees, represents the angle between each of the five bullets in the spread. This angle is divided upon them evenly.", new AcceptableValueRange<float>(0, 360f)));
			_primarySpreadBulletCount = cfg.Bind("3a. Character Primary", "Void Spread Bullets Per Shot", 5, StaticDeclareConfigDescription("Void Spread will fire this many projectiles in a horizontal fan. NOTE: It's a good idea for this value to be an odd number, so at least one bullet goes directly towards the crosshair.", new AcceptableMinimum<int>(1)));
			_primarySpreadShotSpread = cfg.Bind("3a. Character Primary", "Void Spread Spread Factor", new Vector2(0, 0.2f), StaticDeclareConfigDescription("The X component is the minimum spread, and the Y component is the maximum spread, of bullets shot with Void Spread. Both are measured in degrees.", new AcceptableUserDefinedMinMax()));
			_primaryImpulseShotTime = cfg.Bind("3a. Character Primary", "Impulse Shot Time", 0.3f, StaticDeclareConfigDescription("Void Impulse will try to fire all of its bullets in this amount of time.", new AcceptableValueRange<float>(0, 1)));
			_primaryUseExperimentalTripleshotBuff = cfg.Bind("3a. Character Primary", "Void Impulse Bulking", true, "Previously, Void Impulse would increase the number of bullets fired in a constant timespan (see Impulse Shot Time) as attack speed increased. If this is enabled, this behavior is changed, making it so that Void Impulse attempts to always fire in a set amount of *bursts* (for the amount, see Bullets Per Impulse Shot). The additional bullets gained from attack speed are instead evenly distributed among these bursts, making it more like a sequence of shotgun blasts at high attack speeds instead of a spray of bullets.");

			_baseSecondaryDamage = cfg.Bind("3b. Character Secondary", "Void Bomb Damage", 3f, StaticDeclareConfigDescription("The character's Base Damage (see section 4) is multiplied by this value to determine the damage of an individual Void Bomb.", new AcceptableMinimum<float>()));
			_secondaryRadius = cfg.Bind("3b. Character Secondary", "Void Bomb Radius", 12f, StaticDeclareConfigDescription("The radius of the area that Void Bombs can spawn in.", new AcceptableMinimum<float>(1f)));
			_secondaryCount = cfg.Bind("3b. Character Secondary", "Void Bomb Count", 6, StaticDeclareConfigDescription("The amount of Void Bombs spawned when using the secondary ability.", new AcceptableMinimum<int>(1)));
			_secondaryCooldown = cfg.Bind("3b. Character Secondary", "Void Bomb Cooldown", 5f, StaticDeclareConfigDescription("The amount of time, in seconds, that the player must wait before one stock of their secondary recharges.", new AcceptableMinimum<float>()));

			_utilitySpeed = cfg.Bind("3c. Character Utility", "Dive Speed Multiplier", 4f, StaticDeclareConfigDescription("The speed at which Dive moves you, as a multiplied factor of your current movement speed.", new AcceptableMinimum<float>()));
			_utilityRegen = cfg.Bind("3c. Character Utility", "Dive Regeneration", 0.10f, StaticDeclareConfigDescription("The percentage of health you regenerate when using Dive.", new AcceptableValueRange<float>(0, 1)));
			_utilityDuration = cfg.Bind("3c. Character Utility", "Dive Duration", 1f, StaticDeclareConfigDescription("The amount of time, in seconds, that Dive hides and moves the player for.", new AcceptableMinimum<float>()));

			_baseSpecialDamage = cfg.Bind("3d. Character Special", "Base Reave Damage", 60f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "special attack damage output") + " Note that this does not apply to the death effect. As such, this strictly affects the \"Reave\" ability.", new AcceptableMinimum<float>()));
			_reaveCooldown = cfg.Bind("3d. Character Special", "Reave Cooldown", 30f, StaticDeclareConfigDescription("The amount of time, in seconds, that the player must wait before one stock of their special recharges.", new AcceptableMinimum<float>()));
			_reaveHealthCostPercent = cfg.Bind("3d. Character Special", "Reave Cost", 0.5f, StaticDeclareConfigDescription("This is the health cost required to perform the \"Reave\" special. The actual manner in which this is used is determined by another setting in this category.", new AcceptableValueRange<float>(0f, 0.99f)));
			_reaveImmunity = cfg.Bind("3d. Character Special", "Reave Protection", true, "While performing the \"Reave\" special, and if this is true, you will not be able to take damage while locked in the animation.");
			_baseDeathDamage = cfg.Bind("3d. Character Special", "Base Collapse Damage", 750f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "death collapse damage output") + " NOTE: If you wish to cause an instant kill like normal Reavers, input any value greater than or equal to one million (thats six zeros c:).", new AcceptableMinimum<float>()));
			

			_baseDamage = cfg.Bind("4. Character Combat", "Base Damage", 16f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "base damage output") + " Other damage values are multiplied with this.", new AcceptableMinimum<float>()));
			_levelDamage = cfg.Bind("4. Character Combat", "Leveled Damage", 2.4f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "base damage output") + " Other damage values are multiplied with this.", new AcceptableMinimum<float>()));
			_baseCritChance = cfg.Bind("4. Character Combat", "Base Crit Chance", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_levelCritChance = cfg.Bind("4. Character Combat", "Leveled Crit Chance", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_baseAttackSpeed = cfg.Bind("4. Character Combat", "Base Attack Speed", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "attack rate"), new AcceptableMinimum<float>(0f, false, 0.1f)));
			_levelAttackSpeed = cfg.Bind("4. Character Combat", "Leveled Attack Speed", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "attack rate"), new AcceptableMinimum<float>()));

			
			// _useLegacyLunarBase = cfg.Bind("5. Void Reaver Specifics", "Use Legacy Lunar Mechanics", false, "If enabled, legacy abilities from when Lunar mechanics were used for the Primary and Utility slots will be implemented instead of their modern replacements.");
			_useFullSizeCharacter = cfg.Bind("5. Void Reaver Specifics", "(EXPERIMENTAL) Use Full Size Reaver", false, "By default, the mod sets the Reaver's scale to 50% that of its natural size. Turning this on will make you the same size as a normal Reaver. **EXPERIMENTAL WARNING** This setting has not been tested very much and there will be problems with world collisions (you might not be able to traverse the whole world), attacks and interactions, and more. This setting mostly exists for giggles.");
			// _collapseFriendlyFire = cfg.Bind("5. Void Reaver Specifics", "(EXPERIMENTAL) Collapse Harms Players", false, "If enabled, any form of a void collapse (Reave, Collapse/Death) can deal damage to and/or kill friendly players. This mimics the behavior of the Newly Hatched Zoea, where friendly void enemies will still kill players caught in their death implosion. **EXPERIMENTAL WARNING** This may be inconsistent when using Reave.");
			_voidImmunity = cfg.Bind("5. Void Reaver Specifics", "Void Immunity", true, "If enabled, the player will be immune to damage from a void atmosphere and will not have the fog effect applied to them. **EXPERIMENTAL WARNING** There isn't actually a way to tell if you are taking damage from the void. The way I do it is an educated guess. This means you may actually resist completely valid damage types from some enemies, but I have yet to properly test this.");

			_useNewIcons = cfg.Bind("6. Other Options", "Use New Icons", true, "If true, this abandons the old pixel art icons created by LuaFubuki, and replaces them with renders that I made.");
		}

		#endregion
	}
}
