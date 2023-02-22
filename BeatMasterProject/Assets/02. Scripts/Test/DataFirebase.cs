using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class DataFirebase : MonoBehaviour
{
    
    private DatabaseReference _databaseReference = null;

    private void Awake()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new System.Uri("https://beatmaster-60220435-default-rtdb.firebaseio.com/");
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
