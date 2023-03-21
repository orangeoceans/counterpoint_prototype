using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    double timeCreated_s;
    public float assignedTime_s;
    void Start()
    {
        timeCreated_s = SongManager.GetAudioSyncedTimeSec();
    }

    void Update()
    {
        double timeSinceCreated_s = SongManager.GetAudioSyncedTimeSec() - timeCreated_s;
        float progress = (float)(timeSinceCreated_s / (SongManager.Instance.noteHitTime_s * 2));

        
        if (progress > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.Instance.noteStartY, Vector3.up * SongManager.Instance.noteEndY, progress); 
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}