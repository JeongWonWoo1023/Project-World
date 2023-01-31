using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData : SaveData
{
    public StatusData status = new StatusData();
    public int level;
    public int currentHP;
    public int currentMP;
    public int currentEXP;
}
