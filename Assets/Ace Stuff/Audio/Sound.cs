using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[System.Serializable]
[Inspectable]
public class Sound
{
    [Inspectable]
    public string name;

    [Inspectable]
    public AudioClip clip;

}
