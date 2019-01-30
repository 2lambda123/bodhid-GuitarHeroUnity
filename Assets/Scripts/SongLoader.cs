using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.Networking;
using NAudio.Wave;
using mid2chart;

public class SongLoader : MonoBehaviour
{
	private static SongLoader instance;
	public static SongLoader Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("SongLoader").AddComponent<SongLoader>();
			}
			return instance;
		}
	}
	public delegate void OnLoaded(Song song);
	public delegate void OnPrepared();

	//threading
	private Song song;
    private string error;
    private string chartLocation;
    object lockObject = new object();

	public void Load(string chartFile, Song.ChartType type, OnLoaded onLoaded)
	{
		StartCoroutine(LoadCoroutine(chartFile, type, onLoaded));
	}

    public void DestroyObject(GameObject go)
    {
        Destroy(go);
    }

    public void PrepareAudio(Song song, OnPrepared onPrepared)
	{
		StartCoroutine(PrepareCoroutine(song, onPrepared));
	}

	private IEnumerator LoadCoroutine(string chartFile, Song.ChartType type, OnLoaded onLoaded)
	{
		yield return null;
        song = new Song();
        song.ready = false;
        ChartReader testReader = null;

        switch (type)
        {
            case Song.ChartType.chart:
                testReader = new ChartReader();
                song = testReader.ReadChartFile(chartFile);
                break;
            case Song.ChartType.mid:
                SongMID midFile = MidReader.ReadMidi(chartFile, false);
                ChartWriter.WriteChart(midFile, Application.dataPath + "/notes.chart", false);
                testReader = new ChartReader();
                song = testReader.ReadChartFile(@Application.dataPath + "/notes.chart");
                File.Delete(@Application.dataPath + "/notes.chart");
                break;
        }
        chartLocation = chartFile;
        song.ready = true;
        onLoaded(song);
	}

	private IEnumerator PrepareCoroutine(Song song, OnPrepared onPrepared)
	{
		Debug.Log("Loading guitar");
		yield return null;

        string path = Path.GetDirectoryName(chartLocation);
        string[] mp3Files = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);
        string[] oggFiles = Directory.GetFiles(path, "*.ogg", SearchOption.AllDirectories);

        if (mp3Files.Length > 0)
        {
            foreach (string loc in mp3Files)
            {
                GameObject NewAudio = new GameObject();
                AudioSource NewAudioSource = NewAudio.AddComponent<AudioSource>();
                NewAudio.transform.SetParent(SongSelect.AudioObjects.transform);
                NewAudio.name = loc.Substring(path.Length + 1, 6);

                string OutputAudioFilePath = @Application.dataPath + "/temp.wav";
                using (var reader = new Mp3FileReader(loc))
                {
                    WaveFileWriter.CreateWaveFile(OutputAudioFilePath, reader);
                }

                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(@Application.dataPath + "/temp.wav", AudioType.WAV))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.LogError(uwr.error);
                        yield break;
                    }
                    yield return null;
                    NewAudioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                }

                File.Delete(@Application.dataPath + "/temp.wav");
            }
        }

        if (oggFiles.Length > 0)
        {
            foreach (string loc in oggFiles)
            {
                GameObject NewAudio = new GameObject();
                AudioSource NewAudioSource = NewAudio.AddComponent<AudioSource>();
                NewAudio.transform.SetParent(SongSelect.AudioObjects.transform);

                using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(loc, AudioType.OGGVORBIS))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        Debug.LogError(uwr.error);
                        yield break;
                    }
                    yield return null;
                    NewAudioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
        }




        /*Song.Audio audio = new Song.Audio();
		FileInfo guitarFileInfo = new FileInfo(song.fileInfo.Directory.FullName + "/guitar.ogg");
		if (guitarFileInfo.Exists)
		{
			using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(guitarFileInfo.FullName, AudioType.OGGVORBIS))
			{
				yield return uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.LogError(uwr.error);
					yield break;
				}
				yield return null;
				audio.guitar = DownloadHandlerAudioClip.GetContent(uwr);
			}
		}
		Debug.Log("Loading song");
		yield return null;
		FileInfo songFileInfo = new FileInfo(song.fileInfo.Directory.FullName + "/song.ogg");
		if (songFileInfo.Exists)
		{
			using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(songFileInfo.FullName, AudioType.OGGVORBIS))
			{
				yield return uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.LogError(uwr.error);
					yield break;
				}
				yield return null;
				audio.song = DownloadHandlerAudioClip.GetContent(uwr);
			}
		}
		Debug.Log("Loading rhythm");
		yield return null;
		FileInfo rhythmFileInfo = new FileInfo(song.fileInfo.Directory.FullName + "/rhythm.ogg");
		if (rhythmFileInfo.Exists)
		{
			using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(rhythmFileInfo.FullName, AudioType.OGGVORBIS))
			{
				yield return uwr.SendWebRequest();
				if (uwr.isNetworkError || uwr.isHttpError)
				{
					Debug.LogError(uwr.error);
					yield break;
				}
				yield return null;
				audio.rhythm = DownloadHandlerAudioClip.GetContent(uwr);
			}
		}*/
		//song.audio = audio;
		Debug.Log("Audio loaded");
		onPrepared();
	}

	
}