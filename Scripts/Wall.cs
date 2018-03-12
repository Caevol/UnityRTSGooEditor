using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Structure {

    private BoardRunner board;
    private BoardTile tile;
    public Sprite noWallSprite;
    public Sprite oneWallSprite;
    public Sprite twoWallSprite;
    public Sprite twoCornerSprite;
    public Sprite threeWallSprite;
    public Sprite fourWallSprite;
    public int wallHeight;

    private SpriteRenderer sprite;

    // Use this for initialization
    void Start () {
        board = FindObjectOfType<BoardRunner>();
        tile = board.getBoardTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (tile != null) tile.structure = this;


        sprite = GetComponent<SpriteRenderer>();
        updateSprite();
        updateAdjacentSprites();

	}
	


	// Update is called once per frame
	void Update () { 
        	
	}

    void updateAdjacentSprites()
    {
        BoardTile b = board.getBoardTile(Mathf.RoundToInt(transform.position.x + 1), Mathf.RoundToInt(transform.position.y));
        if (b != null && b.structure != null && b.structure.GetComponent<Wall>() != null) b.structure.GetComponent<Wall>().updateSprite();

        b = board.getBoardTile(Mathf.RoundToInt(transform.position.x - 1), Mathf.RoundToInt(transform.position.y));
        if (b != null && b.structure != null && b.structure.GetComponent<Wall>() != null) b.structure.GetComponent<Wall>().updateSprite();

        b = board.getBoardTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y + 1));
        if (b != null && b.structure != null && b.structure.GetComponent<Wall>() != null) b.structure.GetComponent<Wall>().updateSprite();

        b = board.getBoardTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y - 1));
        if (b != null && b.structure != null && b.structure.GetComponent<Wall>() != null) b.structure.GetComponent<Wall>().updateSprite();

    }

    public override void updateSprite()
    {
        bool[] adjacency = getAdjacentSprites();
        int totalConnected = 0;
        foreach(bool i in adjacency) if(i) totalConnected ++;

        transform.eulerAngles = Vector3.zero;

        if(totalConnected == 0)
        {
            sprite.sprite = noWallSprite;
        }
        else if(totalConnected == 1)
        {
            sprite.sprite = oneWallSprite;
            int index = 0;
            for (int i = 0; i < 4; ++i) if (adjacency[i]) index = i;
            transform.Rotate(new Vector3(0, 0, 90 * index));
        }

        else if(totalConnected == 2)
        {
            int index1 = -1;
            int index2 = -1;
            for(int i = 0; i < 4; ++i)
            {
                if (adjacency[i])
                {
                    if (index1 == -1) index1 = i;
                    else index2 = i;
                }
            }
            if(index2 - index1 != 2)
            {
                sprite.sprite = twoCornerSprite;
                if (index2 == 3 && index1 == 0)
                    transform.Rotate(new Vector3(0, 0, 90 * index2));
                else
                    transform.Rotate(new Vector3(0, 0, 90 * index1));
                //corner
            }
            else
            {
                sprite.sprite = twoWallSprite;
                transform.Rotate(new Vector3(0, 0, 90 * index1));
                //straight
            }
            
        }

        else if(totalConnected == 3)
        {
            sprite.sprite = threeWallSprite; //figure out rotation
            int index = 0;
            for (int i = 0; i < 4; ++i) if (adjacency[i] == false) index = i;
            index += 1;
            index %= 4;
            transform.Rotate(new Vector3(0, 0, 90 * index));

        }

        else if(totalConnected == 4)
        {
            sprite.sprite = fourWallSprite;
        }

    }

    bool[] getAdjacentSprites()
    {
        bool[] total = new bool[4];

        BoardTile adjacent = board.getBoardTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y + 1)); //up
        if (adjacent != null && adjacent.getLandHeight() == tile.getLandHeight()) total[0] = true;


        adjacent = board.getBoardTile(Mathf.RoundToInt(transform.position.x-1), Mathf.RoundToInt(transform.position.y)); //left
        if (adjacent != null && adjacent.getLandHeight() == tile.getLandHeight()) total[1] = true;

        adjacent = board.getBoardTile(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y - 1)); //down
        if (adjacent != null && adjacent.getLandHeight() == tile.getLandHeight()) total[2] = true;
        
        adjacent = board.getBoardTile(Mathf.RoundToInt(transform.position.x+1), Mathf.RoundToInt(transform.position.y)); //right
        if (adjacent != null && adjacent.getLandHeight() == tile.getLandHeight()) total[3] = true;




        return total;
    }

    public override int getLandChange()
    {
        return wallHeight;
    }

    public override void hit(float damage)
    {
        curHealth -= damage;
    }

    public override void tick()
    {
        return;
    }

    public override bool isTraversible(Vector2 position)
    {
        if (tile.gooHeight > 2) return false;

        Vector2 t = position - (Vector2)transform.position;
        float state = Mathf.Min(Mathf.Abs(t.x), Mathf.Abs(t.y));
        if (state != 0) return false; //must approach perpendicularly

        BoardTile b = board.getBoardTile(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        if (b != null && Math.Abs(tile.getLandHeight() - b.getLandHeight()) > 0) return false;

        return true;
    }
}
