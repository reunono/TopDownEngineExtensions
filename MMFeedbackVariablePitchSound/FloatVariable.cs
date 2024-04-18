using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "ScriptableObjects/FloatVariable", order = 1)]
public class FloatVariable : ScriptableObject
{
    public float Value;
}
