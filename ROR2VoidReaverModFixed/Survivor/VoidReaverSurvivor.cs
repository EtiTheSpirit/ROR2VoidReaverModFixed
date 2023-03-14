using EntityStates;
using KinematicCharacterController;
using R2API;
using VoidReaverMod.XansTools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using VoidReaverMod.Initialization;
using VoidReaverMod.XansTools;
using RoR2;
using VoidReaverMod.Initialization.Sprites;
using RoR2.Skills;
using VoidReaverMod.Skills.Primary;
using VoidReaverMod.Skills.Undertow;
using VoidReaverMod.Skills.SpecialSkills;
using VoidReaverMod.Skills.Dive;
using ROR2HPBarAPI.API;
using VoidReaverMod.Survivor.Interop;
using Xan.ROR2VoidPlayerCharacterCommon;
using VoidReaverMod.Skills.Spawn;
using VoidReaverMod.Survivor.Render;

namespace VoidReaverMod.Survivor {
	public class VoidReaverSurvivor {

		public static SurvivorDef Reaver { get; private set; }

		public static BodyIndex ReaverIndex { get; private set; }

		// TODO: Try to allow runtime edits by using one of the popular settings mods?
		// Why can't this be like MelonLoader where settings are just exposed out of the box
		public static void UpdateSettings(CharacterBody body) {
			Log.LogTrace("Updating body settings...");
			body.baseMaxHealth = Configuration.BaseMaxHealth;
			body.levelMaxHealth = Configuration.LevelMaxHealth;
			body.baseMaxShield = Configuration.BaseMaxShield;
			body.levelMaxShield = Configuration.LevelMaxShield;
			body.baseArmor = Configuration.BaseArmor;
			body.levelArmor = Configuration.LevelArmor;
			body.baseRegen = Configuration.BaseHPRegen;
			body.levelRegen = Configuration.LevelHPRegen;

			body.baseDamage = Configuration.BaseDamage;
			body.levelDamage = Configuration.LevelDamage;
			body.baseAttackSpeed = Configuration.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.LevelAttackSpeed;
			body.baseCrit = Configuration.BaseCritChance;
			body.levelCrit = Configuration.LevelCritChance;

			body.baseMoveSpeed = Configuration.BaseMoveSpeed;
			body.levelMoveSpeed = Configuration.LevelMoveSpeed;
			body.baseJumpCount = Configuration.BaseJumpCount;
			body.baseJumpPower = Configuration.BaseJumpPower;
			body.levelJumpPower = Configuration.LevelJumpPower;
			body.baseAcceleration = Configuration.BaseAcceleration;
		}

		public static void Init(VoidReaverPlayerPlugin plugin) {
			GameObject playerBodyPrefab = ROR2ObjectCreator.CreateBody("NullifierSurvivorBody", "RoR2/Base/Nullifier/NullifierBody.prefab");

			Log.LogTrace("Setting localPlayerAuthority=true on Jailer Body (this patches a bug)...");
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

			body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.Void | CharacterBody.BodyFlags.ImmuneToVoidDeath;
			// Immunity to void death is intentional. This is because it deletes the character model, which we don't want.
			// Instead, we manually intercept the damage and apply it as a non-voiddeath source. This way it can kill the player and play the death animation without screwing up
			// the killcam / spectator cam by suddenly yoinking the model out of existence.

			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;
			ContentAddition.AddBody(playerBodyPrefab);

			Log.LogTrace("Setting up spawn animation...");
			EntityStateMachine initialStateCtr = playerBodyPrefab.GetComponent<EntityStateMachine>();
			initialStateCtr.initialStateType = UtilCreateSerializableAndNetRegister<Skills.Spawn.SpawnState>();

			Log.LogTrace("Setting up death animation...");
			CharacterDeathBehavior deathBehavior = playerBodyPrefab.GetComponent<CharacterDeathBehavior>();
			deathBehavior.deathState = UtilCreateSerializableAndNetRegister<Skills.Death.DeathState>();

			Log.LogTrace("Setting up interactions and scale dependent properties...");
			Interactor interactor = playerBodyPrefab.GetComponent<Interactor>();
			if (Configuration.UseFullSizeCharacter) {
				Log.LogTrace("Using full size character model. Increasing interaction distance and adding hook to reduce animation speed...");
				interactor.maxInteractionDistance = 11f;
				On.RoR2.ModelLocator.Awake += OnModelLocatorAwakened;
			} else {
				Log.LogTrace("Using small character model. Decreasing interaction distance, scaling down model and colliders...");
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

			// Passive stuff:
			SkillLocator skillLoc = playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Localization.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Localization.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Localization.PASSIVE_DESC;
			skillLoc.passiveSkill.icon = Images.Portrait;
			Log.LogTrace("Finished registering passive details...");

			// Primary, triple shot:
			SkillDef voidImpulsePrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidImpulsePrimary.activationState = UtilCreateSerializableAndNetRegister<VoidImpulseSkill>();
			voidImpulsePrimary.activationStateMachineName = "Weapon";
			voidImpulsePrimary.baseMaxStock = 1;
			voidImpulsePrimary.baseRechargeInterval = 1f;
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, voidImpulsePrimary, "primary", 0);
			Log.LogTrace("Finished registering Void Impulse...");

			// Primary, spread shot:
			SkillDef voidSpreadPrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidSpreadPrimary.activationState = UtilCreateSerializableAndNetRegister<VoidSpreadSkill>();
			voidSpreadPrimary.activationStateMachineName = "Weapon";
			voidSpreadPrimary.baseMaxStock = 1;
			voidSpreadPrimary.baseRechargeInterval = 1.2f;
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, voidSpreadPrimary, "primary", 1);
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, undertowSecondary, "secondary", 0);
			Log.LogTrace("Finished registering Undertow...");

			// Utility
			SkillDef diveUtility = ScriptableObject.CreateInstance<SkillDef>();
			diveUtility.activationState = UtilCreateSerializableAndNetRegister<DiveSkill>();
			diveUtility.activationStateMachineName = "Body";
			diveUtility.baseMaxStock = 1;
			diveUtility.baseRechargeInterval = 4f;
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, diveUtility, "utility", 0);
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, specialWeak, "special", 0);
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
			ROR2ObjectCreator.AddSkill(playerBodyPrefab, specialSuicide, "special", 1);
			Log.LogTrace("Finished registering Collapse...");

			ROR2ObjectCreator.AddReaverSkins(playerBodyPrefab);
			Log.LogTrace("Finished registering skins...");

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Localization.SURVIVOR_DESC;
			survivorDef.displayNameToken = Localization.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Localization.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Localization.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyDisplayPrefab;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.44f;
			Reaver = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			ROR2ObjectCreator.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());

			Log.LogTrace("Adding animation correction hooks for full size reaver.");
			On.EntityStates.BaseCharacterMain.UpdateAnimationParameters += OnAnimationParametersUpdated;
			On.RoR2.CameraRigController.GenerateCameraModeContext += OnGeneratingCameraModeContext;
			On.RoR2.CharacterBody.Awake += OnCharacterBodyAwake;

			BodyCatalog.availability.CallWhenAvailable(() => {
				Registry.RegisterColorProvider(plugin, body.bodyIndex, new HPBarColorMarshaller());
				Log.LogTrace("Custom Void-Style HP Bar colors registered.");
				XanVoidAPI.RegisterAsVoidEntity(plugin, body.bodyIndex);
				XanVoidAPI.CreateAndRegisterBlackHoleBehaviorConfigs(plugin, plugin.Config, body.bodyIndex);
				Localization.LateInit(body.bodyIndex);
				ReaverIndex = body.bodyIndex;
			});
			XanVoidAPI.VerifyProperConstruction(body);

			On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.RebuildMannequinInstance += OnRebuildMannequinInstance;
#if !USE_VOID_CHARACTER_API
			On.RoR2.CharacterMaster.OnBodyDeath += OnDied;
#endif
			Log.LogTrace("Survivor setup completed.");
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
						Log.LogTrace("Player is using the full size reaver model, the animation speed of their latest spawned model has been reduced to 0.825f to correct a speed error.");
						animator.speed = 0.825f;
					} else {
						Log.LogTrace("Player changed models, but no animator was present?");
					}
				}
			}
		}
		private static void OnCharacterBodyAwake(On.RoR2.CharacterBody.orig_Awake originalMethod, CharacterBody @this) {
			originalMethod(@this);
			if (@this.bodyIndex == ReaverIndex) {
				ROR2ObjectCreator.GloballySetJailerSkinTransparency(true);
				@this.gameObject.AddComponent<TransparencyController>();
			}
		}

		private static void OnRebuildMannequinInstance(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_RebuildMannequinInstance originalMethod, RoR2.SurvivorMannequins.SurvivorMannequinSlotController @this) {
			originalMethod(@this);
			if (@this.mannequinInstanceTransform != null && @this.currentSurvivorDef == Reaver) {
				Animator previewAnimator = @this.mannequinInstanceTransform.transform.Find("mdlNullifier").GetComponent<Animator>();
				previewAnimator.SetBool("isGrounded", true); // Fix an animation bug
				previewAnimator.SetFloat("Spawn.playbackRate", 1f);
				previewAnimator.Play("Spawn");
				Util.PlaySound(SpawnState.spawnSoundString, @this.mannequinInstanceTransform.gameObject);
				// EffectManager.SimpleMuzzleFlash(NullifierSpawnState.spawnEffectPrefab, @this.mannequinInstanceTransform.gameObject, "PortalEffect", false);
			}
		}
	}
}
