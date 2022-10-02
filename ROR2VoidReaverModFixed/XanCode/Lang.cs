﻿using FubukiMods;
using FubukiMods.Modules;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ROR2VoidReaverModFixed.XanCode {

	/// <summary>
	/// Localization, except not very well localized (it's just english OMEGALUL).<br/>
	/// Yeah that's right. Get OMEGALUL'd. What are you gonna do about it?
	/// </summary>
	public static class Lang {

		public const string UNIQUE_SURVIVOR_PREFIX = "VOID_REAVER_PLAYER";

		public const string SURVIVOR_NAME = $"{UNIQUE_SURVIVOR_PREFIX}_NAME"; // This MUST be prefixed by _NAME
		public const string SURVIVOR_DESC = $"{UNIQUE_SURVIVOR_PREFIX}_DESCRIPTION"; // This MUST be prefixed by _DESCRIPTION
		public const string SURVIVOR_LORE = $"{UNIQUE_SURVIVOR_PREFIX}_LORE"; // This MUST be prefixed by _LORE
		public const string SURVIVOR_OUTRO = $"{UNIQUE_SURVIVOR_PREFIX}_OUTRO_FLAVOR";

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

		public static void Init() {

			LanguageAPI.Add(SURVIVOR_NAME, "Void Reaver");
			LanguageAPI.Add(SURVIVOR_DESC, LinesToSurvivorDetails(
				"The Void Reaver specializes in low to mid range combat and crowd control, proving to be a versatile support class when things get hectic.",
				"<style=cIsVoid>Void Impulse</style> is good for high damage against single targets at any range. In contrast, <style=cIsVoid>Void Spread</style> is much better for crowd control and targeting more than one monster at once.",
				"<style=cIsVoid>Undertow</style> can greatly assist allies in finishing off targets as well as giving you a moment of breathing room to focus on other monsters.",
				"<style=cIsVoid>Dive</style> is a powerful escape tool. Alongside healing you, it also makes enemies lose track of you, and protects you from incoming damage.",
				"<style=cIsVoid>Collapse</style> (<style=cIsVoid>Reave</style>'s more aggressive counterpart), which is triggered upon your death (either naturally, or by activating the dedicated ability), will cause the same implosion effect seen on ordinary Void Reavers. Closing the distance between yourself and strong enemies when you are about to die could be extremely useful to your fellow survivors!"
			));
			LanguageAPI.Add(SURVIVOR_LORE, THE_LORE);
			LanguageAPI.Add(SURVIVOR_OUTRO, "..and so it left, its first impression of life familiarly destructive.");

			LanguageAPI.Add(SKILL_PRIMARY_TRIPLESHOT_NAME, "<style=cIsVoid>Void Impulse</style>");
			LanguageAPI.Add(SKILL_PRIMARY_TRIPLESHOT_DESC, $"Fire 3 <style=cIsVoid>void pearls</style> in quick succession that hit twice for <style=cIsDamage>2x{Percentage(Configuration.BasePrimaryDamage/2)} damage</style>. <style=cIsUtility>Attack speed</style> increases the number of pearls per volley.");
			LanguageAPI.Add(SKILL_PRIMARY_SPREAD_NAME, "<style=cIsVoid>Void Spread</style>");
			LanguageAPI.Add(SKILL_PRIMARY_SPREAD_DESC, $"Fire 5 <style=cIsVoid>void pearls</style> that each hit twice for <style=cIsDamage>2x{Percentage(Configuration.BasePrimaryDamage/2)} damage</style>. The pearls are shot with a uniform <style=cUserSetting>{Round(Configuration.SpreadShotArcLengthDegs)} degree horizontal spread</style> between each pearl.");

			LanguageAPI.Add(SKILL_SECONDARY_NAME, "<style=cIsVoid>Undertow</style>");
			LanguageAPI.Add(SKILL_SECONDARY_DESC, $"Create a cluster of <style=cUserSetting>{Configuration.SecondaryCount}</style> bombs that each deal <style=cIsDamage>{Percentage(Configuration.BaseSecondaryDamage)} damage</style>. Inflicts <style=cIsVoid>Nullify Stack</style>. <style=cIsUtility>Attack speed</style> increases the number of bombs and the placement radius.");

			LanguageAPI.Add(SKILL_UTILITY_NAME, "<style=cIsVoid>Dive</style>");
			LanguageAPI.Add(SKILL_UTILITY_DESC, $"Temporarily slip into the void, becoming <style=cIsUtility>intangible</style> and recovering <style=cIsHealing>10% health</style>. Monsters that are currently attacking will lose track of your position.");

			LanguageAPI.Add(SKILL_SPECIAL_WEAK_NAME, "<style=cIsVoid>Reave</style>");
			LanguageAPI.Add(SKILL_SPECIAL_WEAK_DESC, $"Sacrifice <style=cIsHealth>{Percentage(Configuration.ReaveCost)} of your health</style> to create a noticably weaker-than-usual <style=cIsVoid>void collapse</style> that deals <style=cIsDamage>{Percentage(Configuration.BaseSpecialDamage)} damage</style> to all monsters caught within.");
			LanguageAPI.Add(SKILL_SPECIAL_SUICIDE_NAME, "<style=cIsVoid>Collapse</style>");
			if (Configuration.IsVoidDeathInstakill) {
				LanguageAPI.Add(SKILL_SPECIAL_SUICIDE_DESC, $"<style=cDeath>{SKULL} Extinguish your life {SKULL}</style> to trigger a natural <style=cIsVoid>void collapse</style> in all its glory, <style=cIsDamage>instantly killing</style> all monsters caught within.");
			} else {
				LanguageAPI.Add(SKILL_SPECIAL_SUICIDE_DESC, $"<style=cDeath>{SKULL} Extinguish your life {SKULL}</style> to trigger a natural <style=cIsVoid>void collapse</style> in all its glory, dealing an incredible <style=cIsDamage>{Percentage(Configuration.BaseDeathDamage)} damage</style> to all monsters caught within.");
			}

		}

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