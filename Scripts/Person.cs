using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : Damageable {

    private BoardRunner board;

    public float speed;

    List<Vector2> path;
    int currentTarget;
    const double thresholdDistance = .5;

    public override void hit(float damage)
    {
        curHealth -= damage;
    }


    public void setPath(List<Vector2> calcedPath)
    {
        path = calcedPath;
        currentTarget = 0;
    }


    // Use this for initialization
    void Start () {
        board = FindObjectOfType<BoardRunner>();
	}
	
	// Update is called once per frame
	void Update () {

        if (path != null) moveOnToNextTarget();

	    if(path != null)
        {
           transform.Translate((path[currentTarget] - (Vector2)transform.position).normalized * speed * Time.deltaTime);
        }
	}

    void moveOnToNextTarget()
    {
        if (Vector2.Distance(transform.position, path[currentTarget]) <= thresholdDistance)
        {


            //close enough, move on to next target
            if (currentTarget == path.Count - 1)
            {
                currentTarget = 0;
                path = null;
        
            }
            else
            {
                currentTarget++;
                //check if nxt tile is traversible, if not, find a new path.

                if (board.getBoardTile(Mathf.RoundToInt(path[currentTarget].x),Mathf.RoundToInt(path[currentTarget].y)) == null 
                    || board.getBoardTile(Mathf.RoundToInt(path[currentTarget].x),Mathf.RoundToInt(path[currentTarget].y))
                    .isTraversable(path[currentTarget -1]) == false){
                    //Next tile is no longer traversible, or does not exist, Find new path
                    path = Assets.Scripts.Pathfinder.getPath(path[currentTarget - 1], path[path.Count-1], board);

                    currentTarget = 0;
                }
                else
                {
                    verifyPath();
                }
            }
        }
    }

    //if path is not traversible, find a new path
    void verifyPath()
    {
        for(int i = currentTarget; i < path.Count; ++i)
        {
            if(board.getBoardTile(Mathf.RoundToInt(path[i].x), Mathf.RoundToInt(path[i].y)).isTraversable(path[i-1]) == false)
            {
                path = Assets.Scripts.Pathfinder.getPath(path[currentTarget - 1], path[path.Count - 1], board);
                currentTarget = 0;
                return;
            }
        }
    }

    public override bool isTraversible(Vector2 position)
    {
        return false;
    }

    public override void tick()
    {
        
    }
}

