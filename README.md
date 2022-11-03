# Void Reaver Player Character
An expansion upon [LuaFubuki's Void Reaver Mod](https://thunderstore.io/package/LuaFubuki/Void_Reaver/) that features my personal improvements, bugfixes, and more!

## Features
* All abilities of LuaFubuki's original Void Reaver mod.
* The addition of the **Collapse** special, the more aggressive alternative to **Reave**. Kill yourself ~~immediately~~ to trigger the Void Reaver's death implosion.
* (Optional) Fresh icons for all the abilities and the survivor.
* Tons of new configuration options to personalize your experience and tweak nearly all facets of the mod.

## Important Notes
* It should be unsaid, but **make sure your config matches everybody else's** in multiplayer. If you see someone floating, or in the ground, your configuration (or theirs) isn't matched. There's a lot of graphics related tweaks here that might make for an inconsistent experience if they aren't matched.
* If you are just starting out, there's lots of configs that are sorted as cleanly as they can be. If you dislike something mechanically, you can probably just change it yourself! If a configuration option is missing, please let me know.

## Need to contact me?
* You can find me in the RoR2 modding discord. I am nicknamed `Xan`, my discriminator is #1760.
* If you prefer another way, I have a website https://etithespir.it/ with more methods.

## Known Bugs / To-Do List (Fixes)
* There are no currently known bugs.
* I am looking into the preview model (pre-game screen) being in the falling animation, I would like to fix this.
* I may implement a stronger variant of Pulverized that subtracts more armor for Reave, the amount being configurable.

## Special Thanks To...
* **LuaFubuki**, for open sourcing the mod.
* **Desync**, **Minno**, **Lordainel**, and **Eggy** (some of my buddies) for doing multiplayer testing with me.
* **ROR2 Modding Discord** for making sure I was stumbling around the game's codebase in what is *mostly* the correct direction (also shoutouts for being an incredibly patient community of modders!)

# Changelogs

## 2.0.6
#### In all fairness, it *was* pretty OP.

### New Features
* Added the option to inflict Pulverized to yourself upon using Reave. This should balance its rather high damage by rendering you weak after using it, hopefully providing a mechanic through which players can further balance the ability in their modpacks. The default duration is 10 seconds. Set the time to 0 in your configuration to disable this drawback.

### Changes
* Reduced the default configuration value for Reave damage (from 6,000% to 5,000%)
* Dramatically reduced the default configuration value for Collapse damage (from 75,000% to 30,000%). It still needs to be OP given that you die upon using it, but not *that* OP.

### Fixes
* Fixed a bug where the camera zoom effect only occurred when using Reave and not Collapse. Both abilities should now properly trigger the zoom out effect.

## 2.0.5
#### Try not to get deleted.

### New Features
* Added a new config option to allow Reave and Collapse to do PvP damage, even if Artifact of Chaos is disabled. This mimics the behavior of the [Newly Hatched Zoea](https://riskofrain2.fandom.com/wiki/Newly_Hatched_Zoea) reavers, whose death will kill friendlies.

### Fixes
* Fixed a bug causing enemies killed with Reave and Collapse to not have their deaths classified as `VoidDeath`, making it look like an ordinary kill had occurred instead of the proper disappearing effect (this requires some jank to work but it's extremely consistent jank, so that's what matters).
* Fixed some phrasing in the menus when combinations of instakill, boss instakill, and pvp were used.
* Fixed the phrasing of some config options.

## 2.0.4
#### hnng.,.. color,,,

### New Features
* Implemented my newly released [HPBarAPI](https://thunderstore.io/package/Xan/HPBarAPI/) to customize the display of the health bar, shields, and barriers of the Void Reaver player character.
* Added a configuration option for the sprint speed multiplier. Setting the sprint speed multiplier to 1 (to disable sprinting) will also disable the FOV increase that comes with sprinting.
* Added a configuration option (on by default) to use exaggerated kill effects when killing with Reave and Collapse. This borrows the visual effect from the Lost-Seer's Lenses (but notably removes the sound effect because that shit got LOUD - it still plays from the actual kill animation itself, so its not *gone*, its just incapable of breaking everybody's eardrums).

### Fixes
* Fixed a bug causing Collapse to screw up the post-run stats, saying players that got kills with it did 9,223,372,036,854,775,807 damage in the run.
* Not really a bug, but an annoying issue nonetheless, because the player has no sprint animation it would look very strange while sprinting (and out of sync). The "sprint animation" is now just "impolitely convincing the system that you aren't actually sprinting" so that the dynamic walk speed is what gets applied instead. tl;dr fix goofy out of sync sprint animation.

### Internal Changes
* The DLL's "File Version" field in Properties now reflects upon the mod version. I don't know if I will be able to actually stick to this but I'll try. I'll also keep track of test number in the fourth digit for some internal stuffs.

## 2.0.3
#### Cutely squishes one bug and vomits out another one. Oops colon three. Oh woah, what's this, there's some new features in the vomit? That's kind of disgusting. Oh, right, your config files are gonna need to be rechecked again, because what's an update to this mod without requiring you to redo the bloody config files again, am I right?

(All joking aside, it's an organizational thing because this mod is still in its active development. In other news, stay tuned for more because I just released an experimental API that allows customizing the healthbar graphics, so you might see that appear soonish!)

### Fixes
* Fixed a regression reintroduced by 2.0.2 where the ragdoll of dead reaver players got stuck after finishing the death animation.

### Mechanical Changes
* Buffed Void Impulse by increasing its firerate (namely, the cooldown starts counting down immediately rather than after the firing period).
* The configuration entry for the amount of time Impulse has to fire in has been edited accordingly (it has a lower maximum, but the value is the same)

### Other Changes
* I changed the GUID of the plugin because right now nobody depends on said plugin. Its dependency string should remain the same, but the code reference and config file name will not.
* Full Size Reaver option is no longer experimental. It works quite nicely!

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
