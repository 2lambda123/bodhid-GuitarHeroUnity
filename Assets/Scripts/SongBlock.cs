using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class SongBlock : MonoBehaviour
{
	public FileInfo fileInfo;
	public SongSelect songSelect;
    public Text Artist;
    public Text SongName;
    public Text Charter;
    public void Play()
	{
		songSelect.LoadSong(fileInfo);
	}
}
