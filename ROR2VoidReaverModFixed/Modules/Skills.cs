using EntityStates;
using EntityStates.GlobalSkills.LunarNeedle;
using EntityStates.Huntress;
using EntityStates.Missions.Arena.NullWard;
using EntityStates.NullifierMonster;
using HG;
using RoR2;
using RoR2.Projectile;
using ROR2VoidReaverModFixed.XanCode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using XanVoidReaverEdit;
using BlinkState = EntityStates.ImpMonster.BlinkState;
using DeathState = EntityStates.NullifierMonster.DeathState;

namespace FubukiMods.Modules {

	/// <summary>
	/// This class contains all skill information. It was originally written by LuaFubuki but has been
	/// practically rewritten by Xan.
	/// </summary>
	public static class Skills {

		#region Projectile Aliases

#pragma warning disable Publicizer001
		/// <summary>
		/// Code originally defined by LuaFubuki, but turned into a function by Xan.<br/>
		/// Constructs a <see cref="FireProjectileInfo"/> that represents the Reaver's primary attack.
		/// </summary>
		/// <param name="aimRay">The ray representing the projectile's aim.</param>
		/// <param name="commonVoidPrimary">The common base class for the two variations of the primary attack.</param>
		/// <returns></returns>
		private static FireProjectileInfo GetVoidPrimaryFireInfo(Ray aimRay, CommonVoidPrimary commonVoidPrimary) {
			FireProjectileInfo voidPrimaryProjectile = default;
			voidPrimaryProjectile.position = aimRay.origin;
			voidPrimaryProjectile.rotation = Quaternion.LookRotation(aimRay.direction);
			voidPrimaryProjectile.crit = commonVoidPrimary.RollCrit();
			voidPrimaryProjectile.damage = commonVoidPrimary.damageStat * commonVoidPrimary.Damage;
			voidPrimaryProjectile.owner = commonVoidPrimary.gameObject;
			voidPrimaryProjectile.procChainMask = default;
			voidPrimaryProjectile.force = 0f;
			voidPrimaryProjectile.useFuseOverride = false;
			voidPrimaryProjectile.useSpeedOverride = false;
			voidPrimaryProjectile.target = null;
			voidPrimaryProjectile.projectilePrefab = commonVoidPrimary.Projectile;
			return voidPrimaryProjectile;
		}

		/// <summary>
		/// Similar to <see cref="GetVoidPrimaryFireInfo(Ray, CommonVoidPrimary)"/>, but designed for getting the reave explosion instead.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="projectile"></param>
		/// <param name="muzzleTransform"></param>
		/// <param name="damageMultiplier"></param>
		/// <param name="canCrit"></param>
		/// <param name="instakill"></param>
		/// <returns></returns>
		public static FireProjectileInfo GetVoidExplosionFireInfo(BaseState state, GameObject projectile, Transform muzzleTransform, float damageMultiplier, bool canCrit = true, bool instakill = false) {
			FireProjectileInfo deletionProjectile = default;
			deletionProjectile.projectilePrefab = projectile;
			deletionProjectile.position = muzzleTransform.position;
			deletionProjectile.rotation = Quaternion.identity;
			deletionProjectile.owner = state.gameObject;
			deletionProjectile.damage = state.damageStat * damageMultiplier;
			deletionProjectile.crit = canCrit && state.characterBody.RollCrit();

			if (instakill) {
				deletionProjectile.damageTypeOverride = DamageType.VoidDeath;
				deletionProjectile.damage = 1000000000;
				deletionProjectile.damageColorIndex = DamageColorIndex.Void;
			}

			return deletionProjectile;
		}
#pragma warning restore Publicizer001

		#endregion

		#region Primary Fire


		/// <summary>
		/// A common base for the two primary attacks.
		/// </summary>
		public abstract class CommonVoidPrimary : BaseState {
			/// <summary>
			/// A reference to the primary projectile.
			/// </summary>
			public GameObject Projectile => MainPlugin.PrimaryProjectile;

			/// <summary>
			/// The default cooldown of this ability with no attack speed. This should be the same as that of the cooldown defined in the survivor.
			/// </summary>
			public float BaseDuration { get; protected set; } = 1f;

			/// <summary>
			/// The effective duration of this ability. This may be shorter than the cooldown, and should be affected by attack speed where applicable.
			/// tries to complete in.
			/// </summary>
			public float Duration { get; protected set; }

			/// <summary>
			/// The damage done per tick. It is default multiplied by 0.5 because it does two half ticks of damage.
			/// </summary>
			public float Damage { get; protected set; } = 0.5f * Configuration.BasePrimaryDamage;
		}

		/// <summary>
		/// This controls the triple-shot ability.
		/// </summary>
		public class VoidPrimarySequenceShot : CommonVoidPrimary {

			private const float RECOIL = 0f;
			
			/// <summary>
			/// Keeps track of the current bullet being fired.
			/// </summary>
			private int _bulletIndex = 0;

			/// <summary>
			/// The amount of bullets that are present when a cluster of bullets is being fired due to requiring more than one for that shot.
			/// </summary>
			private int _numBulletsPerCluster = 0;

			/// <summary>
			/// The number of clusters is always the same as the user's setting. This is the number of stray bullets that occur after the cluster.
			/// This value will always be less than the user's setting for the number of bullets to fire per shot.
			/// </summary>
			private int _numStrayBullets = 0;

			/// <summary>
			/// The amount of bullets per shot, this is set to the configuration.
			/// </summary>
			private int _bulletsPerStringShot = 0;

			/// <summary>
			/// The total number of shots to fire.
			/// </summary>
			private int _totalNumberOfShots = 0;

			/// <summary>
			/// This is set to the user-defined bullet deviation when the state is entered.
			/// </summary>
			private Vector2 _spread = default;

			public override void OnEnter() {
				base.OnEnter();
				Duration = BaseDuration;
				_bulletsPerStringShot = Configuration.BulletsPerImpulseShot;
				_bulletIndex = 0;
				_spread = Configuration.PrimaryImpulseSpread;
				if (Configuration.UseExperimentalSequenceShotBuff) {
					// This is a bit complicated because these behaviors are so different. The names of the variables should help, as well as their documentation.
					// It may help to think of each shot as a "slot" in a sequence.
					// Alongside the 3 (or whatever config says) slots, up to 2 (n.b. "2" comes from 3-1, its always config-1) can trail after.
					int totalNumberOfBullets = Mathf.RoundToInt(_bulletsPerStringShot * attackSpeedStat);
					_numBulletsPerCluster = Mathf.FloorToInt(totalNumberOfBullets / (float)_bulletsPerStringShot);
					_numStrayBullets = totalNumberOfBullets % _bulletsPerStringShot;
					_totalNumberOfShots = _bulletsPerStringShot + _numStrayBullets;
				} else {
					_totalNumberOfShots = Mathf.RoundToInt(_bulletsPerStringShot * attackSpeedStat);
				}
			}

			public override void FixedUpdate() {
				base.FixedUpdate();
				if (_bulletIndex < _totalNumberOfShots) {
					double atkSpeedMod, nextBulletShotAge;
					if (Configuration.UseExperimentalSequenceShotBuff) {
						nextBulletShotAge = Duration * (Configuration.PrimaryImpulseShotTime / _totalNumberOfShots) * _bulletIndex;
					} else {
						atkSpeedMod = (Configuration.PrimaryImpulseShotTime / _bulletsPerStringShot) / attackSpeedStat;
						nextBulletShotAge = Duration * atkSpeedMod * _bulletIndex;
					}
					bool isTimeToFire = (double)fixedAge >= nextBulletShotAge;
					if (isTimeToFire) {
						if (isAuthority) {
							// Using experiment
							bool isMultiShot = Configuration.UseExperimentalSequenceShotBuff && (_bulletIndex < _bulletsPerStringShot);
							Ray aimRay = GetAimRay();
							aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 1, 1, 0f, 0f);
							AddRecoil(-0.4f * RECOIL, -0.8f * RECOIL, -0.3f * RECOIL, 0.3f * RECOIL);

							if (isMultiShot) {
								for (int j = 0; j < _numBulletsPerCluster; j++) {
									ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
									aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 0.5f, 0.5f, 0f, 0f);
									// Note: It is intentional that this affects the already spread direction.
								}
							} else {
								ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
							}
						}
						// TODO: Play multiple sounds if it's a multi-shot? This could quickly devolve into noise hell, so no for now.
						Util.PlaySound(FireLunarNeedle.fireSound, gameObject);
						_bulletIndex++;
					}
				}
				bool timeToExit = isAuthority && fixedAge >= Duration;
				if (timeToExit) {
					outer.SetNextStateToMain();
				}
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.Skill;
			}
		}

		/// <summary>
		/// This controls the five-shot fan.
		/// </summary>
		public class VoidPrimaryFiveSpread : CommonVoidPrimary {

			private Vector2 _spread;

			public override void OnEnter() {
				base.OnEnter();
				Duration = BaseDuration / attackSpeedStat;
				_spread = Configuration.PrimarySpreadShotSpread;
				Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 0.75f);
				Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 1f);
				Util.PlayAttackSpeedSound(FireLunarNeedle.fireSound, gameObject, 1.25f);
				if (isAuthority) {
					float arcLen = Configuration.SpreadShotArcLengthDegs;
					int numBullets = Configuration.BulletsPerSpreadShot;
					float increment = arcLen / numBullets;
					float horzSpreadFactor = (-arcLen / 2) + (increment / 2);
					for (int i = 0; i < numBullets; i++) {
						Ray aimRay = GetAimRay();
						aimRay.direction = Util.ApplySpread(aimRay.direction, _spread.x, _spread.y, 1f, 1f, horzSpreadFactor, 0f);
						horzSpreadFactor += increment;
						ProjectileManager.instance.FireProjectile(GetVoidPrimaryFireInfo(aimRay, this));
						AddRecoil(-0.1f, -0.2f, -0.075f, 0.075f);
					}
				}
			}

			public override void FixedUpdate() {
				base.FixedUpdate();
				bool timeToExit = isAuthority && fixedAge >= Duration;
				if (timeToExit) {
					outer.SetNextStateToMain();
				}
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.Skill;
			}
		}

		#endregion

		/// <summary>
		/// This controls the Void Portal attack that Reavers use, albeit in its player form.
		/// </summary>
		public class VoidSecondary : BaseState {

			private const float MAX_AIM_DISTANCE = 200f;

			private readonly float BASE_RAND_RADIUS = Configuration.SecondarySpread;

			private readonly int BASE_BOMB_COUNT = Configuration.SecondaryCount;

			private readonly float BOMB_DAMAGE = Configuration.BaseSecondaryDamage;

			private GameObject Projectile => MainPlugin.SecondaryProjectile;

			/// <summary>
			/// The effective radius as influenced by attack speed.
			/// </summary>
			private float _realRandRadius;

			/// <summary>
			/// The effective bomb count as influenced by attack speed.
			/// </summary>
			private int _realBombCount;

			/// <summary>
			/// The aim helper that renders on screen.
			/// </summary>
			private GameObject _areaSphere;

			public override void OnEnter() {
				base.OnEnter();
				_realRandRadius = BASE_RAND_RADIUS * (0.75f + attackSpeedStat * 0.25f);
				_realBombCount = Mathf.RoundToInt(attackSpeedStat * BASE_BOMB_COUNT);
				if (isAuthority) {
					_areaSphere = Object.Instantiate(ArrowRain.areaIndicatorPrefab);
					_areaSphere.transform.localScale = Vector3.one * _realRandRadius;
				}
				Util.PlaySound(sfxLocator.barkSound, gameObject);
			}

			public override void Update() {
				base.Update();
				bool isAuthority = base.isAuthority;
				if (isAuthority) {
					LayerMask hitMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;
					bool hit = Physics.Raycast(GetAimRay(), out RaycastHit castResult, MAX_AIM_DISTANCE, hitMask);
					if (hit) {
						_areaSphere.transform.position = castResult.point;
						_areaSphere.transform.up = castResult.normal;
						_areaSphere.transform.localScale = Vector3.one * _realRandRadius;
					} else {
						_areaSphere.transform.localScale = Vector3.zero;
					}
				}
			}

			public override void FixedUpdate() {
				base.FixedUpdate();
				bool hasReleasedKey = isAuthority && !inputBank.skill2.down;
				if (hasReleasedKey) {
					if (isAuthority) {
						Ray aimRay = GetAimRay();
						bool hit = Physics.Raycast(aimRay, out RaycastHit castResult, MAX_AIM_DISTANCE);
						if (hit) {
							for (int i = 0; i < _realBombCount; i++) {
								Vector3 randomDirection = Random.insideUnitSphere * _realRandRadius;
								randomDirection.y = 0f;
								bool isFirstBomb = i == 0;
								if (isFirstBomb) {
									randomDirection = Vector3.zero;
								}
								Vector3 randomSpread = castResult.point + Vector3.up * _realRandRadius + randomDirection;
								float randDiameter = _realRandRadius * 2f;
								LayerMask mask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;
								bool hitSpread = Physics.Raycast(randomSpread, Vector3.down, out RaycastHit randomSpreadHit, randDiameter, mask);
								if (hitSpread) {
									randomSpread = randomSpreadHit.point;
								} else {
									randomSpread += Vector3.down * Random.Range(0f, randDiameter);
								}
								ProjectileManager.instance.FireProjectile(
									Projectile,
									randomSpread,
									Quaternion.identity,
									gameObject,
									damageStat * BOMB_DAMAGE,
									200f,
									Util.CheckRoll(critStat, characterBody.master),
									DamageColorIndex.Void
								);
							}
						} else {
							skillLocator.secondary.stock++;
						}
						outer.SetNextStateToMain();
					}
				}
			}

			public override void OnExit() {
				if (isAuthority) {
					Destroy(_areaSphere.gameObject);
				}
				EffectManager.SimpleMuzzleFlash(FirePortalBomb.muzzleflashEffectPrefab, gameObject, FirePortalBomb.muzzleString, true);
				base.OnExit();
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.PrioritySkill;
			}
		}

		/// <summary>
		/// The "Dive" ability. This makes the player invisible, moves them in a predetermined direction based on either their movement
		/// </summary>
		public class VoidUtility : BaseState {

			private const float EFFECT_DURATION = 1f;

			// TODO: Make this configurable?
			private const float SPEED_BOOST = 4f;

			private float _currentTime;

			private Vector3 _moveVector = Vector3.zero;

			private float _yVelocity = 0f;

			private CharacterModel _characterModel;

			private HurtBoxGroup _hurtboxGroup;

			public override void OnEnter() {
				// Test: Start aim mode for a single frame and snap to that rotation before doing this ability.
				// Reason: When not moving, it falls back to the rotation of the character. A recent change made the rotation of the character
				// not necessarily match the direction that the player is attacking in, which may cause an unexpected move direction when using this utility.
				// By forcing the character to align with the camera just before activating this, they will now move in the direction of their camera, which
				// feels more natural to the average joe.
				StartAimMode(Time.fixedDeltaTime, true);

				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = characterBody.bestFitRadius
				}, false);
				Util.PlayAttackSpeedSound(BlinkState.beginSoundString, gameObject, 0.6f);
				Util.PlayAttackSpeedSound(BlinkState.beginSoundString, gameObject, 0.7f);
				base.OnEnter();
				Transform modelTransform = GetModelTransform();
				if (modelTransform != null) {
					_characterModel = modelTransform.GetComponent<CharacterModel>();
					_hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
				}
				if (_characterModel != null) {
					_characterModel.invisibilityCount++;
				}
				if (_hurtboxGroup != null) {
					_hurtboxGroup.hurtBoxesDeactivatorCounter++;
				}
				if (NetworkServer.active) {
					characterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, EFFECT_DURATION);
					characterBody.AddTimedBuff(RoR2Content.Buffs.CrocoRegen, 0.75f);
				}
				_moveVector = ((inputBank.moveVector == Vector3.zero) ? characterDirection.forward : inputBank.moveVector).normalized;
				_yVelocity = characterMotor.velocity.y;
			}

			public override void FixedUpdate() {
				base.FixedUpdate();
				_currentTime += Time.fixedDeltaTime;
				if (characterMotor != null && characterDirection != null) {
					characterMotor.velocity = Vector3.zero;
					characterMotor.rootMotion += _moveVector * (moveSpeedStat * SPEED_BOOST * Time.fixedDeltaTime) * Mathf.Sin(_currentTime / EFFECT_DURATION * 2.3561945f); // 135deg
					characterMotor.rootMotion += new Vector3(0f, _yVelocity * Time.fixedDeltaTime * Mathf.Cos(_currentTime / EFFECT_DURATION * 1.57079637f), 0f);
				}
				bool timeToExit = _currentTime >= EFFECT_DURATION && isAuthority;
				if (timeToExit) {
					outer.SetNextStateToMain();
				}
			}

			public override void OnExit() {
				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = characterBody.bestFitRadius
				}, false);
				Util.PlayAttackSpeedSound(BlinkState.endSoundString, gameObject, 1f);
				if (_characterModel != null) {
					_characterModel.invisibilityCount--;
				}
				if (_hurtboxGroup != null) {
					_hurtboxGroup.hurtBoxesDeactivatorCounter--;
				}
				base.OnExit();
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.Frozen;
			}
		}

		#region Special Ability

		/// <summary>
		/// This is the "on-demand" special, also known as Reave.
		/// This variant does a fixed percentage of damage to the current health of the character, and spawns the Reaver's death explosion.<br/>
		/// <br/>
		/// The reaver explosion is composed of two parts. The first is the radial effect ("the black hole"), and the second is the explosion at the end ("the portal").
		/// Keep these terms in mind when looking at the rest of the code, as they will show up elsewhere.
		/// </summary>
		public class VoidSpecialOnDemand : BaseState {

			/// <summary>
			/// A reference to the projectile that spawns the explosion effect.
			/// </summary>
			public GameObject Projectile => MainPlugin.ReaveProjectile;

			/// <summary>
			/// This is used to late-fire the portal effect in FixedUpdate, which is the little poof that happens after the black hole.
			/// </summary>
			private bool _hasCreatedVoidPortalEffect = false;

			/// <summary>
			/// The amount of damage that will be done to the local player.
			/// </summary>
			private float _selfDamage;

			/// <summary>
			/// The amount of ticks to do the damage in.
			/// </summary>
			private int _ticks;

			/// <summary>
			/// This controls the zoom effect when looking at the black hole.
			/// </summary>
			private CameraTargetParams.CameraParamsOverrideRequest _zoomOutParams;

			/// <summary>
			/// The references the zoom effect itself.
			/// </summary>
			private CameraTargetParams.CameraParamsOverrideHandle _zoomOutHandle;

			public override void OnEnter() {
				base.OnEnter();
				_selfDamage = healthComponent.combinedHealth * Configuration.ReaveCost;
				_ticks = 0;
				_zoomOutParams = default;
				_zoomOutParams.cameraParamsData.idealLocalCameraPos = new Vector3(0f, 1f, -30f);
				_zoomOutParams.cameraParamsData.pivotVerticalOffset = 0f;
				_zoomOutHandle = GetComponent<CameraTargetParams>().AddParamsOverride(_zoomOutParams, 2f);
				Transform muzzleTransform = FindModelChild(DeathState.muzzleName);
				PlayCrossfade("Body", "Death", "Death.playbackRate", VoidDeath.REAVE_DURATION, 0.1f);
				if (isAuthority) {
					if (muzzleTransform != null && isAuthority) {
						ProjectileManager.instance.FireProjectile(GetVoidExplosionFireInfo(this, Projectile, muzzleTransform, Configuration.BaseSpecialDamage, true));
					}
				}
				if (Configuration.ReaveImmunity) {
					HurtBoxGroup component = GetModelTransform().GetComponent<HurtBoxGroup>();
					component.hurtBoxesDeactivatorCounter++;
				}
			}

			public override void FixedUpdate() {
				bool doSelfDamage = NetworkServer.active && fixedAge >= VoidDeath.REAVE_DURATION * 0.2f * _ticks;
				if (doSelfDamage) {
					DamageInfo damageInfo = new DamageInfo {
						attacker = gameObject,
						crit = false,
						damage = _selfDamage / 3f,
						damageType = DamageType.NonLethal | DamageType.BypassArmor | DamageType.BypassBlock,
						inflictor = gameObject,
						position = transform.position,
						procCoefficient = 0f
					};
					healthComponent.TakeDamage(damageInfo);
					_ticks++;
				}
				bool isEndOfEffect = fixedAge >= VoidDeath.REAVE_DURATION;
				if (isEndOfEffect) {
					if (!_hasCreatedVoidPortalEffect) {
						_hasCreatedVoidPortalEffect = true;
						base.PlayAnimation("Gesture, Additive", "Empty");
						base.PlayAnimation("Gesture, Override", "Empty");
						outer.SetNextStateToMain();
						return;
					}
				}
				characterMotor.velocity = Vector3.zero;
				base.FixedUpdate();
			}

			public override void OnExit() {
				GetComponent<CameraTargetParams>().RemoveParamsOverride(_zoomOutHandle, 1f);
				float scale = 30f;
				EffectManager.SpawnEffect(GenericCharacterDeath.voidDeathEffect, new EffectData {
					origin = characterBody.corePosition,
					scale = scale
				}, false);
				if (Configuration.ReaveImmunity) {
					HurtBoxGroup component = GetModelTransform().GetComponent<HurtBoxGroup>();
					component.hurtBoxesDeactivatorCounter--;
				}
				base.OnExit();
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.Death;
			}

		}

		/// <summary>
		/// The Collapse ability. This one kills you. That's all it does.
		/// </summary>
		public class VoidSpecialSuicide : BaseState {

			/// <summary>
			/// Press R to drink bleach flavored toaster bath water (gamer girl certified)
			/// </summary>
			public override void OnEnter() {
				characterBody.gameObject.GetComponent<HealthComponent>().Suicide(gameObject, gameObject, DamageType.BypassArmor | DamageType.BypassBlock | DamageType.Silent);
			}

		}

		#endregion

		/// <summary>
		/// Designed by LuaFubuki, modified by Xan.<br/>
		/// The original function only created the portal. The edit now causes the death explosion as well.
		/// </summary>
		public class VoidDeath : GenericCharacterDeath {

			/// <summary>
			/// The settings indicate to the user that setting the damage boost to a value greater than or equal to this number will
			/// override the void death effect to work just like normal void enemies, instakilling anything in range.
			/// </summary>
			public const float VOID_DELETION_THRESHOLD = 1000000f;

			/// <summary>
			/// The duration that the effect lasts for. This is something the game itself defines.
			/// </summary>
			public const float REAVE_DURATION = 2.5f;

			public override void OnEnter() {
				base.OnEnter();
				PlayCrossfade("Body", "Death", "Death.playbackRate", REAVE_DURATION, 0.1f);

				if (isAuthority) {
					// Fall back to the game object's transform on death. I don't think the muzzle goes away but /shrug
					Transform muzzleTransform = FindModelChild(DeathState.muzzleName).Or(gameObject.transform);
					if (muzzleTransform != null) {
						ProjectileManager.instance.FireProjectile(GetVoidExplosionFireInfo(
							this,
							MainPlugin.ReaveProjectile,
							muzzleTransform,
							Configuration.BaseDeathDamage,
							false,
							false
						));
					} else {
						Log.LogError("Missing any transform to fire the Reave projectile from!");
					}
					EffectManager.SpawnEffect(voidDeathEffect, new EffectData {
						origin = characterBody.corePosition,
						scale = 4f // TODO: I believe this needs to be changed in the real scale option.
					}, true);
				}

				if (cachedModelTransform) {
					// This is what the VoidDeath type does, I just have this delayed.
					Object.Destroy(cachedModelTransform.gameObject, 2.5f);
					cachedModelTransform = null;
				}
			}
		}

		/// <summary>
		/// Not yet implemented. This is the legacy version of the primary attack that used Lunar effects.
		/// </summary>
		public class VoidLunarPrimary : BaseState {

			private float _duration;

			public override void OnEnter() {
				base.OnEnter();
				_duration = 0.1f / attackSpeedStat;
				if (isAuthority) {
					Ray aimRay = GetAimRay();
					aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, FireLunarNeedle.maxSpread, 1f, 1f, 0f, 0f);
					FireProjectileInfo fireProjectileInfo = default;
					fireProjectileInfo.position = aimRay.origin;
					fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
					fireProjectileInfo.crit = characterBody.RollCrit();
					fireProjectileInfo.damage = characterBody.damage * FireLunarNeedle.damageCoefficient;
					fireProjectileInfo.damageColorIndex = 0;
					fireProjectileInfo.owner = gameObject;
					fireProjectileInfo.procChainMask = default;
					fireProjectileInfo.force = 0f;
					fireProjectileInfo.useFuseOverride = false;
					fireProjectileInfo.useSpeedOverride = false;
					fireProjectileInfo.target = null;
					fireProjectileInfo.projectilePrefab = FireLunarNeedle.projectilePrefab;
					ProjectileManager.instance.FireProjectile(fireProjectileInfo);
				}
				float recoilAmplitude = FireLunarNeedle.recoilAmplitude;
				AddRecoil(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
				characterBody.AddSpreadBloom(FireLunarNeedle.spreadBloomValue);
				// StartAimMode(2f, false);
				EffectManager.SimpleMuzzleFlash(FireLunarNeedle.muzzleFlashEffectPrefab, gameObject, "Head", false);
				Util.PlaySound(FireLunarNeedle.fireSound, gameObject);
			}

			public override void FixedUpdate() {
				base.FixedUpdate();
				bool isTimeToExit = isAuthority && fixedAge >= _duration;
				if (isTimeToExit) {
					outer.SetNextStateToMain();
				}
			}

			public override InterruptPriority GetMinimumInterruptPriority() {
				return InterruptPriority.PrioritySkill;
			}
		}

		/// <summary>
		/// Not yet implemented. This is a duplicate of the pre-existing lunar utility replacement that I don't remember the name of.
		/// </summary>
		public class VoidLunarUtility : GhostUtilitySkillState { }
	}
}
