using System;
using System.ComponentModel;
using System.IO;

namespace NiohSaveOrganizer {
	class Save : IEditableObject, IComparable {
		public string name { get; set; }
		public string indexName { get; set; }
		public string missionName { get; }
		public ushort mission;
		public string regionName { get; }
		public byte region;
		public string lastModifiedName { get; }
		public DateTime lastModified;

		public string path;
		public string backupName;

		public Save Copy() {
			return (Save)this.MemberwiseClone();
		}

		void IEditableObject.BeginEdit() {
			backupName = name;
		}

		void IEditableObject.EndEdit() { }

		void IEditableObject.CancelEdit() {
			name = backupName;
		}

		int IComparable.CompareTo(object obj) {
			Save other = (Save)obj;
			return name.CompareTo(other.name);
		}




		public string GetFilePath() {
			return path + "\\SAVEDATA.BIN";
		}

		public Save(int i) {
			this.name = "New Save";
			this.indexName = String.Format("{0:00}", i);
			this.missionName = "";
			this.regionName = "";
		}

		public Save(string name, string path) {
			if (Int32.TryParse(name, out int j) && j < 100) { indexName = name; this.name = "New Save"; } else this.name = name;
			FileSystemInfo info = new FileInfo(path);
			lastModified = info.LastWriteTime;
			lastModifiedName = lastModified.ToString();
			this.path = Path.GetDirectoryName(path);
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
				reader.ReadBytes(56);
				region = reader.ReadByte();
				if (MissionData.regions.ContainsKey(region)) regionName = MissionData.regions[region];
				else regionName = "Unknown";
				reader.ReadBytes(3);
				mission = reader.ReadUInt16();
				if (MissionData.missions.ContainsKey(mission)) missionName = MissionData.missions[mission];
				else missionName = "Unknown";
			}
		}

		public Save(string path) : this(Path.GetFileName(path).Substring(8), path + "\\SAVEDATA.BIN") { }

	}
}
