using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cinemachine;
using TMPro;
using UnityEngine;

public class TurnOrderEntity : MonoBehaviour
{
    [Header("Turn Stats")]
    public int team = 0;
    public int characterIndex = 0;
    public int startDelay = 0;
    public int delayPerTurn = 60;
    public Color color;
    
    [Header("Stats (instantiated by Character SO)")]
    public Character character;
    [Space(20)]
    public string name;
    public int maxHp;
    public int maxSp;
    public int strength;
    public int resilience;
    public int intelligence;
    public int mind;
    public int agility;
    public int critChance;
    public int critDamage;
    public List<Skill> skills = new List<Skill>();
    public List<DamageType> damageTypes = new List<DamageType>();
    
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
    
    
    [Space(20)]
    public TextMeshPro hpTextBox;
    public TextMeshPro shieldTextBox;
    public TextMeshPro turnIndicator;
    public TextMeshPro defenseIndicator;
    public StatusbarController statusbar;
    public List<Renderer> objectRenderers;
    public Transform DmgPosition;
    public CinemachineVirtualCamera camera;
    public TurnOrderEntity EntityToAttackTemp;
    private bool isActiveTurn = false;
    private bool isTargeted = false;
    private bool died = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (character != null)
        {
            List<int> stats = GetEquipmentStats();

            name = character.name;
            maxHp = character.maxHp + stats[0];
            currentHp = maxHp;
            maxSp = character.maxSp + stats[1];
            currentSp = maxSp;
            strength = character.strength;
            resilience = character.resilience;
            intelligence = character.intelligence;
            mind = character.mind;
            agility = character.agility;
            critChance = character.critChance;
            critDamage = character.critDamage;
            skills = character.skills;
            damageTypes = character.damageTypes;
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

        CombatManager.Instance.TargetChanged += CheckTargeting;
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
        
        if (character.weapon != null)
        {
            stats[2] += character.weapon.strength;
            stats[4] += character.weapon.intelligence;
        }

        if (character.headpiece != null)
        {
            stats[0] += character.headpiece.hp;
            stats[1] += character.headpiece.sp;
            stats[4] += character.headpiece.intelligence;
        }

        if (character.chestpiece != null)
        {
            stats[0] += character.chestpiece.hp;
            stats[1] += character.chestpiece.sp;
            stats[3] += character.chestpiece.resilience;
            stats[5] += character.chestpiece.mind;
        }

        if (character.gloves != null)
        {
            stats[2] += character.gloves.strength;
            stats[7] += character.gloves.critChance;
            stats[8] += character.gloves.critDamage;
        }

        if (character.legs != null)
        {
            stats[0] += character.legs.hp;
            stats[1] += character.legs.sp;
            stats[3] += character.legs.resilience;
            stats[5] += character.legs.mind;
            stats[6] += character.legs.agility;
        }

        if (character.boots != null)
        {
            stats[6] += character.boots.agility;
            stats[7] += character.boots.critChance;
            stats[8] += character.boots.critDamage;
        }

        if (character.accessory != null)
        {
            stats[2] += character.accessory.strength;
            stats[4] += character.accessory.intelligence;
            stats[7] += character.accessory.critChance;
            stats[8] += character.accessory.critDamage;
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
            entity.TakeDamage(this.strength, damageTypes, 1f);
            await Task.Delay(500);
            entity.GetUntargeted();
            EndTurn(); // temp
        }
        else
        {
            entity.TakeDamage(this.strength, damageTypes, 1f);
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

        statusbar.UpdateStatusbar(-damageTaken, DmgPosition);
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
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }

    public void GetShield(int shielding)
    {
        statusbar.UpdateStatusbar(0, DmgPosition);
        shield = shielding;
    }

    public void UseItem(Consumable consumable, TurnOrderEntity target) //List target for aoe when aoe implemented 
    {
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
    }
}

public enum State {
    Idle,
    Busy,
    Defending
}
