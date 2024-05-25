using System;
using UnityEngine;

namespace G_Effects
{
	/// <summary>
	/// Holds personal values of G effects per kerbal
	/// </summary>
	public class KerbalGState
	{
		
		/// This is the cumulative effect of G's which is increasingly negative to represent a redout condition 
        /// and increasingly positive to represent a blackout condition. As G forces continue, the cumulativeG
        /// value is increased, by larger amounts for higher current G levels, to allow for longer periods of G
        /// force tolerance at lower G levels.
        public double cumulativeG = 0;
        public double downwardG = 0;
        public double forwardG = 0;
        public Vector3d previousAcceleration = Vector3d.zero; //this is used to track sudden G peaks caused by the imperfectness of physics
		public double gLocFadeAmount = 0;
		
		private int breathNeeded = 0;
		private double agsmStart = -1;
			
		public KerbalGState() {

		}
		
		public void startAGSM(double time) {
			if (!isAGSMStarted()) {
				breathNeeded = 0;
				agsmStart = time;
			}
		}
		
		public void stopAGSM(double time) {
			if (isAGSMStarted()) {
				double deltaTime = getSeverity() * (time - agsmStart);
				if (deltaTime > Configuration.breathThresholdTime) {
					breathNeeded = (int)Mathf.Clamp((float)(2 + (deltaTime - Configuration.breathThresholdTime) / 2), (float)Configuration.minBreaths, (float)Configuration.maxBreaths);
				} else {
					breathNeeded = 0;
				}
				agsmStart = -1;
			}
		}
		
		public bool isAGSMStarted() {
			return agsmStart > 0;			
		}
		
		public int getBreathNeeded() {
			return breathNeeded;
		}
		
		public int takeBreath() {
			return breathNeeded > 0 ? breathNeeded-- : 0;
		}
		
		public void resetBreath() {
			breathNeeded = 0;
		}
		
		public bool isGLocCondition() {
			return Math.Abs(cumulativeG) > Configuration.GLOC_CUMULATIVE_G;			
		}
		
		public bool isCriticalCondition() {
			return Configuration.gDeathEnabled && (Math.Abs(cumulativeG) > (Configuration.gLocStartCoeff + 0.8 * (Configuration.gDeathCoeff - Configuration.gLocStartCoeff)) * Configuration.MAX_CUMULATIVE_G);
		}
		
		public bool isDeathCondition() {
			return Configuration.gDeathEnabled && (Math.Abs(cumulativeG) > Configuration.gDeathCoeff * Configuration.MAX_CUMULATIVE_G);
		}
		
		public float getSeverity() {
			return (float)(Math.Abs(cumulativeG) / Configuration.MAX_CUMULATIVE_G);
		}

		public float getSeverityWithThreshold(float threshold) {
			float abs = (float)Math.Abs(cumulativeG);
			return (float)(Mathf.Clamp(abs - threshold, 0, abs) / (Configuration.MAX_CUMULATIVE_G - threshold));
		}

	}
}
