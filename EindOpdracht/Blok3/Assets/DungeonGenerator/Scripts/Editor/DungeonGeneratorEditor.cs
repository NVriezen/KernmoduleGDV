using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        DungeonGenerator dungeonGenerator = target as DungeonGenerator;

        if (GUILayout.Button("Clear Dungeon")) {
            dungeonGenerator.ClearDungeon();
        }
        if (GUILayout.Button("Generate Dungeon")) {
            dungeonGenerator.GenerateDungeon();
        }

    }
}
