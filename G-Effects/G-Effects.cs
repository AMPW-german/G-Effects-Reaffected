﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace G_Effects
{
    /// <summary>
    /// The G-Effects plugin for Kerbal Space Program
    /// 
    ///Copyright (C) 2024 AMPW
    ///This program is free software; you can redistribute it and/or
    ///modify it under the terms of the GNU General Public License
    ///as published by the Free Software Foundation; either version 2
    ///of the License, or (at your option) any later version.
    ///
    ///This program is distributed in the hope that it will be useful,
    ///but WITHOUT ANY WARRANTY; without even the implied warranty of
    ///MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ///GNU General Public License for more details.
    /// 
    ///You should have received a copy of the GNU General Public License
    ///along with this program.  If not, see <http://www.gnu.org/licenses/>.
    /// </summary>

    //this class is used to register the settings gui to the events so it's always working
    [KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
    public class G_Effects_Always : MonoBehaviour
    {
        const string APP_NAME = "G-Effects";
        protected void Start()
        {
            GameEvents.OnGameSettingsApplied.Add(ApplySettings);
            GameEvents.onGamePause.Add(OpenSettings);
        }



        /*protected void OnDestroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(ApplySettings);
            GameEvents.onGamePause.Remove(OpenSettings);
        }*/

        void OpenSettings()
        {
            KSPLog.print("G-EFFECTS: Pause");
            Configuration.OpenSettings();
        }

        void ApplySettings()
        {
            Configuration.ApplySettings(APP_NAME);
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class G_Effects : MonoBehaviour
    {

        //TODO simulate orientation loss on G-LOC

        const string APP_NAME = "G-Effects";
        const string CONTROL_LOCK_ID = "G_EFFECTS_LOCK";
        public const double G_CONST = 9.81;
        public const double MAX_GLOC_FADE = 4.0;

        ProtoCrewMember commander = null;
        double downwardG;
        double forwardG;
        bool playEffects = false;
        bool paused = false;

        GEffectsAudio gAudio = new GEffectsAudio();
        GEffectsVisuals visuals;

        //Specialization priority to be a commander
        Dictionary<string, int> priorities = new Dictionary<string, int>() {
            {"pilot", 3}, {"engineer", 2}, {"scientist", 2}, {"tourist", 0}
        };

        //This is for G effects persistance
        Dictionary<string, KerbalGState> kerbalGDict = new Dictionary<string, KerbalGState>();

        protected void Start()
        {

            // Hook into the rendering queue to draw the G effects
            //RenderingManager.AddToPostDrawQueue(3, new Callback(drawGEffects));
            GameEvents.onGamePause.Add(onPause);
            GameEvents.onGameUnpause.Add(onUnPause);
            GameEvents.onCrash.Add(onCrewKilled);
            GameEvents.onCrashSplashdown.Add(onCrewKilled);
            //GameEvents.onCrewKilled.Add(onCrewKilled);
            GameEvents.onVesselChange.Add(onVesselChange);

            ProtoCrewMember.doStockGCalcs = false;

            //This corrects KSP bug with reference part not restored when switching to IVA and back
            if (FlightGlobals.ActiveVessel.GetReferenceTransformPart() == null)
            {
                FlightGlobals.ActiveVessel.FallBackReferenceTransform();
            }
            FlightGlobals.ActiveVessel.SetReferenceTransform(FlightGlobals.ActiveVessel.GetReferenceTransformPart(), true);
        }

        protected void OnDestroy()
        {
            //RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGEffects));
            GameEvents.onGamePause.Remove(onPause);
            GameEvents.onGameUnpause.Remove(onUnPause);
            GameEvents.onCrash.Remove(onCrewKilled);
            GameEvents.onCrashSplashdown.Remove(onCrewKilled);
            //GameEvents.onCrewKilled.Remove(onCrewKilled);
            GameEvents.onVesselChange.Remove(onVesselChange);
        }

        /*protected void LateUpdate() {
			//PORTRAIT_AGENT.LateUpdate();
			if (commander != null) {
				KerbalPortrait portrait = KerbalPortraitGallery.Instance.Portraits.Find(p => p.crewMemberName.Equals(commander.name));
				if (portrait != null) {
					Texture2D texture = new Texture2D(1,1);
					texture.SetPixel(0,0,new Color(0,0,0,0.5f));
					texture.Resize((int)portrait.portrait.rectTransform.rect.width, (int)portrait.portrait.rectTransform.rect.height);
					portrait.overlayImg = texture;
					portrait.OverlayUpdate(true, 0);
				}
			}
		}*/

        private void applyControlLock()
        {
            if (InputLockManager.GetControlLock(CONTROL_LOCK_ID) != ControlTypes.None)
            {
                FlightGlobals.ActiveVessel.ctrlState.NeutralizeStick();
            }
        }

        void onPause()
        {
            paused = true;
            gAudio.pauseAllSounds(true);
        }

        void onUnPause()
        {
            paused = false;
            gAudio.pauseAllSounds(false);
        }

        void onCrewKilled(EventReport eventReport)
        {
            resetValues();
            gAudio.stopAllSounds();
            gAudio.removeFilters();
        }

        void onVesselChange(Vessel vessel)
        {
            resetValues();
            gAudio.stopAllSounds();
            gAudio.removeFilters();
        }

        void resetValues()
        {
            commander = null;
            kerbalGDict.Clear(); //kerbalGDict is for persistence actually but if not cleared the G effects on crew will be "frozen" on switch out/in vessel
            InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
            visuals.disableCameraFilters();
        }

        protected void Awake()
        {
            Configuration.loadConfiguration(APP_NAME.ToUpper());
            if (!gAudio.isInitialized())
            {
                gAudio.initialize(Configuration.gruntsVolume, Configuration.breathVolume, Configuration.heartBeatVolume, Configuration.femaleVoicePitch, Configuration.breathSoundPitch);
            }

            visuals = GEffectsVisuals.initialize();

            GameParameters.AdvancedParams pars = HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>();
            pars.GKerbalLimits = true;
        }

        public void FixedUpdate()
        {

            applyControlLock();

            if (paused || !Configuration.gLimits || TimeWarp.WarpMode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRate != 1)
            {
                return;
            }

            Vessel vessel = FlightGlobals.ActiveVessel;

            commander = null; //the commander is recalculated every update
            playEffects = false; //this changes to true later if all necessary conditions are met

            //Return without any effect if the vessel hasn't any crew
            if ((vessel.GetCrewCount() == 0))
            {
                return;
            }

            /*if (!gAudio.isBoundToTransform()) {
				gAudio.bindToTransform(FlightCamera.fetch.mainCamera.transform);
			}*/
            if (vessel.isEVA)
            {
                commander = vessel.GetVesselCrew()[0];
            }
            else
            {
                Part controlSourcePart = vessel.GetReferenceTransformPart(); //doesn't work well since KSP 1.1.1. Seems to be a bug.
                                                                             //Part controlSourcePart = vessel.parts.Find(p => p.isControlSource);
                if (controlSourcePart.CrewCapacity > 0)
                { //otherwise the vessel is controlled via probe core
                  //Find a crew member that most likely is controlling the vessel. So he is the one who sees the effects.
                    foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew())
                    {
                        if (crewMember.seat == null //this is the case of a kerbal in an external seat
                            || crewMember.seat.part.FindModuleImplementing<ModuleCommand>() != null)
                        {
                            commander = bestCommander(commander, crewMember);
                        }
                        /*if (crewMember.seat.part.isControlSource == Vessel.ControlLevel.PARTIAL_MANNED ||
						    crewMember.seat.part.isControlSource == Vessel.ControlLevel.FULL) {
							commander = bestCommander(commander, crewMember);
						}*/
                    }
                }//!!(Collision)(referencePart.currentCollisions.ToArray()[0]).
            }
            if (commander == null)
            { //if there's still no commander in the vessel then control lock must be removed because it is probably a probe core that has control at the moment
                InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
            }

            bool isIVA = CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA;

            playEffects = (commander != null) &&
                (
                (Configuration.IVAGreyout && isIVA) ||
                (Configuration.mainCamGreyout && !isIVA)
                ) &&
                !MapView.MapIsEnabled;

            //greyOutAllowed = isIVA && Configuration.IVAGreyout || !isIVA && Configuration.mainCamGreyout;
            visuals.flightCameraFilter.setBypass(isIVA || !playEffects);
            visuals.internalCameraFilter.setBypass(!isIVA || !playEffects);
            gAudio.setAudioEnabled(playEffects);
            //PORTRAIT_AGENT.enableText(!playEffects);

            //Calcualte g-effects for each crew member
            foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew())
            {

                if (isRosterDead(crewMember))
                {
                    continue;
                }

                KerbalGState gState;
                if (!kerbalGDict.TryGetValue(crewMember.name, out gState))
                {
                    gState = new KerbalGState();
                    kerbalGDict.Add(crewMember.name, gState);
                }
                //Calculate modifer by Kerbal individual charateristics
                float kerbalModifier = 1;
                Configuration.traitModifiers.TryGetValue(crewMember.experienceTrait.Title, out kerbalModifier);
                if (crewMember.gender == ProtoCrewMember.Gender.Female)
                {
                    kerbalModifier *= Configuration.femaleModifier;
                }

                //Calculate G forces
                Vector3d gAcceleration = FlightGlobals.getGeeForceAtPosition(vessel.GetWorldPos3D()) - vessel.acceleration;
                Vector3d cabinAcceleration = vessel.transform.InverseTransformDirection(gAcceleration); //vessel.transform is an active part's transform
                writeDebug("crew=" + crewMember.name + " cabinAcceleration=" + cabinAcceleration);
                cabinAcceleration = dampAcceleration(cabinAcceleration, gState.previousAcceleration);
                gState.previousAcceleration = cabinAcceleration;
                gState.downwardG = cabinAcceleration.z / G_CONST; //These are true G values
                gState.forwardG = cabinAcceleration.y / G_CONST;
                downwardG = gState.downwardG * (gState.downwardG - 1 > 0 ? Configuration.downwardGMultiplier : Configuration.upwardGMultiplier);//These are modified G values for usage in further calculations
                forwardG = gState.forwardG * (gState.forwardG > 0 ? Configuration.forwardGMultiplier : Configuration.backwardGMultiplier);

                float deltaTime = TimeWarp.fixedDeltaTime;
                gState.cumulativeG -= Math.Sign(gState.cumulativeG) * Configuration.gResistance * kerbalModifier * deltaTime * Configuration.gReductionMultiplier;
                //gAudio.applyFilter(1 - Mathf.Clamp01((float)(1.25 * Math.Pow(Math.Abs(gData.cumulativeG) / conf.MAX_CUMULATIVE_G, 2) - 0.2)));
                if (crewMember.Equals(commander))
                {
                    visuals.doGreyout(gState);
                }
                if ((downwardG > Configuration.positiveThreshold) || (downwardG < Configuration.negativeThreshold) || (forwardG > Configuration.positiveThreshold) || (forwardG < Configuration.negativeThreshold))
                {

                    double rebCompensation = Configuration.gResistance * kerbalModifier - Configuration.deltaGTolerance * Configuration.deltaGTolerance / kerbalModifier; //this is calculated so the rebound is in equilibrium with cumulativeG at the very point of G threshold
                    gState.cumulativeG +=
                        ((Math.Sign(downwardG - 1 + forwardG) * rebCompensation + (Math.Abs(downwardG - 1) * (downwardG - 1) + Math.Abs(forwardG) * forwardG) * Configuration.gGainMultiplier) / kerbalModifier) * deltaTime;
                    gAudio.stopBreath();
                    gState.resetBreath();

                    if (gState.isGLocCondition())
                    {
                        loseConsciousness(crewMember, gState, crewMember.Equals(commander), playEffects);
                        gAudio.stopHeartBeats();
                    }
                    else if (crewMember.Equals(commander))
                    {
                        //Positive and frontal G sound effects
                        if (((downwardG > Configuration.positiveThreshold) || (forwardG > Configuration.positiveThreshold)) && (gState.cumulativeG > 0.1 * Configuration.MAX_CUMULATIVE_G))
                        {
                            //gData.needBreath = Mathf.Max(gData.needBreath, (gData.cumulativeG > 0.6 * conf.MAX_CUMULATIVE_G) ? (int)(MAX_BREATHS * gData.getSeverity() - 1) : 0);
                            gState.startAGSM(Planetarium.GetUniversalTime());
                            gAudio.playGrunt(commander.gender.Equals(ProtoCrewMember.Gender.Female), -1f /*(float)((Math.Max(Math.Max(downwardG, forwardG), 10.0 + conf.positiveThreshold) - conf.positiveThreshold) / 10.0)*/);
                            //Negative G sound effects
                        }
                        else if (gState.cumulativeG < -0.1 * Configuration.MAX_CUMULATIVE_G)
                        {
                            if (gAudio.isHeartBeatsPlaying())
                            {
                                gAudio.setHeartBeatsVolume(Math.Min((float)(2 * Math.Abs(gState.cumulativeG + 0.1 * Configuration.MAX_CUMULATIVE_G) / (1 - 0.1) / Configuration.MAX_CUMULATIVE_G * Configuration.heartBeatVolume * GameSettings.VOICE_VOLUME), GameSettings.VOICE_VOLUME * Configuration.heartBeatVolume));
                            }
                            else
                            {
                                gAudio.playHeartBeats();
                            }
                        }
                    }

                }
                else
                {
                    //Breath back sound effect
                    gState.stopAGSM(Planetarium.GetUniversalTime());
                    int breathNeeded = gState.getBreathNeeded();
                    if ((breathNeeded > 0) && (gState.gLocFadeAmount == 0))
                    {
                        if (gAudio.tryPlayBreath(commander.gender.Equals(ProtoCrewMember.Gender.Female),
                                                 UnityEngine.Random.Range((float)Mathf.Clamp(breathNeeded - 2, 1, Configuration.maxBreaths), (float)breathNeeded) / (float)Configuration.maxBreaths * Configuration.breathVolume * GameSettings.VOICE_VOLUME,
                                                 1f - 0.2f * (1 - (float)breathNeeded / (float)Configuration.maxBreaths)))
                        {
                            gState.takeBreath();
                        }
                    }
                    if (Math.Abs(gState.cumulativeG) < 0.1 * Configuration.MAX_CUMULATIVE_G)
                    {
                        reboundConsciousness(crewMember, gState, crewMember.Equals(commander));
                    }
                }


                if (crewMember.Equals(commander))
                {
                    //gAudio.applyFilters((float)(1 - gState.cumulativeG / conf.MAX_CUMULATIVE_G));
                    gAudio.applyFilters((float)Math.Pow(1 - gState.gLocFadeAmount / MAX_GLOC_FADE, 2));
                }

                passValuesToStock(crewMember, gState);

                writeDebug("crew=" + crewMember.name + " cumulativeG=" + gState.cumulativeG);

                //If out of danger then stop negative G sound effects
                if (gState.cumulativeG > -0.3 * Configuration.MAX_CUMULATIVE_G)
                {
                    gAudio.stopHeartBeats();
                }

                if (gState.isDeathCondition())
                {
                    crewMember.Die();
                    kerbalGDict.Remove(crewMember.name);
                }
            }
        }

        //Damps acceleration peak if it is detected for the current frame.
        //Most likely the peaks are caused by imperfect physics and need to be damped for not causing unnatural effects on crew.
        //(Acceleration of a kerbal going EVA is about 44G)
        Vector3d dampAcceleration(Vector3d current_acc, Vector3d prev_acc)
        {
            double magnitude = (current_acc - prev_acc).magnitude;
            if ((current_acc - prev_acc).magnitude > Configuration.gDampingThreshold * G_CONST)
            {
                writeLog("Peak detected. G=" + ((current_acc - prev_acc).magnitude / G_CONST) + " threshold=" + Configuration.gDampingThreshold);
                return prev_acc;
            }
            else
            {
                return current_acc;
            }
        }

        //Determines who among of the two crew members is most likely in command 
        ProtoCrewMember bestCommander(ProtoCrewMember current, ProtoCrewMember candidate)
        {
            if (current == null)
            {
                return candidate;
            }
            else if (candidate == null)
            {
                return current;
            }
            //None of crewMemebers are null further

            int current_pr = -1;
            int candidate_pr = -1;

            priorities.TryGetValue(current.experienceTrait.Title.ToLower(), out current_pr);
            priorities.TryGetValue(candidate.experienceTrait.Title.ToLower(), out candidate_pr);
            if (candidate_pr > current_pr)
            { //return the one with the highest priority
                return candidate;
            }
            else if (current_pr > candidate_pr)
            {
                return current;
            }
            else
            { //or the one with the highest experience
                if (candidate.experienceLevel > current.experienceLevel)
                {
                    return candidate;
                }
                else
                {
                    return current;
                }
            }
        }

        void loseConsciousness(ProtoCrewMember crewMember, KerbalGState kerbalGData, bool isCommander, bool outputAllowed)
        {
            kerbalGData.stopAGSM(0);
            kerbalGData.resetBreath();
            kerbalGData.gLocFadeAmount += TimeWarp.fixedDeltaTime / Configuration.gLocFadeInSpeed;
            if (kerbalGData.gLocFadeAmount > G_Effects.MAX_GLOC_FADE)
            {
                kerbalGData.gLocFadeAmount = G_Effects.MAX_GLOC_FADE;
                if (isCommander)
                {
                    GEffectsAudio.prepareAudioSources(FindObjectsOfType(typeof(AudioSource)) as AudioSource[]); //a barbarian way to prevent audiosources from being immune to G-LOC mute
                    if (outputAllowed && (Configuration.gLocScreenWarning != null) && (Configuration.gLocScreenWarning.Length > 0) &&
                       !HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().GKerbalLimits)
                    {
                        ScreenMessages.PostScreenMessage(Configuration.gLocScreenWarning);
                    }
                    Vessel vessel = crewMember.KerbalRef.InVessel;
                    /*if (!hasProbeCore(vessel))
                    {
                        vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                    }*/
                    InputLockManager.SetControlLock(ControlTypes.ALL_SHIP_CONTROLS, CONTROL_LOCK_ID);
                    crewMember.SetInactive(60000, false);
                }
            }
        }


        void reboundConsciousness(ProtoCrewMember crewMember, KerbalGState kerbalGData, bool isCommander)
        {
            kerbalGData.gLocFadeAmount -= TimeWarp.fixedDeltaTime / Configuration.gLocFadeOutSpeed;
            if (kerbalGData.gLocFadeAmount <= 0)
            {
                kerbalGData.gLocFadeAmount = 0;
                if (isCommander)
                {
                    InputLockManager.RemoveControlLock(CONTROL_LOCK_ID);
                    if (crewMember.inactive)
                    {
                        crewMember.SetInactive(0, false);
                    }
                }
            }
        }

        bool hasProbeCore(Vessel vessel)
        {
            foreach (Part part in vessel.Parts)
            {
                if ((part.isControlSource == Vessel.ControlLevel.PARTIAL_MANNED) && (part.CrewCapacity == 0))
                    return true;
            }
            return false;
        }

        bool isRosterDead(ProtoCrewMember crewMember)
        {
            return crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Dead) || crewMember.rosterStatus.Equals(ProtoCrewMember.RosterStatus.Missing);
        }

        void passValuesToStock(ProtoCrewMember crewMember, KerbalGState gState)
        {
            GameParameters.AdvancedParams pars = HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>();
            crewMember.gExperienced =
                Math.Abs(gState.cumulativeG) / Configuration.GLOC_CUMULATIVE_G * PhysicsGlobals.KerbalGThresholdLOC * pars.KerbalGToleranceMult * ProtoCrewMember.GToleranceMult(crewMember);
        }

        void OnGUI()
        {
            if (paused || TimeWarp.WarpMode == TimeWarp.Modes.HIGH && TimeWarp.CurrentRate != 1 || !playEffects)
            {
                return;
            }

            KerbalGState kerbalGData;
            if ((commander == null) || !kerbalGDict.TryGetValue(commander.name, out kerbalGData))
            {
                return;
            }

            visuals.drawGEffects(kerbalGData);
        }



        void writeDebug(string text)
        {
            if (Configuration.enableLogging)
            {
                writeLog(text);
            }
        }

        void writeLog(string text)
        {
            KSPLog.print(APP_NAME + ": " + text);
        }

    }
}
