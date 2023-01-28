using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortNote : Note
{
    private int _startSample;
    private int _endSample;
    protected override void Init()
    {
        noteCreator = FindObjectOfType<ShortNoteCreater>();
        // _spawnPos = GameObject.Find("SpawnPos").transform.localPosition;
        destroyPos = GameObject.Find("DestroyPos").transform.localPosition;
    }
}
