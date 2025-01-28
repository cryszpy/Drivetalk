using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/PassengerExpression")]
public class PassengerExpression : ScriptableObject
{

    public string animatorTrigger;

    public bool runExpressionTimer = false;

    public bool allowedAsDefault = false;
}