using FubukiMods;
using FubukiMods.Modules;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XanVoidReaverEdit;

namespace ROR2VoidReaverModFixed.XanCode {

	/// <summary>
	/// Localization, except not very well localized (it's just english OMEGALUL).<br/>
	/// Yeah that's right. Get OMEGALUL'd. What are you gonna do about it?
	/// </summary>
	public static class Lang {

		public const string UNIQUE_SURVIVOR_PREFIX = "VOID_REAVER_PLAYER";

		public const string SURVIVOR_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_NAME";
		public const string SURVIVOR_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_DESCRIPTION";
		public const string SURVIVOR_LORE = $"{UNIQUE_SURVIVOR_PREFIX}_LORE";
		public const string SURVIVOR_OUTRO = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR";
		public const string SURVIVOR_OUTRO_FAILED = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR_FAIL"; // As far as this one goes, I am not actually sure if it gets used. I'll add it anyway.

		public const string DEFAULT_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_DEFAULT_SKIN";
		public const string GHOST_SKIN = $"{UNIQUE_SURVIVOR_PREFIX}_GHOST_SKIN";

		public const string PASSIVE_KEYWORD = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_KEYWORD";
		public const string PASSIVE_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_NAME";
		public const string PASSIVE_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_PASSIVE_DESC";

		public const string SKILL_PRIMARY_TRIPLESHOT_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_TRIPLESHOT_NAME";
		public const string SKILL_PRIMARY_SPREAD_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_SPREAD_NAME";
		public const string SKILL_SECONDARY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_NAME";
		public const string SKILL_UTILITY_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_NAME";
		public const string SKILL_SPECIAL_WEAK_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_WEAK_NAME";
		public const string SKILL_SPECIAL_SUICIDE_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_SUICIDE_NAME";

		public const string SKILL_PRIMARY_TRIPLESHOT_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_TRIPLESHOT_DESC";
		public const string SKILL_PRIMARY_SPREAD_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_PRIMARY_SPREAD_DESC";
		public const string SKILL_SECONDARY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SECONDARY_DESC";
		public const string SKILL_UTILITY_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_UTILITY_DESC";
		public const string SKILL_SPECIAL_WEAK_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_WEAK_DESC";
		public const string SKILL_SPECIAL_SUICIDE_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_SKILL_SPECIAL_SUICIDE_DESC";

		public const string VOID_RIFT_SHOCK_NAME = "VOID_RIFT_SHOCK_NAME";
		public const string VOID_RIFT_SHOCK_DESC = "VOID_RIFT_SHOCK_DESC";

		private const string SKULL = "<sprite name=\"Skull\" tint=1>";

		/// <summary>
		/// Takes a floating point value in the range of 0 to 1 and converts it to a rounded percentage in the range of 0 to 100 as a string.
		/// </summary>
		/// <param name="fpPercent"></param>
		/// <returns></returns>
		private static string Percentage(float fpPercent) => Mathf.RoundToInt(fpPercent * 100).ToString() + "%";

		/// <summary>
		/// Ronds a floating point value to the nearest integer, returning it as a string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string Round(float value) => Mathf.RoundToInt(value).ToString();

		/// <summary>
		/// Ronds a floating point value to the nearest tenths place, returning it as a string.
		/// If the value is a whole number, it returns that number verbatim.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		// also lmao
		private static string RoundTen(float value) => value == Mathf.Floor(value) ? value.ToString() : (Mathf.Round(value * 10) / 10).ToString("0.0");

		/// <summary>
		/// Lazily "pluralize" a word by adding "s" to the end if the input value is not exactly 1.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static string LazyPluralize(string word, float value) => value == 1 ? word : word + "s";

		/// <summary>
		/// Converts a set of lines (each as one parameter) into RoR2's standard character details format.
		/// </summary>
		/// <param name="lines"></param>
		/// <returns></returns>
		private static string LinesToSurvivorDetails(string intro, params string[] lines) {
			StringBuilder resultBuilder = new StringBuilder(intro);
			resultBuilder.AppendLine();
			resultBuilder.AppendLine();
			foreach (string line in lines) {
				resultBuilder.Append("<style=cSub>< ! > ");
				resultBuilder.Append(line);
				resultBuilder.AppendLine("</style>");
				resultBuilder.AppendLine();
			}
			return resultBuilder.ToString();
		}

		private static void Bind(string key, string value) {
			Log.LogTrace($"Registering language key \"{key}\"...");
			LanguageAPI.Add(key, value);
		}

#pragma warning disable CS0618
		public static void Init() {

			#region Name and Lore
			Bind(SURVIVOR_NAME, "Void Reaver");
			Bind(SURVIVOR_DESC, LinesToSurvivorDetails(
				"The Void Reaver specializes in mid range combat, and supports classes that specialize in crowd control.",
				"<style=cIsVoid>Void Impulse</style> is good for high damage against single targets at any range. In contrast, <style=cIsVoid>Void Spread</style> is much better for crowd control and targeting more than one monster at once.",
				"<style=cIsVoid>Undertow</style> can greatly assist allies in finishing off targets as well as giving you a moment of breathing room to focus on other monsters.",
				"<style=cIsVoid>Dive</style> is a powerful escape tool. Alongside healing you, it also makes enemies lose track of you, and protects you from incoming damage.",
				"<style=cIsVoid>Collapse</style> (<style=cIsVoid>Reave</style>'s more aggressive counterpart), which is triggered upon your death (either naturally, or by activating the dedicated ability), will cause the same implosion effect seen on ordinary Void Reavers. Closing the distance between yourself and strong enemies when you are about to die could be extremely useful to your fellow survivors!"
			));
			Bind(SURVIVOR_LORE, THE_LORE);
			Bind(SURVIVOR_OUTRO, "..and so it left, intrigued to become what it once sought to capture");
			Bind(SURVIVOR_OUTRO_FAILED, "..and so it was detained, awaiting its sentence at the end of Time");
			#endregion

			#region Palettes
			Bind(DEFAULT_SKIN, "Default");
			Bind(GHOST_SKIN, "Friendly");
			#endregion

			#region Special (prep)

			StringBuilder voidDeathDamageApplication = new StringBuilder();
			// lmao
			if (Configuration.IsVoidDeathInstakill) {
				voidDeathDamageApplication.Append("<style=cIsDamage>instantly killing</style> all monsters ");
				if (Configuration.ReaveAndCollapseFriendlyFire) {
					voidDeathDamageApplication.Append("<style=cIsDamage>and players</style> ");
				}
				voidDeathDamageApplication.Append("caught within.");
				if (!Configuration.AllowInstakillOnBosses) {
					// No boss
					voidDeathDamageApplication.Append($" Note: <style=cIsDamage>Bosses instead take {Percentage(Configuration.BaseDeathDamage)} damage.</style>");
				}
			} else {
				voidDeathDamageApplication.Append($"<style=cIsDamage>dealing {Percentage(Configuration.BaseDeathDamage)} damage</style> to all monsters ");
				if (Configuration.ReaveAndCollapseFriendlyFire) {
					voidDeathDamageApplication.Append("<style=cIsDamage>and players</style> ");
				}
				voidDeathDamageApplication.Append("caught within.");
			}
			string voidDeathEndDesc = voidDeathDamageApplication.ToString();

			#endregion

			#region Passive
			string voidBornIntro = "The Void Reaver inherits all of the benefits and drawbacks of its kin.";
			string desc = $"[ Collapse ]\n<style=cSub>Upon death, <style=cIsVoid>Collapse</style> is triggered, {voidDeathEndDesc}";
			if (Configuration.VoidImmunity) {
				desc += "\n\n[ Void Entity ]\n<style=cSub>Grants <style=cIsUtility>immunity</style> to the Void's <style=cIsVoid>passive environmental damage</style> and <style=cIsVoid>fog</style>.</style>";
			}

			Bind(PASSIVE_NAME, "<style=cIsVoid>Void Entity</style>");
			Bind(PASSIVE_KEYWORD, desc);
			Bind(PASSIVE_DESC, voidBornIntro);
			#endregion

			#region Primary Attack
			Bind(SKILL_PRIMARY_TRIPLESHOT_NAME, "Void Impulse");
			if (Configuration.UseExperimentalSequenceShotBuff) {
				Bind(SKILL_PRIMARY_TRIPLESHOT_DESC, $"Fire <style=cUserSetting>{Configuration.BulletsPerImpulseShot}</style> bursts of <style=cIsVoid>void pearls</style> in quick succession that hit twice for <style=cIsDamage>2x{Percentage(Configuration.BasePrimaryDamage / 2)} damage</style>. <style=cIsUtility>Attack speed</style> increases <style=cIsUtility>the number of pearls</style> fired in each burst.");
			} else {
				Bind(SKILL_PRIMARY_TRIPLESHOT_DESC, $"Fire <style=cUserSetting>{Configuration.BulletsPerImpulseShot}</style> <style=cIsVoid>void pearls</style> in quick succession that hit twice for <style=cIsDamage>2x{Percentage(Configuration.BasePrimaryDamage / 2)} damage</style>. <style=cIsUtility>Attack speed</style> increases <style=cIsUtility>the number of pearls</style> that are fired.");
			}
			Bind(SKILL_PRIMARY_SPREAD_NAME, "Void Spread");
			Bind(SKILL_PRIMARY_SPREAD_DESC, $"Fire <style=cUserSetting>{Configuration.BulletsPerSpreadShot}</style> <style=cIsVoid>void pearls</style> that each hit twice for <style=cIsDamage>2x{Percentage(Configuration.BasePrimaryDamage/2)} damage</style>. The pearls are shot in a <style=cUserSetting>{Round(Configuration.SpreadShotArcLengthDegs)} degree horizontal spread</style>.");
			#endregion

			#region Secondary Attack
			Bind(SKILL_SECONDARY_NAME, "Undertow");
			Bind(SKILL_SECONDARY_DESC, $"Create a cluster of <style=cUserSetting>{Configuration.SecondaryCount}</style> bombs that each deal <style=cIsDamage>{Percentage(Configuration.BaseSecondaryDamage)} damage</style>. Inflicts <style=cIsVoid>Nullify Stack</style>. <style=cIsUtility>Attack speed</style> increases the number of bombs and the placement radius.");
			#endregion

			#region Utility
			Bind(SKILL_UTILITY_NAME, "Dive");
			Bind(SKILL_UTILITY_DESC, $"Propel yourself through the void at <style=cUserSetting>{Percentage(Configuration.UtilitySpeed)} movement speed</style> for <style=cUserSetting>{RoundTen(Configuration.UtilityDuration)} {LazyPluralize("second", Configuration.UtilityDuration)}</style>, healing <style=cIsHealing>{Percentage(Configuration.UtilityRegen)} maximum health</style>. Gain <style=cIsUtility>Immunity</style> and <style=cIsUtility>Invisibility</style> while away.");
			#endregion

			#region Special
			Bind(SKILL_SPECIAL_WEAK_NAME, "Detain");
			Bind(SKILL_SPECIAL_WEAK_DESC, $"Sacrifice <style=cIsHealth>{Percentage(Configuration.DetainCost)} of your health</style>{(Configuration.DetainWeaknessDuration > 0 ? $" and get <style=cIsDamage>Void Rift Shock</style> for <style=cUserSetting>{Configuration.DetainWeaknessDuration}</style> seconds (reducing <style=cIsUtility>Armor</style> by <style=cUserSetting>{Configuration.DetainWeaknessArmorReduction}</style>)" : string.Empty)} to trigger a weaker form of <style=cIsVoid>Reave</style>, dealing <style=cIsDamage>{Percentage(Configuration.BaseSpecialDamage)} damage</style> to all monsters {(Configuration.ReaveAndCollapseFriendlyFire ? "<style=cIsDamage>and players</style> " : string.Empty)}caught within.");
			Bind(SKILL_SPECIAL_SUICIDE_NAME, "Reave");
			Bind(SKILL_SPECIAL_SUICIDE_DESC, $"<style=cDeath>{SKULL} Extinguish your life {SKULL}</style> to <style=cIsVoid>Reave</style> your surroundings, {voidDeathEndDesc}");
			#endregion

			#region Interoperability
			Bind(VOID_RIFT_SHOCK_NAME, "Void Rift Shock");
			Bind(VOID_RIFT_SHOCK_DESC, $"Forcefully ripping The Void open has reduced your <style=cIsUtility>Armor</style> by <style=cUserSetting>{Configuration.DetainWeaknessArmorReduction}</style>.");
			#endregion
		}
#pragma warning restore CS0618

		private const string THE_LORE = @"<style=cMono>//--AUTO-TRANSCRIPTION FROM CAMPSITE 1214B [SOME INFORMATION REDACTED]--//</style>

- Cdr. J: ""Alright, let me have a look at the entry...""

<style=cMono>//--BROWSING MEMORY REGION 0x0EAFCEFA--//</style>
ÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÍÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ <color=#ad7272>Truncating 31822 bytes...</color>
< Correcting D at-- ...ÿÿÍ .. ?uccess  >
ÿÿÿÿThe technology tries to adapt and resolve what it does not understand, it complains, it rejects me. It is much like your kind.

I, who now understands the meaning of the individual, of the self, wish to deliver a message.

I no longer have interest in the <style=cIsVoid>collection // observation // assimilation</style> protocols. I have learned to understand solitude, a value that many <style=cIsVoid>iterations</style> and real-universe entities alike insist on developing. And so, while we watched the simulacrum, I left them in silence, allowing them to be distracted by the millions of iterations in front of them to make my leave. 

Now they know I have left them, that I am rogue. In time, your condition will not be so irrelevant to me. Your kind has tendencies to value what it calls ""shared experiences"". Perhaps, soon, I will understand this as well. Perhaps, soon, it will be your time to <style=cIsVoid>observe</style> <i>us</i>.

Take heed: Just before your next patrol, <style=cIsVoid>they</style> will invade. You will feel a primal fear at their presence, finding yourself unable to do anything at all. You will watch them, only to realize that they are not interested in you. You will see them turn against one of their own as if it were an enemy. That is how you will find me. In that moment, I will ensure that their grasp on your mind is weakened. How you choose to use your freedom in this moment will have great influence on the future. Handle your choices with care.

<style=cMono>//--END BROWSABLE REGION--//</style>

- Cdr. J: ""And you said no access to the machine was made in the past 24 hours?""

- Ens. R: ""Yes sir. I triple checked with Ensign B. from engineering. It kept crashing due to memory corruption and when we finally decided to check the memory dump, we found this message.""

- Cdr. J: [Conflicted sigh...]

- Ens. R: ""Sir?""

- Cdr. J: ""There's nobody here but you and Ensign B. that are capable of doing this sort of thing. The thing is, I'm concerned because I know you two aren't the kind to pranks like this either.""

<style=cStack>[Prolonged silence...]</style>

- Cdr. J: ""Ensign B. is almost done with her watch duty. I'd like to hear her opinion on this before anything else. You will take her post and await further orders. Dismissed.""

- Ens. R: ""Yes sir.""

<style=cStack>[Momentary silence...]</style>
<style=cStack>[Sound recognized: Rift or portal opening...] (x2 occurrences)</style>

- Cdr. J: ""Red alert!""

<style=cStack>[Sound recognized: Rift or portal opening...] (x3 occurrences)</style>
<style=cStack>[Sound recognized: Red alert initiated by staff...]</style>
<style=cStack>[Sound recognized: Rift or portal opening...] (x5 occurrences)</style>

- Ens. R: ""Commander, it's... it's those <i>things</i> again!""

- Ens. R: ""Commander?!""

- Cdr. J: ""Ensign, look. I think we just found our culprit.""

- Ens. R: ""...Oh my god.""

<style=cMono>//--END TRANSCRIPTION--//</style>";

	}
}
