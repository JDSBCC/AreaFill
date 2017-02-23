using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block
{
    private GameObject game_object;
    
    public Block()
    {

    }

    public GameObject GetGameObject()
    {
        return game_object;
    }

    public void SetGameObject(GameObject game_object)
    {
        this.game_object = game_object;
    }
}
