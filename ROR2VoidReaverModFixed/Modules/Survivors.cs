using EntityStates;
using KinematicCharacterController;
using R2API;
using R2API.Utils;
using RoR2;
using ROR2VoidReaverModFixed.XanCode;
using ROR2VoidReaverModFixed.XanCode.Image;
using UnityEngine;
using UnityEngine.Networking;
using XanVoidReaverEdit;
using GhostUtilitySkillState = On.EntityStates.GhostUtilitySkillState;
using LunarPrimaryReplacementSkill = On.RoR2.Skills.LunarPrimaryReplacementSkill;
using SerializableEntityStateType = EntityStates.SerializableEntityStateType;
using SkillDef = RoR2.Skills.SkillDef;

namespace FubukiMods.Modules
{

	/// <summary>
	/// The survivor main module. This was originally written by LuaFubuki but was rewritten and refactored by Xan.
	/// </summary>
	public class Survivors {
		public static void Init() {
			GameObject playerBodyPrefab = Tools.CreateBody("PlayerNullifierBody", "RoR2/Base/Nullifier/NullifierBody.prefab");
			GameObject playerBodyLocator = PrefabAPI.InstantiateClone(playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerNullifierBodyDisplay");
			playerBodyLocator.AddComponent<NetworkIdentity>();
			CharacterBody body = playerBodyPrefab.GetComponent<CharacterBody>();

			Interactor interactor = playerBodyPrefab.GetComponent<Interactor>();

			if (Configuration.UseFullSizeCharacter) {
				interactor.maxInteractionDistance = 11f;
				ModelLocator locator = playerBodyPrefab.GetComponent<ModelLocator>();
				locator.onModelChanged += OnPlayerReaverModelChanged;
			} else {
				// We ARE NOT using full size character. Downscale.
				GameObject baseTransform = playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
				baseTransform.transform.localScale = Vector3.one * 0.5f;
				baseTransform.transform.Translate(new Vector3(0f, 4f, 0f));
				foreach (KinematicCharacterMotor kinematicCharacterMotor in playerBodyPrefab.GetComponentsInChildren<KinematicCharacterMotor>()) {
					// * 0.4f
					// + 1.25f
					// Up to *0.5f and +1.5f
					// This seems to fix the bungus issue!
					kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.5f, kinematicCharacterMotor.Capsule.height * 0.5f, 1.5f);
				}
				interactor.maxInteractionDistance = 5f;
			}
			CameraTargetParams camTargetParams = playerBodyPrefab.GetComponent<CameraTargetParams>();
			CharacterCameraParams characterCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
			camTargetParams.cameraParams = characterCameraParams;
			if (!Configuration.UseFullSizeCharacter) {
				characterCameraParams.data.idealLocalCameraPos = new Vector3(0f, 2f, -12f);
				characterCameraParams.data.pivotVerticalOffset = 0f;
			} else {
				characterCameraParams.data.idealLocalCameraPos = new Vector3(0f, 4f, -16f);
				characterCameraParams.data.pivotVerticalOffset = 0f;
			}

			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			CharacterDeathBehavior deathBehavior = playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = new SerializableEntityStateType(typeof(Skills.VoidDeath));
			
			body.aimOriginTransform.Translate(new Vector3(0f, 0f, 0f));
			body.bodyColor = new Color(0.867f, 0.468f, 0.776f);
			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void | CharacterBody.BodyFlags.ImmuneToVoidDeath;
			body.baseDamage = Configuration.BaseDamage;
			body.levelDamage = Configuration.LevelDamage;
			body.baseCrit = Configuration.BaseCritChance;
			body.levelCrit = Configuration.LevelCritChance;
			body.baseMaxHealth = Configuration.BaseMaxHealth;
			body.levelMaxHealth = Configuration.LevelMaxHealth;
			body.baseMaxShield = Configuration.BaseMaxShield;
			body.levelMaxShield = Configuration.LevelMaxShield;
			body.baseArmor = Configuration.BaseArmor;
			body.levelArmor = Configuration.LevelArmor;
			body.baseRegen = Configuration.BaseHPRegen;
			body.levelRegen = Configuration.LevelHPRegen;
			body.baseMoveSpeed = Configuration.BaseMoveSpeed;
			body.levelMoveSpeed = Configuration.LevelMoveSpeed;
			body.baseAcceleration = Configuration.BaseAcceleration;
			body.baseJumpCount = Configuration.BaseJumpCount;
			body.baseJumpPower = Configuration.BaseJumpPower;
			body.baseAttackSpeed = Configuration.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.LevelAttackSpeed;
			body.baseNameToken = Lang.SURVIVOR_NAME;
			body.portraitIcon = CommonImages.Portrait.texture;
			ContentAddition.AddBody(playerBodyPrefab);

			// Passive stuff:
			SkillLocator skillLoc = playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Lang.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Lang.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Lang.PASSIVE_DESC;
			skillLoc.passiveSkill.icon = CommonImages.Passive;

			// Primary, triple shot:
			ContentAddition.AddEntityState<Skills.VoidPrimarySequenceShot>(out _);
			SkillDef primaryTriple = ScriptableObject.CreateInstance<SkillDef>();
			primaryTriple.activationState = new SerializableEntityStateType(typeof(Skills.VoidPrimarySequenceShot));
			primaryTriple.activationStateMachineName = "Weapon";
			primaryTriple.baseMaxStock = 1;
			primaryTriple.baseRechargeInterval = 1f;
			primaryTriple.beginSkillCooldownOnSkillEnd = true;
			primaryTriple.canceledFromSprinting = false;
			primaryTriple.cancelSprintingOnActivation = true;
			primaryTriple.dontAllowPastMaxStocks = false;
			primaryTriple.forceSprintDuringState = false;
			primaryTriple.fullRestockOnAssign = true;
			primaryTriple.interruptPriority = 0;
			primaryTriple.isCombatSkill = true;
			primaryTriple.mustKeyPress = false;
			primaryTriple.rechargeStock = primaryTriple.baseMaxStock;
			primaryTriple.requiredStock = 1;
			primaryTriple.stockToConsume = 0;
			primaryTriple.skillNameToken = Lang.SKILL_PRIMARY_TRIPLESHOT_NAME;
			primaryTriple.skillDescriptionToken = Lang.SKILL_PRIMARY_TRIPLESHOT_DESC;
			primaryTriple.icon = CommonImages.PrimaryTripleShotIcon;
			Tools.AddSkill(playerBodyPrefab, primaryTriple, "primary", 0);

			// Primary, spread shot:
			ContentAddition.AddEntityState<Skills.VoidPrimaryFiveSpread>(out _);
			SkillDef primarySpread = ScriptableObject.CreateInstance<SkillDef>();
			primarySpread.activationState = new SerializableEntityStateType(typeof(Skills.VoidPrimaryFiveSpread));
			primarySpread.activationStateMachineName = "Weapon";
			primarySpread.baseMaxStock = 1;
			primarySpread.baseRechargeInterval = 1.2f;
			primarySpread.beginSkillCooldownOnSkillEnd = true;
			primarySpread.canceledFromSprinting = false;
			primarySpread.cancelSprintingOnActivation = true;
			primarySpread.dontAllowPastMaxStocks = false;
			primarySpread.forceSprintDuringState = false;
			primarySpread.fullRestockOnAssign = true;
			primarySpread.interruptPriority = 0;
			primarySpread.isCombatSkill = true;
			primarySpread.mustKeyPress = false;
			primarySpread.rechargeStock = primarySpread.baseMaxStock;
			primarySpread.requiredStock = 1;
			primarySpread.stockToConsume = 0;
			primarySpread.skillNameToken = Lang.SKILL_PRIMARY_SPREAD_NAME;
			primarySpread.skillDescriptionToken = Lang.SKILL_PRIMARY_SPREAD_DESC;
			primarySpread.icon = CommonImages.PrimarySpreadShotIcon;
			Tools.AddSkill(playerBodyPrefab, primarySpread, "primary", 1);

			// Secondary
			ContentAddition.AddEntityState<Skills.VoidSecondary>(out _);
			SkillDef secondary = ScriptableObject.CreateInstance<SkillDef>();
			secondary.activationState = new SerializableEntityStateType(typeof(Skills.VoidSecondary));
			secondary.activationStateMachineName = "Weapon";
			secondary.baseMaxStock = 1;
			secondary.baseRechargeInterval = Configuration.SecondaryCooldown;
			secondary.beginSkillCooldownOnSkillEnd = true;
			secondary.canceledFromSprinting = true;
			secondary.cancelSprintingOnActivation = true;
			secondary.dontAllowPastMaxStocks = false;
			secondary.forceSprintDuringState = false;
			secondary.fullRestockOnAssign = true;
			secondary.interruptPriority = InterruptPriority.Skill;
			secondary.isCombatSkill = true;
			secondary.mustKeyPress = true;
			secondary.rechargeStock = secondary.baseMaxStock;
			secondary.requiredStock = 1;
			secondary.stockToConsume = 1;
			secondary.skillNameToken = Lang.SKILL_SECONDARY_NAME;
			secondary.skillDescriptionToken = Lang.SKILL_SECONDARY_DESC;
			secondary.icon = CommonImages.SecondaryIcon;
			Tools.AddSkill(playerBodyPrefab, secondary, "secondary", 0);

			// Utility
			ContentAddition.AddEntityState<Skills.VoidUtility>(out _);
			SkillDef utility = ScriptableObject.CreateInstance<SkillDef>();
			utility.activationState = new SerializableEntityStateType(typeof(Skills.VoidUtility));
			utility.activationStateMachineName = "Body";
			utility.baseMaxStock = 1;
			utility.baseRechargeInterval = 4f;
			utility.beginSkillCooldownOnSkillEnd = true;
			utility.canceledFromSprinting = false;
			utility.cancelSprintingOnActivation = false;
			utility.dontAllowPastMaxStocks = false;
			utility.forceSprintDuringState = true;
			utility.fullRestockOnAssign = true;
			utility.interruptPriority = InterruptPriority.PrioritySkill;
			utility.isCombatSkill = false;
			utility.mustKeyPress = false;
			utility.rechargeStock = utility.baseMaxStock;
			utility.requiredStock = 1;
			utility.stockToConsume = 1;
			utility.skillNameToken = Lang.SKILL_UTILITY_NAME;
			utility.skillDescriptionToken = Lang.SKILL_UTILITY_DESC;
			utility.icon = CommonImages.UtilityIcon;
			Tools.AddSkill(playerBodyPrefab, utility, "utility", 0);

			ContentAddition.AddEntityState<Skills.VoidSpecialOnDemand>(out _);
			SkillDef specialWeak = ScriptableObject.CreateInstance<SkillDef>();
			specialWeak.activationState = new SerializableEntityStateType(typeof(Skills.VoidSpecialOnDemand));
			specialWeak.activationStateMachineName = "Body";
			specialWeak.baseMaxStock = 1;
			specialWeak.baseRechargeInterval = Configuration.SpecialCooldown;
			specialWeak.beginSkillCooldownOnSkillEnd = true;
			specialWeak.canceledFromSprinting = false;
			specialWeak.cancelSprintingOnActivation = true;
			specialWeak.dontAllowPastMaxStocks = true;
			specialWeak.forceSprintDuringState = false;
			specialWeak.fullRestockOnAssign = true;
			specialWeak.interruptPriority = InterruptPriority.PrioritySkill;
			specialWeak.isCombatSkill = true;
			specialWeak.mustKeyPress = false;
			specialWeak.rechargeStock = specialWeak.baseMaxStock;
			specialWeak.requiredStock = 1;
			specialWeak.stockToConsume = 1;
			specialWeak.skillNameToken = Lang.SKILL_SPECIAL_WEAK_NAME;
			specialWeak.skillDescriptionToken = Lang.SKILL_SPECIAL_WEAK_DESC;
			specialWeak.icon = CommonImages.SpecialWeakIcon;
			Tools.AddSkill(playerBodyPrefab, specialWeak, "special", 0);

			ContentAddition.AddEntityState<Skills.VoidSpecialSuicide>(out _);
			SkillDef specialSuicide = ScriptableObject.CreateInstance<SkillDef>();
			specialSuicide.activationState = new SerializableEntityStateType(typeof(Skills.VoidSpecialSuicide));
			specialSuicide.activationStateMachineName = "Body";
			specialSuicide.baseMaxStock = 1;
			specialSuicide.baseRechargeInterval = 1;
			specialSuicide.beginSkillCooldownOnSkillEnd = true;
			specialSuicide.canceledFromSprinting = false;
			specialSuicide.cancelSprintingOnActivation = true;
			specialSuicide.dontAllowPastMaxStocks = true;
			specialSuicide.forceSprintDuringState = false;
			specialSuicide.fullRestockOnAssign = true;
			specialSuicide.interruptPriority = InterruptPriority.Death;
			specialSuicide.isCombatSkill = true;
			specialSuicide.mustKeyPress = false;
			specialSuicide.rechargeStock = 1;
			specialSuicide.requiredStock = 1;
			specialSuicide.stockToConsume = 1;
			specialSuicide.skillNameToken = Lang.SKILL_SPECIAL_SUICIDE_NAME;
			specialSuicide.skillDescriptionToken = Lang.SKILL_SPECIAL_SUICIDE_DESC;
			specialSuicide.icon = CommonImages.SpecialSuicideIcon;
			Tools.AddSkill(playerBodyPrefab, specialSuicide, "special", 1);

			Tools.AddSkins(playerBodyPrefab);

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Lang.SURVIVOR_DESC;
			survivorDef.displayNameToken = Lang.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Lang.SURVIVOR_OUTRO;
			survivorDef.displayPrefab = playerBodyLocator;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.44f;
			ContentAddition.AddSurvivorDef(survivorDef);

			Tools.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());

			if (Configuration.UseLegacyLunarMechanics) {
				GhostUtilitySkillState.OnEnter += OverrideLunarGhostStateDuration;
				LunarPrimaryReplacementSkill.GetRechargeInterval += OverrideLunarPrimaryRechargeInterval;
				LunarPrimaryReplacementSkill.GetMaxStock += OverrideLunarPrimaryMaxStock;
				LunarPrimaryReplacementSkill.GetRechargeStock += OverrideLunarPrimaryRechargeStock;
			}
			if (Configuration.VoidImmunity) {
				On.RoR2.CharacterBody.SetBuffCount += InterceptBuffsEvent;
				On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamage;
			}
		}

		private static void OnPlayerReaverModelChanged(Transform obj) {
			Animator animator = obj.GetComponent<Animator>();
			
			if (animator != null) {
				Log.LogMessage("Player is using the full size reaver model, the animation speed of their latest spawned model has been reduced to 0.825f to correct a speed error.");
				animator.speed = 0.825f;
				return;
			}
			Log.LogMessage("Player changed models, but no animator was present? " + obj?.ToString() ?? "null");
		}

		private static void InterceptTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent @this, DamageInfo dmg) {
			if (@this.body != null && @this.body.baseNameToken == Lang.SURVIVOR_NAME) {
				if (dmg.attacker == null && dmg.inflictor == null && dmg.damageType == (DamageType.BypassBlock | DamageType.BypassArmor)) {
					Log.LogDebug("Aborting what I believe to be Void atmosphere damage (no source, no inflictor, type bypasses blocks and armor only).");
					return;
				}
			}
			orig(@this, dmg);
		}

		private static void InterceptBuffsEvent(On.RoR2.CharacterBody.orig_SetBuffCount orig, CharacterBody @this, BuffIndex buffType, int newCount) {
			if (@this.baseNameToken == Lang.SURVIVOR_NAME) {
				BuffIndex megaVoidFog = DLC1Content.Buffs.VoidRaidCrabWardWipeFog.buffIndex;
				BuffIndex voidFog = RoR2Content.Buffs.VoidFogStrong.buffIndex;
				BuffIndex voidFogLite = RoR2Content.Buffs.VoidFogMild.buffIndex;
				if (buffType == megaVoidFog || buffType == voidFogLite || buffType == voidFog) {
					orig(@this, buffType, 0); // Always 0
					return;
				}
			}
			
			orig(@this, buffType, newCount);
		}

		#region Legacy Lunar Ability Hooks
		private static float OverrideLunarPrimaryRechargeInterval(LunarPrimaryReplacementSkill.orig_GetRechargeInterval orig, RoR2.Skills.LunarPrimaryReplacementSkill self, GenericSkill skillSlot) {
			float rechargeTime = orig.Invoke(self, skillSlot);
			bool takesLongerThan1s = rechargeTime > 1f;
			float result;
			if (takesLongerThan1s) {
				result = rechargeTime;
			} else {
				result = 1f;
			}
			return result;
		}

		private static int OverrideLunarPrimaryMaxStock(LunarPrimaryReplacementSkill.orig_GetMaxStock orig, RoR2.Skills.LunarPrimaryReplacementSkill self, GenericSkill skillSlot) {
			int rechargeStock = orig.Invoke(self, skillSlot);
			bool hasOverSix = rechargeStock > 6;
			int result;
			if (hasOverSix) {
				result = rechargeStock;
			} else {
				result = 6;
			}
			return result;
		}

		private static int OverrideLunarPrimaryRechargeStock(LunarPrimaryReplacementSkill.orig_GetRechargeStock orig, RoR2.Skills.LunarPrimaryReplacementSkill self, GenericSkill skillSlot) {
			int rechargeStock = orig.Invoke(self, skillSlot);
			bool hasOverSix = rechargeStock > 6;
			int result;
			if (hasOverSix) {
				result = rechargeStock;
			} else {
				result = 6;
			}
			return result;
		}

		private static void OverrideLunarGhostStateDuration(On.EntityStates.GhostUtilitySkillState.orig_OnEnter orig, EntityStates.GhostUtilitySkillState self) {
			orig.Invoke(self);
			bool durationIsZero = Reflection.GetFieldValue<float>(self, "duration") == 0f;
			if (durationIsZero) {
				Reflection.SetFieldValue(self, "duration", 1.5f);
			}
		}
		#endregion

	}
}
