using CommNet.Network;
using LibNoise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace G_Effects
{

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class SettingsLoader : MonoBehaviour
    {
        public void Awake()
        {
            // load settings when game start

        }
    }

    /// <summary>
    /// Reads and holds configuration parameters
    /// </summary>
    static class Configuration
    {

        //Configurable parameters:
        public static bool gLimits = true; //Enables g limits for kerbals (uses internal calculations instead of stock)
        public static double gResistance = 300; //Expresses the ability of the blood system to resist G effectsloat
        public static float downwardGMultiplier = 1.0f; //Multiplier of a G force that pulls a kerbal downwards
        public static float upwardGMultiplier = 2.0f; //Multiplier of a G force that pulls a kerbal upwards
        public static float forwardGMultiplier = 0.5f; //Multiplier of a G force that pulls a kerbal forward
        public static float backwardGMultiplier = 0.75f; //Multiplier of a G force that pulls a kerbal backward
        public static double deltaGTolerance = 3; //The G effects start if the G value is below 1 - tolerance or above 1 + tolerance
        public static float gGainMultiplier = 11.0f; //Multiplies the cumulative G gain per frame to compensate for too much gain.
        public static float gReductionMultiplier = 0.9f; //Multiplies the reduction of cumulative g per frame to correct for too fast reduction
        public static float gDampingThreshold = 100; //Threshold for damping unnatural acceleration peaks caused by imperfect physics (in G per frame)
        public static float gLocStartCoeff = 1.1f; //How much more should our poor kerbal suffer after complete loss of vision to have a G-LOC
        public static float gDeathCoeff = 20.0f; //How much more should a kerbal suffer to die of a sustained over-g
        public static bool gDeathEnabled = false; //Will the critical conditions and g-deaths take place or not

        //Greyout is a post-processing effect. It may conflict with other post-processing effects like b/w cameras etc, so disable greyouts if necessary.
        public static bool IVAGreyout = true; //Greyout effect in IVA view
        public static bool mainCamGreyout = true; //mainCam is used in 3rd person view. The effect is disabled by default because it eats up stock reenty and mach visual effects
        public static String gLocScreenWarning = null; //Text of a warning displayed when a kerbal loses consience. Leave empty to disable.
        public static Color redoutRGB = Color.red; //Red, green, blue components of a redout color (in case you are certain that green men must have green blood, for example)
        public static float gLocFadeSpeed = 4f; //Speed of fade-out visual effect when a kerbal is losing consciousness

        //Sound
        public static float breathThresholdTime = 8; //Time threshold in seconds for a kerbal needed to breathe after AGSM
        public static int maxBreaths = 6; //Maximum possible breath sounds to be played
        public static int minBreaths = 2; //Minimum breath sounds to be played
                                          //You can disable specific sound effects by specifying 0 volumes.
                                          //Volumes are specified as a fraction of KSP voice volume global setting (less than 1 means quiter, greater than 1 means louder)
        public static float masterVolume = 1.0f; //Used to make all soundeffects equally loader or quiter
        public static float gruntsVolume = 2.0f; //Volume of grunts when a kerbal tries to push blood back in his head on positive and frontal over-G
        public static float breathVolume = 2.0f; //Volume of heavy breath when a kerbal rests after over-G
        public static float heartBeatVolume = 2.0f; //Volume of blood beating in kerbal's ears on negative over-G
        public static float femaleVoicePitch = 1.4f; //How much female kerbals' voice pitch is higher than males' one
        public static float breathSoundPitch = 1.8f; //Pitch of heavy breath's sounds

        public static bool enableLogging = true; //Enable this only in debug purposes as it floods the logs very much

        //Kerbal personal modifiers are used as multipliers for the gResistance parameter and also affect the speed of G effects accumulation  
        public static float femaleModifier = 1; //How much stronger are females than males
                                                //Modifiers by specialization traits:
        public static Dictionary<string, float> traitModifiers = new Dictionary<string, float>() {
            {"Pilot", 1.3f}, {"Engineer", 1.0f}, {"Scientist", 1.0f}, {"Tourist", 0.6f}
        };

        //Calculated parameters:
        public static double positiveThreshold = 4;
        public static double negativeThreshold = -2;
        public static double MAX_CUMULATIVE_G = 1000;
        public static double GLOC_CUMULATIVE_G = 1100;

        //functions

        public static void OpenSettings()
        {
            G_Effects_gLimits_0 GLimits_0 = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>();
            GLimits_0.gLimits = gLimits;
            GLimits_0.gResistance = (float)gResistance;
            GLimits_0.downwardGMultiplier = downwardGMultiplier;
            GLimits_0.upwardGMultiplier = upwardGMultiplier;
            GLimits_0.forwardGMultiplier = forwardGMultiplier;
            GLimits_0.backwardGMultiplier = backwardGMultiplier;
            GLimits_0.deltaGTolerance = (float)deltaGTolerance;
            GLimits_0.gDampingThreshold = gDampingThreshold;

            G_Effects_gLimits_1 Glimits_1 = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>();
            Glimits_1.gLocStartCoeff = gLocStartCoeff;
            Glimits_1.gDeathCoeff = gDeathCoeff;
            Glimits_1.gDeathEnabled = gDeathEnabled;

            G_Effects_Visuals Visual = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>();
            Visual.gLocFadeSpeed = gLocFadeSpeed;
            Visual.red = Convert.ToInt32(redoutRGB.r * 255f);
            Visual.green = Convert.ToInt32(redoutRGB.g * 255f);
            Visual.blue = Convert.ToInt32(redoutRGB.b * 255f);

            Visual.EnableLogging = enableLogging;

            G_Effects_Sound_0 Sound_0 = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>();
            Sound_0.masterVolume = masterVolume;
            Sound_0.gruntsVolume = gruntsVolume;
            Sound_0.breathVolume = breathVolume;
            Sound_0.heartBeatVolume = heartBeatVolume;
            Sound_0.femaleVoicePitch = femaleVoicePitch;
            Sound_0.breathSoundPitch = breathSoundPitch;

            G_Effects_Sound_1 Sound_1 = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_1>();
            Sound_1.breathThresholdTime = breathThresholdTime;
            Sound_1.maxBreaths = maxBreaths;
            Sound_1.minBreaths = minBreaths;

            G_Effects_KerbalModifiers KerbalModifier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>();
            KerbalModifier.femaleModifier = femaleModifier;
            KerbalModifier.Pilot = traitModifiers["Pilot"];
            KerbalModifier.Engineer = traitModifiers["Engineer"];
            KerbalModifier.Scientist = traitModifiers["Scientist"];
        }

        public static void ApplySettings(string APP_NAME)
        {
            KSPLog.print("G-EFFECTS: ApplySettings");
            KSPLog.print("G-EFFECTS: " + redoutRGB.r * 255);


            //configurable parameters
            gLimits = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().gLimits;
            gResistance = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().gResistance;
            downwardGMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().downwardGMultiplier;
            upwardGMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().upwardGMultiplier;
            forwardGMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().forwardGMultiplier;
            backwardGMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().backwardGMultiplier;
            deltaGTolerance = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().deltaGTolerance;
            gDampingThreshold = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_0>().gDampingThreshold;
            gLocStartCoeff = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>().gLocStartCoeff;
            gDeathCoeff = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>().gDeathCoeff;
            gDeathEnabled = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>().gDeathEnabled;
            gGainMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>().gGainMultiplier;
            gReductionMultiplier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_gLimits_1>().gReductionMultiplier;

            //visuals
            mainCamGreyout = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().mainCamGreyout;
            IVAGreyout = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().IVAGreyout;
            gLocFadeSpeed = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().gLocFadeSpeed;
            redoutRGB.r = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().red / 255;
            redoutRGB.g = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().green / 255;
            redoutRGB.b = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().blue / 255;

            //sound
            breathThresholdTime = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_1>().breathThresholdTime;
            maxBreaths = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_1>().maxBreaths;
            minBreaths = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_1>().minBreaths;
            masterVolume = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().masterVolume;
            gruntsVolume = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().gruntsVolume;
            breathVolume = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().breathVolume;
            heartBeatVolume = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().heartBeatVolume;
            femaleVoicePitch = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().femaleVoicePitch;
            breathSoundPitch = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Sound_0>().breathSoundPitch;

            //Kerbal modifiers
            femaleModifier = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>().femaleModifier;
            traitModifiers["Pilot"] = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>().Pilot;
            traitModifiers["Engineer"] = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>().Engineer;
            traitModifiers["Scientist"] = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>().Scientist;
            traitModifiers["Tourist"] = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_KerbalModifiers>().Tourist;

            enableLogging = HighLogic.CurrentGame.Parameters.CustomParams<G_Effects_Visuals>().EnableLogging;

            saveConfiguration(APP_NAME.ToUpper());
            return;
        }

        //load the configuration from the G-Effects.cfg file
        public static void loadConfiguration(string root)
        {
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(root);

            if ((nodes == null) || (nodes.Length == 0))
            {
                return;
            }
            bool.TryParse(nodes[0].GetValue("gLimits"), out gLimits);
            double.TryParse(nodes[0].GetValue("gResistance"), out gResistance);
            float.TryParse(nodes[0].GetValue("downwardGMultiplier"), out downwardGMultiplier);
            float.TryParse(nodes[0].GetValue("upwardGMultiplier"), out upwardGMultiplier);
            float.TryParse(nodes[0].GetValue("fowardGMultiplier"), out forwardGMultiplier);
            float.TryParse(nodes[0].GetValue("backwardGMultiplier"), out backwardGMultiplier);
            double.TryParse(nodes[0].GetValue("deltaGTolerance"), out deltaGTolerance);
            float.TryParse(nodes[0].GetValue("gGainMultiplier"), out gGainMultiplier);
            float.TryParse(nodes[0].GetValue("gReductionMultiplier"), out gReductionMultiplier);
            float.TryParse(nodes[0].GetValue("gDampingThreshold"), out gDampingThreshold);
            float.TryParse(nodes[0].GetValue("gLocStartCoeff"), out gLocStartCoeff);
            float.TryParse(nodes[0].GetValue("gDeathCoeff"), out gDeathCoeff);
            bool.TryParse(nodes[0].GetValue("gDeathEnabled"), out gDeathEnabled);

            float.TryParse(nodes[0].GetValue("femaleModifier"), out femaleModifier);
            ConfigNode traitNode = nodes[0].GetNode("TRAIT_MODIFIERS");
            if (traitNode != null)
            {
                foreach (string valueName in traitNode.values.DistinctNames())
                {
                    float value;
                    if (float.TryParse(traitNode.GetValue(valueName), out value))
                    {
                        if (traitModifiers.ContainsKey(valueName))
                        {
                            traitModifiers[valueName] = value;
                        }
                        else
                        {
                            traitModifiers.Add(valueName, value);
                        }
                    }
                }
            }
            else KSPLog.print("" + root + " node not found");

            bool.TryParse(nodes[0].GetValue("ivaGreyout"), out IVAGreyout);
            bool.TryParse(nodes[0].GetValue("mainCamGreyout"), out mainCamGreyout);
            gLocScreenWarning = nodes[0].GetValue("gLocScreenWarning");
            string redoutColor = nodes[0].GetValue("redoutRGB");
            string[] redoutComponents;
            if ((redoutColor != null) && ((redoutComponents = redoutColor.Split(',')).Length == 3))
            {
                float r, g, b;
                if (float.TryParse(redoutComponents[0].Trim(' '), out r) &&
                    float.TryParse(redoutComponents[1].Trim(' '), out g) &&
                    float.TryParse(redoutComponents[2].Trim(' '), out b))
                {
                    redoutRGB.r = r / 255;
                    redoutRGB.g = g / 255;
                    redoutRGB.b = b / 255;
                }
            }
            float.TryParse(nodes[0].GetValue("gLocFadeSpeed"), out gLocFadeSpeed);
            float.TryParse(nodes[0].GetValue("breathThresholdTime"), out breathThresholdTime);
            int.TryParse(nodes[0].GetValue("maxBreaths"), out maxBreaths);
            int.TryParse(nodes[0].GetValue("minBreaths"), out minBreaths);
            float.TryParse(nodes[0].GetValue("masterVolume"), out masterVolume);
            float.TryParse(nodes[0].GetValue("gruntsVolume"), out gruntsVolume);
            float.TryParse(nodes[0].GetValue("breathVolume"), out breathVolume);
            float.TryParse(nodes[0].GetValue("heartBeatVolume"), out heartBeatVolume);
            float.TryParse(nodes[0].GetValue("femaleVoicePitch"), out femaleVoicePitch);
            float.TryParse(nodes[0].GetValue("breathSoundPitch"), out breathSoundPitch);

            bool.TryParse(nodes[0].GetValue("enableLogging"), out enableLogging);

            positiveThreshold = 1 + deltaGTolerance;
            negativeThreshold = 1 - deltaGTolerance;
            MAX_CUMULATIVE_G = 10 * gResistance;
            GLOC_CUMULATIVE_G = gLocStartCoeff * MAX_CUMULATIVE_G;
        }

        //saves the config to the configuration.cs
        public static void saveConfiguration(string root)
        {
            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(root);
            if ((nodes == null) || (nodes.Length == 0))
            {
                return;
            }

            //some of the strings have \n and \t at the end, this is done to keep the descriptions of the original config file.
            //I'm not able to restore the exact layout but I tried to keep it as close as possible.

            //Configurable parameters:
            nodes[0].SetValue("gLimits", gLimits, "Enables g limits for kerbals (uses internal calculations instead of stock)");
            nodes[0].SetValue("gResistance", gResistance, "Expresses the ability of the blood system to resist G effects. Value of 100 is equal to a rookie not wearing a g-suit");
            nodes[0].SetValue("downwardGMultiplier", downwardGMultiplier, "Multiplier of a G force that pulls a kerbal downwards");
            nodes[0].SetValue("upwardGMultiplier", upwardGMultiplier, "Multiplier of a G force that pulls a kerbal upwards");
            nodes[0].SetValue("forwardGMultiplier", forwardGMultiplier, "Multiplier of a G force that pulls a kerbal forward");
            nodes[0].SetValue("backwardGMultiplier", backwardGMultiplier, "Multiplier of a G force that pulls a kerbal backward");
            nodes[0].SetValue("deltaGTolerance", deltaGTolerance, "The G effects start if the G value is below 1 - tolerance or above 1 + tolerance");
            nodes[0].SetValue("gGainMultiplier", gGainMultiplier, "Multiplies the cumulative G gain per frame to compensate for too much gain");
            nodes[0].SetValue("gReductionMultiplier", gReductionMultiplier, "Multiplies the reduction of cumulative g per frame to correct for too fast reduction");
            nodes[0].SetValue("gDampingThreshold", gDampingThreshold, "Threshold for damping unnatural acceleration peaks caused by imperfect physics (in G per frame)");
            nodes[0].SetValue("gLocStartCoeff", gLocStartCoeff, "How much more our poor kerbal should suffer after complete loss of vision to have a G-LOC");
            nodes[0].SetValue("gDeathCoeff", gDeathCoeff, "How much more should a kerbal suffer to die of a sustained over-g");
            nodes[0].SetValue("gDeathEnabled", gDeathEnabled, "Will the critical conditions and g-deaths take place or not\n\n\t//Greyout is a post-processing effect. It may conflict with other post-processing effects like b/w cameras etc, so disable greyouts if necessary.");

            //Greyout
            nodes[0].SetValue("IVAGreyout", IVAGreyout, "Greyout effect in IVA view");
            nodes[0].SetValue("mainCamGreyout", mainCamGreyout, "mainCam is used in 3rd person view. The effect is not disabled by default but it eats up stock reenty and mach visual effects");
            nodes[0].SetValue("gLocFadeSpeed", gLocFadeSpeed, "Speed of fade-out visual effect when a kerbal is losing consciousness");
            nodes[0].SetValue("gLocScreenWarning", gLocScreenWarning, "Text of a warning displayed when a kerbal loses consciousness. Leave empty to disable.");
            nodes[0].SetValue("redoutRGB", $"{redoutRGB.r*255},{redoutRGB.g*255},{redoutRGB.b*255}", "Red, green, blue components of redout color (you can change it even to greenout in case you are certain that green men must have green blood)\n\n\t//You can disable specific sound effects by specifying 0 volumes.\n\t//Volumes are specified as a multiplier to KSP voice volume global setting (less than 1 means quieter, greater than 1 means louder)");

            //Sound
            nodes[0].SetValue("masterVolume", masterVolume, "Total volume multiplier (affects only the volume of this mod)");
            nodes[0].SetValue("gruntsVolume", gruntsVolume, "Volume of grunts and breath when a kerbal tries to push blood back in his head on positive and frontal over-G");
            nodes[0].SetValue("breathVolume", breathVolume, "Volume of heavy breath when a kerbal rests after over-G");
            nodes[0].SetValue("heartBeatVolume", heartBeatVolume, "Volume of blood beating in kerbal's ears on negative over-G");
            nodes[0].SetValue("femaleVoicePitch", femaleVoicePitch, "How much female kerbals' voice pitch is higher than males' one");
            nodes[0].SetValue("breathSoundPitch", breathSoundPitch, "Pitch of heavy breath's sounds");
            nodes[0].SetValue("breathThresholdTime", breathThresholdTime, "Time threshold in seconds for a kerbal needed to breathe after AGSM");
            nodes[0].SetValue("maxBreaths", maxBreaths, "Maximum possible breath sounds to be played");
            nodes[0].SetValue("minBreaths", minBreaths, "Minimum breath sounds to be played\n\n\t\t//Kerbal personal modifiers are used as multipliers for the gResistance parameter and also affect the speed of G effects accumulation\n");

            //Kerbal modifiers
            nodes[0].SetValue("femaleModifier", femaleModifier, "How much stronger are females than males\n\n//Modifiers by specialization traits. You can add any specialization here with the name matching its description in the game:");

            nodes[0].GetNode("TRAIT_MODIFIERS").SetValue("Pilot", traitModifiers["Pilot"]);
            nodes[0].GetNode("TRAIT_MODIFIERS").SetValue("Engineer", traitModifiers["Engineer"]);
            nodes[0].GetNode("TRAIT_MODIFIERS").SetValue("Scientist", traitModifiers["Scientist"]);
            nodes[0].GetNode("TRAIT_MODIFIERS").SetValue("Tourist", traitModifiers["Tourist"]);

            nodes[0].SetValue("enableLogging", enableLogging, "Enable this only in debug purposes as it floods the logs very much");

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../G-Effects.cfg";

            if (enableLogging)
            {
                KSPLog.print("G-EFFECTS: " + path);
            }

            KSPLog.print("G-EFFECTS: " + path);


            ConfigNode node = new ConfigNode();
            nodes[0].name = "G-EFFECTS";
            node.AddNode(nodes[0]);
            node.Save(path);
        }
    }
}
