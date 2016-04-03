using UnityEngine;
using System.Collections;

[System.Serializable]
public  class Buff
{
    [System.Serializable]
    public enum BuffType
    {
        Passive, // constant effect as long as buff is active
        Board, // an effect that is recalculated every time the board state changes
        Combat // an effect that only impacts combat interactions
    }

    public Unit unit; // the unit this buff belongs to
    public BuffType type;
    

    public Buff(Unit u)
    {
        unit = u;
    }

    public virtual void Destroy() { } // all buffs must cleanup their effects on destruction
}
