using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.Networking;

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
	object lockObject = new object();

	public void Load(string chartFile, OnLoaded onLoaded)
	{
		StartCoroutine(LoadCoroutine(chartFile, onLoaded));
	}

	public void PrepareAudio(Song song, OnPrepared onPrepared)
	{
		StartCoroutine(PrepareCoroutine(song, onPrepared));
	}

	private IEnumerator LoadCoroutine(string chartFile, OnLoaded onLoaded)
	{
		yield return null;
		song = new Song();
		song.ready = false;

        ChartReader testReader = new ChartReader();
        song = testReader.ReadChartFile(chartFile);

        song.ready = true;
        onLoaded(song);
	}

	private IEnumerator PrepareCoroutine(Song song, OnPrepared onPrepared)
	{
		Debug.Log("Loading guitar");
		yield return null;
		Song.Audio audio = new Song.Audio();
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
		}
		song.audio = audio;
		Debug.Log("Audio loaded");
		onPrepared();
	}

	
}