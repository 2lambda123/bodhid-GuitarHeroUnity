using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]
public class Song
{
	//Data
	public FileInfo fileInfo;
	public bool ready;
	public Data data;

	public enum Difficulty
	{
		Easy,
		Medium,
		Hard,
		Expert
	}

    public enum ChartType
    {
        mid = 9,
        chart = 11
    }

    public enum ReadableAudioType
    {
        ogg = 0,
        mp3 = 1
    }

    [System.Serializable]
	public class Data
	{
		public Notes notes;
		public Info info;
		public List<SyncTrack> syncTrack;
		public List<SongEvent> events;
	}

	[System.Serializable]
	public class Info
	{		
        public string chartName = "Unknown";
        public string chartArtist = "Unknown";
        public string chartCharter = "Unknown";
        public double offset = 0.0;
        public long resolution = 192;
        public string player2 = "Unknown";
        public Difficulty difficulty = Difficulty.Easy - 1;
        public double previewStart = 0.0;
        public double previewEnd = 0.0;
        public string genre = "Unknown";
        public string mediaType = "CD";
        public string musicStream = "Unknown";
        public string guitarStream = "Unknown";
        public string bassStream = "Unknown";
        public string rhythmStream = "Unknown";
	}

	[System.Serializable]
	public class Notes
	{
		public List<Note> easy, medium, hard, expert;
	}

	[System.Serializable]
	public class Note
	{
		public Note(uint _timestamp,  uint _fred, uint _duration, bool _star, bool _hammerOn)
		{
			timestamp = _timestamp;
			duration = _duration;
			fred = _fred;
			star = _star;
			hammerOn = _hammerOn;
		}
		public uint timestamp, duration, fred;
		public bool star, hammerOn;
	}

	[System.Serializable]
	public class SongEvent
	{
		public SongEvent(uint _timestamp, string _name)
		{
			timestamp = _timestamp;
			name = _name;
		}
		public uint timestamp;
		public string name;
	}

	[System.Serializable]
	public class SyncTrack
	{
		public SyncTrack(uint _timestamp,string _command, uint _value)
		{
			timestamp = _timestamp;
			command = _command;
			value = _value;
		}
		public uint timestamp, value;
		public string command;
	}    
}