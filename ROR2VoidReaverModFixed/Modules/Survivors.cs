using EntityStates;
using KinematicCharacterController;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using ROR2HPBarAPI.API;
using ROR2VoidReaverModFixed.XanCode;
using ROR2VoidReaverModFixed.XanCode.Data;
using ROR2VoidReaverModFixed.XanCode.Image;
using ROR2VoidReaverModFixed.XanCode.Interop;
using UnityEngine;
using UnityEngine.Networking;
using XanVoidReaverEdit;
using SerializableEntityStateType = EntityStates.SerializableEntityStateType;
using SkillDef = RoR2.Skills.SkillDef;
using NullifierSpawnState = EntityStates.NullifierMonster.SpawnState;

namespace FubukiMods.Modules {

	/// <summary>
	/// The survivor main module. This was originally written by LuaFubuki but was rewritten and refactored by Xan.
	/// </summary>
	public class Survivors {

		private static SurvivorDef reaver;

		public static void Init(MainPlugin plugin) {
			GameObject playerBodyPrefab = Tools.CreateBody("PlayerNullifierBody", "RoR2/Base/Nullifier/NullifierBody.prefab");
			GameObject playerBodyLocator = PrefabAPI.InstantiateClone(playerBodyPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject, "PlayerNullifierBodyDisplay");
			playerBodyLocator.AddComponent<NetworkIdentity>();
			CharacterBody body = playerBodyPrefab.GetComponent<CharacterBody>();
			Interactor interactor = playerBodyPrefab.GetComponent<Interactor>();
			
			/*
			// Fix the falling animation starting.
			Animator previewAnimator = playerBodyLocator.transform.Find("mdlNullifier").GetComponent<Animator>();
			previewAnimator.SetBool("isGrounded", true);
			*/

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
			playerBodyPrefab.GetComponent<SetStateOnHurt>().canBeHitStunned = false;

			Log.LogTrace("Registering death state to network...");
			ContentAddition.AddEntityState<Skills.VoidDeath>(out _);
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
			body.sprintingSpeedMultiplier = Configuration.SprintSpeedMultiplier;
			body.levelMoveSpeed = Configuration.LevelMoveSpeed;
			body.baseAcceleration = Configuration.BaseAcceleration;
			body.baseJumpCount = Configuration.BaseJumpCount;
			body.baseJumpPower = Configuration.BaseJumpPower;
			body.baseAttackSpeed = Configuration.BaseAttackSpeed;
			body.levelAttackSpeed = Configuration.LevelAttackSpeed;
			body.baseNameToken = Lang.SURVIVOR_NAME;
			body.portraitIcon = CommonImages.Portrait.texture;
			ContentAddition.AddBody(playerBodyPrefab);
			Log.LogTrace("Finished setting up base stats and registering the body...");

			// Passive stuff:
			SkillLocator skillLoc = playerBodyPrefab.GetComponent<SkillLocator>();
			skillLoc.passiveSkill = default;
			skillLoc.passiveSkill.enabled = true;
			skillLoc.passiveSkill.keywordToken = Lang.PASSIVE_KEYWORD;
			skillLoc.passiveSkill.skillNameToken = Lang.PASSIVE_NAME;
			skillLoc.passiveSkill.skillDescriptionToken = Lang.PASSIVE_DESC;
			skillLoc.passiveSkill.icon = CommonImages.Passive;
			Log.LogTrace("Finished registering passive details...");

			// Primary, triple shot:
			ContentAddition.AddEntityState<Skills.VoidPrimarySequenceShot>(out _);
			SkillDef voidImpulsePrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidImpulsePrimary.activationState = new SerializableEntityStateType(typeof(Skills.VoidPrimarySequenceShot));
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
			voidImpulsePrimary.skillNameToken = Lang.SKILL_PRIMARY_TRIPLESHOT_NAME;
			voidImpulsePrimary.skillDescriptionToken = Lang.SKILL_PRIMARY_TRIPLESHOT_DESC;
			voidImpulsePrimary.icon = CommonImages.PrimaryTripleShotIcon;
			Tools.AddSkill(playerBodyPrefab, voidImpulsePrimary, "primary", 0);
			Log.LogTrace("Finished registering Void Impulse...");

			// Primary, spread shot:
			ContentAddition.AddEntityState<Skills.VoidPrimaryFiveSpread>(out _);
			SkillDef voidSpreadPrimary = ScriptableObject.CreateInstance<SkillDef>();
			voidSpreadPrimary.activationState = new SerializableEntityStateType(typeof(Skills.VoidPrimaryFiveSpread));
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
			voidSpreadPrimary.skillNameToken = Lang.SKILL_PRIMARY_SPREAD_NAME;
			voidSpreadPrimary.skillDescriptionToken = Lang.SKILL_PRIMARY_SPREAD_DESC;
			voidSpreadPrimary.icon = CommonImages.PrimarySpreadShotIcon;
			Tools.AddSkill(playerBodyPrefab, voidSpreadPrimary, "primary", 1);
			Log.LogTrace("Finished registering Void Spread...");

			// Secondary
			ContentAddition.AddEntityState<Skills.VoidSecondary>(out _);
			SkillDef undertowSecondary = ScriptableObject.CreateInstance<SkillDef>();
			undertowSecondary.activationState = new SerializableEntityStateType(typeof(Skills.VoidSecondary));
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
			undertowSecondary.skillNameToken = Lang.SKILL_SECONDARY_NAME;
			undertowSecondary.skillDescriptionToken = Lang.SKILL_SECONDARY_DESC;
			undertowSecondary.icon = CommonImages.SecondaryIcon;
			Tools.AddSkill(playerBodyPrefab, undertowSecondary, "secondary", 0);
			Log.LogTrace("Finished registering Undertow...");

			// Utility
			ContentAddition.AddEntityState<Skills.VoidUtility>(out _);
			SkillDef diveUtility = ScriptableObject.CreateInstance<SkillDef>();
			diveUtility.activationState = new SerializableEntityStateType(typeof(Skills.VoidUtility));
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
			diveUtility.skillNameToken = Lang.SKILL_UTILITY_NAME;
			diveUtility.skillDescriptionToken = Lang.SKILL_UTILITY_DESC;
			diveUtility.icon = CommonImages.UtilityIcon;
			Tools.AddSkill(playerBodyPrefab, diveUtility, "utility", 0);
			Log.LogTrace("Finished registering Dive...");

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
			Log.LogTrace("Finished registering Reave...");

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
			Log.LogTrace("Finished registering Collapse...");

			Tools.AddReaverSkins(playerBodyPrefab);
			Log.LogTrace("Finished registering skins...");

			SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
			survivorDef.bodyPrefab = playerBodyPrefab;
			survivorDef.descriptionToken = Lang.SURVIVOR_DESC;
			survivorDef.displayNameToken = Lang.SURVIVOR_NAME;
			survivorDef.outroFlavorToken = Lang.SURVIVOR_OUTRO;
			survivorDef.mainEndingEscapeFailureFlavorToken = Lang.SURVIVOR_OUTRO_FAILED;
			survivorDef.displayPrefab = playerBodyLocator;
			survivorDef.displayPrefab.transform.localScale = Vector3.one * 0.3f;
			survivorDef.primaryColor = new Color(0.5f, 0.5f, 0.5f);
			survivorDef.desiredSortPosition = 44.44f;
			reaver = survivorDef;
			ContentAddition.AddSurvivorDef(survivorDef);
			Log.LogTrace("Finished registering survivor...");

			Tools.FinalizeBody(playerBodyPrefab.GetComponent<SkillLocator>());

			if (Configuration.VoidImmunity) {
				Log.LogTrace("Void immunity is enabled. Registering callbacks.");
				On.RoR2.CharacterBody.SetBuffCount += InterceptBuffsEvent;
				On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForVoidResist;
			}
			if (Configuration.IsVoidDeathInstakill) {
				Log.LogTrace("Void death instakill is enabled. Registering callback.");
				On.RoR2.HealthComponent.TakeDamage += InterceptTakeDamageForInstakill;
			}
			if (Configuration.ReaveAndCollapseFriendlyFire) {
				Log.LogTrace("Reave and Collapse do friendly damage.");
				On.RoR2.Projectile.ProjectileController.DispatchOnInitialized += BeforeDispatchingProjectileInit;
			}
			Log.LogTrace("Adding hook to enforce the void death effect on Reave and Collapse.");
			On.RoR2.HealthComponent.TakeDamage += EnsureReaveAndCollapseAreVoidDeath;

			Log.LogTrace("Adding animation correction hooks for full size reaver.");
			On.EntityStates.BaseCharacterMain.UpdateAnimationParameters += OnAnimationParametersUpdated;
			On.RoR2.CameraRigController.GenerateCameraModeContext += OnGeneratingCameraModeContext;

			Log.LogTrace("Adding custom armor reduction effect.");
			On.RoR2.CharacterBody.RecalculateStats += InterceptRecalculateStats;

			BodyCatalog.availability.CallWhenAvailable(() => {
				Registry.RegisterColorProvider(plugin, body.bodyIndex, new HPBarColorMarshaller());
				Log.LogTrace("Custom Void-Style HP Bar colors registered.");
			});

			On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.RebuildMannequinInstance += OnRebuildMannequinInstance;

			Log.LogTrace("Survivor setup completed.");
		}
#pragma warning disable Publicizer001

		private static void BeforeDispatchingProjectileInit(On.RoR2.Projectile.ProjectileController.orig_DispatchOnInitialized originalMethod, ProjectileController @this) {
			DamageAPI.ModdedDamageTypeHolderComponent dmg = @this.gameObject.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
			if (dmg != null && dmg.Has(XanConstants.DetainorReaveDamage)) {
				Log.LogTrace("A projectile with Reave was created. Setting its team to none.");
				TeamFilter filter = @this.gameObject.AddComponent<TeamFilter>();
				filter.teamIndex = TeamIndex.Neutral;
				@this.teamFilter = filter;
			}
			originalMethod(@this);
		}

		/// <summary>
		/// Ensures that Reave and Collapse both cause the VFX associated with VoidDeath. This goes over a hurdle with Reave specifically
		/// that prevents it from being able to use the VoidDeath type (this causes an instakill, and reave does not instakill).
		/// Still, if reave kills, it needs to look like the instakill, so this handles that edge case.
		/// </summary>
		/// <param name="originalMethod"></param>
		/// <param name="this"></param>
		/// <param name="damageInfo"></param>
		private static void EnsureReaveAndCollapseAreVoidDeath(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			// Now instakill will be handled by another callback in this code (see InterceptTakeDamageForInstakill).
			// We just need to handle death effects.
			if (damageInfo.HasModdedDamageType(XanConstants.DetainorReaveDamage)) {

				// Real quick:
				Log.LogTrace("Set proc coefficient to 0.");
				damageInfo.procCoefficient = 0; // Prevent void crit glasses from working (and anything else for that matter).
				originalMethod(@this, damageInfo);

				if (!@this.alive && @this.wasAlive) {
					// We just died, and the attack classified as reave/collapse
					// Depending on user pref, we might need to spawn the effect.
					// Regardless of this, we need to quickly and (im)politely edit the last damage type for the character death.
					// However, we should *not* do this if the receiver is immune to void death.
					if (!@this.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToVoidDeath)) {
						Log.LogTrace("No immunity to void death. The character was just killed, so now I will quitely add the void death type to the killing damage type.");
						@this.killingDamageType |= DamageType.VoidDeath;
						// There, now all default death systems (and systems that choose to care about this property) will handle it appropriately.
					}

					if (Configuration.ExaggeratedReaveAndCollapse) {
						Log.LogTrace("Exaggerated detain/reave deaths are on. Spawning the special effect (if possible).");
						// Spawn the cool VFX if the user opted into that.
						Vector3 pos = @this.body.corePosition;
						float radius = @this.body.bestFitRadius;

						CharacterBody attacker = damageInfo.attacker?.GetComponent<CharacterBody>();
						if (attacker != null) {
							// In case the question comes up wrt "what if it already happened?"
							// Reave and Collapse have a 0 proc coefficient. It can't.
							EffectManager.SpawnEffect(
								XanConstants.SilentVoidCritDeathEffect,
								new EffectData {
									origin = pos,
									scale = radius
								},
								true
							);
						}
					}
				}

				return;
			}

			originalMethod(@this, damageInfo);
		}

		private static void OnAnimationParametersUpdated(On.EntityStates.BaseCharacterMain.orig_UpdateAnimationParameters originalMethod, BaseCharacterMain @this) {
			if (!@this.hasCharacterBody) {
				originalMethod(@this);
				return;
			}

			CharacterBody body = @this.characterBody;
			if (body.baseNameToken != Lang.SURVIVOR_NAME) {
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
			if (@this.targetBody.baseNameToken != Lang.SURVIVOR_NAME) {
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
				if (body.baseNameToken == Lang.SURVIVOR_NAME) {
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

		private static void InterceptTakeDamageForInstakill(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			if (damageInfo.HasModdedDamageType(XanConstants.ReaveDamage)) {
				bool isBoss = @this.body.isBoss;
				bool canInstakill = (!isBoss) || (isBoss && Configuration.AllowInstakillOnBosses);
				// canInstakill if:
				// 1: The feature is on (this hook is not done if the feature is off), AND
				// 2a: The character is not a boss, OR
				// 2b: The character is a boss, and instakilling bosses is allowed
				// 3: It's not me

				canInstakill = canInstakill && (damageInfo.attacker != @this.gameObject);

				if (canInstakill) {
					Log.LogTrace($"Instakill is good to go. Goodbye, {@this.body.GetDisplayName()}");
					damageInfo.damageType |= DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection;
					damageInfo.damage = @this.combinedHealth * 4;
				} else {
					Log.LogTrace("Instakill was rejected. Entity is a boss and instakill bosses is off.");
				}
			}
			originalMethod(@this, damageInfo);
		}

		private static void InterceptTakeDamageForVoidResist(On.RoR2.HealthComponent.orig_TakeDamage originalMethod, HealthComponent @this, DamageInfo damageInfo) {
			if (damageInfo.rejected) {
				originalMethod(@this, damageInfo);
				return;
			}

			if (@this.body != null && @this.body.baseNameToken == Lang.SURVIVOR_NAME) {
				if (damageInfo.attacker == null && damageInfo.inflictor == null && damageInfo.damageType == (DamageType.BypassBlock | DamageType.BypassArmor)) {
					Log.LogTrace("Rejecting damage for what I believe to be Void atmosphere damage (it has no source/attacker, and the damage type bypasses blocks and armor only).");
					damageInfo.rejected = true;
				}
			}

			originalMethod(@this, damageInfo);
		}

		private static void InterceptBuffsEvent(On.RoR2.CharacterBody.orig_SetBuffCount originalMethod, CharacterBody @this, BuffIndex buffType, int newCount) {
			if (@this.baseNameToken == Lang.SURVIVOR_NAME) {
				if (buffType == MegaVoidFog || buffType == NormVoidFog || buffType == WeakVoidFog) {
					Log.LogTrace("Rejecting attempt to add fog to player's status effects.");
					originalMethod(@this, buffType, 0); // Always 0
					return;
				}
			}

			originalMethod(@this, buffType, newCount);
		}

		private static void InterceptRecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats originalMethod, CharacterBody @this) {
			originalMethod(@this);
			if (@this.HasBuff(XanConstants.DetainInstability)) {
				@this.armor -= Configuration.DetainWeaknessArmorReduction;
			}
		}
		
		private static void OnRebuildMannequinInstance(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_RebuildMannequinInstance originalMethod, RoR2.SurvivorMannequins.SurvivorMannequinSlotController @this) {
			originalMethod(@this);
			if (@this.mannequinInstanceTransform != null && @this.currentSurvivorDef == reaver) {
				Animator previewAnimator = @this.mannequinInstanceTransform.transform.Find("mdlNullifier").GetComponent<Animator>();
				previewAnimator.SetBool("isGrounded", true); // Fix an animation bug
				previewAnimator.SetFloat("Spawn.playbackRate", 1f);
				previewAnimator.Play("Spawn");
				Util.PlaySound(NullifierSpawnState.spawnSoundString, @this.mannequinInstanceTransform.gameObject);
				// EffectManager.SimpleMuzzleFlash(NullifierSpawnState.spawnEffectPrefab, @this.mannequinInstanceTransform.gameObject, "PortalEffect", false);
			}
		}

#pragma warning restore Publicizer001

		#region Values To Remember

		/// <summary>
		/// A reference to the highest power void fog.
		/// </summary>
		public static BuffIndex MegaVoidFog {
			get {
				if (_megaVoidFog == BuffIndex.None) {
					_megaVoidFog = DLC1Content.Buffs.VoidRaidCrabWardWipeFog.buffIndex;
				}
				return _megaVoidFog;
			}
		}

		/// <summary>
		/// A reference to ordinary void fog.
		/// </summary>
		public static BuffIndex NormVoidFog {
			get {
				if (_normVoidFog == BuffIndex.None) {
					_normVoidFog = RoR2Content.Buffs.VoidFogStrong.buffIndex;
				}
				return _normVoidFog;
			}
		}

		/// <summary>
		/// A reference to weak void fog.
		/// </summary>
		public static BuffIndex WeakVoidFog {
			get {
				if (_weakVoidFog == BuffIndex.None) {
					_weakVoidFog = RoR2Content.Buffs.VoidFogMild.buffIndex;
				}
				return _weakVoidFog;
			}
		}

		private static BuffIndex _megaVoidFog = BuffIndex.None;
		private static BuffIndex _normVoidFog = BuffIndex.None;
		private static BuffIndex _weakVoidFog = BuffIndex.None;

		#endregion
	}
}
