# G-Effects mod for Kerbal Space Program

Supported KSP version: 1.12.5 <br/>
It is likely that KSP >= 1.8 will work but there is NO guarantee.<br/>
This mod might be incompatible with other mods that change how Kerbals are affected by g-forces because it replaces the default system with a custom one.<br/>
It is recommended to enable g force limits on kerbals (via advanced difficulty settings) to see how far kerbals are away from G-LOC, but currently this setting is ignored by this mod.

The work was originally started by russnash37 who gave a permission to Ser (SerTheGreat) to take over and extend [his project](https://forum.kerbalspaceprogram.com/topic/95579-plugin-090-wip-g-force-v004-alpha-experience-the-effects-of-g-forces-on-your-kerbals).<br/>
Ser has maintained the project until [KSP 1.3](https://forum.kerbalspaceprogram.com/topic/113341-130-122-g-effects-blackouts-redouts-g-locs-v042-2017-jun-25/).<br/>
Since Ser and russnash37 weren't active on github or the KSP forum for a long time I wasn't able to contact them.

Ser tried to make focus on realism as much as he was able to research the subject so the default settings assume that all kerbals including tourists are trained astronauts wearing G-suits.
Pilots are more trained than others, of course.

**The following G-effects are simulated:**
* Blackouts/redouts
* Loss of color vision a.k.a greyout
* Tunnel vision as G rises
* Kerbals grunt while they perform AGSM (anti-G straining maneuver) and take a heavy breath after
* Blood beating in kerbal's ears on redout (wear headphones with good bass and you'll feel it)
* G-LOC (G-induced loss of consciousness)
* Kerbal deaths of a sustained over-G
 
G forces have different severity in four directions: upward, downward, backward, forward, so you may find that a kerbal launched in a rocket stands more G than
a kerbal piloting a plane upside down on a circular trajectory.
Kerbal's specialization also affects how much he can stand.

**IMPORTANT**<br/>
Currently a simple picture is used as an overlay which affects the entire KSP window, including the UI (the ui of some mods may be unaffected).<br/>
Until I figured out how to use the shader correctly (not my main priority) it'll stay that way.<br/>
I don't know much about g-effects currently but Sers default config feels unrealistic, my personel recommended settings are included.<br/>
If you know more about this topic feel free to contact me to help me improve this mod.

**Installation:**

Place contents of the GameData folder of the zip into your KSP/GameData folder

**Configuration:**
Configuration of the mod is done via G-Effects.cfg file. Look through it to have an exhaustive description of its parameters.
I have also added a ingame menu accessible via [modifier key (alt-gr for me on qwertz layout)](https://wiki.kerbalspaceprogram.com/wiki/Key_bindings) + F8<br/>
This menu allows to enable/disable IVA/maincam visual effects and Kerbal g limits, change the overall sound effect volume from this mod, the g-resistance and the delta g tolerance.<br/>

**Known issues and limitations:**
- Be careful with various tweaks that may lead to some sudden acceleration rise or collision lags, for example ejection or undocking force tweaks, as they may cause crew to lose consciousness or even die of excessive g forces.
- G-forces caused by a ship's rotation don't affect because they are not likely to be severe enough to induce any significant effects.
- Effects are calculated for the active vessel's crew only. You can switch to another vessel and back and have effects applied as if they have just started.
- The overlay can sometimes partially stay and won't fade away.
- Negative g-effects (redout) can be countered by positive over g and vice versa.
  
**To be implemented:**
- using Shaders instead of pictures
