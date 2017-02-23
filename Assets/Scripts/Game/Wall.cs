using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall: Block {

    public Wall() : base()
    {
        this.SetGameObject(Resources.Load("Prefabs/Game/Wall") as GameObject);
    }
}
