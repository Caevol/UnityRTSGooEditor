using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerrainEditor : MonoBehaviour {

    public bool editorEnabled;
    public BoardRunner board;
    public string editorMode;
    public int editorSize;
    public int editorStrength;
    public bool increment;
    public Color editorColor;

    private double EDGEX = .85;
    private double EDGEY = .15;

    void raiseLand(Vector2 pos, int sign)
    {
        GameObject g = board.getTile((int)pos[0],(int)pos[1]);

        if(g != null) g.GetComponent<BoardTile>().addLand(sign * editorStrength);
    }

    void setLand(Vector2 pos)
    {
        BoardTile g = board.getBoardTile((int)pos[0], (int)pos[1]);

        if (g != null) g.landHeight = editorStrength;
    }

    void setGoo(Vector2 pos)
    {
        GameObject g = board.getTile((int)pos[0], (int)pos[1]);

        if (g != null) g.GetComponent<BoardTile>().gooHeight = editorStrength;
    }

    void raiseGoo(Vector2 pos, int sign)
    {
        GameObject g = board.getTile((int)pos.x,
                      (int)pos.y);
        if (g != null)
        {
            g.GetComponent<BoardTile>().addGoo(editorStrength * sign);
        }
    }

    void setColor(Vector2 pos)
    {
        GameObject g = board.getTile((int)pos.x,
                      (int)pos.y);
        if (g != null)
        {
            g.GetComponent<SpriteRenderer>().color = editorColor;
        }
    }

    //colors all directly connected land pieces of the same height to one color (diagonal is not directly connected)
    void fillColoring(Vector2 pos, int height)
    {
        BoardTile g = board.getBoardTile((int)pos.x, (int)pos.y);
        if (g != null)
        {
            if (g.getRealLandHeight() != height || g.GetComponent<SpriteRenderer>().color == editorColor) return;

            g.GetComponent<SpriteRenderer>().color = editorColor;

            fillColoring(new Vector2(pos.x + 1, pos.y), height);
            fillColoring(new Vector2(pos.x - 1, pos.y), height);
            fillColoring(new Vector2(pos.x, pos.y + 1), height);
            fillColoring(new Vector2(pos.x, pos.y - 1), height);

        }
    }


	// Use this for initialization
	void Start () {

        board = FindObjectOfType<BoardRunner>();
        
        
	}
	
	// Update is called once per frame
	void Update () {

        if (!editorEnabled) return;


        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if(Camera.main.ScreenToViewportPoint(Input.mousePosition).x >= EDGEX || Camera.main.ScreenToViewportPoint(Input.mousePosition).y < EDGEY) return;

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;

        if(editorMode == "Fill" && Input.GetMouseButton(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BoardTile g = board.getBoardTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            if (g != null) fillColoring(pos, g.getRealLandHeight());
            return;
        }

        for(int x = -editorSize; x <= editorSize; ++x)
        {
            for(int y = -editorSize; y <= editorSize; ++y)
            {
                if(Mathf.Pow(Mathf.Abs(x),2) + Mathf.Pow(Mathf.Abs(y),2) <= Mathf.Pow(editorSize,2))
                {
                    if(editorMode == "land")
                    {
                        if (Input.GetMouseButton(0)) {
                            if (!increment)
                                setLand(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F));
                            else
                                raiseLand(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F), 1);
                            }

                        else raiseLand(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F), -1);
                    }
                    else if(editorMode == "goo")
                    {
                        if (Input.GetMouseButton(0))
                        {
                            if (!increment)
                                setGoo(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F));
                            else
                                raiseGoo(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F), 1);
                        }
                        else raiseGoo(new Vector2(transform.position.x + x + .5F, transform.position.y - y + .5F), -1);
                    }
                    else if(editorMode == "color")
                    {
                        if (Input.GetMouseButton(0))
                            setColor(new Vector2(transform.position.x + x + .5F,  transform.position.y - y + .5F));
                    }
                }

            }
        }

        



    }

    public void setEditorMode(string s)
    {
        editorMode = s;
    }

    public void changeEditorSize(int i)
    {
        editorSize += i;
        editorSize = Mathf.Max(0, editorSize);
    }
}
