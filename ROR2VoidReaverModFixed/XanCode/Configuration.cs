// #define USE_CUSTOM_MINMAX

using BepInEx.Configuration;
using FubukiMods.Modules;
using ROR2VoidReaverModFixed.XanCode.ConfigurationUtil;
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

		#region Entries

		#region Public Properties

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

		public static float BasePrimaryDamage => _basePrimaryDamage.Value;
		//public static float LevelPrimaryDamage => _levelPrimaryDamage.Value;
		public static Vector2 PrimaryImpulseSpread => _primaryImpulseSpread.Value;
		public static Vector2 PrimarySpreadShotSpread => _primarySpreadShotSpread.Value;
		public static float PrimaryImpulseShotTime => _primaryImpulseShotTime.Value;
		public static int BulletsPerImpulseShot => _primaryImpulseBulletCount.Value;
		public static float SpreadShotArcLengthDegs => _primarySpreadArcDegrees.Value;

		public static float BaseSecondaryDamage => _baseSecondaryDamage.Value;
		public static float SecondarySpread => _secondaryRadius.Value;
		public static int SecondaryCount => _secondaryCount.Value;
		//public static float LevelSecondaryDamage => _levelSecondaryDamage.Value;
		public static float SecondaryCooldown => _secondaryCooldown.Value;

		public static float BaseSpecialDamage => _baseSpecialDamage.Value;
		//public static float LevelSpecialDamage => _levelSpecialDamage.Value;
		public static float SpecialCooldown => _specialCooldown.Value;

		public static float BaseDeathDamage => _baseDeathDamage.Value;

		/// <summary>
		/// Not a configurable setting. This is identical to <c><see cref="BaseDeathDamage"/> &gt;= <see cref="Skills.VoidDeath.VOID_DELETION_THRESHOLD"/></c>
		/// </summary>
		public static bool IsVoidDeathInstakill => BaseDeathDamage >= Skills.VoidDeath.VOID_DELETION_THRESHOLD;
		//public static float LevelDeathDamage => _levelDeathDamage.Value;

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

		public static float ReaveCost => _reaveHealthCostPercent.Value;
		public static bool ReaveImmunity => _reaveImmunity.Value;
		public static bool UseLegacyLunarMechanics => false;//_useLegacyLunarBase.Value;

		public static bool UseExperimentalSequenceShotBuff => _primaryUseExperimentalTripleshotBuff.Value;

		public static bool UseFullSizeCharacter => _useFullSizeCharacter.Value;

		#endregion

		#region Backing Settings
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

		private static ConfigEntry<float> _basePrimaryDamage;
		//private static ConfigEntry<float> _levelPrimaryDamage;
		private static ConfigEntry<Vector2> _primaryImpulseSpread;
		private static ConfigEntry<Vector2> _primarySpreadShotSpread;
		private static ConfigEntry<float> _primaryImpulseShotTime;


		private static ConfigEntry<float> _baseSecondaryDamage;
		private static ConfigEntry<float> _secondaryRadius;
		private static ConfigEntry<int> _secondaryCount;
		//private static ConfigEntry<float> _levelSecondaryDamage;
		private static ConfigEntry<float> _secondaryCooldown;

		private static ConfigEntry<float> _baseSpecialDamage;
		//private static ConfigEntry<float> _levelSpecialDamage;
		private static ConfigEntry<float> _specialCooldown;

		private static ConfigEntry<float> _baseDeathDamage;
		//private static ConfigEntry<float> _levelDeathDamage;

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

		private static ConfigEntry<float> _reaveHealthCostPercent;
		private static ConfigEntry<bool> _reaveImmunity;
		//private static ConfigEntry<bool> _reaveCostIsAbsolute;
		private static ConfigEntry<int> _primaryImpulseBulletCount;
		private static ConfigEntry<float> _primarySpreadArcDegrees;
		private static ConfigEntry<bool> _useLegacyLunarBase;
		private static ConfigEntry<bool> _primaryUseExperimentalTripleshotBuff;
		private static ConfigEntry<bool> _useFullSizeCharacter;
		#endregion

		#endregion

		// lmao
		private static T DefVal<T>(this ConfigEntry<T> cfg) where T : struct => (T)cfg.DefaultValue;

		private const string FMT_DEFAULT = "The base {0} that the character has on a new run.";
		private const string FMT_LEVELED = "For each level the player earns, the base {0} increases by this amount.";

		public static ConfigDescription StaticDeclareConfigDescription(string desc, AcceptableValueBase limit = null) {
#if USE_CUSTOM_MINMAX
			return new ConfigDescription(desc, limit);
#else
			if (limit is not AcceptableMinimum && limit is not AcceptableUserDefinedMinMax) {
				return new ConfigDescription(desc, limit);
			} else {
				return new ConfigDescription(desc);
			}
#endif
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

			_basePrimaryDamage = cfg.Bind("3a. Character Primary", "Base Primary Damage", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "primary attack damage output") + " Since damage is dealt in two ticks, each tick doing half damage, this value is the total of both ticks combined.", new AcceptableMinimum<float>()));
			_primaryImpulseBulletCount = cfg.Bind("3a. Character Primary", "Bullets Per Impulse Shot", 3, StaticDeclareConfigDescription("When using Void Impulse as your primary, this is the amount of bullets fired per shot by default.", new AcceptableMinimum<int>(1)));
			_primarySpreadArcDegrees = cfg.Bind("3a. Character Primary", "Spread Shot Arc Length", 8f, "When using Void Spread as your primary, this value, measured in degrees, represents the angle between each of the five bullets in the spread individually.");
			_primaryImpulseSpread = cfg.Bind("3a. Character Primary", "Impulse Spread Factor", new Vector2(0, 1), StaticDeclareConfigDescription("The X component is the minimum spread, and the Y component is the maximum spread, of bullets shot with Void Impulse. Both are measured in degrees.", new AcceptableUserDefinedMinMax()));
			_primarySpreadShotSpread = cfg.Bind("3a. Character Primary", "Void Spread Spread Factor", new Vector2(0, 0.2f), StaticDeclareConfigDescription("The X component is the minimum spread, and the Y component is the maximum spread, of bullets shot with Void Spread. Both are measured in degrees.", new AcceptableUserDefinedMinMax()));
			_primaryImpulseShotTime = cfg.Bind("3a. Character Primary", "Impulse Shot Time", 0.3f, StaticDeclareConfigDescription("Void Impulse will try to fire all of its bullets in this amount of time.", new AcceptableValueRange<float>(0, 1)));
			_primaryUseExperimentalTripleshotBuff = cfg.Bind("3a. Character Primary", "Void Impulse Bulking", true, "Previously, Void Impulse would increase the number of bullets fired in a constant timespan (Impulse Shot Time) as attack speed increased. If this is enabled, this behavior is altered so that it prefers shooting the defined burst count (Bullets Per Impulse Shot), adding more bullets to each individual shot so that it's more like a sequence of shotgun bursts.");
			//_levelPrimaryDamage = cfg.Bind("3a. Character Primary", "Leveled Primary Damage", 2f, string.Format(FMT_LEVELED, "primary attack damage output") + " Since damage is dealt in two ticks, each tick doing half damage, this value is the total of both ticks combined.");

			_baseSecondaryDamage = cfg.Bind("3b. Character Secondary", "Void Bomb Damage", 3f, StaticDeclareConfigDescription("The character's Base Damage (see section 4) is multiplied by this value to determine the damage of an individual Void Bomb.", new AcceptableMinimum<float>()));
			_secondaryRadius = cfg.Bind("3b. Character Secondary", "Void Bomb Radius", 12f, StaticDeclareConfigDescription("The radius of the area that Void Bombs can spawn in.", new AcceptableMinimum<float>(1f)));
			_secondaryCount = cfg.Bind("3b. Character Secondary", "Void Bomb Count", 6, StaticDeclareConfigDescription("The amount of Void Bombs spawned when using the secondary ability.", new AcceptableMinimum<int>(1)));
			//_levelSecondaryDamage = cfg.Bind("3b. Character Secondary", "Leveled Secondary Damage", 0.125f, string.Format(FMT_LEVELED, "secondary attack damage output"));
			_secondaryCooldown = cfg.Bind("3b. Character Secondary", "Secondary Cooldown", 5f, StaticDeclareConfigDescription("The amount of time, in seconds, that the player must wait before one stock of their secondary recharges.", new AcceptableMinimum<float>()));
			
			_baseSpecialDamage = cfg.Bind("3c. Character Special", "Base Special Damage", 100f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "special attack damage output") + " Note that this does not apply to the death effect. As such, this strictly affects the \"Reave\" ability.", new AcceptableMinimum<float>()));
			//_levelSpecialDamage = cfg.Bind("3c. Character Special", "Leveled Special Damage", 1.5f, string.Format(FMT_LEVELED, "special attack damage output") + " Note that this does not apply to the death effect. As such, this strictly affects the \"Reave\" ability.");
			_specialCooldown = cfg.Bind("3c. Character Special", "Special Cooldown", 30f, StaticDeclareConfigDescription("The amount of time, in seconds, that the player must wait before one stock of their special recharges.", new AcceptableMinimum<float>()));
			
			_baseDeathDamage = cfg.Bind("3d. Character Death", "Base Death Damage", 750f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "death collapse damage output") + " NOTE: If you wish to cause an instant kill like normal Reavers, input any value greater than or equal to one million (thats six zeros c:).", new AcceptableMinimum<float>()));
			//_levelDeathDamage = cfg.Bind("3d. Character Death", "Leveled Death Damage", 5000f, string.Format(FMT_LEVELED, "death collapse damage output"));

			_baseDamage = cfg.Bind("4. Character Combat", "Base Damage", 16f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "base damage output") + " Other damage values are multiplied with this.", new AcceptableMinimum<float>()));
			_levelDamage = cfg.Bind("4. Character Combat", "Leveled Damage", 2.4f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "base damage output") + " Other damage values are multiplied with this.", new AcceptableMinimum<float>()));
			_baseCritChance = cfg.Bind("4. Character Combat", "Base Crit Chance", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_levelCritChance = cfg.Bind("4. Character Combat", "Leveled Crit Chance", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "critical hit chance") + " This is an integer percentage from 0 to 100, not 0 to 1.", new AcceptableValueRange<float>(0, 100)));
			_baseAttackSpeed = cfg.Bind("4. Character Combat", "Base Attack Speed", 1f, StaticDeclareConfigDescription(string.Format(FMT_DEFAULT, "attack rate"), new AcceptableMinimum<float>(0f, false, 0.1f)));
			_levelAttackSpeed = cfg.Bind("4. Character Combat", "Leveled Attack Speed", 0f, StaticDeclareConfigDescription(string.Format(FMT_LEVELED, "attack rate"), new AcceptableMinimum<float>()));

			_reaveImmunity = cfg.Bind("5. Void Reaver Specifics", "Reave Protection", true, "While performing the \"Reave\" special, and if this is true, you will not be able to take damage while locked in the animation.");
			_reaveHealthCostPercent = cfg.Bind("5. Void Reaver Specifics", "Reave Cost", 0.5f, StaticDeclareConfigDescription("This is the health cost required to perform the \"Reave\" special. The actual manner in which this is used is determined by another setting in this category.", new AcceptableValueRange<float>(0f, 0.99f)));
			// _useLegacyLunarBase = cfg.Bind("5. Void Reaver Specifics", "Use Legacy Lunar Mechanics", false, "If enabled, legacy abilities from when Lunar mechanics were used for the Primary and Utility slots will be implemented instead of their modern replacements.");
			_useFullSizeCharacter = cfg.Bind("5. Void Reaver Specifics", "(EXPERIMENTAL) Use Full Size Reaver", false, "By default, the mod sets the Reaver's scale to 75% that of its natural size. Turning this on will make you the same size as a normal Reaver, which may not look very good when you try to get through some parts of the world.");

		}
	}
}
