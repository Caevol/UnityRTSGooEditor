using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleBlaster : MonoBehaviour {

    public int range;
    public int explosiveRange;
    public float gooDamage;
    public float weaponCooldown;

    public bool active;
    public bool targetNearest;

    private BoardRunner board;
    private float cooldown;

	// Use this for initialization
	void Start () {
        cooldown = weaponCooldown;
        board = FindObjectOfType<BoardRunner>();

    }
	
	// Update is called once per frame
	void Update () {

        cooldown -= Time.deltaTime;
        if (cooldown <= 0 && active)
        {
            fire();
            cooldown = weaponCooldown;
        }


	}

    Vector2 getMaxGooTarget()
    {
        double maxGoo = 0;
        Vector2 position = transform.position;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; ++y)
            {
                if (Mathf.Pow(Mathf.Abs(x), 2) + Mathf.Pow(Mathf.Abs(y), 2) <= Mathf.Pow(range, 2))
                {
                    GameObject g1 = board.getTile(
                        Mathf.RoundToInt(transform.position.x) + x,
                        Mathf.RoundToInt(transform.position.y) + y);

                    if (g1 != null && g1.GetComponent<BoardTile>().gooHeight > maxGoo)
                    {
                        maxGoo = g1.GetComponent<BoardTile>().gooHeight;
                        position = g1.GetComponent<BoardTile>().gameObject.transform.position;
                    }
                }
            }
        }
        return position;
    }

    Vector2 getNearestGooTarget()
    {
        double minDistance = double.MaxValue;
        Vector2 position = transform.position;
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; ++y)
            {
                if (Mathf.Pow(Mathf.Abs(x), 2) + Mathf.Pow(Mathf.Abs(y), 2) <= Mathf.Pow(range, 2))
                {
                    GameObject g1 = board.getTile(
                        Mathf.RoundToInt(transform.position.x) + x,
                        Mathf.RoundToInt(-transform.position.y) - y);

                    if (g1 != null && g1.GetComponent<BoardTile>().gooHeight > 0 && Mathf.Abs(x) + Mathf.Abs(y) < minDistance)
                    {
                        minDistance = Mathf.Abs(x) + Mathf.Abs(y);
                        position = g1.transform.position;
                    }
                }
            }
        }
        return position;
    }

    void clearGooInCircle(Vector2 position)
    {
        //hit target
        for (int x = -explosiveRange; x <= explosiveRange; ++x)
        {
            for (int y = -explosiveRange; y <= explosiveRange; ++y)
            {
                if (Mathf.Pow(Mathf.Abs(x), 2) + Mathf.Pow(Mathf.Abs(y), 2) <= Mathf.Pow(explosiveRange, 2))
                {

                    GameObject g = board.getTile(
                        Mathf.RoundToInt(position.x) + x,
                        Mathf.RoundToInt(position.y) - y);

                    if (g != null && g.GetComponent<BoardTile>().gooHeight > 0)
                    {
                        g.GetComponent<BoardTile>().hit(gooDamage);
                    }
                }
            }
        }
    }

    void fire()
    {
        //find target
        Vector2 position;
        if (targetNearest) position = getNearestGooTarget();
        else position = getMaxGooTarget();
        

        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(Vector3.up, position, 2*Mathf.PI, 360));

        var dir = position - (Vector2)transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        clearGooInCircle(position);

    }

}
