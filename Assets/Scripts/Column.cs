using Melanchall.DryWetMidi.Interaction;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName[] notesToShow;

    public int midiTrackId;
    public KeyCode input;
    public GameObject targetPrefab;
    List<Target> targets = new List<Target>();
    public List<double> targetTsList_s = new List<double>();

    int spawnIdx = 0;
    int hitIdx = 0;

    void Start()
    {
        
    }

    public void SetTargetTimestamps(Note[] noteArray)
    {
        foreach (Note note in noteArray)
        {
            if (notesToShow.Contains(note.NoteName))
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.midiFile.GetTempoMap());
                targetTsList_s.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);
            }
        }
    }

    void Update()
    {
        if (spawnIdx < targetTsList_s.Count)
        {
            double totalTime_s = SongManager.GetAudioSyncedTimeSec() - (SongManager.Instance.inputDelay_ms / 1000.0);
            while (totalTime_s >= targetTsList_s[spawnIdx])
            {
                // Instantiate creates a new copy of an GameObject with the given transform
                print($"Creating target with ts {targetTsList_s[spawnIdx]} at time {totalTime_s}");
                var newTarget = Instantiate(targetPrefab, transform);
                newTarget.GetComponent<Target>().assignedTime_s = (float)targetTsList_s[spawnIdx];
                targets.Add(newTarget.GetComponent<Target>());
                spawnIdx++;
            }
        }

        if (hitIdx < targetTsList_s.Count)
        {
            double targetTs_s = targetTsList_s[hitIdx];
            double noteHitTime_s = SongManager.Instance.noteHitTime_s;
            double marginOfError_s = SongManager.Instance.marginOfError_s;
            double audioTime_s = SongManager.GetAudioTimeSec() - (SongManager.Instance.inputDelay_ms / 1000.0);
            
            if (Input.GetKeyDown(input))
            {
                if (System.Math.Abs(audioTime_s - targetTs_s) < marginOfError_s)
                {
                    Hit();
                    print($"Hit {hitIdx} target!");
                    Destroy(targets[hitIdx].gameObject);
                    hitIdx++;
                }
                else
                {
                    print($"Hit inaccurate on {hitIdx} note with {System.Math.Abs(audioTime_s - targetTs_s)} delay");
                }
            }
            while (hitIdx < targetTsList_s.Count && targetTsList_s[hitIdx] + marginOfError_s <= audioTime_s)
            {
                Miss();
                print($"Missed {hitIdx} note!");
                hitIdx++;
            }
        }
    }

    private void Hit()
    {
        ScoreManager.Hit();
    }
    private void Miss()
    {
        ScoreManager.Miss();
    }
}
