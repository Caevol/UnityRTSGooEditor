using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Structure : Damageable
{
    public abstract int getLandChange();
    public abstract void updateSprite();
}

