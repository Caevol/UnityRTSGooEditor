using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseEditorControl : MonoBehaviour {

    public string editorMode;
    public TerrainEditor terrainEditor;

	// Use this for initialization
	void Start () {
		if(terrainEditor == null)
        {
            terrainEditor = FindObjectOfType<TerrainEditor>();
        }
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(setEditorMode);
	}

    void setEditorMode()
    {
        terrainEditor.setEditorMode(editorMode);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
