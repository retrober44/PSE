using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract. Managers inherit from this class for polymorphism.
/// </summary>
public abstract class ManagerComponent : MonoBehaviour
{
    public abstract void Init();
}
