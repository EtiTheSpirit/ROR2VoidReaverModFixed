using EntityStates;
using KinematicCharacterController;
using R2API;
using R2API.Utils;
using RoR2;
using ROR2VoidReaverModFixed.FubukiMods.XanInterop;
using ROR2VoidReaverModFixed.XanCode;
using UnityEngine;
using UnityEngine.Networking;
using XanVoidReaverEdit;
using GhostUtilitySkillState = On.EntityStates.GhostUtilitySkillState;
using LunarPrimaryReplacementSkill = On.RoR2.Skills.LunarPrimaryReplacementSkill;
using SerializableEntityStateType = EntityStates.SerializableEntityStateType;
using SkillDef = RoR2.Skills.SkillDef;

namespace FubukiMods.Modules {

	/// <summary>
	/// The survivor main module. This was originally written by LuaFubuki but was rewritten and refactored by Xan.
	/// </summary>
	public class Survivors {
		public static void Init() {
			GameObject playerBodyContainer = Tools.CreateBody("PlayerNullifierBody", "RoR2/Base/Nullifier/NullifierBody.prefab");
			GameObject playerBodyLocator = PrefabAPI.InstantiateClone(playerBodyContainer.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerNullifierBodyDisplay");
			playerBodyLocator.AddComponent<NetworkIdentity>();

			Interactor interactor = playerBodyContainer.GetComponent<Interactor>();
			
			if (!Configuration.UseFullSizeCharacter) {
				// We ARE NOT using full size character. Downscale.
				GameObject baseTransform = playerBodyContainer.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
				baseTransform.transform.localScale = Vector3.one * 0.5f;
				baseTransform.transform.Translate(new Vector3(0f, 4f, 0f));
				
				foreach (KinematicCharacterMotor kinematicCharacterMotor in playerBodyContainer.GetComponentsInChildren<KinematicCharacterMotor>()) {
					kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.4f, kinematicCharacterMotor.Capsule.height * 0.4f, 1.25f);
				}
				interactor.maxInteractionDistance = 5f;
			} else {
				interactor.maxInteractionDistance = 10f;
			}
			CameraTargetParams camTargetParams = playerBodyContainer.GetComponent<CameraTargetParams>();
			CharacterCameraParams characterCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
			camTargetParams.cameraParams = characterCameraParams;
			if (!Configuration.UseFullSizeCharacter) {
				characterCameraParams.data.idealLocalCameraPos = new Vector3(0f, 2f, -12f);
				characterCameraParams.data.pivotVerticalOffset = 0f;
			} else {
				characterCameraParams.data.idealLocalCameraPos = new Vector3(0f, 4f, -16f);
				characterCameraParams.data.pivotVerticalOffset = 0f;
			}

			playerBodyContainer.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			playerBodyContainer.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(Skills.VoidDeath));

			CharacterBody body = playerBodyContainer.GetComponent<CharacterBody>();
			body.aimOriginTransform.Translate(new Vector3(0f, 0f, 0f));
			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void;
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
			body.portraitIcon = Images.Portrait.texture;
			ContentAddition.AddBody(playerBodyContainer);

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
			primaryTriple.icon = Images.PrimaryTripleShotIcon;
			Tools.AddSkill(playerBodyContainer, primaryTriple, "primary", 0);

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
			primarySpread.icon = Images.PrimarySpreadShotIcon;
			Tools.AddSkill(playerBodyContainer, primarySpread, "primary", 1);

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
			secondary.icon = Images.SecondaryIcon;
			Tools.AddSkill(playerBodyContainer, secondary, "secondary", 0);

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
			utility.icon = Images.UtilityIcon;
			Tools.AddSkill(playerBodyContainer, utility, "utility", 0);

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
			specialWeak.icon = Images.SpecialIcon;
			Tools.AddSkill(playerBodyContainer, specialWeak, "special", 0);

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
			specialSuicide.icon = Images.SpecialIcon;
			Tools.AddSkill(playerBodyContainer, specialSuicide, "special", 1);

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyContainer;
			survivorDef.descriptionToken = Lang.SURVIVOR_DESC;
			survivorDef.displayNameToken = Lang.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Lang.SURVIVOR_OUTRO;
			survivorDef.displayPrefab = playerBodyLocator;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.44f;
			ContentAddition.AddSurvivorDef(survivorDef);

			Tools.FinalizeBody(playerBodyContainer.GetComponent<SkillLocator>());

			if (Configuration.UseLegacyLunarMechanics) {
				GhostUtilitySkillState.OnEnter += OverrideLunarGhostStateDuration;
				LunarPrimaryReplacementSkill.GetRechargeInterval += OverrideLunarPrimaryRechargeInterval;
				LunarPrimaryReplacementSkill.GetMaxStock += OverrideLunarPrimaryMaxStock;
				LunarPrimaryReplacementSkill.GetRechargeStock += OverrideLunarPrimaryRechargeStock;
			}
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
