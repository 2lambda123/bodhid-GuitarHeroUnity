using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using mid2chart;

public class SongScanning : MonoBehaviour
{
	public static List<SongInfo> allSongs;
    public string dir;
    public delegate void OnFinished(List<SongInfo> songs);
	public Image loadingImage;
	private List<SongInfo> songs = null;
	System.Object lockObject = new System.Object();
	
	[System.Serializable]
	public class SongInfo
	{
		public string fileLoction;
        public string Artist { get; set; }
        public string SongName { get; set; }
        public string Charter { get; set; }
        public string Album { get; set; }
        public long offset { get; set; }
        public long PreviewStartTime { get; set; }
        public Song.ChartType type { get; set; }
    }

	private void Start()
	{
        dir = Application.dataPath + "/Songs/";

        Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.Low;
		StartCoroutine(ScanAndContinue(delegate (List<SongInfo> songs)
		{
			allSongs = songs;
		}));
	}

	private IEnumerator ScanAndContinue(OnFinished onFinished)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;
		Thread thread = new Thread(ScanForSongsRecursively);
		thread.IsBackground = true;
		thread.Start(new DirectoryInfo(dir));
		while (true)
		{
			yield return null;
			lock (lockObject)
			{
				if (songs != null) break;
			}
		}
		thread.Abort();
		allSongs = songs;
		while (asyncLoad.isDone)
		{
			Debug.Log("Still loading scene: " + asyncLoad.progress);
			if (asyncLoad.progress >= 0.9) break;
			yield return null;
		}

		while (loadingImage.color.a > 0)
		{
			loadingImage.color -= new Color(0, 0, 0, Time.deltaTime);
			yield return null;
		}

		asyncLoad.allowSceneActivation = true;
	}

	private void ScanForSongsRecursively(object folder)
	{
		List<SongInfo> list = new List<SongInfo>();

        string[] chartFiles = Directory.GetFiles(dir, "*.chart", SearchOption.AllDirectories);
        string[] midFiles = Directory.GetFiles(dir, "*.mid", SearchOption.AllDirectories);
        string[] combinedFiles = chartFiles.Concat(midFiles).ToArray();

        foreach (string s in combinedFiles)
        {
            switch (s.Substring(s.Length - 1, 1))
            {
                case "t":
                    try
                    {
                        ChartReader testReader = new ChartReader();
                        Song _testCurrentChart = new Song();
                        _testCurrentChart = testReader.ReadChartFile(s);
                        list.Add(CreateSongInfo(s, Song.ChartType.chart));
                    }
                    catch (Exception e)
                    {

                    }
                    break;

                case "d":
                    try
                    {
                        SongMID midFile = MidReader.ReadMidi(s, false);
                        ChartWriter.WriteChart(midFile, dir + "/notes.chart", false);
                        ChartReader testReader = new ChartReader();
                        Song _testCurrentChart = new Song();
                        _testCurrentChart = testReader.ReadChartFile(@dir + "/notes.chart");
                        File.Delete(@dir + "/notes.chart");
                        list.Add(CreateSongInfo(s, Song.ChartType.mid));
                    }
                    catch (Exception e)
                    {
                       
                    }
                    break;
            }
        }
        
        lock (lockObject)
        {
            songs = list;
        }
	}

    private SongInfo CreateSongInfo(string s, Song.ChartType type)
    {
        SongInfo temp = new SongInfo();

        string path = Path.GetDirectoryName(s);
        string[] songINIFile = Directory.GetFiles(path, "*.ini", SearchOption.AllDirectories);

        if (songINIFile.Length > 0 && File.Exists(path + "//song.ini"))
        {
            foreach (string line in File.ReadAllLines(path + "//song.ini"))
            {
                if (line.Contains("artist ") || line.Contains("artist="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    temp.Artist = spaceRemove.Substring(7);
                }

                if (line.Contains("name ") || line.Contains("name="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    temp.SongName = spaceRemove.Substring(5);
                }

                if (line.Contains("charter ") || line.Contains("charter="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    temp.Charter = spaceRemove.Substring(8);
                }
                if (line.Contains("album ") || line.Contains("album="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    temp.Album = spaceRemove.Substring(6);
                }

                if (line.Contains("delay ") || line.Contains("delay="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    temp.offset = Math.Abs((long)Convert.ToUInt64(spaceRemove.Substring(6)));
                }

                if (line.Contains("preview_start_time ") || line.Contains("preview_start_time="))
                {
                    string spaceRemove = line.Replace("= ", "=").Replace(" =", "=");
                    string value = spaceRemove.Substring(19);
                    uint _time = 0;
                    uint.TryParse(value, out _time);
                    long time = Math.Abs(_time);
                    temp.PreviewStartTime = time;                    
                }
            }
            temp.type = type;
            temp.fileLoction = s;

            return temp;
        }
        else
        {
            ChartReader chartReader = new ChartReader();
            Song _currentChart = new Song();
            _currentChart = chartReader.ReadChartFile(s);

            temp.Artist = _currentChart.data.info.chartArtist;
            temp.SongName = _currentChart.data.info.chartName;
            temp.Charter = _currentChart.data.info.chartCharter;
            temp.PreviewStartTime = (long)_currentChart.data.info.previewStart;
            temp.type = type;
            temp.offset = (long)_currentChart.data.info.offset;
            temp.Album = "Album Unknown";
            temp.fileLoction = s;
        }

        return temp;
    }
}
	
	
