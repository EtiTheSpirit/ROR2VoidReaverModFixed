# Void Reaver Player Character
An expansion upon [LuaFubuki's Void Reaver Mod](https://thunderstore.io/package/LuaFubuki/Void_Reaver/) that features my personal improvements, bugfixes, and more!

## Features
* All abilities of LuaFubuki's original Void Reaver mod
  * This notably lacks the legacy abilities using Lunar item mechanics. I have not yet implemented them and plan to remove them. If any of you reading this are particularly outspoken about this, please contact me. The GitHub repo is attached to this mod.
* The addition of the **Collapse** special, the more aggressive alternative to **Reave**. Kill yourself ~~immediately~~ to trigger the Void Reaver's death implosion.
  * As implied, dying will cause an implosion. Use this to your advantage!
  * Currently this does not kill players without Chaos enabled. I am looking to add it as an option in the future.
* Dozens of tweaks to behavior and fixes to mild issues.
* (Optional) Fresh icons for all the abilities and the survivor.
* Tons of new configuration options to personalize your experience.

## Important Notes
* A new major version has just released. Expect potentially strange issues, especially with new settings!
* It should be unsaid, but **make sure your config matches everybody else's** in multiplayer. If you see someone floating, or in the ground, your configuration (or theirs) isn't matched. There's a lot of graphics related tweaks here that might make for an inconsistent experience if they aren't matched.
* If you are just starting out, there's lots of configs that are sorted as cleanly as they can be. If you dislike something mechanically, you can probably just change it yourself.

## Known Bugs
* No other bugs are currently known.

## Special Thanks To...
* **LuaFubuki**, for open sourcing the mod.
* **Desync**, **Minno**, **Lordaniel**, and **Eggy** (some of my buddies) for doing multiplayer testing with me.
* **ROR2 Modding Discord** for making sure I was stumbling around the game's codebase in what is *mostly* the correct direction (also shoutouts for being an incredibly patient community of modders!)

# Changelogs

## 2.0.3
#### If this fix gets any hotter, I might not be able to contain myself!

### Fixes
* Fixed a regression reintroduced by 2.0.2 where the ragdoll of dead reaver players got stuck after finishing the death animation.

## 2.0.2
#### Hotfix for some bugs in 2.0.1

### Fixes
* Fixed a bug causing Reave to do instakill damage (only Collapse should do this)
* Fixed a bug where the player was not properly disposed of after death, causing enemies to continue to attack an empty space, and breaking the spectator camera.

## 2.0.1
#### Some stragglers I wanted to release after playtesting some more.

### The Spotlight
* Added a new option to replace a previous mechanic for Collapse instakill.
  * Collapse damage should no longer be set to 1 million or above to make it instakill.
  * There is now a dedicated config option to enable (or disable) Collapse's instakill.
  * To prevent balance issues, there is an additional config option to prevent Collapse from instakilling bosses, falling back to the damage stat.

### Fixes
* Corrected the mod manifest missing its dependency on HookGenPatcher
* Fixed a bug preventing the animation speed from being corrected when using the full size option.
* Fixed a bug where using Collapse would not set shield and barrier to 0, sometimes giving the illusion of some health still remaining.

### Mechanical Changes
* Slightly buffed the default base damage from 16 => 20
* Buffed health from 200 => 220

### Art Changes
* The shield bar and overlay are both always rendered in the Void style on the Reaver player, regardless of whether or not the player has a Plasma Shrimp.

## 2.0.0
#### A major update! What could I put here, in the update's subtitle? What is something that is memorable and unique?

### The Spotlight
* **Added new icons** for the character and all of its abilities. For the purists to the classic mod out there, this is an option, and you can use your configuration to make it use the old icons. This will be supported for the rest of the foreseeable future.
* **Configs were reorganized. DOUBLE CHECK YOUR CONFIG FILES!**
* **Added skins** for the character!
  * The default skin looks like an ordinary Void Reaver (who woulda thunk?)
  * The second skin mimics that of a Void Reaver spawned from a [Newly Hatched Zoea](https://riskofrain2.fandom.com/wiki/Newly_Hatched_Zoea) (friend colored)
  * Currently, there is no mastery skin. I do not have plans to add one at this time.

### Fixes
* *Ideally* fixed the issue where Bustling Fungus would not spawn its AoE. The player might float a slight, almost imperceptible height above the ground, but it looks close enough to ordinary Void Reavers that it doesn't really matter.
* Fixed bugs relating to the Full Size option and placement of the player.
* Fixed a bug causing Reave to not do the right amount of self-damage.
* Fixed a bug preventing proper replication of the Void Reaver's death state, causing players to T-pose after dying on remote players' screens.

### Mechanical Changes
* [EXPERIMENTAL] Added the option to become immune to environmental void damage and fog. This setting works very well, and is **enabled** by default for new users especially. *The primary issue is that it might unintentionally block damage from other valid sources.* This is because it has to guess if something is void damage.
  * For the record, the determining factors are that the damage has no source (it counts as "The Planet" when you get killed), and that it specifically is set to ONLY bypass armor and blocking. Damage coming from NPCs always has the attacker value set.

### Art and Lore Changes
* Added a passive ability slot, and did some tidying up on the description of the survivor and its abilities.

### Configuration Changes
* Tweaked the descriptions of some entries that were accidentally misleading.
* Added the ability to configure the speed, regeneration, and duration of Dive.
* As mentioned above, immunity to Void environments can be toggled.

***

## 1.1.1
#### Includes a change I meant to include in 1.1.0 but forgot to add. Nice.

### UI Changes
* Rephrased some mod information that was accidentally misleading.
* Rephrased the description of Void Impulse to reflect upon the new behavior. This should make it much easier to understand.

### Ability Changes (Primary)
* Added a configuration option to change the number of shots fired in Void Spread.
* Changed the configuration option for the spread angle of Void Spread so that it is the entire angle rather than the individual increment between bullets. This way you don't have to do *math* to find the correct angle.

### Ability Changes (Special)
* Nerfed the default damage of Reave (100x => 60x) which better suits the health cost. Collapse was not changed.

## 1.1.0
#### This comes with a lot of very big changes and so many users will need to review their configuration files. It is recommended to delete it and let the mod regenerate it.

### Configuration Additions
* Added limits to all compatible config values to prevent unwanted data.
* Added the ability to configure bullet spread on Void Impulse.
* Added the ability to configure the amount of time it takes to fully fire Void Impulse.

### Ability Changes (Primary)
* Added an experimental behavior switch to Void Impulse that makes it "bulk up" bullets when attack speed increases beyond 2x. This means you still fire 3 shots (or whatever you configured), but 2x attack speed makes it fire in pairs, 3x in triplets, so on. This is on by default. (The main reason is that there is a certain point in which your firerate gets capped due to Unity's update rate, this prevents that).
* Slightly nerfed the firerate of Void Spread to prevent it from having such a sharp DPS advantage over Void Impulse.
* Slightly buffed the default setting for base damage of the character to make it match up with other survivors a bit more.
* Slightly tweaked the bullet spread of Void Impulse so that it is uniform across all bullets instead of "building up" over the course of the shots.

### Ability Changes (Special)
* Fixed an issue where the radius was too small, preventing it from killing things right on the edge.
* Reduced the default damage of the player death from 12500x => 2250x. A mathematical error when translating damage values resulted in this being a little (read: "way") too high for what it was worth. This value may still be too high, but I want the damage to be big and powerful still. Configure this as you so please.
* Configuring the death damage to a value greater than or equal to 1000000x will make it instakill like normal void attacks.
* Reave and Collapse are now also tagged to bypass one-shot protection, allowing it to have full potential when playing with Artifact of Chaos.
* Fixed a bug where Reave and Collapse could be resisted and/or blocked by items such as Tougher Times and Safer Spaces, as well as by Voidtouched enemies.
* Reave's self-damage is no longer blocked by items or resisted by armor.

## 1.0.1
#### Tweaks, tweaks everywhere!

### Lore Changes
* Added unique outro text when using the escape ship. This outro text fits into the story presented in the survivor lore.
* Greatly improved the presentation, general storytelling, and flow of the survivor lore entry. This should make it easier to follow along with, keeping it mysterious but not flat out confusing.
* Corrected some typos and incorrect tags in the survivor lore, like &amp;gt; showing instead of &gt;

### UI Changes
* Tweaked the survivor overview tooltip describing Collapse to more closely relate it as a means of manually triggering your death, which in turn causes the explosion (where it previously implied that it caused the explosion and killed you in the process). It's minor, but meaningful.
* Tweaked the text formatting of the overview panel to match that of other survivors.

## 1.0.0
#### Nice

* Initial release. Mostly intended for friends but it should be suitable for a public release now.
