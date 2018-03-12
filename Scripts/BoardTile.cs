using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{

    const int MAX_HEIGHT = 35;
    const int GOO_MAX = 40;

    public int gooLayers;

    public double gooHeight;
    public int landHeight;

    private int prevLandHeight;
    private double gooChange;

    private BoardRunner board;

    public Damageable structure; //For items on tiles
    public SpriteRenderer gooSprite;
    public GameObject sideBorder;
    public GameObject topBottomBorder;
    private SpriteRenderer landSprite;

    private List<GameObject> terrainSides;

    public int getLandHeight()
    {
        if(structure != null && structure.GetComponent<Structure>() != null)
        {
            return structure.GetComponent<Structure>().getLandChange() + landHeight;
        }
        return landHeight;
    }

    public void setLandHeight(int newHeight)
    {
        landHeight = newHeight;
    }

    //can this tile be traversed from currentPosition
    public bool isTraversable(Vector2 currentPosition)
    {
        BoardTile b = board.getBoardTile((int)currentPosition.x, (int)currentPosition.y);
        if (structure != null)
        {
            return structure.isTraversible(currentPosition);
        }

        if (gooHeight > 2) return false;
        if (b != null && Mathf.Abs(b.landHeight - landHeight) > 1) return false;

        return true;
    }

    void updateGooSprite()
    {
        gooSprite.color = new Color(gooSprite.color.r, gooSprite.color.g, gooSprite.color.b, Mathf.Round((float)gooHeight)/gooLayers);
    }


    // Use this for initialization
    void Start()
    {
        terrainSides = new List<GameObject>();
        prevLandHeight = landHeight;
        landSprite = GetComponent<SpriteRenderer>();
        updateGooSprite();

    }

    public void setBoard(BoardRunner b)
    {
        board = b;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevLandHeight != landHeight)
        {
            updateLandSprite();
            prevLandHeight = landHeight;

            if (transform.position.x < board.sizeX - 1) board.getBoardTile((int)transform.position.x + 1, (int)transform.position.y).updateLandSprite();
            if (transform.position.x != 0) board.getBoardTile((int)transform.position.x - 1, (int)transform.position.y).updateLandSprite();
            if (transform.position.y != 0) board.getBoardTile((int)transform.position.x, (int)transform.position.y - 1).updateLandSprite();
            if (transform.position.y < board.sizeY - 1) board.getBoardTile((int)transform.position.x, (int)transform.position.y + 1).updateLandSprite();

        }
        updateGooSprite();
    }

    public void tick()
    {
        if(structure != null) structure.tick();
    }
    
    public void updateLandSprite()
    {
        foreach (GameObject g in terrainSides)
        {
            Destroy(g.gameObject);
        }

        int landDif = 0;
        if(transform.position.x < board.sizeX - 1) //Right
        {
            landDif = board.getBoardTile((int)transform.position.x + 1, (int)transform.position.y).landHeight - landHeight;
            if(landDif > 0)
            {
                terrainSides.Add(Instantiate(sideBorder, new Vector3(transform.position.x + .5F, transform.position.y), transform.rotation, transform));//right
                if (landDif > 1)
                {
                    terrainSides.Add(Instantiate(sideBorder, new Vector3(transform.position.x + .55F, transform.position.y), transform.rotation, transform));//right
                }
            }
        }
        if (transform.position.x != 0) //Left
        {
            landDif = board.getBoardTile((int)transform.position.x - 1, (int)transform.position.y).landHeight- landHeight;
            if (landDif > 0)
            {
                terrainSides.Add(Instantiate(sideBorder, new Vector3(transform.position.x - .5F, transform.position.y), transform.rotation, transform));//right
                if (landDif > 1)
                {
                    terrainSides.Add(Instantiate(sideBorder, new Vector3(transform.position.x - .55F, transform.position.y), transform.rotation, transform));//right
                }
            }
        }
        if (Mathf.Abs(transform.position.y) != 0) //Up
        {
            landDif = board.getBoardTile((int)transform.position.x , -1 + (int)transform.position.y).landHeight - landHeight;
            if (landDif > 0)
            {
                terrainSides.Add(Instantiate(topBottomBorder, new Vector3(transform.position.x, transform.position.y - .5F), transform.rotation, transform));//right
                if (landDif > 1)
                {
                    terrainSides.Add(Instantiate(topBottomBorder, new Vector3(transform.position.x, transform.position.y - .55F), transform.rotation, transform));//right
                }
            }
        }
        if (Mathf.Abs(transform.position.y) < board.sizeY - 1 ) //Down
        {
            landDif = board.getBoardTile((int)transform.position.x, 1 + (int)transform.position.y).landHeight - landHeight;
            if (landDif > 0)
            {
                terrainSides.Add(Instantiate(topBottomBorder, new Vector3(transform.position.x, transform.position.y + .5F), transform.rotation, transform));//right
                if(landDif > 1)
                {
                    terrainSides.Add(Instantiate(topBottomBorder, new Vector3(transform.position.x, transform.position.y + .55F), transform.rotation, transform));//right
                }
            }
        }
        if(structure != null && structure.GetComponent<Structure>() != null)
        {
            structure.GetComponent<Structure>().updateSprite();
        }
    }
    

    public double getTotalHeight()
    {
        if(structure != null && structure.GetComponent<Structure>() != null)
        {
            return structure.GetComponent<Structure>().getLandChange() + gooHeight + landHeight;
        }

        return gooHeight + landHeight;
    }

    public int getRealLandHeight()
    {
        return landHeight;
    }

    public void applyGooChange()
    {
        gooHeight += gooChange;
        gooChange = 0;
    }

    public void addGooChange(double change)
    {
        gooChange += change;
    }

    public void hit(double damage)
    {
        gooHeight -= damage;
        gooHeight = Mathf.Max((float)gooHeight, 0);
    }

    public void addGoo(float goo)
    {
        gooHeight += goo;
        gooHeight = Mathf.Max((float)gooHeight, 0);
    }

    public void addLand(int landIncrement)
    {
        landHeight += landIncrement;
        landHeight = Mathf.Max(landHeight, 0);
        landHeight = Mathf.Min(landHeight, MAX_HEIGHT);
    }
}
