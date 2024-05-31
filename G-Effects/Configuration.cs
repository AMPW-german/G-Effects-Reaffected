using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.Diagnostics;

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
        public static double gResistance = 300; //Expresses the ability of the blood system to resist G effectsloat 
        public static float downwardGMultiplier = 1.0f; //Multiplier of a G force that pulls a kerbal downwards
        public static float upwardGMultiplier = 2.0f; //Multiplier of a G force that pulls a kerbal upwards
        public static float forwardGMultiplier = 0.5f; //Multiplier of a G force that pulls a kerbal forward
        public static float backwardGMultiplier = 0.75f; //Multiplier of a G force that pulls a kerbal backward
        public static double deltaGTolerance = 3; //The G effects start if the G value is below 1 - tolerance or above 1 + tolerance
        public static float gDampingThreshold = 100; //Threshold for damping unnatural acceleration peaks caused by imperfect physics (in G per frame)
        public static float gLocStartCoeff = 1.1f; //How much more should our poor kerbal suffer after complete loss of vision to have a G-LOC
        public static float gDeathCoeff = 20.0f; //How much more should a kerbal suffer to die of a sustained over-g
        public static bool gDeathEnabled = false; //Will the critical conditions and g-deaths take place or not
        
        //Greyout is a post-processing effect. It may conflict with other post-processing effects like b/w cameras etc, so disable greyouts if necessary.
        public static bool IVAGreyout = true; //Greyout effect in IVA view
        public static bool mainCamGreyout = true; //mainCam is used in 3rd person view. The effect is disabled by default because it eats up stock reenty and mach visual effects
        public static String gLocScreenWarning = null; //Text of a warning displayed when a kerbal loses consience. Leave empty to disable.
        public static Color redoutRGB = Color.red; //Red, green, blue components of a redout color (in case you are certain that green men must have green blood, for example)
		public static int gLocFadeSpeed = 4; //Speed of fade-out visual effect when a kerbal is losing consciousness
		
		public static int breathThresholdTime = 8; //Time threshold in seconds for a kerbal needed to breathe after AGSM
		public static int maxBreaths = 6; //Maximum possible breath sounds to be played
		public static int minBreaths = 2; //Minimum breath sounds to be played
		//You can disable specific sound effects by specifying 0 volumes.
		//Volumes are specified as a fraction of KSP voice volume global setting (less than 1 means quiter, greater than 1 means louder)
        public static float masterVolume = 1.0f; //Used to make all soundeffects equally loader or quiter, controlled by the UI slider
		public static float gruntsVolume = 2.0f; //Volume of grunts when a kerbal tries to push blood back in his head on positive and frontal over-G
		public static float breathVolume = 2.0f; //Volume of heavy breath when a kerbal rests after over-G
		public static float heartBeatVolume = 2.0f; //Volume of blood beating in kerbal's ears on negative over-G
		public static float femaleVoicePitch = 1.4f; //How much female kerbals' voice pitch is higher than males' one
		public static float breathSoundPitch = 1.8f; //Pitch of heavy breath's sounds
		
		public static bool enableLogging = false; //Enable this only in debug purposes as it floods the logs very much
		
		//Kerbal personal modifiers are used as multipliers for the gResistance parameter and also affect the speed of G effects accumulation  
		public static float femaleModifier = 1; //How stronger are females than males
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
        public static void loadConfiguration(string root) {
        	ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes(root);
        	if ((nodes == null) || (nodes.Length == 0)) {
        	    return;
        	}
        	Double.TryParse(nodes[0].GetValue("gResistance"), out gResistance);
        	float.TryParse(nodes[0].GetValue("downwardGMultiplier"), out downwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("upwardGMultiplier"), out upwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("fowardGMultiplier"), out forwardGMultiplier);
        	float.TryParse(nodes[0].GetValue("backwardGMultiplier"), out backwardGMultiplier);
        	Double.TryParse(nodes[0].GetValue("deltaGTolerance"), out deltaGTolerance);
        	float.TryParse(nodes[0].GetValue("gDampingThreshold"), out gDampingThreshold);
        	float.TryParse(nodes[0].GetValue("gLocStartCoeff"), out gLocStartCoeff);
        	float.TryParse(nodes[0].GetValue("gDeathCoeff"), out gDeathCoeff);
        	bool.TryParse(nodes[0].GetValue("gDeathEnabled"), out gDeathEnabled);
        	        	
        	float.TryParse(nodes[0].GetValue("femaleModifier"), out femaleModifier);
        	ConfigNode traitNode = nodes[0].GetNode("TRAIT_MODIFIERS");
        	if (traitNode != null) {
        		foreach (string valueName in traitNode.values.DistinctNames()) {
        			float value;
        			if (float.TryParse(traitNode.GetValue(valueName), out value)) {
        				if (traitModifiers.ContainsKey(valueName)) {
        					traitModifiers[valueName] = value;
        				} else {
        					traitModifiers.Add(valueName, value);
        				}
    				}
        		}
        	} else KSPLog.print("" + root + " node not found");
        	
        	bool.TryParse(nodes[0].GetValue("IVAGreyout"), out IVAGreyout);
        	bool.TryParse(nodes[0].GetValue("mainCamGreyout"), out mainCamGreyout);
        	gLocScreenWarning = nodes[0].GetValue("gLocScreenWarning");
        	string redoutColor = nodes[0].GetValue("redoutRGB");
        	string[] redoutComponents;
        	if ((redoutColor != null)  && ((redoutComponents = redoutColor.Split(',')).Length == 3) ) {
        		float r, g, b;
        		if (float.TryParse(redoutComponents[0].Trim(' '), out r) &&
        		    float.TryParse(redoutComponents[1].Trim(' '), out g) &&
        		    float.TryParse(redoutComponents[2].Trim(' '), out b) ) {
        			redoutRGB.r = r / 255;
        			redoutRGB.g = g / 255;
        			redoutRGB.b = b / 255;
        		}
        	}
        	int.TryParse(nodes[0].GetValue("gLocFadeSpeed"), out gLocFadeSpeed);
        	int.TryParse(nodes[0].GetValue("breathThresholdTime"), out breathThresholdTime);
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
    }
}
