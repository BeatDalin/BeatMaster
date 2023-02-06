using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileEditor : EditorWindow
{
    [MenuItem("TileEditor/LevelGenerator")]

    static void Init()
    {
        var window = (TileEditor)GetWindow(typeof(TileEditor));

        window.position = new Rect(0, 0, 800, 600);
        window.Show();
    }

    private bool _showLevelOptions = false;
    private bool _paintMode = false;
    private int _sizeLevelWidth = 1;    // 가로
    private int _sizeLevelHeight = 1;   // 세로
    private int _sizeGridHeight = 1;    // 층 높이
    private int _sizeGridSize = 1;      // 그리드 크기

    private Transform _tileGenerator;
    private Vector2 _cellSize = new Vector2(1, 1);

    public void OnGUI()
    {
        _showLevelOptions = EditorGUILayout.Foldout(_showLevelOptions, "Map Creator");

        if (_showLevelOptions)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal(); // Level Value
            {
                EditorGUILayout.BeginVertical("HelpBox");
                EditorGUILayout.LabelField("Level Value");
                _sizeLevelWidth = EditorGUILayout.IntField("가로", _sizeLevelWidth);
                _sizeLevelHeight = EditorGUILayout.IntField("세로", _sizeLevelHeight);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
        }
    }
}
