using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SongBlock : MonoBehaviour
{
    public SongSelect songSelect;
    public string ChartLocation;
    public Text Artist;
    public Text SongName;
    public Image CoverImage;
    public Text Charter;
    public Song.ChartType type;
    float startTime = 15f;
    Song song;
    public void Play()
    {
        AudioHelper.resetAudio();

        if (SongLoader.SelectedTitle != null)
        {
            if (SongLoader.SelectedTitle.name == gameObject.name)
            {
                songSelect.LoadSong(ChartLocation, type);
                return;
            }
        }
        StartCoroutine(SelectSong());
    }

    private IEnumerator SelectSong()
    {
        SongLoader.Instance.Load(ChartLocation, type, delegate (Song _song)
        {
            song = _song;
        });

        bool prepared = false;
        SongLoader.Instance.PrepareAudio(song, delegate ()
        {
            prepared = true;
        });
        while (!prepared) yield return null;

        AudioHelper.stopAllAudio();

        SongLoader.setSelected(gameObject);

        if (song.data.info.previewStart / 1000 > 15)
            startTime = (float)song.data.info.previewStart / 1000;

        AudioHelper.setAllAudioTime(startTime);
        AudioHelper.playAllAudio(0);
    }
}
