using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiohSaveOrganizer {
	class MissionData {
		public static Dictionary<byte, string> regions = new Dictionary<byte, string>{
			{ 130, "Prologue" }, 
			{ 129, "Kyushu"}, 
			{ 128, "Chugoku" }, 
			{ 135, "Kinki" },
			{ 134, "Tokai" }, 
			{ 133, "Sekigahara"}, 
			{ 132, "Omi"},
			{ 138, "Tohoku"},
			{ 136, "Osaka (Summer)"},
			{ 137, "Osaka (Winter)"}

		};
		public static Dictionary<ushort, string> missions = new Dictionary<ushort, string> {
			{ 36905, "Region Map" },
			{ 36940, "The Man with the Guardian Spirit" },
			{ 37117, "Isle of Demons" },
			{ 37211, "Deep in the Shadows" },
			{ 37080, "The Spirit Stone Slumbers" },
			{ 37359, "The Silver Mine Writhes" },
			{ 37210, "The Ocean Roars Again" },
			{ 36869, "Spider Nest Castle" },
			{ 36971, "Falling Snow" },
			{ 37264, "The Demon of Mount Hiei" },
			{ 36884, "The Iga Escape" },
			{ 37110, "Memories of Death-Lilies" },
			{ 37127, "The Defiled Castle" },
			{ 36870, "Immortal Flame" },
			{ 37083, "Sekigahara" },
			{ 36873, "The Source of Evil" },
			{ 36888, "A Defiled Holy Mountain" },
			{ 37019, "The Samurai from Sawayama" },
			{ 37064, "The Demon King Revealed" },
			{ 37215, "The Queen's Eyes" },
			{ 45891, "Yokai Country" },
			{ 40279, "The One-Eyed Dragon's Castle" },
			{ 38303, "Spirit Stone Huntress" },
			{ 38284, "Siege of Osaka (Winter)" },
			{ 38679, "Scion of Virtue" },
			{ 35886, "The Sanada's Resolve" },
			{ 34579, "Resentment Unleashed" },
			{ 34313, "The Last Samurai" }
		};

	}
}
