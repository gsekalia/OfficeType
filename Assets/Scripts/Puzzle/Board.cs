using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

   
    public int width = 16;
    public int height = 16;
    public GameObject[] dots;
    //Prefab for the tile
    public GameObject tilePrefab;

    public DotBroker dotBroker;
    //2D array which holds all the tiles
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;

	// Use this for initialization
	void Start ()
    {
        allTiles = new BackgroundTile[width, height];
        allDots  = new GameObject[width, height];
        SetUp();
	}
	
    //Instantiate Background Tiles
    private void SetUp()
    {
        Vector2 pos = gameObject.transform.position;
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
               
                Vector2 tempPos = pos + new Vector2(w, h);
                //actually create the tiles
                //allTiles[w, h];
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
               // backgroundTile.name = "[ " + w + ", " + h + "]";

                //create dot
                int dotInd = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotInd], tempPos, Quaternion.identity);
                //dot.transform.parent = this.transform;
                dot.name = "[ " + w + ", " + h + "]"; ;

                allDots[w, h] = dot;
            }

        }
    }

 
    //public void AddCommand(DotCommand cmd)
    //{
    //    //dotBroker.AddObject();
    //      dotBroker.AddCommand(cmd);
    //}

    public void AddObject(Dot dot)
    {
        dotBroker.AddObject(dot);
        //dotBroker.AddCommand(cmd);
    }

    public void ProcessCommands()
    {
        dotBroker.ProcessCommands();
    }

    public void DestroyRow(int leftmost, int rightmost, int row)
    {
        for (int i = leftmost; i <= rightmost; i++)
        {
            Destroy(allDots[i, row]);
            //Just checks to see if there is 
            if (row >= height - 1)
            {
                CreateDotAtTop(i);
            }
        }
    }

    public void DestroyColumn(int downmost, int upmost, int column)
    {
        for (int i = downmost; i <= upmost; i++)
        {
            Destroy(allDots[column, i]);
        }

        if (upmost >= height-1) CreateDotAtTop(column);
    }

    public void CreateDotAtTop(int column)
    {
        int dotInd = Random.Range(0, dots.Length);
        Vector2 offset = gameObject.transform.position;
        Vector2 pos = offset + new Vector2(column, height-1);
        GameObject dot = Instantiate(dots[dotInd], pos, Quaternion.identity);
        allDots[column, height - 1] = dot;
    }
}
