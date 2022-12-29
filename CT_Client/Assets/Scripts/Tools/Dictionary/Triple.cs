using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Triple<A, B, C>
{
    public A Item1;
    public B Item2;
    public C Item3;

    public Triple(A _Item1, B _Item2, C _Item3)
    {
        Item1 = _Item1;
        Item2 = _Item2;
        Item3 = _Item3;
    }
}
