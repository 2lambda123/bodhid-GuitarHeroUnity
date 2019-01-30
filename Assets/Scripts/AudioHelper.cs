using UnityEngine;

public class AudioHelper
{
    public static AudioSource currentPlaybackSource;

    public static void stopAllAudio()
    {
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.GetComponent<AudioSource>() != null)
                t.gameObject.GetComponent<AudioSource>().Stop();
        }
    }

    public static void resetAudio()
    {
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == "AudioObjects")
                continue;

            SongLoader.Instance.DestroyObject(t.gameObject);
        }
    }

    public static AudioSource GetCertainAudio(string type)
    {
        AudioSource temp = null;
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.name == type)
            {
                temp = t.gameObject.GetComponent<AudioSource>();
                break;
            }
        }
        return temp;
    }

    public static void setAllAudioTime(float time)
    {
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.GetComponent<AudioSource>() != null)
                t.gameObject.GetComponent<AudioSource>().time = time;
        }
    }

    public static void playAllAudio(float delay)
    {
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.GetComponent<AudioSource>() != null)
                t.gameObject.GetComponent<AudioSource>().PlayScheduled(AudioSettings.dspTime + delay);
        }
    }

    public static AudioSource FindCurrentPlayback()
    {
        Transform[] ts = SongSelect.AudioObjects.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.gameObject.GetComponent<AudioSource>() != null)
                if (t.gameObject.GetComponent<AudioSource>().clip != null)
                    return t.gameObject.GetComponent<AudioSource>();
        }

        return null;
    }
}