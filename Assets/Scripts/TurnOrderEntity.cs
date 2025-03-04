using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This is not a manager, but the main acting unit’s script. It contains a lot of logic for all actions and states, which you can get more information about in the inline comments.
/// If you want a character to do something, or be affected by something, this is where it is implemented. This script is also extensively used as a marker that a game object is a character entity, such as in the TargetingManager or TurnController.
/// Statistics of the entity are loaded in from a scriptable object at the Initialize method.
/// </summary>
public class TurnOrderEntity : MonoBehaviour
{
    [Header("Turn Stats")]
    public int team = 0;
    public int characterIndex = 0;
    public int startDelay = 0;
    public int delayPerTurn = 60;
    public Color color;
    
    [FormerlySerializedAs("character")] [Header("Stats (instantiated by Character SO)")]
    public Character characterScriptableObject;
    [FormerlySerializedAs("characterInstance")]
    [Tooltip("DO NOT SET, instantiated based on character scriptable object above")]
    public Character character;
    
    [Space(20)]
    
    [Header("Stats (live)")]
    [Tooltip("Instantiated based on maxHp")]
    public int currentHp;
    [Tooltip("Instantiated based on maxSp")]
    public int currentSp;
    public int shield;
    [Tooltip("Instantiated based on strength")]
    public int activeStrength;
    [Tooltip("Instantiated based on resilience")]
    public int activeResilience;
    [Tooltip("Instantiated based on intelligence")]
    public int activeIntelligence;
    [Tooltip("Instantiated based on mind")]
    public int activeMind;
    [Tooltip("Instantiated based on agility")]
    public int activeAgility;
    [Tooltip("Instantiated based on critChance")]
    public int activeCritChance;
    [Tooltip("Instantiated based on critDamage")]
    public int activeCritDamage;
    public State state = State.Idle;
    [Tooltip("Instantiated based on Consumables")]
    
    
    [Space(20)]
    public TextMeshPro hpTextBox;
    public TextMeshPro shieldTextBox;
    public TextMeshPro turnIndicator;
    public TextMeshPro defenseIndicator;
    public StatusbarController statusbar;
    public List<Renderer> objectRenderers;
    public Transform DmgPosition;
    public CinemachineVirtualCamera camera;
    private bool isActiveTurn = false;
    private bool isTargeted = false;
    private bool died = false;
    public void Initialize()
    {
        if (characterScriptableObject != null)
        {
            character = PrepareScriptableObjects(characterScriptableObject);
            List<int> stats = GetEquipmentStats();

            name = character.name;
            character.maxHp += stats[0];
            currentHp = character.maxHp;
            character.maxSp += stats[1];
            currentSp = character.maxSp;
            activeStrength = character.strength + stats[2];
            activeResilience = character.resilience + stats[3];
            activeIntelligence = character.intelligence + stats[4];
            activeMind = character.mind + stats[5];
            activeAgility = character.agility + stats[6];
            activeCritChance = character.critChance + stats[7];
            activeCritDamage = character.critDamage + stats[8];

            // Set damagetype resistances.
            foreach (var weakness in character.weaknesses)
            {
                character.damageWeaknesses[weakness.type] = weakness.weaknessValue;
            }
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Initialize();

        CombatManager.Instance.TargetChanged += CheckTargeting;
    }

    private Character PrepareScriptableObjects(Character character)
    {
        Character result = Instantiate(character);

        int i = 0;
        foreach(Equipment equipment in result.equipment)
        {
            result.equipment[i] = equipment ? Instantiate(equipment) : equipment;
            i++;
        }
        result.skills = result.skills.Select(Instantiate).ToList();
        result.consumables = result.consumables.Select(Instantiate).ToList();
        
        return result;
    }

    private List<int> GetEquipmentStats()
    {
        List<int> stats = new List<int>
        {
            0, //hp 0
            0, //sp 1
            0, //strength 2
            0, //resilience 3 
            0, //intelligence 4
            0, //mind 5
            0, //agility 6
            0, //critchance 7
            0 //critdamage 8
        };
        int i = 0;
        foreach(Equipment equipment in character.equipment)
        {
            if(equipment == null)
            {
                continue;
            }

            foreach(StatValuePair _stat in equipment.stats)
            {
                switch(_stat.stat)
                {
                    case Stats.HP:
                    stats[0] += _stat.value;
                    break;
                    case Stats.SP:
                    stats[1] += _stat.value;
                    break; 
                    case Stats.Strength:
                    stats[2] += _stat.value;
                    break; 
                    case Stats.Resilience:
                    stats[3] += _stat.value;
                    break; 
                    case Stats.Intelligence:
                    stats[4] += _stat.value;
                    break; 
                    case Stats.Mind:
                    stats[5] += _stat.value;
                    break; 
                    case Stats.Agility:
                    stats[6] += _stat.value;
                    break; 
                    case Stats.CritChance:
                    stats[7] += _stat.value;
                    break; 
                    case Stats.CritDamage:
                    stats[8] += _stat.value;
                    break;  
                    default:
                    break;
                }
            }
        }

        return stats;
    }

    void Start()
    {
        foreach (var renderer in objectRenderers)
        {
            renderer.material.SetColor("_Color", color);
        }
    }
    
    void Update()
    {
        hpTextBox.text = currentHp.ToString();
        shieldTextBox.text = shield != 0 ? "+" + shield : "";
        
        //turnIndicator.color = isActiveTurn ? Color.white : Color.red;
        //turnIndicator.enabled = isActiveTurn || isTargeted;

        if (!died && currentHp == 0)
        {
            StartCoroutine(Death());
            died = true;
            if (team == 0)
            {
                CombatManager.Instance.targetingManager.RemoveFriendly(this);
            }
            else
            {
                CombatManager.Instance.targetingManager.RemoveEnemy(this);
            }
            
        }

        defenseIndicator.enabled = state == State.Defending;
    }

    private IEnumerator Death()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = startPosition + Vector3.up * 7f; // Moving down by 6.8 units
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(75f, 180f, 0f); // Target rotation

        float elapsedTime = 0f;
        float transitionDuration = 1f; // Duration for the transition, adjust as needed

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the interpolation factor
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            // Smoothly interpolate the position and rotation
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null; // Wait for the next frame
        }

        // Ensure the final position and rotation are set
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public void CheckTargeting()
    {
        if (team == 1)
        {
            isTargeted = CombatManager.Instance.targetingManager.IsActivelyTargeting && CombatManager.Instance.targetingManager.TargetedEnemy.Equals(this) && CombatManager.Instance.targetingManager.targetingEnemies;
        }
        else if (isTargeted != true)
        {
            isTargeted = CombatManager.Instance.targetingManager.IsActivelyTargeting && CombatManager.Instance.targetingManager.TargetedFriendly.Equals(this) && !CombatManager.Instance.targetingManager.targetingEnemies;
        }
        
        // arrows
        turnIndicator.color = isActiveTurn ? Color.white : Color.red;
        turnIndicator.enabled = isActiveTurn || isTargeted;
        
        // outline
        if (isTargeted)
        {
            foreach (var renderer in objectRenderers)
            {
                renderer.materials[1].SetFloat("_Outline_Thickness", 0.01f);
                renderer.materials[1].SetColor("_Outline_Color", Color.red);
            }
        }
        else if (isActiveTurn)
        {
            foreach (var renderer in objectRenderers)
            {
                renderer.materials[1].SetFloat("_Outline_Thickness", 0.01f);
                renderer.materials[1].SetColor("_Outline_Color", Color.white);
            }
        }
        else
        {
            foreach (var renderer in objectRenderers)
            {
                renderer.materials[1].SetFloat("_Outline_Thickness", 0f);
                renderer.materials[1].SetColor("_Outline_Color", Color.black);
            }
        }
    }

    // all these should be through pub/sub
    public void StartTurn()
    {
        isActiveTurn = true;
        state = State.Idle;
        CheckTargeting();
    }

    public void GetTargeted()
    {
        isTargeted = true;
        CheckTargeting();
    }
    
    public void EndTurn()
    {
        isActiveTurn = false;
        CombatManager.Instance.DoEntityCompletedAction();
        CombatManager.Instance.DoTargetChanged();
    }
    
    public void GetUntargeted()
    {
        isTargeted = false;
        CheckTargeting();
    }
    
    public async void Attack(TurnOrderEntity entity)
    {
        if (team == 1) // temp, for enemy AI
        {
            entity.GetTargeted();
            await Task.Delay(1000);
            entity.TakeDamage(character.strength, character.damageTypes, 1f);
            await Task.Delay(500);
            entity.GetUntargeted();
            EndTurn(); // temp
        }
        else
        {
            entity.TakeDamage(character.strength, character.damageTypes, 1f);
            EndTurn(); // temp
        } 
    }

    public async void UseSkill(Skill skillUsed, TurnOrderEntity target/*, TurnOrderEntity? actingEntity*/)
    {
        if (team == 1) // simulate targeting and buffer
        {
            target.GetTargeted();
            await Task.Delay(1000);
        }
        statusbar.UpdateStatusbar(0, DmgPosition);
        switch(skillUsed.skilltype)
        {
            case Skill.Skilltype.PDamage:
                target.TakeDamage(skillUsed.value * activeStrength, skillUsed.damageTypes, skillUsed.powerModifier);
            break;
            case Skill.Skilltype.MDamage:
                target.TakeDamage(skillUsed.value * activeIntelligence, skillUsed.damageTypes, skillUsed.powerModifier);
            break;
            case Skill.Skilltype.Defense:
                /*target.*/GetShield(skillUsed.value * activeResilience);
            break;
            case Skill.Skilltype.Healing:
                /*target.*/GetHealed(skillUsed.value * activeMind); 
            break;
        }
        if (team == 1) // simulate detargeting and buffer
        {
            await Task.Delay(500);
            target.GetUntargeted();
        }
        EndTurn();
    }

    public void TakeDamage(int damageStat, List<DamageType> damageTypes, float powerModifier)
    {
        int defendingStat;
        if (damageTypes.Contains(DamageType.Magical))
        {
            defendingStat = activeMind;
        }
        else
        {
            defendingStat = activeResilience;
        }

        float defending = 1f; // Default modifier, resulting in no effect on damage taken.
        if(state == State.Defending)
        {
            defending = 0.5f; // Modifier now halves damage taken.
        }
        UnityEngine.Debug.Log(damageTypes);
        float damageTypeMultiplier = 1f;
        foreach (DamageType damageType in damageTypes)
        {
            if (character.damageWeaknesses.ContainsKey(damageType))
            {
                float multiplier = character.damageWeaknesses[damageType] - 100f; // Calculates the % effectivness (200 - 100 = 100, becoming + 1 to the multiplier).
                damageTypeMultiplier += multiplier / 100f; // Changes the % effectiveness into a multiplier.
            }
        }

        int damageTaken = Mathf.Clamp(Convert.ToInt32(((damageStat * powerModifier) - (defendingStat / 2)) * defending * damageTypeMultiplier), 1, int.MaxValue);

        int trueDamage = damageTaken - shield;
        if (trueDamage > 0)
        {
            currentHp -= trueDamage;
            if (currentHp < 0)
            {
                currentHp = 0;
            }
        }
        shield -= damageTaken;
        if (shield < 0)
        {
            shield = 0;
        }

        if(statusbar != null)
        {
            statusbar.UpdateStatusbar(-damageTaken, DmgPosition);
        }
    }

    public void Defend()
    {
        statusbar.UpdateStatusbar(0, DmgPosition);
        state = State.Defending; // gets set to idle in the start of the next turn
        EndTurn();
    }

    public void GetHealed(int healing)
    {
        statusbar.UpdateStatusbar(healing, DmgPosition);
        currentHp += healing;
        if (currentHp > character.maxHp)
        {
            currentHp = character.maxHp;
        }
    }

    public void GetShield(int shielding)
    {
        statusbar.UpdateStatusbar(0, DmgPosition);
        shield = shielding;
    }

    public void UseItem(Consumable consumable, [CanBeNull] TurnOrderEntity target) //List target for aoe when aoe implemented 
    {
        if (target == null)
        {
            target = consumable.useOnFriendlies ? this : CombatManager.Instance.targetingManager.TargetedEnemy;
        }
        switch(consumable.effect)
        {
            case ConsumableEffect.Heal:
            target.GetHealed(consumable.effectStrength);
            break;
            case ConsumableEffect.Shield:
            target.GetShield(consumable.effectStrength);
            break;
            case ConsumableEffect.Revive: // not implemented
            break;
            case ConsumableEffect.Buff: // not implemented
            break;
        }

        consumable.amountOfUses --;
        if (consumable.amountOfUses == 0)
        {
            character.consumables.Remove(consumable);
        }
        EndTurn();
    }
}

public enum State {
    Idle,
    Busy,
    Defending
}
