# Void Reaver Player Character
An expansion upon [LuaFubuki's Void Reaver Mod](https://thunderstore.io/package/LuaFubuki/Void_Reaver/) that integrates my extensions' features as standard abilities. This also fixes a number of bugs, introduces survivor lore, and introduces more configuration options.

## Introduction
Please take your time to give thanks to LuaFubuki for allowing me to continue their work. Without their work (and the interest of some of my friends + myself), and their willingness to allow me to continue where they left off, this would not have been created.

**Currently, this mod is in a slight testing phase.** Most of it works but there's gonna be inevitable kinks to iron out.

## Features
* All abilities of LuaFubuki's original Void Reaver mod (with the exception of the legacy Lunar alt abilities, which have yet to be implemented).
* The addition of the **Collapse** special, the more aggressive alternative to **Reave**. Kill yourself ~~immediately~~ to trigger the Void Reaver's death implosion.
* Dozens of tweaks to behavior and fixes to mild issues.
* Tons of new configuration options to personalize your experience.
* As implied, dying will cause the implosion. Use this to your advantage!
* Slight tweaks to animations. Firing will no longer twist your body, allowing you to be the sentry you always dreamed of being.

## Known Bugs
* I anticipate issues for Full Size option (this may be removed in the future, it's more of a test for giggles)

## Special Thanks To...
* LuaFubuki, for open sourcing the mod.
* Desync and Minno (some of my buddies) for doing multiplayer testing with me.
* ROR2 Modding Discord for making sure I was stumbling around the game's codebase in at least the *mostly* correct direction.

## Changelog
### 2.0.0
Wohaho!

#### The Big Changes
* **Added new icons** for the character and all of its abilities. For the purists to the classic mod out there, this is an option, and you can use your configuration to make it use the old icons. This will be supported for the rest of the foreseeable future.
* **Added skins** for the character, one of which is the default skin and the other one which makes you look like the variant from the [Newly Hatched Zoea](https://riskofrain2.fandom.com/wiki/Newly_Hatched_Zoea) item (friend colored) ((please stop shooting me it was only funny that one time))

#### Fixes
* *Ideally* fixed the issue where Bustling Fungus would not spawn its AoE. The player might float a slight, almost imperceptible height above the ground, but it looks close enough to ordinary Void Reavers that it doesn't really matter.
* Fixed a bug causing Reave to not do the right amount of self-damage.
* Fixed a bug causing replication to fail when remote players died (or, this fixes the issue where your friendly reaver players would get "stuck" when dying and not poof away).

#### Mechanical Changes
* [EXPERIMENTAL] Added the option to become immune to environmental void damage and fog, for the people who got irked by the fact that the literal void entity was hurt by its native environment (read: me). This should mostly work well, *the primary issue is that it might unintentionally block damage from other valid sources.* It has to guess if the damage is void, so it's not very easy.

#### Art and Lore Changes
* Added a passive ability slot, and reorganized the description of the survivor and abilities.

#### Configuration Changes
* Added the ability to configure the speed, regeneration, and duration of Dive.
* As mentioned above, immunity to Void environments can be toggled.

### 1.1.1
Includes a change I meant to include in 1.1.0 but forgot to add. Nice.

#### UI Changes
* Rephrased some mod information that was accidentally misleading.
* Rephrased the description of Void Impulse to reflect upon the new behavior. This should make it much easier to understand.

#### Ability Changes (Primary)
* Added a configuration option to change the number of shots fired in Void Spread.
* Changed the configuration option for the spread angle of Void Spread so that it is the entire angle rather than the individual increment between bullets. This way you don't have to do *math* to find the correct angle.

#### Ability Changes (Special)
* Nerfed the default damage of Reave (100x => 60x) which better suits the health cost. Collapse was not changed.

### 1.1.0
1.1.0 comes with a lot of very big changes and so many users will need to review their configuration files. It is recommended to delete it and let the mod regenerate it.

Some stuff still isn't configurable, but it will be later.

#### Configuration Additions
* Added limits to all compatible config values to prevent unwanted data.
* Added the ability to configure bullet spread on Void Impulse.
* Added the ability to configure the amount of time it takes to fully fire Void Impulse.

#### Ability Changes (Primary)
* Added an experimental behavior switch to Void Impulse that makes it "bulk up" bullets when attack speed increases beyond 2x. This means you still fire 3 shots (or whatever you configured), but 2x attack speed makes it fire in pairs, 3x in triplets, so on. This is on by default. (The main reason is that there is a certain point in which your firerate gets capped due to Unity's update rate, this prevents that).
* Slightly nerfed the firerate of Void Spread to prevent it from having such a sharp DPS advantage over Void Impulse.
* Slightly buffed the default setting for base damage of the character to make it match up with other survivors a bit more.
* Slightly tweaked the bullet spread of Void Impulse so that it is uniform across all bullets instead of "building up" over the course of the shots.

#### Ability Changes (Special)
* Fixed an issue where the radius was too small, preventing it from killing things right on the edge.
* Reduced the default damage of the player death from 12500x => 2250x. A mathematical error when translating damage values resulted in this being a little (read: "way") too high for what it was worth. This value may still be too high, but I want the damage to be big and powerful still. Configure this as you so please.
* Configuring the death damage to a value greater than or equal to 1000000x will make it instakill like normal void attacks.
* Reave and Collapse are now also tagged to bypass one-shot protection, allowing it to have full potential when playing with Artifact of Chaos.
* Fixed a bug where Reave and Collapse could be resisted and/or blocked by items such as Tougher Times and Safer Spaces, as well as by Voidtouched enemies.
* Reave's self-damage is no longer blocked by items or resisted by armor.


### 1.0.1
#### Lore Changes
* Added unique outro text when using the escape ship. This outro text fits into the story presented in the survivor lore.
* Greatly improved the presentation, general storytelling, and flow of the survivor lore entry. This should make it easier to follow along with, keeping it mysterious but not flat out confusing.
* Corrected some typos and incorrect tags in the survivor lore, like &amp;gt; showing instead of &gt;

#### UI Changes
* Tweaked the survivor overview tooltip describing Collapse to more closely relate it as a means of manually triggering your death, which in turn causes the explosion (where it previously implied that it caused the explosion and killed you in the process). It's minor, but meaningful.
* Tweaked the text formatting of the overview panel to match that of other survivors.

### 1.0.0
* Initial release. Mostly intended for friends but it should be suitable for a public release now.
