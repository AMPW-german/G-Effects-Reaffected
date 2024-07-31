using System.Collections;
using System.Reflection;

namespace G_Effects
{
    public class G_Effects_gLimits_0 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "cofigurable parameters (1)"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (1)"; } }
        public override string DisplaySection { get { return "G-Effects (1)"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        #region gLimits
        [GameParameters.CustomParameterUI("g limits")]
        public bool gLimits = true;

        [GameParameters.CustomFloatParameterUI("gResistance", toolTip = "Expresses the ability of the blood system to resist G effectsloat", minValue = 0, maxValue = 600, addTextField = true, asPercentage = false, displayFormat = "0")]
        public float gResistance = 300;

        [GameParameters.CustomFloatParameterUI("downwardGMultiplier", toolTip = "Multiplier of a G force that pulls a kerbal downwards", minValue = 0, maxValue = 2, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float downwardGMultiplier = 1.0f;

        [GameParameters.CustomFloatParameterUI("upwardGMultiplier", toolTip = "Multiplier of a G force that pulls a kerbal upwards", minValue = 0, maxValue = 4, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float upwardGMultiplier = 2.0f;

        [GameParameters.CustomFloatParameterUI("forwardGMultiplier", toolTip = "Multiplier of a G force that pulls a kerbal forward", minValue = 0, maxValue = 2, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float forwardGMultiplier = 0.5f;

        [GameParameters.CustomFloatParameterUI("backwardGMultiplier", toolTip = "Multiplier of a G force that pulls a kerbal backward", minValue = 0, maxValue = 2, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float backwardGMultiplier = 0.75f;

        [GameParameters.CustomFloatParameterUI("deltaG Tolerance", toolTip = "The G effects start if the G value is below 1 - tolerance or above 1 + tolerance", minValue = 0, maxValue = 6, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float deltaGTolerance = 3.0f;

        [GameParameters.CustomFloatParameterUI("gDampingThreshold", toolTip = "Threshold for damping unnatural acceleration peaks caused by imperfect physics (in G per frame)", minValue = 0, maxValue = 200, addTextField = true, asPercentage = false, displayFormat = "0")]
        public float gDampingThreshold = 100;
        #endregion

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

    public class G_Effects_gLimits_1 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "cofigurable parameters (2)"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (1)"; } }
        public override string DisplaySection { get { return "G-Effects (1)"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        #region gLimits
        [GameParameters.CustomFloatParameterUI("gLoc Start coefficient", toolTip = "How much more should our poor kerbal suffer after complete loss of vision to have a G-LOC", minValue = 0, maxValue = 4, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float gLocStartCoeff = Configuration.gLocStartCoeff;

        [GameParameters.CustomFloatParameterUI("gDeathCoeff", toolTip = "How much more should a kerbal suffer to die of a sustained over-g", minValue = 0, maxValue = 600, addTextField = true, asPercentage = false, displayFormat = "0")]
        public float gDeathCoeff = Configuration.gDeathCoeff;

        [GameParameters.CustomParameterUI("gDeath Enabled")]
        public bool gDeathEnabled = Configuration.gDeathEnabled;

        [GameParameters.CustomFloatParameterUI("gGainMultiplier", toolTip = "Multiplies the cumulative G gain per frame to compensate for too much gain", minValue = 1, maxValue = 16, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float gGainMultiplier = Configuration.gGainMultiplier;

        [GameParameters.CustomFloatParameterUI("gReductionMultiplier", toolTip = "Multiplies the reduction of cumulative g per frame to correct for too fast reduction", minValue = 0, maxValue = 2, addTextField = true, asPercentage = false, displayFormat = "0.00")]
        public float gReductionMultiplier = Configuration.gReductionMultiplier;
        #endregion


        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

    public class G_Effects_Visuals : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Visuals"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (1)"; } }
        public override string DisplaySection { get { return "G-Effects (1)"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }

        public string empty = string.Empty;

        #region visuals
        [GameParameters.CustomStringParameterUI("Visuals", lines = 1)]
        public string visualText = string.Empty;

        [GameParameters.CustomParameterUI("Maincam effects")]
        public bool mainCamGreyout = Configuration.mainCamGreyout;

        [GameParameters.CustomParameterUI("IVA effects")]
        public bool IVAGreyout = Configuration.IVAGreyout;

        [GameParameters.CustomFloatParameterUI("gLoc Fadespeed", toolTip = "Speed of fade-out visual effect when a kerbal is losing consciousness", minValue = 0f, maxValue = 10f, addTextField = true, asPercentage = false)]
        public float gLocFadeSpeed = Configuration.gLocFadeSpeed;

        [GameParameters.CustomStringParameterUI("Redout RGB:")]
        public string RedoutText = string.Empty;

        [GameParameters.CustomIntParameterUI("Red:", minValue = 0, maxValue = 255, stepSize = 1)]
        public int red = (int)Configuration.redoutRGB.r;

        [GameParameters.CustomIntParameterUI("Green:", minValue = 0, maxValue = 255, stepSize = 1)]
        public int green = (int)Configuration.redoutRGB.g;

        [GameParameters.CustomIntParameterUI("Blue:", minValue = 0, maxValue = 255, stepSize = 1)]
        public int blue = (int)Configuration.redoutRGB.b;
        #endregion

        [GameParameters.CustomParameterUI("enableLogging")]
        public bool EnableLogging = true;

        [GameParameters.CustomStringParameterUI("All parameters can be set to a higher/lower maximum number via the textfield.", lines = 4)]
        public string InfoText = string.Empty;

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

    public class G_Effects_Sound_0 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Sound (1)"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (2)"; } }
        public override string DisplaySection { get { return "G-Effects (2)"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        #region Sound
        [GameParameters.CustomStringParameterUI("You can disable specific sound effects by specifying 0 volumes.\nVolumes are a multiplier of KSP voice volume global setting.", lines = 4)]
        public string VolumeInfoText = string.Empty;

        [GameParameters.CustomFloatParameterUI("masterVolume", toolTip = "Used to make all soundeffects equally loader or quiter", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float masterVolume = Configuration.masterVolume;

        [GameParameters.CustomFloatParameterUI("gruntsVolume", toolTip = "Volume of grunts when a kerbal tries to push blood back in his head on positive and frontal over-G", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float gruntsVolume = Configuration.gruntsVolume;

        [GameParameters.CustomFloatParameterUI("breathVolume", toolTip = "Volume of heavy breath when a kerbal rests after over-G", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float breathVolume = Configuration.breathVolume;

        [GameParameters.CustomFloatParameterUI("breathVolume", toolTip = "Volume of blood beating in kerbal's ears on negative over-G", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float heartBeatVolume = Configuration.heartBeatVolume;

        [GameParameters.CustomFloatParameterUI("femaleVoicePitch", toolTip = "How much female kerbals' voice pitch is higher than males' one", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float femaleVoicePitch = Configuration.femaleVoicePitch;

        [GameParameters.CustomFloatParameterUI("breathSoundPitch", toolTip = "Pitch of heavy breath's sounds", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float breathSoundPitch = Configuration.breathSoundPitch;
        #endregion

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
    public class G_Effects_Sound_1 : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Sound (2)"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (2)"; } }
        public override string DisplaySection { get { return "G-Effects (2)"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        #region Sound
        [GameParameters.CustomFloatParameterUI("breathThresholdTime", toolTip = "Time threshold in seconds for a kerbal needed to breathe after AGSM", minValue = 0, maxValue = 16, stepCount = 16, displayFormat = "0.0", addTextField = true)]
        public float breathThresholdTime = Configuration.breathThresholdTime;

        [GameParameters.CustomIntParameterUI("maximum breaths", toolTip = "Maximum possible breath sounds to be played", minValue = 0, maxValue = 20, stepSize = 1)]
        public int maxBreaths = Configuration.maxBreaths;

        [GameParameters.CustomIntParameterUI("minimum breaths", toolTip = "Minimum possible breath sounds to be played", minValue = 0, maxValue = 20, stepSize = 1)]
        public int minBreaths = Configuration.minBreaths;
        #endregion

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }

    public class G_Effects_KerbalModifiers : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "KerbalModifiers"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects (2)"; } }
        public override string DisplaySection { get { return "G-Effects (2)"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }

        #region KerbalModifiers
        [GameParameters.CustomFloatParameterUI("female modifier", toolTip = "How much stronger are females than males", minValue = 0, maxValue = 2, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float femaleModifier = Configuration.femaleModifier;

        [GameParameters.CustomStringParameterUI("specialization modifiers:\n(contains only the stock ones)", lines = 2)]
        public string SpecializationModifierText = string.Empty;

        [GameParameters.CustomFloatParameterUI("Pilot:", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float Pilot = Configuration.traitModifiers["Pilot"];

        [GameParameters.CustomFloatParameterUI("Engineer:", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float Engineer = Configuration.traitModifiers["Engineer"];

        [GameParameters.CustomFloatParameterUI("Scientist:", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float Scientist = Configuration.traitModifiers["Scientist"];

        [GameParameters.CustomFloatParameterUI("Tourist:", minValue = 0, maxValue = 5, addTextField = true, asPercentage = false, displayFormat = "0.0")]
        public float Tourist = Configuration.traitModifiers["Tourist"];
        #endregion

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            return true;
        }
        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }
    }
}
