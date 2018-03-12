using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseVillagerControl : MonoBehaviour {

    private BoardRunner board;
    private TerrainEditor terEdit;
    public bool editorEnabled;
    public GameObject wallPrefab;
    private GameObject selected;

	// Use this for initialization
	void Start () {
        board = FindObjectOfType<BoardRunner>();
        terEdit = FindObjectOfType<TerrainEditor>();

	}
	
	// Update is called once per frame
	void Update () {
        if (!editorEnabled || terrainEditorActive()) return;

        if (Input.GetMouseButtonDown(0))
        {
            
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero, 100f);

            if (hit)
            {
                
                if (hit.transform != null && hit.transform.GetComponent<Person>())
                {
                    selected = hit.transform.gameObject;
                }                              
            }
            else if (selected != null && selected.GetComponent<Person>() != null && board.getBoardTile(Mathf.RoundToInt(ray.x),Mathf.RoundToInt(ray.y)) != null)
            {
                List<Vector2> path = Assets.Scripts.Pathfinder.getPath(new Vector2(Mathf.RoundToInt(selected.transform.position.x), Mathf.RoundToInt(selected.transform.position.y)),
                    new Vector2(Mathf.RoundToInt(ray.x), Mathf.RoundToInt(ray.y)),
                    board);
                selected.GetComponent<Person>().setPath(path);
            }

        }

        if (Input.GetMouseButton(2))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BoardTile b = board.getBoardTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            if(b != null && b.structure == null)
            {
                Instantiate(wallPrefab, new Vector2(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y)), Quaternion.Euler(0,0,0), null);
            }
        }

	}

    bool terrainEditorActive()
    {
        if(terEdit == null || terEdit.editorEnabled == false)
        {
            return false;
        }
        return true;
    }

}
