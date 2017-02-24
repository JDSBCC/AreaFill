using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BuildField : MonoBehaviour {

    private List<List<FieldState>> field;//0-nothing...1-block...2-temp_block...3-hero....4-floodfill
    private List<List<FieldState>> old_field;//necessário para evitr redesenhar objetos
    private bool bridgeone = false, bridgetwo = false;
    public int orig_x, orig_y, size_x, size_y;//TODO alterar para private quando tiver a ser inserido pelo menu

    public Wall block;
    public Hero hero;

    // Use this for initialization
    void Start () {
        //initialize variables
        field = new List<List<FieldState>>();
        old_field = new List<List<FieldState>>();
        block = new Wall();
        hero = new Hero(orig_x, orig_y);

        //build field
        var block_object = block.GetGameObject();
        for (int i=0; i< size_x; i++){
            field.Add(new List<FieldState>());
            old_field.Add(new List<FieldState>());
            for (int j = 0; j < size_y; j++) {
                field[i].Add(0);
                old_field[i].Add(0);
                if (i==0 || j==0 || i == size_x-1 || j == size_y-1) {
                    field[i][j]= FieldState.BLOCK;
                    old_field[i][j]= FieldState.BLOCK;
                    Instantiate(block_object, new Vector3((i + orig_x) * 0.51f, (j + orig_y) * 0.51f, 0), Quaternion.identity);
                }
            }
        }

        //init hero
        var pos = hero.GetPosition();
        field[pos.GetX()-orig_x][pos.GetY()-orig_y] = FieldState.HERO;
        old_field[pos.GetX()-orig_x][pos.GetY()-orig_y] = FieldState.HERO;
        Instantiate(hero.GetGameObject(), new Vector3(pos.GetX() * 0.51f, pos.GetY() * 0.51f, -1), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateField();
        justfortest();
    }

    public FieldState GetField(int i, int j)
    {
        return field[i][j];
    }
    public void SetField(int i, int j, FieldState value)
    {
        field[i][j] = value;
    }

    public int GetFieldCount()
    {
        try
        {
            return field.Count;
        }
        catch (NullReferenceException)
        {
            return 0;
        }
    }

    public int GetOrigX()
    {
        return orig_x;
    }

    public int GetOrigY()
    {
        return orig_y;
    }

    public int GetSizeX()
    {
        return size_x;
    }

    public int GetSizeY()
    {
        return size_y;
    }

    public void UpdateTryingBridge(int x, int y)
    {
        if (!bridgeone && field[x][y]==FieldState.EMPTY) {
            bridgeone = true;
        } else if (bridgeone && !bridgetwo && (field[x][y] == FieldState.BLOCK || field[x][y] == FieldState.TEMP_BLOCK)) {
            bridgetwo = true;
        }
    }

    private void FloodFill(int x, int y, FieldState target, FieldState replace)
    {
        if (field[x][y]!=target) return;

        field[x][y] = replace;

        if (x > 0) FloodFill(x - 1, y, target, replace);
        if (x < size_x-1) FloodFill(x + 1, y, target, replace);
        if (y > 0) FloodFill(x, y - 1, target, replace);
        if (y < size_y-1) FloodFill(x, y + 1, target, replace);
    }

    private List<int> GetSidePoints()
    {
        List<int> points = new List<int>();
        bool end = false;
        for(int i = 0; i< size_x && !end; i++){
            for (int j = 0; j < size_y && !end; j++) {
                int x = i;
                int y = j;
                if (x > 0 && x < size_x - 1 && field[x-1][y]==FieldState.EMPTY && field[x+1][y]== FieldState.EMPTY)
                {
                    points.Add(x - 1);
                    points.Add(y);
                    end = true;
                } else if (y > 0 && y < size_y - 1 && field[x][y-1] == FieldState.EMPTY && field[x][y+1] == FieldState.EMPTY)
                {
                    points.Add(x);
                    points.Add(y - 1);
                    end = true;
                }
            }
        }

        return points;
    }

    private FieldState GetLowestArea()
    { //retorna o valor da área mais pequena - 0 ou 4 (preencher os 2 depois)
        int count_zero = 0, count_four = 0; ;

        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++)
            {
                if (field[i][j] == FieldState.EMPTY) count_zero++;
                if (field[i][j] == FieldState.FLOOD_FILL) count_four++;
            }
        }
        if (count_zero - count_four > 0) return FieldState.FLOOD_FILL;
        if (count_zero - count_four < 0) return FieldState.EMPTY;
        System.Random rnd = new System.Random();
        return (FieldState) (rnd.Next(2) * 4);
    }

    private void FillArea(FieldState value, FieldState replace)
    {
        for (int i = 0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                if (field[i][j] == value)
                {
                    field[i][j] = replace;
                }
            }
        }
    }

    public void UpdateField()
    {
        if(bridgeone && bridgetwo)
        {
            bridgeone = false;
            bridgetwo = false;

            List<int> side_points = GetSidePoints();
            FloodFill(side_points[0], side_points[1], FieldState.EMPTY, FieldState.FLOOD_FILL);
            FieldState value = GetLowestArea();
            FillArea(value, FieldState.BLOCK);//replace do 0 ou 4 por 1
            FillArea(FieldState.TEMP_BLOCK, FieldState.BLOCK);//replace dos 2 (o caminho realizado) por 1
            FillArea(FieldState.FLOOD_FILL, FieldState.EMPTY);//replace dos 4 restantes por 0
        }
    }

    public void Draw()
    {
        var block_object = block.GetGameObject();
        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                if(field[i][j]!=old_field[i][j]){
                    if (field[i][j] == FieldState.BLOCK || field[i][j] == FieldState.TEMP_BLOCK) {
                        Instantiate(block_object, new Vector3((i + orig_x) * 0.51f, (j + orig_y) * 0.51f, 0), Quaternion.identity);
                    }
                }
            }
        }

        for (int i = 0; i < size_x; i++) {
            for (int j = 0; j < size_y; j++) {
                old_field[i][j]=field[i][j];
            }
        }
    }

    private void justfortest()//TODO: remover isto
    {
        string test = "";
        for (int j = field[0].Count-1; j >=0 ; j--)
        {
            for (int i = 0; i < field.Count; i++) {
                test += (int)field[i][j] + " ";
            }
            test += "\n";
        }
        Debug.Log(test);
    }
}
