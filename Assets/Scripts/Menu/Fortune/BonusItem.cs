using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BonusItem : IComparable
{
    //public int ID;
    public int Amount;
    [Range(0, 100)]
    public float ProbabilityPercent;
    public float Angle;

    public float Percent
    {
        get { return ProbabilityPercent / 100; }
    }

    public int CompareTo(object obj)
    {
        return Percent.CompareTo(((BonusItem)obj).Percent);
    }
}
