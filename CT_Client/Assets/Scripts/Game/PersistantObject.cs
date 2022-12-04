using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistantObject
{
    public readonly long ID;

    public PersistantObject(long ID)
    {
        this.ID = ID;
    }
}
