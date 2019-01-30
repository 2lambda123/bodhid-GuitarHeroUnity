using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    public Session session;
    public List<SongScanning.SongInfo> songs;
    public SongBlock songblockPrefab;
    public GameObject selectScreen;
    public static GameObject AudioObjects;
    public GameObject NonStaticAudioObjects;
    public RawImage fade;
    public void Awake()
    {
        AudioObjects = NonStaticAudioObjects;
        songs = SongScanning.allSongs;
        if (songs == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
            return;
        }

        fade.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeOutStart());


        selectScreen.SetActive(true);
        for (int i = 0; i < songs.Count; ++i)
        {
            SongBlock newBlock = Instantiate(songblockPrefab.gameObject).GetComponent<SongBlock>();
            newBlock.transform.SetParent(songblockPrefab.transform.parent);
            newBlock.transform.localPosition = Vector3.zero;
            newBlock.transform.localScale = Vector3.one;
            newBlock.Artist.text = songs[i].Artist;
            newBlock.SongName.text = songs[i].SongName;
            newBlock.Charter.text = songs[i].Charter;
            newBlock.ChartLocation = songs[i].fileLoction;
            newBlock.type = songs[i].type;
        }
        if (songs.Count == 0)
        {
            songblockPrefab.SongName.text = "No Songs found";
            songblockPrefab.GetComponent<Button>().enabled = false;
        }
        else
        {
            songblockPrefab.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeOutStart()
    {
        while (fade.color.a > 0)
        {
            yield return null;
            fade.color -= new Color(0, 0, 0, Time.deltaTime);
        }
        fade.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (session.playing)
            {
                session.playing = false;
                StartCoroutine(EndingSong());

            }
        }
    }

    public void LoadSong(string ChartLocation, Song.ChartType type)
    {
        StartCoroutine(StartingSong(ChartLocation, type));
    }
    private IEnumerator StartingSong(string ChartLocation, Song.ChartType type)
    {
        Debug.Log("Loading");

        Song song = null;
        SongLoader.Instance.Load(ChartLocation, type, delegate (Song _song)
        {
            song = _song;
        });        

        fade.color = new Color(0, 0, 0, 0);
        fade.gameObject.SetActive(true);
        while (fade.color.a < 1)
        {
            fade.color += new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        while (song == null) yield return null;
        Debug.Log("Loading audio");
        bool prepared = false;
        SongLoader.Instance.PrepareAudio(song, delegate ()
         {
             prepared = true;
         });

        while (!prepared) yield return null;
        Session.PlayerInfo[] players = new Session.PlayerInfo[]
        {
			//new Session.PlayerInfo(Song.Difficulty.Easy),
			//new Session.PlayerInfo(Song.Difficulty.Medium),
			//new Session.PlayerInfo(Song.Difficulty.Hard),
			new Session.PlayerInfo(Song.Difficulty.Expert)
        };
        session.Initialize(song, players);
        selectScreen.SetActive(false);
        Debug.Log("Ready to play");
        while (fade.color.a > 0)
        {
            fade.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        fade.gameObject.SetActive(false);
        System.GC.Collect();
        session.StartPlaying();
    }

    private IEnumerator EndingSong()
    {
        fade.color = new Color(0, 0, 0, 0);
        fade.gameObject.SetActive(true);
        while (fade.color.a < 1)
        {
            fade.color += new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        session.EndSession();
        selectScreen.SetActive(true);
        while (fade.color.a > 0)
        {
            fade.color -= new Color(0, 0, 0, Time.deltaTime);
            yield return null;
        }
        fade.gameObject.SetActive(false);
    }

    public void DestroyObj(GameObject go)
    {
        Destroy(go);
    }
}
	
	
