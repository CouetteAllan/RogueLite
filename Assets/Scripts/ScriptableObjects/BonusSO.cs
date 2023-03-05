using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BonusSO : ItemsSO
{
    public string description = "Insert Bonus description to display";
    public new string name = "Bonus Name";

    public abstract override void DoEffect(MainCharacterScript3D player);
}
