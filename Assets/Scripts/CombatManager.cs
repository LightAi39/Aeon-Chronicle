using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public CombatSequenceController combatSequenceController;
    public TurnController turnController;
    
    void Awake()
    {
        Instance = this;
    }
}
