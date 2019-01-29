using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using static Song;
using System;

public class ChartReader 
{
    private string chartPath;    
    public string Path
    {
        get
        {
            return chartPath;
        }
    }

    private Song _chart;
    public Song Chart
    {
        get
        {
            return _chart;
        }
    }

    public ChartReader()
    {
        chartPath = "";
        _chart = new Song();
    }

    public Song ReadChartFile(string path)
    {
        try
        {
            string[] chart = File.ReadAllLines(path);
            Song.Notes notes = new Song.Notes();
            notes.easy = new List<Song.Note>();
            notes.medium = new List<Song.Note>();
            notes.hard = new List<Song.Note>();
            notes.expert = new List<Song.Note>();
            List<Song.SyncTrack> syncTrack = new List<Song.SyncTrack>();
            List<Song.SongEvent> events = new List<Song.SongEvent>();
            Song.Info info = new Song.Info();
            //Debug.Log(chart.Length);
            for (int i = 0; i < chart.Length; ++i)
            {
                if (chart[i].Contains("[Song]")) { i = LoadChartSong(info, chart, i); continue; }
                if (chart[i].Contains("[SyncTrack]")) { i = LoadChartSyncTrack(syncTrack, chart, i); continue; }
                if (chart[i].Contains("[Events]")) { i = LoadChartEvents(events, chart, i); continue; }
                if (chart[i].Contains("[ExpertSingle]")) { i = LoadChartNotes(chart, i, notes.expert, info.resolution); continue; }
                if (chart[i].Contains("[HardSingle]")) { i = LoadChartNotes(chart, i, notes.hard, info.resolution); continue; }
                if (chart[i].Contains("[MediumSingle]")) { i = LoadChartNotes(chart, i, notes.medium, info.resolution); continue; }
                if (chart[i].Contains("[EasySingle]")) { i = LoadChartNotes(chart, i, notes.easy, info.resolution); continue; }
            }
            Song.Data data = new Song.Data();
            data.syncTrack = syncTrack;
            data.info = info;
            data.events = events;
            data.notes = notes;
            _chart.data = data;
        }
        catch (System.Exception e)
        {

        }

        return Chart;
    }

    private int LoadChartSong(Song.Info info, string[] chart, int i)
    {
        int timeout = 100000;
        while (i < timeout)
        {
            if (chart[i].Contains("{"))
            {
                //Debug.Log("Start reading song info");
                i++;
                break;
            }
            i++;
        }
        while (i < timeout)
        {
            string[] data = chart[i].Split(new string[] { " = " }, StringSplitOptions.None);

            string slot = data[0].Replace(" ", string.Empty);
            slot = slot.Remove(0, 1);
            string attribute = string.Empty;

            if (data.Length > 1)
                attribute = data[1].Replace("\"", string.Empty);

            switch (slot)
            {
                case "Name":
                    info.chartName = attribute;
                    break;

                case "Artist":
                    info.chartArtist = attribute;
                    break;

                case "Charter":
                    info.chartCharter = attribute;
                    break;

                case "Offset":
                    info.offset = double.Parse(attribute);
                    break;

                case "Resolution":
                    info.resolution = long.Parse(attribute);
                    break;

                case "Player2":
                    info.player2 = attribute;
                    break;

                case "Difficulty":
                    info.difficulty = (Difficulty)int.Parse(attribute);
                    break;

                case "PreviewStart":
                    info.previewStart = double.Parse(attribute);
                    break;

                case "PreviewEnd":
                    info.previewEnd = double.Parse(attribute);
                    break;

                case "Genre":
                    info.genre = attribute;
                    break;

                case "MediaType":
                    info.mediaType = attribute;
                    break;

                case "MusicStream":
                    info.musicStream = attribute;
                    break;

                case "RhythmStream":
                    info.rhythmStream = attribute;
                    break;

                case "GuitarStream":
                    info.guitarStream = attribute;
                    break;

                case "BassStream":
                    info.bassStream = attribute;
                    break;

                default:
                    break;
            }
            if (chart[i].Contains("}"))
            {
                //Debug.Log("End reading song info");
                break;
            }

            i++;
        }
        return i;
    }
    private int LoadChartSyncTrack(List<Song.SyncTrack> syncTrack, string[] chart, int i)
    {
        int timeout = 100000;
        while (i < timeout)
        {
            if (chart[i].Contains("{"))
            {
                //Debug.Log("Start reading SyncTrack");
                i++;
                break;
            }
            i++;
        }

        while (i < timeout)
        {
            if (chart[i].Contains("}")) break;
            string line = chart[i];
            if (line.Contains(" = "))
            {
                string[] splitted = line.Split(new string[] { " = " }, System.StringSplitOptions.None);
                string[] commandValue = splitted[1].Split(" "[0]);
                syncTrack.Add(new Song.SyncTrack(uint.Parse(splitted[0]), commandValue[0], uint.Parse(commandValue[1])));
            }
            i++;
        }
        return i;
    }

    private int LoadChartEvents(List<Song.SongEvent> events, string[] chart, int i)
    {
        int timeout = 100000;
        while (i < timeout)
        {
            if (chart[i].Contains("{"))
            {
                //Debug.Log("Start reading Events");
                i++;
                break;
            }
            i++;
        }

        while (i < timeout)
        {
            if (chart[i].Contains("}"))
            {
                //Debug.Log("End reading Events");
                break;
            }
            string line = chart[i];
            if (line.Contains(" = E "))
            {
                string[] splitted = line.Split(new string[] { " = E " }, System.StringSplitOptions.None);
                events.Add(new Song.SongEvent(uint.Parse(splitted[0]), splitted[1]));
            }
            i++;
        }
        return i;
    }
    private int LoadChartNotes(string[] chart, int i, List<Song.Note> list, long resolution)
    {
        int timeout = 100000;
        while (i < timeout)
        {
            if (chart[i].Contains("{"))
            {
                //Debug.Log("Start reading Notes");
                i++;
                break;
            }
            i++;
        }
        uint starPowerEndsAt = 0;
        while (i < timeout)
        {
            if (chart[i].Contains("}"))
            {
                //Debug.Log("End reading Notes");
                break;
            }
            string line = chart[i];
            if (line.Contains(" = "))
            {
                string[] splitted = line.Split(new string[] { " = " }, System.StringSplitOptions.None);
                string[] noteSplitted = splitted[1].Split(" "[0]);
                uint timestamp = uint.Parse(splitted[0]);
                if (noteSplitted[0] == "N")
                {
                    bool hammeron = false;
                    uint fred = uint.Parse(noteSplitted[1]);
                    Song.Note previousNote = null;
                    if (list.Count > 0)
                    {
                        previousNote = list[list.Count - 1];
                        if (previousNote.timestamp == timestamp)//double notes no hammeron
                        {
                            previousNote.hammerOn = false;
                        }
                        else
                        {
                            hammeron = (timestamp < previousNote.timestamp + (resolution / 2)) && (previousNote.fred != fred) && (previousNote.timestamp != timestamp);
                        }
                    }
                    if (uint.Parse(noteSplitted[1]) < 5)
                    {
                        list.Add(new Song.Note(timestamp, fred, uint.Parse(noteSplitted[2]), timestamp <= starPowerEndsAt, hammeron));
                    }
                }
                if (noteSplitted[0] == "S")
                {

                    starPowerEndsAt = timestamp + uint.Parse(noteSplitted[2]);
                    //also set previous note to star
                    int traceBack = 1;
                    while (traceBack < 5)
                    {
                        if (list.Count > 1)
                        {
                            if (list[list.Count - traceBack].timestamp == timestamp)
                            {
                                list[list.Count - traceBack].star = true;
                                traceBack++;
                                continue;
                            }
                        }
                        break;
                    }
                }
            }
            i++;
        }
        return i;
    }
}