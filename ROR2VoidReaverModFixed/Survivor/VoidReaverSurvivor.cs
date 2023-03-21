#pragma warning disable Publicizer001
using EntityStates;
using KinematicCharacterController;
using R2API;
using UnityEngine.Networking;
using UnityEngine;
using VoidReaverMod.Initialization;
using RoR2;
using VoidReaverMod.Initialization.Sprites;
using RoR2.Skills;
using VoidReaverMod.Skills.Primary;
using VoidReaverMod.Skills.Undertow;
using VoidReaverMod.Skills.SpecialSkills;
using VoidReaverMod.Skills.Dive;
using Xan.ROR2VoidPlayerCharacterCommon;
using VoidReaverMod.Skills.Spawn;
using Xan.ROR2VoidPlayerCharacterCommon.SurvivorHelper;
using UnityEngine.AddressableAssets;
using VoidReaverMod.Skills.NoSkill;

namespace VoidReaverMod.Survivor {
	public class VoidReaverSurvivor {

		public static SurvivorDef Reaver { get; private set; }

		public static BodyIndex ReaverIndex { get; private set; }

		private static bool _survivorReady = false;
		private static SkillLocator _prefabSkillLocator;
		private static ExtraPassiveSkill _modNotice;

		// TODO: Try to allow runtime edits by using one of the popular settings mods?
		// Why can't this be like MelonLoader where settings are just exposed out of the box
		public static void UpdateSettings(CharacterBody body) {
			Log.LogTrace("Updating body settings...");
			body.baseMaxHealth = Configuration.CommonVoidEnemyConfigs.BaseMaxHealth;
			body.levelMaxHealth = Configuration.CommonVoidEnemyConfigs.LevelMaxHealth;
			body.baseMaxShield = Configuration.CommonVoidEnemyConfigs.BaseMaxShield;
			body.levelMaxShield = Configuration.CommonVoidEnemyConfigs.LevelMaxShield;
			body.baseArmor = Configuration.CommonVoidEnemyConfigs.BaseArmor;
			body.levelArmor = Configuration.CommonVoidEnemyConfigs.LevelArmor;
			body.baseRegen = Configuration.CommonVoidEnemyConfigs.BaseHPRegen;
			body.levelRegen = Configuration.CommonVoidEnemyConfigs.LevelHPRegen;

			body.baseDamage = Configuration.CommonVoidEnemyConfigs.BaseDamage;
			body.levelDamage = Configuration.CommonVoidEnemyConfigs.LevelDamage;
			body.baseAttackSpeed = Configuration.CommonVoidEnemyConfigs.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.CommonVoidEnemyConfigs.LevelAttackSpeed;
			body.baseCrit = Configuration.CommonVoidEnemyConfigs.BaseCritChance;
			body.levelCrit = Configuration.CommonVoidEnemyConfigs.LevelCritChance;

			body.baseMoveSpeed = Configuration.CommonVoidEnemyConfigs.BaseMoveSpeed;
			body.levelMoveSpeed = Configuration.CommonVoidEnemyConfigs.LevelMoveSpeed;
			body.baseJumpCount = Configuration.CommonVoidEnemyConfigs.BaseJumpCount;
			body.baseJumpPower = Configuration.CommonVoidEnemyConfigs.BaseJumpPower;
			body.levelJumpPower = Configuration.CommonVoidEnemyConfigs.LevelJumpPower;
			body.baseAcceleration = Configuration.CommonVoidEnemyConfigs.BaseAcceleration;


			if (_survivorReady) {
				SkillLocator locator = body.skillLocator;
				UpdateCooldowns(_prefabSkillLocator);
				if (locator) UpdateCooldowns(locator);
				Localization.ReloadStattedTexts(body.bodyIndex);
			}
		}

		private static void UpdateCooldowns(SkillLocator locator) {
			locator.primary.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.PrimaryImpulseCooldown;
			locator.primary.skillFamily.variants[1].skillDef.baseRechargeInterval = Configuration.PrimarySpreadCooldown;

			locator.secondary.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.SecondaryCooldown;

			locator.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.UtilityCooldown;

			locator.special.skillFamily.variants[0].skillDef.baseRechargeInterval = Configuration.SpecialCooldown;
		}

		public static void Initialize(VoidReaverPlayerPlugin plugin) {
			//GameObject playerBodyPrefab = ROR2ObjectCreator.CreateBody("NullifierSurvivorBody", "RoR2/Base/Nullifier/NullifierBody.prefab");
			GameObject playerBodyPrefab = SurvivorHollower.CreateBodyWithSkins("NullifierSurvivorBody", "RoR2/Base/Nullifier/NullifierBody.prefab", "RoR2/Base/Nullifier/NullifierAllyBody.prefab");

			Log.LogTrace("Setting localPlayerAuthority=true on Reaver Body (this patches a bug)...");
			NetworkIdentity netID = playerBodyPrefab.GetComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			Log.LogTrace("Duplicating the body for display in the Survivor Selection screen...");
			GameObject playerBodyDisplayPrefab = PrefabAPI.InstantiateClone(playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "NullifierSurvivorBodyDisplay");
			netID = playerBodyDisplayPrefab.AddComponent<NetworkIdentity>();
			netID.localPlayerAuthority = true;

			Log.LogTrace("Preparing the Body's style and stats...");
			CharacterBody body = playerBodyPrefab.GetComponent<CharacterBody>();
			UpdateSettings(body);
			body.bodyColor = new Color(0.867f, 0.468f, 0.776f);
			body.baseNameToken = Localization.SURVIVOR_NAME;
			body.subtitleNameToken = Localization.SURVIVOR_UMBRA;
			body.portraitIcon = Images.Portrait.texture;

			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void;
			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			ContentAddition.AddBody(playerBodyPrefab);

			Log.LogTrace("Setting up spawn animation...");
			EntityStateMachine initialStateCtr = playerBodyPrefab.GetComponent<EntityStateMachine>();
			initialStateCtr.initialStateType = new SerializableEntityStateType(typeof(EntityStates.NullifierMonster.SpawnState));//UtilCreateSerializableAndNetRegister<SpawnState>();

			Log.LogTrace("Setting up death animation...");
			CharacterDeathBehavior deathBehavior = playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Skills.Death.DeathState>();

			
			// Passive stuff:
			SkillLocator skillLoc = playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Localization.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Localization.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Localization.PASSIVE_DESC;
			skillLoc.passiveSkill.icon = Images.Portrait;
			_prefabSkillLocator = skillLoc;

			ExtraPassiveSkill modNotice = playerBodyPrefab.AddComponent<ExtraPassiveSkill>();
			modNotice.beforeNormalPassive = true;
			modNotice.additionalPassive.enabled = !Configuration.HideNotice;
			modNotice.additionalPassive.skillNameToken = Localization.VOID_COMMON_API_MESSAGE_NAME;
			modNotice.additionalPassive.skillDescriptionToken = Localization.VOID_COMMON_API_MESSAGE_DESC;
			modNotice.additionalPassive.keywordToken = Localization.VOID_COMMON_API_MESSAGE_CONTENT;
			modNotice.additionalPassive.icon = Images.GenericWarning;
			_modNotice = modNotice;

			Log.LogTrace("Finished registering passive details...");

			// Primary, triple shot:
			SkillDef voidImpulsePrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidImpulsePrimary.activationState = UtilCreateSerializableAndNetRegister<VoidImpulseSkill>();
			voidImpulsePrimary.activationStateMachineName = "Weapon";
			voidImpulsePrimary.baseMaxStock = 1;
			voidImpulsePrimary.baseRechargeInterval = Configuration.PrimaryImpulseCooldown;
			voidImpulsePrimary.beginSkillCooldownOnSkillEnd = false;
			voidImpulsePrimary.canceledFromSprinting = false;
			voidImpulsePrimary.cancelSprintingOnActivation = true;
			voidImpulsePrimary.dontAllowPastMaxStocks = true;
			voidImpulsePrimary.forceSprintDuringState = false;
			voidImpulsePrimary.fullRestockOnAssign = true;
			voidImpulsePrimary.interruptPriority = 0;
			voidImpulsePrimary.isCombatSkill = true;
			voidImpulsePrimary.mustKeyPress = false;
			voidImpulsePrimary.rechargeStock = voidImpulsePrimary.baseMaxStock;
			voidImpulsePrimary.requiredStock = 1;
			voidImpulsePrimary.stockToConsume = 0;
			voidImpulsePrimary.skillNameToken = Localization.SKILL_PRIMARY_TRIPLESHOT_NAME;
			voidImpulsePrimary.skillDescriptionToken = Localization.SKILL_PRIMARY_TRIPLESHOT_DESC;
			voidImpulsePrimary.icon = Images.VoidImpulseIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, voidImpulsePrimary, SurvivorHollower.SlotType.Primary, 0);
			Log.LogTrace("Finished registering Void Impulse...");

			// Primary, spread shot:
			SkillDef voidSpreadPrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidSpreadPrimary.activationState = UtilCreateSerializableAndNetRegister<VoidSpreadSkill>();
			voidSpreadPrimary.activationStateMachineName = "Weapon";
			voidSpreadPrimary.baseMaxStock = 1;
			voidSpreadPrimary.baseRechargeInterval = Configuration.PrimarySpreadCooldown;
			voidSpreadPrimary.beginSkillCooldownOnSkillEnd = true;
			voidSpreadPrimary.canceledFromSprinting = false;
			voidSpreadPrimary.cancelSprintingOnActivation = true;
			voidSpreadPrimary.dontAllowPastMaxStocks = false;
			voidSpreadPrimary.forceSprintDuringState = false;
			voidSpreadPrimary.fullRestockOnAssign = true;
			voidSpreadPrimary.interruptPriority = 0;
			voidSpreadPrimary.isCombatSkill = true;
			voidSpreadPrimary.mustKeyPress = false;
			voidSpreadPrimary.rechargeStock = voidSpreadPrimary.baseMaxStock;
			voidSpreadPrimary.requiredStock = 1;
			voidSpreadPrimary.stockToConsume = 0;
			voidSpreadPrimary.skillNameToken = Localization.SKILL_PRIMARY_SPREAD_NAME;
			voidSpreadPrimary.skillDescriptionToken = Localization.SKILL_PRIMARY_SPREAD_DESC;
			voidSpreadPrimary.icon = Images.VoidSpreadIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, voidSpreadPrimary, SurvivorHollower.SlotType.Primary, 1);
			Log.LogTrace("Finished registering Void Spread...");

			// Secondary
			SkillDef undertowSecondary = ScriptableObject.CreateInstance<SkillDef>();
			undertowSecondary.activationState = UtilCreateSerializableAndNetRegister<UndertowSkill>();
			undertowSecondary.activationStateMachineName = "Weapon";
			undertowSecondary.baseMaxStock = 1;
			undertowSecondary.baseRechargeInterval = Configuration.SecondaryCooldown;
			undertowSecondary.beginSkillCooldownOnSkillEnd = true;
			undertowSecondary.canceledFromSprinting = true;
			undertowSecondary.cancelSprintingOnActivation = true;
			undertowSecondary.dontAllowPastMaxStocks = true;
			undertowSecondary.forceSprintDuringState = false;
			undertowSecondary.fullRestockOnAssign = true;
			undertowSecondary.interruptPriority = InterruptPriority.Skill;
			undertowSecondary.isCombatSkill = true;
			undertowSecondary.mustKeyPress = true;
			undertowSecondary.rechargeStock = undertowSecondary.baseMaxStock;
			undertowSecondary.requiredStock = 1;
			undertowSecondary.stockToConsume = 0;
			undertowSecondary.skillNameToken = Localization.SKILL_SECONDARY_NAME;
			undertowSecondary.skillDescriptionToken = Localization.SKILL_SECONDARY_DESC;
			undertowSecondary.icon = Images.UndertowIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, undertowSecondary, SurvivorHollower.SlotType.Secondary, 0);
			Log.LogTrace("Finished registering Undertow...");

			// Utility
			SkillDef diveUtility = ScriptableObject.CreateInstance<SkillDef>();
			diveUtility.activationState = UtilCreateSerializableAndNetRegister<DiveSkill>();
			diveUtility.activationStateMachineName = "Body";
			diveUtility.baseMaxStock = 1;
			diveUtility.baseRechargeInterval = Configuration.UtilityCooldown;
			diveUtility.beginSkillCooldownOnSkillEnd = true;
			diveUtility.canceledFromSprinting = false;
			diveUtility.cancelSprintingOnActivation = false;
			diveUtility.dontAllowPastMaxStocks = false;
			diveUtility.forceSprintDuringState = true;
			diveUtility.fullRestockOnAssign = true;
			diveUtility.interruptPriority = InterruptPriority.PrioritySkill;
			diveUtility.isCombatSkill = false;
			diveUtility.mustKeyPress = false;
			diveUtility.rechargeStock = diveUtility.baseMaxStock;
			diveUtility.requiredStock = 1;
			diveUtility.stockToConsume = 1;
			diveUtility.skillNameToken = Localization.SKILL_UTILITY_NAME;
			diveUtility.skillDescriptionToken = Localization.SKILL_UTILITY_DESC;
			diveUtility.icon = Images.DiveIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, diveUtility, SurvivorHollower.SlotType.Utility, 0);
			Log.LogTrace("Finished registering Dive...");
			
			SkillDef specialWeak = ScriptableObject.CreateInstance<SkillDef>();
			specialWeak.activationState = UtilCreateSerializableAndNetRegister<BlackHoleOnDemandSkill>();
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
			specialWeak.skillNameToken = Localization.SKILL_SPECIAL_WEAK_NAME;
			specialWeak.skillDescriptionToken = Localization.SKILL_SPECIAL_WEAK_DESC;
			specialWeak.icon = Images.DetainIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, specialWeak, SurvivorHollower.SlotType.Special, 0);
			Log.LogTrace("Finished registering Reave...");

			SkillDef specialSuicide = ScriptableObject.CreateInstance<SkillDef>();
			specialSuicide.activationState = UtilCreateSerializableAndNetRegister<SuicideSkill>();
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
			specialSuicide.skillNameToken = Localization.SKILL_SPECIAL_SUICIDE_NAME;
			specialSuicide.skillDescriptionToken = Localization.SKILL_SPECIAL_SUICIDE_DESC;
			specialSuicide.icon = Images.ReaveIcon;
			SurvivorHollower.AddSkill(playerBodyPrefab, specialSuicide, SurvivorHollower.SlotType.Special, 1);
			Log.LogTrace("Finished registering Collapse...");

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Localization.SURVIVOR_DESC;
			survivorDef.displayNameToken = Localization.SURVIVOR_NAME;
			survivorDef.cachedName = survivorDef.displayNameToken;
			survivorDef.outroFlavorToken = Localization.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Localization.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyDisplayPrefab;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.44f;
			Reaver = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			SurvivorHollower.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());

			Log.LogTrace("Adding animation correction hooks for full size reaver.");
			On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.RebuildMannequinInstance += OnRebuildMannequinInstance;
			On.EntityStates.BaseCharacterMain.UpdateAnimationParameters += OnAnimationParametersUpdated;
			On.RoR2.CameraRigController.GenerateCameraModeContext += OnGeneratingCameraModeContext;
			On.RoR2.ModelLocator.Awake += OnModelLocatorAwakened;
			Configuration.OnStatConfigChanged += StatChanged;
			Configuration.OnHideNoticeChanged += HideNoticeChanged;

			BodyCatalog.availability.CallWhenAvailable(() => {
				ReaverIndex = body.bodyIndex;
				XanVoidAPI.RegisterAsVoidEntity(plugin, ReaverIndex);
				Configuration.LateInit(plugin, ReaverIndex);
				Localization.ReloadStattedTexts(ReaverIndex);
			});
			XanVoidAPI.VerifyProperConstruction(body);

			Log.LogTrace("Survivor setup completed.");

			_survivorReady = true;
			UpdateSettings(body);
		}

		private static void HideNoticeChanged(bool hide) {
			_modNotice.additionalPassive.enabled = !hide;
		}

		private static void StatChanged() {
			if (_survivorReady) Localization.ReloadStattedTexts(ReaverIndex);
		}

		private static void SetupScale(CharacterBody body) {
			Log.LogTrace("Setting up interactions and scale dependent properties...");
			Interactor interactor = body.gameObject.GetComponent<Interactor>();
			if (Configuration.CommonVoidEnemyConfigs.UseFullSizeCharacter) {
				Log.LogTrace("Using full size character model. Increasing interaction distance...");
				interactor.maxInteractionDistance = 9f;
			} else {
				Log.LogTrace("Using small character model. Decreasing interaction distance, scaling down model and colliders...");
				// We ARE NOT using full size character. Downscale.
				GameObject baseTransform = body.gameObject.GetComponent<ModelLocator>().modelBaseTransform.gameObject;
				baseTransform.transform.localScale = Vector3.one * 0.5f;
				baseTransform.transform.Translate(new Vector3(0f, 4f, 0f));
				foreach (KinematicCharacterMotor kinematicCharacterMotor in body.gameObject.GetComponentsInChildren<KinematicCharacterMotor>()) {
					// * 0.4f
					// + 1.25f
					// Up to *0.5f and +1.5f
					// This seems to fix the bungus issue!
					kinematicCharacterMotor.SetCapsuleDimensions(kinematicCharacterMotor.Capsule.radius * 0.5f, kinematicCharacterMotor.Capsule.height * 0.5f, 1.5f);
				}
				interactor.maxInteractionDistance = 5f;
			}
			Log.LogTrace("Done setting up interactions and scale dependent properties.");
		}

		/// <summary>
		/// Utility: Use <see cref="ContentAddition.AddEntityState{T}(out bool)"/> to register the provided entity state type, then return a new instance of <see cref="SerializableEntityStateType"/> using the same type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static SerializableEntityStateType UtilCreateSerializableAndNetRegister<T>() where T : EntityState {
			// java when the typeof(T)
			// when
			//  the when the t
			// t
			Log.LogTrace($"Registering EntityState {typeof(T).FullName} and returning a new instance of {nameof(SerializableEntityStateType)} of that type...");
			ContentAddition.AddEntityState<T>(out _);
			return new SerializableEntityStateType(typeof(T));
		}


		private static void OnAnimationParametersUpdated(On.EntityStates.BaseCharacterMain.orig_UpdateAnimationParameters originalMethod, BaseCharacterMain @this) {
			if (!@this.hasCharacterBody) {
				originalMethod(@this);
				return;
			}

			CharacterBody body = @this.characterBody;
			if (body.bodyIndex == ReaverIndex) {
				originalMethod(@this);
				return;
			}

			// Basically, spoof the animator into treating it as if the character is never sprinting.
			bool originalSprinting = @this.characterBody._isSprinting;
			@this.characterBody._isSprinting = false;
			originalMethod(@this);
			@this.characterBody._isSprinting = originalSprinting;
		}

		private static void OnGeneratingCameraModeContext(On.RoR2.CameraRigController.orig_GenerateCameraModeContext originalMethod, CameraRigController @this, out RoR2.CameraModes.CameraModeBase.CameraModeContext result) {
			if (@this.targetBody == null) {
				originalMethod(@this, out result);
				return;
			}
			if (@this.targetBody.sprintingSpeedMultiplier > 1) {
				originalMethod(@this, out result);
				return;
			}
			if (@this.targetBody.bodyIndex == ReaverIndex) {
				originalMethod(@this, out result);
				return;
			}

			// If the sprint speed multiplier is less than or equal to 1, do not apply the FOV boost.
			// To do this, trick the camera system into thinking we are not sprinting by changing the non-replicated variable.
			bool originalSprinting = @this.targetBody._isSprinting;
			@this.targetBody._isSprinting = false;
			originalMethod(@this, out result);
			@this.targetBody._isSprinting = originalSprinting;
		}

		private static void OnModelLocatorAwakened(On.RoR2.ModelLocator.orig_Awake originalMethod, ModelLocator @this) {
			originalMethod(@this);

			CharacterBody body = @this.GetComponent<CharacterBody>();
			if (body != null) {
				if (body.bodyIndex == ReaverIndex) {
					Log.LogTrace("A body spawned that is a player reaver.");
					Animator animator = @this.modelTransform.GetComponent<Animator>();
					if (animator != null) {
						if (Configuration.CommonVoidEnemyConfigs.UseFullSizeCharacter) {
							Log.LogTrace("Player is using the full size reaver model, the animation speed of their latest spawned model has been reduced to 0.825f to correct a speed error.");
							animator.speed = 0.825f;
						} else {
							Log.LogTrace("Player is using the small reaver. No changes are being made to the animator.");
						}
					} else {
						Log.LogTrace("Player changed models, but no animator was present?");
					}

					Log.LogTrace("Updating transparency systems...");
					TransparencyController ctr = @this.gameObject.AddComponent<TransparencyController>();
					ctr.getTransparencyInCombat = () => Configuration.CommonVoidEnemyConfigs.TransparencyInCombat;
					ctr.getTransparencyOutOfCombat = () => Configuration.CommonVoidEnemyConfigs.TransparencyOutOfCombat;

					Log.LogTrace("Updating camera systems...");
					CameraController cam = @this.gameObject.AddComponent<CameraController>();
					cam.getCameraOffset = () => Configuration.CommonVoidEnemyConfigs.CameraOffset;
					cam.getCameraPivot = () => Configuration.CommonVoidEnemyConfigs.CameraPivotOffset;
					cam.getUseFullSizeCharacter = () => Configuration.CommonVoidEnemyConfigs.UseFullSizeCharacter;

					Log.LogTrace("Updating body size...");
					UpdateSettings(body);
					SetupScale(body);
				}
			}
		}

		private static void OnRebuildMannequinInstance(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_RebuildMannequinInstance originalMethod, RoR2.SurvivorMannequins.SurvivorMannequinSlotController @this) {
			originalMethod(@this);
			if (@this.mannequinInstanceTransform != null && @this.currentSurvivorDef == Reaver) {
				Animator previewAnimator = @this.mannequinInstanceTransform.Find("mdlNullifier").GetComponent<Animator>();
				previewAnimator.SetBool("isGrounded", true); // Fix an animation bug
				previewAnimator.SetFloat("Spawn.playbackRate", 1f);
				previewAnimator.Play("Spawn");
				Util.PlaySound(SpawnState.spawnSoundString, @this.mannequinInstanceTransform.gameObject);
				Transform effectSpawnerTransform = @this.mannequinInstanceTransform.Find("mdlNullifier").Find("NullifierArmature").Find("PortalEffect");
				EffectData fx = new EffectData {
					origin = effectSpawnerTransform.position - new Vector3(0, 1f, 0),
					rotation = Quaternion.AngleAxis(30, Vector3.up),
					scale = 0.4f,
					// rootObject = @this.mannequinInstanceTransform.gameObject
				};
				// effectSpawnerTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				EffectManager.SpawnEffect(EffectProvider.SpawnEffect, fx, false);
				// EffectManager.SimpleMuzzleFlash(SpawnState.spawnEffectPrefab, @this.mannequinInstanceTransform.gameObject, "PortalEffect", false);
			}
		}
	}
}
