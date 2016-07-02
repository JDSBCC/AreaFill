using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildField : MonoBehaviour {

    public GameObject block;
    public GameObject hero;
    private List<List<int>> field;//0-nothing...1-block...2-temp_block...3-hero....4-floodfill
    private bool bridgeone = false, bridgetwo = false;
    public int size_x, size_y;
    private int free_field_size = 0, area_count = 0;

    // Use this for initialization
    void Start () {
        free_field_size = (size_x-2) * (size_y-2);
        field = new List<List<int>>();

        //build field
        for(int i=1; i<= size_x; i++){
            field.Add(new List<int>());
            for (int j = 1; j <= size_y; j++) {
                field[i-1].Add(0);
                if (i==1 || j==1 || i == size_x || j == size_y) {
                    field[i-1][j-1]=1;
                    Instantiate(block, new Vector3((i-12)*0.51f, (j-8)*0.51f,0), Quaternion.identity);
                }
            }
        }

        //init hero
        field[0][size_y-1] = 3;
        Instantiate(hero, new Vector3(-5.61f, 3.57f, -1), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        updateField();
        justfortest();
    }

    public int getField(int i, int j)
    {
        return field[i][j];
    }
    public void setField(int i, int j, int value)
    {
        field[i][j] = value;
    }

    public void updateTryingBridge(int x, int y)
    {
        if (!bridgeone && field[x][y]==0) {
            bridgeone = true;
            Debug.Log("bridgeone = " + bridgeone);
        } else if (bridgeone && !bridgetwo && field[x][y] == 1) {
            bridgetwo = true;
            Debug.Log("secod = " + bridgetwo);
        }
    }

    private void floodFill(int x, int y, int target, int replace)
    {
        if (field[x][y]!=target) return;

        field[x][y] = replace;

        if (x > 0) floodFill(x - 1, y, target, replace);
        if (x < size_x-1) floodFill(x + 1, y, target, replace);
        if (y > 0) floodFill(x, y - 1, target, replace);
        if (y < size_y-1) floodFill(x, y + 1, target, replace);
    }

    private List<int> getSidePoints()
    {
        List<int> points = new List<int>();
        bool end = false;
        for(int i = 0; i< size_x && !end; i++){
            for (int j = 0; j < size_y && !end; j++) {
                int x = i;
                int y = j;
                if (x > 0 && x < size_x - 1 && field[x-1][y]==0 && field[x+1][y]==0)
                {
                    points.Add(x - 1);
                    points.Add(y);
                    end = true;
                } else if (y > 0 && y < size_y - 1 && field[x][y-1] == 0 && field[x][y+1] == 0){
                    points.Add(x);
                    points.Add(y - 1);
                    end = true;
                }
            }
        }

        return points;
    }

    private int getLowestArea()
    { //retorna o valor da área mais pequena - 0 ou 4 (preencher os 2 depois)
        int count = 0;

        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++)
            {
                if (field[i][j] == 4) count++;
            }
        }
        if (free_field_size - count > 0) return 4;
        return 0;
    }

    private void fillArea(int value, int replace)
    {
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                if (field[i][j] == value) field[i][j] = replace;
            }
        }
    }

    public void updateField()
    {
        if(bridgeone && bridgetwo)
        {
            bridgeone = false;
            bridgetwo = false;

            List<int> side_points = getSidePoints();
            floodFill(side_points[0], side_points[1], 0, 4);
            int value = getLowestArea();
            fillArea(value,1);
            fillArea(2, 1);
        }
    }

    public void justfortest()
    {
        string test = "";
        for (int j = field[0].Count-1; j >=0 ; j--)
        {
            for (int i = 0; i < field.Count; i++) {
                test += field[i][j] + " ";
            }
            test += "\n";
        }
        Debug.Log(test);
    }
}
