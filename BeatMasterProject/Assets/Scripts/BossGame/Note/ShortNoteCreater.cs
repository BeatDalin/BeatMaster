using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortNoteCreater : NoteCreator
{
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GetObject();
        }
    }
    
    protected override void Init()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            CreateNewObject();
        }
    }
    
    protected override GameObject CreateNewObject()
    {
        GameObject go = base.CreateNewObject();
        go.AddComponent<ShortNote>();
        return go;
    }
}
