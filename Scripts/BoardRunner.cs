using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRunner : MonoBehaviour {

    public class tileInfo
    {
        public Color tileColor;
        public int landHeight;
        public double gooHeight;

        public tileInfo (Color tc, int lh, double gh)
        {
            tileColor = tc;
            landHeight = lh;
            gooHeight = gh;
        }
    }



    public GameObject TilePrefab;


    public float tickTime;
    private float counter;
    private float prevTime;
    
    public int sizeX;
    public int sizeY;
    public float gooRatio;
    public float spreadThreshold;



    private BoardTile[,] tileBoard;

    void Awake()
    {
        //construct board if there is no board
        if (tileBoard == null)
        {
            tileBoard = new BoardTile[sizeY, sizeX];

            for (int y = 0; y < sizeY; ++y)
            {
                for (int x = 0; x < sizeX; ++x)
                {

                    tileBoard[y, x] = Instantiate(TilePrefab, new Vector3(this.transform.position.x + x, this.transform.position.y + y), transform.rotation, this.transform).GetComponent<BoardTile>();
                    tileBoard[y, x].setBoard(this);
                    tileBoard[y, x].name = (string)(x + "_" + y);
                }
            }
        }
    }

    void Start()
    {
        prevTime = Time.time;
        counter = 0;

	}
	
    public GameObject getTile(int x, int y)
    {

        if(x < 0 || y < 0 || x >= sizeX || y >= sizeY)
        {
            return null;
        }
        return tileBoard[y, x].gameObject;
    }

    public BoardTile getBoardTile(int x, int y)
    {

        if (x < 0 || y < 0 || x >= sizeX || y >= sizeY)
        {
            return null;
        }
        return tileBoard[y, x];
    }

    // Update is called once per frame
    void Update ()
    {
		if(Time.time - prevTime > tickTime)
        {
            counter++;
            prevTime = Time.time;
            tick();

            if(counter % 50 == 0)
            {
                //saveData(); //save every 50 ticks
            }

        }
	}

    void tick()
    {
        //prepGooUpdates();
        prepGooUpdatesEfficient();
        applyGooUpdates();

        foreach(BoardTile tile in tileBoard)
        {
            tile.tick();
        }

    }


    // Preps changes to goo heights based on neighbors, rounded to two decimals.
    void prepGooUpdates()
    {
        for(int y = 0; y < sizeY; ++y)
        {
            for(int x = 0; x < sizeX; ++x)
            {
                double change = 0.0;

                BoardTile b = tileBoard[y, x];
                //find how a tile should change goo-wise based on surrounding tiles.
                if (y != 0) change += getChangeToOtherTile(tileBoard[y - 1, x],b);
                if (y < sizeY - 1) change += getChangeToOtherTile(tileBoard[y + 1, x], b);
                if (x != 0) change += getChangeToOtherTile(tileBoard[y, x - 1],b);
                if (x < sizeX - 1) change += getChangeToOtherTile(tileBoard[y, x + 1], b);

                if (y != 0 && x != 0) change += getChangeToOtherTile(tileBoard[y-1, x - 1], b);
                if (y < sizeY - 1 && x < sizeX - 1) change += getChangeToOtherTile(tileBoard[y+1, x + 1], b);
                if (y < sizeY - 1 && x != 0) change += getChangeToOtherTile(tileBoard[y+1, x - 1], b);
                if (y != 0 && x < sizeX - 1) change += getChangeToOtherTile(tileBoard[y-1, x + 1], b);

                b.addGooChange(change);
            }
        }
    }

    //calculates gooUpdates only based on goo running downhill
    void prepGooUpdatesEfficient()
    {
        for (int y = 0; y < sizeY; ++y)
        {
            for (int x = 0; x < sizeX; ++x)
            {

                BoardTile b = tileBoard[y, x];
                //find how a tile should change goo-wise based on surrounding tiles.
                if (y != 0) calcAndApplyChange(b, x, y - 1);
                if (y < sizeY - 1) calcAndApplyChange(b, x, y + 1);
                if (x != 0) calcAndApplyChange(b, x-1, y);
                if (x < sizeX - 1) calcAndApplyChange(b, x+1,y);

                if (y != 0 && x != 0) calcAndApplyChange(b, x-1, y - 1);
                if (y < sizeY - 1 && x < sizeX - 1) calcAndApplyChange(b, x+1, y + 1);
                if (y < sizeY - 1 && x != 0) calcAndApplyChange(b, x-1, y + 1);
                if (y != 0 && x < sizeX - 1) calcAndApplyChange(b, x+1, y - 1);
            }
        }
    }

    void calcAndApplyChange(BoardTile b, int x, int y)
    {
        double change = ChangeAdjacentTile(b, tileBoard[y, x]);
        tileBoard[y, x].addGooChange(change);
        b.addGooChange(-change);
    }

    //how should b change based on a?
    double getChangeToOtherTile(BoardTile a, BoardTile b)
    {
        double returnVal;
        if (a.getTotalHeight() < b.getLandHeight()) returnVal = System.Math.Round((-1 * b.gooHeight) * gooRatio, 2); // a is shorter than b, goo runs downhill.
        else if (b.getTotalHeight() < a.getLandHeight()) returnVal = System.Math.Round(a.gooHeight * gooRatio, 2);

        else //a is taller than or equal to b, b will need to look at a.
        {
            //get difference in goo
            //a.getTotalHeight() - b.getTotalHeight()

            returnVal = System.Math.Round((a.getTotalHeight() - b.getTotalHeight()) * gooRatio, 2); //function like most goo
        }

        if (Mathf.Abs((float)returnVal) < spreadThreshold) return 0;
        return returnVal;
    }

    //How should both a and b changed based on a
    double ChangeAdjacentTile(BoardTile a, BoardTile b)
    {
        

        double returnVal = 0;
        if (a.getTotalHeight() <= b.getTotalHeight() || a.gooHeight == 0) return 0; //a is shorter than or equal to b, goo does not run uphill
        else if (b.getTotalHeight() < a.getLandHeight()) returnVal = System.Math.Round(a.gooHeight * gooRatio, 2);
        else //a is taller than or equal to b, b will need to look at a.
        {
            //get difference in goo
            //a.getTotalHeight() - b.getTotalHeight()

            returnVal = System.Math.Round((a.getTotalHeight() - b.getTotalHeight()) * gooRatio, 2); //a goo higher than b goo
        }

        if (Mathf.Abs((float)returnVal) < spreadThreshold) return 0;
        return returnVal;
    }

    void applyGooUpdates()
    {
        foreach(BoardTile g in tileBoard)
        {
            g.applyGooChange();
        }
    }

}
