using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class Aliment_Settings : ScriptableObject
{
    [SerializeField] Color m_alimentColor;
    [SerializeField] Sprite m_alimentIcon;
    public Color alimentColor => m_alimentColor;
    public Sprite alimentIcon => m_alimentIcon;
}