using System.Collections.Generic;
using XanVoidReaverEdit;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ROR2VoidReaverModFixed.XanCode;
using ROR2VoidReaverModFixed.XanCode.Image;
using static ROR2VoidReaverModFixed.XanCode.Data.IconTexJobImproved;
using ROR2VoidReaverModFixed.XanCode.Data;

namespace FubukiMods.Modules {

	/// <summary>
	/// Originally written by LuaFubuki, improved by Xan
	/// </summary>
	public static class Tools {
		/// <summary>
		/// Added by Xan.<br/>
		/// Converts a char 0-9 and a-f into a numeric value for its hex counterpart.
		/// </summary>
		/// <param name="c">The char to convert.</param>
		/// <returns></returns>
		public static int Hex2Num(char c) {
			return int.Parse(c.ToString(), System.Globalization.NumberStyles.HexNumber);
		}

		/// <summary>
		/// Designed by LuaFubuki, modified very slightly by Xan.<para/>
		/// Takes in an encoded texture as a string that is a 16-color palette lookup table. The string's length is identical to the number of pixels.
		/// </summary>
		/// <param name="textureData">A list of hex digits that correspond to an entry in <paramref name="palette"/>. The length of this string must be a power of two.</param>
		/// <param name="palette">An array of 16 colors that correspond to a hex digit in textureData.</param>
		/// <param name="flipY">If true, the texture is flipped on its Y axis. If false, it is flipped on its X axis.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">If the length of <paramref name="textureData"/> is not a power of two.</exception>
		public static Sprite SpriteFromString(string textureData, Color[] palette, bool flipY = false) {
			if (!Mathf.IsPowerOfTwo(textureData.Length)) throw new System.ArgumentException("The amount of characters is not a power of two.", nameof(textureData));

			float resolution = Mathf.Floor(Mathf.Sqrt(textureData.Length));
			int resolutionI = (int)resolution;
			Texture2D tex = new Texture2D(resolutionI, resolutionI, TextureFormat.RGBA32, false);
			tex.filterMode = FilterMode.Point;
			tex.wrapMode = TextureWrapMode.Clamp;
			int currentPixelIndex = 0;
			for (int y = 0; y < resolution; y++) {
				for (int x = 0; x < resolution; x++) {
					char px = textureData[currentPixelIndex];
					if (flipY) {
						tex.SetPixel(x, resolutionI - 1 - y, palette[Hex2Num(px)]);
					} else {
						tex.SetPixel(resolutionI - 1 - x, y, palette[Hex2Num(px)]);
					}
					currentPixelIndex++;
				}
			}
			tex.Apply();
			return Sprite.Create(tex, new Rect(resolution, resolution, -resolution, -resolution), new Vector2(resolution * 0.5f, resolution * 0.5f), resolution);
		}

		/// <summary>
		/// Added by Xan.<br/>
		/// Creates a <see cref="SkillFamily"/> with one variant slot. It has no other data defined.
		/// </summary>
		/// <returns></returns>
		private static SkillFamily CreateSingleVariantFamily() {
			SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
			skillFamily.variants = new SkillFamily.Variant[1];
			return skillFamily;
		}

#pragma warning disable Publicizer001
		/// <summary>
		/// Designed by LuaFubuki, modified by Xan.<br/>
		/// Creates a new container for a <see cref="CharacterBody"/> and sets up its default, blank skills.
		/// </summary>
		/// <param name="bodyReplacementName">The name of the body prefab.</param>
		/// <param name="bodyDir">The location of the body prefab.</param>
		/// <returns></returns>
		public static GameObject CreateBody(string bodyReplacementName, string bodyDir = "RoR2/Base/Commando/CommandoBody.prefab") {
			GameObject newBody = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(bodyDir).WaitForCompletion(), bodyReplacementName);
			foreach (GenericSkill preExistingSkill in newBody.GetComponentsInChildren<GenericSkill>()) {
				Object.DestroyImmediate(preExistingSkill);
			}

			// Prevent it from being classified as a monster.
			DeathRewards deathRewards = newBody.GetComponent<DeathRewards>();
			if (deathRewards != null) Object.Destroy(deathRewards);

			SkillLocator skillLocator = newBody.GetComponent<SkillLocator>();
			skillLocator.allSkills = System.Array.Empty<GenericSkill>();
			skillLocator.primary = newBody.AddComponent<GenericSkill>();
			skillLocator.secondary = newBody.AddComponent<GenericSkill>();
			skillLocator.utility = newBody.AddComponent<GenericSkill>();
			skillLocator.special = newBody.AddComponent<GenericSkill>();
			SkillFamily primaryFamily = CreateSingleVariantFamily();
			SkillFamily secondaryFamily = CreateSingleVariantFamily();
			SkillFamily utilityFamily = CreateSingleVariantFamily();
			SkillFamily specialFamily = CreateSingleVariantFamily();

			skillLocator.primary._skillFamily = primaryFamily;
			skillLocator.secondary._skillFamily = secondaryFamily;
			skillLocator.utility._skillFamily = utilityFamily;
			skillLocator.special._skillFamily = specialFamily;

			return newBody;
		}

		/// <summary>
		/// Added by Xan.
		/// Intended to be called after the Body has all of its stuff attached, this prevents an error in the console that would
		/// result in the rejection of these skill families being added to game data.
		/// </summary>
		/// <param name="skillLocator"></param>
		public static void FinalizeBody(SkillLocator skillLocator) {
			ContentAddition.AddSkillFamily(skillLocator.primary._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.secondary._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.utility._skillFamily);
			ContentAddition.AddSkillFamily(skillLocator.special._skillFamily);
		}

		/// <summary>
		/// Adds a skill variant to the given slot of a <see cref="CharacterBody"/>-containing <see cref="GameObject"/>.
		/// Originally written by LuaFubuki but redone by Xan.
		/// </summary>
		/// <param name="bodyContainer">The <see cref="GameObject"/> that has a <see cref="CharacterBody"/> component on it.</param>
		/// <param name="definition">The actual skill to add.</param>
		/// <param name="slotName">The name of the slot. This must be <c>primary</c>, <c>secondary</c>, <c>utility</c>, or <c>special</c>.</param>
		/// <param name="variantIndex">The index of this variant. If this index is larger than the number of variants the <see cref="SkillFamily"/> can contain, its array is resized.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">If <paramref name="slotName"/> is not valid.</exception>
		public static void AddSkill(GameObject bodyContainer, SkillDef definition, string slotName = "primary", int variantIndex = 0) {
			ContentAddition.AddSkillDef(definition);
			SkillLocator skillLocator = bodyContainer.GetComponent<SkillLocator>();
			skillLocator.allSkills = System.Array.Empty<GenericSkill>();
			GenericSkill target;
			slotName = slotName.ToLower();
			switch (slotName) {
				case "primary":
					target = skillLocator.primary;
					break;
				case "secondary":
					target = skillLocator.secondary;
					break;
				case "utility":
					target = skillLocator.utility;
					break;
				case "special":
					target = skillLocator.special;
					break;
				default:
					throw new System.ArgumentOutOfRangeException(nameof(slotName), "Invalid slot name! Expecting either \"primary\", \"secondary\", \"utility\", or \"special\"!");
			}

			SkillFamily family = target.skillFamily;
			SkillFamily.Variant[] variants = family.variants;
			if (variants.Length >= variantIndex) {
				Log.LogTrace("Expanding Skill Family Variants array...");
				System.Array.Resize(ref variants, variantIndex + 1);
			}
			SkillFamily.Variant newVariant = default;
			newVariant.skillDef = definition;
			newVariant.viewableNode = new ViewablesCatalog.Node(definition.skillName + "_VIEW", false, null);
			variants[variantIndex] = newVariant;
			family.variants = variants;
			Log.LogTrace($"Appended new skill in slot \"{slotName}\": {definition.skillNameToken}");
		}

		/// <summary>
		/// Added by Xan. Temporary solution for the lack of skins on the model.
		/// </summary>
		/// <param name="bodyContainer">The prefab containing the character body and all the related stuff.</param>
		public static void AddReaverSkins(GameObject bodyContainer) {
			Renderer[] renderers = bodyContainer.GetComponentsInChildren<Renderer>();
			ModelLocator component = bodyContainer.GetComponent<ModelLocator>();
			GameObject effectiveRoot = component.modelTransform.gameObject;

			LoadoutAPI.SkinDefInfo defaultSkin = new LoadoutAPI.SkinDefInfo {
				Icon = SkinIconCreator.CreateSkinIcon(
					new Color32(24, 1, 33, 255),
					new Color32(52, 84, 108, 255),
					new Color32(239, 151, 227, 255),
					new Color32(11, 34, 127, 255)
				),
				NameToken = Lang.DEFAULT_SKIN,
				RootObject = effectiveRoot,
				BaseSkins = Ext.NewEmpty<SkinDef>(),
				GameObjectActivations = Ext.NewEmpty<SkinDef.GameObjectActivation>(),
				RendererInfos = new CharacterModel.RendererInfo[] {
					// TODO: Do I actually have to create these? Can I copy them from the vanilla reaver?
					new CharacterModel.RendererInfo {
						defaultMaterial = renderers[0].material,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[0]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = renderers[1].material,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[1]
					},
				},
				ProjectileGhostReplacements = Ext.NewEmpty<SkinDef.ProjectileGhostReplacement>(),
				MinionSkinReplacements = Ext.NewEmpty<SkinDef.MinionSkinReplacement>()
			};

			// lmfao
			// if one of you knows how to actually store + address the materials for this let me know
			GameObject ally = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierAllyBody.prefab").WaitForCompletion();
			Renderer[] allyRenderers = ally.GetComponentsInChildren<Renderer>();

			Material mtl0 = allyRenderers[0].material;
			Material mtl1 = allyRenderers[1].material;

			LoadoutAPI.SkinDefInfo ghostSkin = new LoadoutAPI.SkinDefInfo {
				Icon = SkinIconCreator.CreateSkinIcon(
					new Color32(183, 172, 175, 255),
					new Color32(78, 117, 145, 255),
					new Color32(152, 151, 227, 255),
					new Color32(54, 169, 226, 255)
				),
				NameToken = Lang.GHOST_SKIN,
				RootObject = effectiveRoot,
				BaseSkins = Ext.NewEmpty<SkinDef>(),
				GameObjectActivations = Ext.NewEmpty<SkinDef.GameObjectActivation>(),
				RendererInfos = new CharacterModel.RendererInfo[] {
					// TODO: Do I actually have to create these?
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl0,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[0]
					},
					new CharacterModel.RendererInfo {
						defaultMaterial = mtl1,
						defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
						ignoreOverlays = false,
						renderer = renderers[1]
					},
				},
				ProjectileGhostReplacements = Ext.NewEmpty<SkinDef.ProjectileGhostReplacement>(),
				MinionSkinReplacements = Ext.NewEmpty<SkinDef.MinionSkinReplacement>()
			};

			ModelSkinController ctrl = effectiveRoot.GetOrCreateComponent<ModelSkinController>(out bool justCreatedController);
			if (justCreatedController) {
				ctrl.characterModel = bodyContainer.GetComponent<CharacterModel>();
				ctrl.skins = Ext.NewEmpty<SkinDef>();
			}

			LoadoutAPI.AddSkinToCharacter(bodyContainer, defaultSkin);
			LoadoutAPI.AddSkinToCharacter(bodyContainer, ghostSkin);
		}
#pragma warning restore Publicizer001
	}
}
