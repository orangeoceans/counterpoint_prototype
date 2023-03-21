using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;

public class SongManager : MonoBehaviour
{

    public static SongManager Instance;

    public AudioSource audioSource;
    public static MidiFile midiFile;
    public string midiFilePath;

    public Column[] columns;

    public float songDelay_s;
    private double totalSongDelay_s;

    private double timeSinceStart_s;
    public double timeAudioStarted_s;
    public double marginOfError_s;
    public int inputDelay_ms;
    public float noteHitTime_s;

    public float noteStartY;
    public float noteHitY;
    public float noteEndY
    {
        get
        {
            return noteHitY - (noteStartY - noteHitY);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ReadFromFile();
    
        totalSongDelay_s = songDelay_s + noteHitTime_s;

        timeSinceStart_s = 0;
        timeAudioStarted_s = -1;
        // Invoke calls a method after some time delay
        Invoke(nameof(StartAudio), (float)totalSongDelay_s);
    }

    private void ReadFromFile()
    {
        midiFile = MidiFile.Read(midiFilePath);
        var trackChunks = midiFile.GetTrackChunks().ToArray();
        Note[][] tracksArray = new Note[trackChunks.Count()][];
        print($"Track chunks: {trackChunks.Count()}");
        for (int i = 0; i < trackChunks.Count(); i++)
        {
            var notes = trackChunks[i].GetNotes();
            tracksArray[i] = new Note[notes.Count];
            notes.CopyTo(tracksArray[i], 0);
        }

        foreach (var column in columns) column.SetTargetTimestamps(tracksArray[column.midiTrackId]);
    }

    public void StartAudio()
    {
        timeAudioStarted_s = timeSinceStart_s;
        audioSource.Play();
    }

    public static double GetAudioTimeSec()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    public static double GetAudioSyncedTimeSec()
    {
        if (Instance.timeAudioStarted_s < 0)
            return Instance.timeSinceStart_s;
        return Instance.timeAudioStarted_s + GetAudioTimeSec();
    }

    void Update()
    {
        timeSinceStart_s += Time.deltaTime;
    }
}
