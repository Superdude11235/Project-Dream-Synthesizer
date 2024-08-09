using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour
{
    public delegate void EventHandler();

    public static event EventHandler Enemydied;
    public static event EventHandler Saveprogress;
    public static event EventHandler Loadprogress;
    public static event EventHandler Enemycountered;
    
    public static void EnemyDied()
    {
        if (Enemydied != null) Enemydied();
    }

    public static void SaveProgress()
    {
        if (Saveprogress != null) Saveprogress();
    }

    public static void LoadProgress()
    {
        if (Loadprogress != null) Loadprogress();
    }

}
