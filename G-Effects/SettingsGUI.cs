using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace G_Effects
{
    public class SettingsGUI : GameParameters.CustomParameterNode
    {
        public override string Title { get { return ""; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "G-Effects"; } }
        public override string DisplaySection { get { return "G-Effects"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("G-Lmits")]
        public bool GLimits = true;

        [GameParameters.CustomParameterUI("Maincam effects")]
        public bool mainCamGreyout = true;

        [GameParameters.CustomParameterUI("IVA effects")]
        public bool IVAGreyout = true;

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
