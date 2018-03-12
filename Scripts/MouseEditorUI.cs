using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MouseEditorUI : MonoBehaviour
{
    private BoardRunner board;
    private TerrainEditor b;
    public Toggle increment;
    public Toggle enabler;
    public InputField strength;
    public InputField radius;
    private double gooDepth;
    private int landHeight;
    public Text gooDepthText;
    public Text landHeightText;

    // Use this for initialization
    void Start()
    {
        b = FindObjectOfType<TerrainEditor>();
        board = FindObjectOfType<BoardRunner>();

        radius.onEndEdit.AddListener(delegate { setRadius(); });
        strength.onEndEdit.AddListener(delegate { setStrength(); });
        increment.onValueChanged.AddListener(delegate { setIncrement(); });
        enabler.onValueChanged.AddListener(delegate { enable(); });
    }

    void enable()
    {
        b.editorEnabled = enabler.isOn;
    }

    void setRadius()
    {
        try {
            int rad = int.Parse(radius.text);
            b.editorSize = rad;
        }
        catch
        {

        }
    }

    void setStrength()
    {
        try {
            int str = int.Parse(strength.text);
            b.editorStrength = str;
        }
        catch
        {

        }
    }

    void setIncrement()
    {
        b.increment = increment.isOn;
    }

    // Update is called once per frame
    void Update()
    {
        //record mouse tile goo and land height
        BoardTile selectedTile = board.getBoardTile(Mathf.RoundToInt(b.transform.position.x), Mathf.RoundToInt(b.transform.position.y));
        if (selectedTile != null)
        { 
            if (selectedTile.gooHeight != gooDepth)
            {
                gooDepth = selectedTile.gooHeight;
                gooDepthText.text = Math.Round(gooDepth,2).ToString();
            }
            if (selectedTile.getLandHeight() != landHeight)
            {
                landHeight = selectedTile.getLandHeight();
                landHeightText.text = landHeight.ToString();
            }
        }
        


        if (Input.GetKeyDown(KeyCode.KeypadEnter) && strength.isFocused) setStrength();
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && radius.isFocused) setRadius();
    }
}
