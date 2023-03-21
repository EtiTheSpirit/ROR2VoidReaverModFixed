using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VoidReaverMod.Initialization.Sprites;
using VoidReaverMod.Survivor;
using Xan.ROR2VoidPlayerCharacterCommon;
using Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs;
using Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs.Networked;
using Xan.ROR2VoidPlayerCharacterCommon.ROOInterop;
using static Xan.ROR2VoidPlayerCharacterCommon.AdvancedConfigs.CommonModAndCharacterConfigs;

namespace VoidReaverMod.Initialization {

	/// <summary>
	/// A universal hub for referencing all configuration values quickly and easily.
	/// </summary>
	public static class Configuration {

		private static ConfigFile _cfg = null;

		private static AdvancedConfigBuilder _advCfg = null;

		#region Configuration Entries

		#region Public Properties

		/// <summary>
		/// All basic stats, and some configs for the void enemy. If a setting seems to be missing, it's probably just in here.
		/// </summary>
		[ReplicatedConfiguration]
		public static CommonModAndCharacterConfigs CommonVoidEnemyConfigs { get; private set; }

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
		public static ReplicatedMinMaxWrapper PrimaryImpulseSpread => _primaryImpulseSpread;

		/// <summary>
		/// The amount of time that Void Impulse tries to fire all projectiles in.
		/// </summary>
		public static float PrimaryImpulseShotTime => _primaryImpulseShotTime.Value;

		/// <summary>
		/// The amount of bursts fired when Void Impulse is used.
		/// </summary>
		public static int BulletsPerImpulseShot => _primaryImpulseBulletCount.Value;

		/// <summary>
		/// The cooldown of Void Impulse
		/// </summary>
		public static float PrimaryImpulseCooldown => _primaryImpulseCooldown.Value;
		#endregion

		#region Void Spread
		/// <summary>
		/// The amount of bullets that will be in the fan of shots
		/// </summary>
		public static int BulletsPerSpreadShot => _primarySpreadBulletCount.Value;

		/// <summary>
		/// The min and max deviation for each projectile in the spread shot.
		/// </summary>
		public static ReplicatedMinMaxWrapper PrimarySpreadShotSpread => _primarySpreadShotSpread;

		/// <summary>
		/// The width of the arc that shots are evenly distributed within.
		/// </summary>
		public static float SpreadShotArcLengthDegs => _primarySpreadArcDegrees.Value;

		/// <summary>
		/// The cooldown of Void Spread
		/// </summary>
		public static float PrimarySpreadCooldown => _primarySpreadCooldown.Value;
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

		/// <summary>
		/// The duration of Nullify when used on monsters.
		/// </summary>
		public static float NullifyDurationMonsters => _secondaryNullifyDuration.Value;
		
		/// <summary>
		/// The duration of Nullify when used on bosses.
		/// </summary>
		public static float NullifyDurationBosses => _secondaryNullifyDurationBoss.Value;

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

		/// <summary>
		/// The amount of time the player has to wait to recharge 1 stock of Dive.
		/// </summary>
		public static float UtilityCooldown => _utilityCooldown.Value;
		#endregion

		#region Special

		/// <summary>
		/// The amount of armor subtracted from a character's stats when they have Void Rift Shock
		/// </summary>
		public static float DetainWeaknessArmorReduction => _detainWeaknessArmorReductionConstant.Value;

		#region Detain

		/// <summary>
		/// The cooldown of Detain in seconds.
		/// </summary>
		public static float SpecialCooldown => _detainCooldown.Value;

		/// <summary>
		/// The amount of health the player must spend to perform Detain.
		/// </summary>
		public static float DetainCost => _detainHealthCostPercent.Value;

		/// <summary>
		/// If true, the player cannot take damage whilst performing their Detain special.
		/// </summary>
		public static bool DetainImmunity => _detainImmunity.Value;

		/// <summary>
		/// If larger than zero, the player is inflicted with negative armor for a brief time after using Detain.
		/// </summary>
		public static float DetainWeaknessDuration => _detainWeaknessDuration.Value;

		/// <summary>
		/// The damage dealt by Detain.
		/// </summary>
		public static float DetainDamage => _detainDamage.Value;
		#endregion

		/// <summary>
		/// If true, the configuration changes notice on the survivor screen are hidden.
		/// </summary>
		public static bool HideNotice => _hideNotice.Value;

		#endregion

		#endregion

		#region Backing Settings Objects

		#region Primary Attack
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _basePrimaryDamage;
		[ReplicatedConfiguration]
		private static ReplicatedMinMaxWrapper _primaryImpulseSpread;
		[ReplicatedConfiguration]
		private static ReplicatedMinMaxWrapper _primarySpreadShotSpread;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _primaryImpulseShotTime;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _primaryImpulseBulletCount;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _primarySpreadBulletCount;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _primarySpreadArcDegrees;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<bool> _primaryUseExperimentalTripleshotBuff;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _primaryImpulseCooldown;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _primarySpreadCooldown;
		#endregion

		#region Secondary Attack
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _baseSecondaryDamage;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryRadius;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<int> _secondaryCount;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryCooldown;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryNullifyDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _secondaryNullifyDurationBoss;
		#endregion

		#region Utility
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _utilitySpeed;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _utilityRegen;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _utilityDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _utilityCooldown;
		#endregion

		#region Special
		#region Detain
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _detainCooldown;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _detainHealthCostPercent;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<bool> _detainImmunity;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _detainWeaknessDuration;
		[ReplicatedConfiguration]
		private static ReplicatedConfigEntry<float> _detainWeaknessArmorReductionConstant;
		[ReplicatedConfiguration]
		private static ReplicatedPercentageWrapper _detainDamage;
		#endregion

		#endregion

		private static ConfigEntry<bool> _hideNotice;

		#endregion

		#endregion

		#region Backing Code

		private const string FMT_DEFAULT = "The base {0} that the character has on a new run.";
		private const string FMT_LEVELED = "For each level the player earns, the base {0} increases by this amount.";
		private const string FMT_TRANSPARENCY = "The transparency of the character when you are {0}.\n\nThis can be used to make it easier to see enemies by making your body transparent to prevent it from getting in the way.\n\nA value of 0 means fully opaque, and a value of 100 means as invisible as possible.";

		public static void Initialize(ConfigFile cfg) {
			if (_cfg != null) throw new InvalidOperationException($"{nameof(Configuration)} has already been initialized!");
			_cfg = cfg;

			AdvancedConfigBuilder aCfg = new AdvancedConfigBuilder(typeof(Configuration), cfg, Images.Portrait, VoidReaverPlayerPlugin.PLUGIN_GUID, VoidReaverPlayerPlugin.DISPLAY_NAME, "Play as a Void Reaver!\n\nThese settings include all stats for every single individual component of every ability. Handle with care!");
			CommonVoidEnemyConfigs = new CommonModAndCharacterConfigs(aCfg, new CommonModAndCharacterConfigs.Defaults {
				BaseMaxHealth = 220f,
				LevelMaxHealth = 40f,
				BaseHPRegen = 1f,
				LevelHPRegen = 0.2f,
				BaseArmor = 12f,
				LevelArmor = 0f,
				BaseMaxShield = 0f,
				LevelMaxShield = 0f,
				BaseMoveSpeed = 7f,
				LevelMoveSpeed = 0f,
				SprintSpeedMultiplier = 1.45f,
				BaseAcceleration = 80f,
				BaseJumpCount = 1,
				BaseJumpPower = 20f,
				LevelJumpPower = 0f,
				BaseAttackSpeed = 1f,
				LevelAttackSpeed = 0f,
				BaseDamage = 20f,
				LevelDamage = 2.4f,
				BaseCritChance = 1f,
				LevelCritChance = 0f,
				UseFullSizeCharacter = false,
				TransparencyInCombat = 0,
				TransparencyOutOfCombat = 0,
				CameraPivotOffset = -1.25f,
				CameraOffset = new Vector3(0f, 3.5f, -14f)
			});

			aCfg.SetCategory("Primary Skills");
			_basePrimaryDamage = aCfg.BindFloatPercentageReplicated("Base Primary Damage", "This value is what amount of base damage is applied to the primary skills' projectiles.", 100, 0, 10000, restartRequired: AdvancedConfigBuilder.RestartType.NoRestartRequired);

			aCfg.SetCategory("Void Impulse");
			_primaryImpulseBulletCount = aCfg.BindReplicated("Impulse Bullet Count", "The amount of bullets that are fired per impulse shot at 1x attack speed.", 3, 1, 30, restartRequired: AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_primaryImpulseSpread = aCfg.BindMinMaxReplicated("Impulse Bullet Deviation", "<style=cIsUtility>Measured in degrees</style>, this represents the bullet spread angle that applies to individual projectiles. A value of 90 means bullets could shoot perfectly left or right.", 0f, 1f, 0f, 90f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}°");
			_primaryImpulseShotTime = aCfg.BindReplicated("Impulse Shot Time", "This is how long it takes for all the bullets fired by Void Impulse to actually be fired. The shots are divided evenly into this timeframe.", 0.3f, 0f, 5f, restartRequired: AdvancedConfigBuilder.RestartType.NoRestartRequired, formatString: "{0}s");
			_primaryUseExperimentalTripleshotBuff = aCfg.BindReplicated("Impulse Burstfire", "If true, Void Impulse will bulk the shots together so that it always fires <style=cUserSetting>(Impulse Bullet Count)</style> <i>bursts</i> - if your attack speed is 2, each of the bursts will fire 2 bullets per shot, so you still get 2x bullet count.\n\nDisabling this reverts to older behavior where the timing window is shrunken instead, making it more like spewing out a bunch of bullets really quickly instead of a burstfire. This method is prone to being soft-limited at very high attack speeds, though.", true, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_primaryImpulseCooldown = aCfg.BindReplicated("Impulse Cooldown", "The amount of time between Impulse shots.", 1f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");

			aCfg.SetCategory("Void Spread");
			_primarySpreadArcDegrees = aCfg.BindReplicated("Spread Arc Length", "Void Spread fires all of its shots together. The path of these bullets is evenly spaced at an angle. This value represents the angle between each bullet (thus, adding more bullets will make a wider spread unless this is reduced accordingly).", 20f, 0f, 90f, 1f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}°");
			_primarySpreadBulletCount = aCfg.BindReplicated("Spread Bullets Per Shot", "The amount of bullets fired at once by Void Spread.", 5, 1, 30);
			_primarySpreadShotSpread = aCfg.BindMinMaxReplicated("Spread Bullet Deviation", "<style=cIsUtility>Measured in degrees</style>, this represents the bullet spread angle. A value of 90 means the first (left-most) bullet shoots straight left, and the last (right-most) bullet shoots straight right.", 0f, 0.2f, 0f, 90f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}°");
			_primarySpreadCooldown = aCfg.BindReplicated("Spread Cooldown", "The amount of time between Spread shots.", 1.2f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");

			aCfg.SetCategory("Undertow");
			_baseSecondaryDamage = aCfg.BindFloatPercentageReplicated("Void Bomb Damage", "This value is what amount of base damage is applied to the secondary skill's bombs.", 300, 0, 1000, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_secondaryRadius = aCfg.BindReplicated("Void Bomb Placement Radius", "This value is the radius of the area that the bombs spawn into when placing them.", 12f, 1f, 30f, 0.5f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}m");
			_secondaryCount = aCfg.BindReplicated("Void Bomb Count", "The amount of bombs that are placed when using the secondary.", 6, 1, 30, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_secondaryCooldown = aCfg.BindReplicated("Undertow Cooldown", "The cooldown applied to Undertow by default.", 6f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");
			_secondaryNullifyDuration = aCfg.BindReplicated("Undertow Nullify Duration", "The duration of Nullify when applied to standard monsters, not including bosses.", 10f, 1f, 30f, 0.5f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}s");
			_secondaryNullifyDurationBoss = aCfg.BindReplicated("Undertow Nullify Duration (Boss)", "The duration of Nullify when applied to bosses only.", 5f, 1f, 30f, 0.5f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}s");

			aCfg.SetCategory("Dive");
			_utilitySpeed = aCfg.BindFloatPercentageReplicated("Dive Speed Multiplier", "The speed at which Dive moves you, as a multiplier factor of your current movement speed.", 400f, 0, 1000f, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_utilityRegen = aCfg.BindFloatPercentageReplicated("Dive Regeneration", "The percentage of health that is regenerated upon using Dive", 15f, restartRequired: AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_utilityDuration = aCfg.BindReplicated("Dive Duration", "The amount of time, in seconds, that Dive hides and moves the player for.", 1f, 0.5f, 5f, 0.5f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}s");
			_utilityCooldown = aCfg.BindReplicated("Dive Cooldown", "The amount of time, in seconds, that the player must wait before one stock of Dive recharges.", 6f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");

			aCfg.SetCategory("Special Skills");
			// No damage here. This uses the common API!
			_detainCooldown = aCfg.BindReplicated("Detain Cooldown", "The amount of time, in seconds, that the player must wait before one stock of Detain recharges.", 30f, 0.5f, 120f, 0.5f, AdvancedConfigBuilder.RestartType.NextRespawn, "{0}s");
			_detainHealthCostPercent = aCfg.BindFloatPercentageReplicated("Detain Health Cost", "The amount of health that is taken upon using Detain.", 50f, restartRequired: AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_detainImmunity = aCfg.BindReplicated("Detain Protection", "While performing the Detain special, the player will not be able to take damage during the animation if this is true.", true, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_detainWeaknessDuration = aCfg.BindReplicated("Detain Weakness Duration", "After performing the Detain special, the player can optionally be inflicted by a status effect (<style=cIsVoid>Void Rift Shock</style>) that reduces their armor for this duration of time. Setting this to 0 will disable the effect.", 10f, 0f, 60f, 1f, AdvancedConfigBuilder.RestartType.NoRestartRequired, "{0}s");
			_detainWeaknessArmorReductionConstant = aCfg.BindReplicated("Detain Weakness Armor Reduction", "How much armor is removed from the player when afflicted by Void Rift Shock.", 100f, 0f, 1000f, 10f, AdvancedConfigBuilder.RestartType.NoRestartRequired);
			_detainDamage = aCfg.BindFloatPercentageReplicated("Detain Damage", "How much damage Detain does, as a percentage of base damage.", 10000, 0, 100000);

			aCfg.SetCategory("Mod Meta, Graphics, Gameplay");
			_hideNotice = aCfg.BindLocal("Hide Config Notice", "Stops showing the warning on the stats screen that (probably) directed you here in the first place. Changes when you click on a different survivor in the pre-game screen.", false, AdvancedConfigBuilder.RestartType.NoRestartRequired);

			Log.LogInfo("Registering all configs for network synchronization...");
			aCfg.CreateConfigAutoReplicator();

			Log.LogInfo("Registering change hooks...");

			CommonVoidEnemyConfigs.OnStatConfigChanged += () => OnStatConfigChanged?.Invoke();
			_basePrimaryDamage.SettingChanged += OnFloatChanged;
			_primaryImpulseBulletCount.SettingChanged += OnReplicatedIntChanged;
			_primaryImpulseSpread.SettingChanged += OnMinMaxChanged;
			_primaryImpulseShotTime.SettingChanged += OnReplicatedFloatChanged;
			_primaryUseExperimentalTripleshotBuff.SettingChanged += OnReplicatedBoolChanged;
			_primaryImpulseCooldown.SettingChanged += OnReplicatedFloatChanged;
			_primarySpreadArcDegrees.SettingChanged += OnReplicatedFloatChanged;
			_primarySpreadBulletCount.SettingChanged += OnReplicatedIntChanged;
			_primarySpreadShotSpread.SettingChanged += OnMinMaxChanged;
			_primarySpreadCooldown.SettingChanged += OnReplicatedFloatChanged;
			_baseSecondaryDamage.SettingChanged += OnFloatChanged;
			_secondaryRadius.SettingChanged += OnReplicatedFloatChanged;
			_secondaryCount.SettingChanged += OnReplicatedIntChanged;
			_secondaryCooldown.SettingChanged += OnReplicatedFloatChanged;
			_secondaryNullifyDuration.SettingChanged += OnReplicatedFloatChanged;
			_secondaryNullifyDurationBoss.SettingChanged += OnReplicatedFloatChanged;
			_utilitySpeed.SettingChanged += OnFloatChanged;
			_utilityRegen.SettingChanged += OnFloatChanged;
			_utilityDuration.SettingChanged += OnReplicatedFloatChanged;
			_utilityCooldown.SettingChanged += OnReplicatedFloatChanged;
			_detainCooldown.SettingChanged += OnReplicatedFloatChanged;
			_detainHealthCostPercent.SettingChanged += OnFloatChanged;
			_detainImmunity.SettingChanged += OnReplicatedBoolChanged;
			_detainWeaknessDuration.SettingChanged += OnReplicatedFloatChanged;
			_detainWeaknessArmorReductionConstant.SettingChanged += OnReplicatedFloatChanged;
			_hideNotice.SettingChanged += (_, _) => OnHideNoticeChanged?.Invoke(_hideNotice.Value);

			_advCfg = aCfg;
			Log.LogInfo("User configs initialized.");
		}

		internal static void LateInit(BaseUnityPlugin registrar, BodyIndex bodyIndex) {
			XanVoidAPI.CreateAndRegisterBlackHoleBehaviorConfigs(registrar, _advCfg, bodyIndex);
		}
#pragma warning restore CS0618

		private static void OnAnyChanged() => OnStatConfigChanged?.Invoke();
		private static void OnBoolChanged(bool value) => OnStatConfigChanged?.Invoke();
		private static void OnFloatChanged(float value) => OnStatConfigChanged?.Invoke();
		private static void OnIntChanged(float value) => OnStatConfigChanged?.Invoke();
		private static void OnMinMaxChanged(float min, float max) => OnStatConfigChanged?.Invoke();
		private static void OnVectorChanged(Vector3 vector) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedBoolChanged(bool value, bool fromHost) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedFloatChanged(float value, bool fromHost) => OnStatConfigChanged?.Invoke();
		private static void OnReplicatedIntChanged(int value, bool fromHost) => OnStatConfigChanged?.Invoke();

		/// <summary>
		/// Fires when any config that pertains to stats changes.
		/// </summary>
		public static event StatConfigChanged OnStatConfigChanged;

		/// <summary>
		/// This event fires when the desire to hide the notice changes.
		/// </summary>
		public static event Action<bool> OnHideNoticeChanged;


		#endregion
	}
}
