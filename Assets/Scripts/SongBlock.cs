using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class SongBlock : MonoBehaviour
{
	public SongSelect songSelect;
    public string ChartLocation;
    public Text Artist;
    public Text SongName;
    public Text Charter;
    public Song.ChartType type;
    public void Play()
	{
		songSelect.LoadSong(ChartLocation, type);
	}
}
