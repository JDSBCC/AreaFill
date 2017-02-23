using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero: Block
{

    private int lifes;
    private Position position;

    public Hero(int x, int y) : base()
    {
        lifes = 3;
        position = new Position(x, y);
        this.SetGameObject(Resources.Load("Prefabs/Game/Hero") as GameObject);
    }

    public void DecreaseLife()
    {
        lifes--;
    }

    public void SetPosition(int x, int y)
    {
        position.SetPosition(x, y);
    }

    public Position GetPosition()
    {
        return position;
    }
}
