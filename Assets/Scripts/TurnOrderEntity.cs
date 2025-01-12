using System;
using System.Collections;
using System.Collections.Generic;
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
    public Character.DamageType damageType;
    public Character.Element element;
    
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
    public void Initialize() //Unit test
    {
        if (character != null)
        {
            name = character.name;
            maxHp = character.maxHp;
            currentHp = character.maxHp;
            maxSp = character.maxSp;
            currentSp = character.maxSp;
            strength = character.strength;
            resilience = character.resilience;
            intelligence = character.intelligence;
            mind = character.mind;
            agility = character.agility;
            critChance = character.critChance;
            critDamage = character.critDamage;
            skills = character.skills;
            damageType = character.damageType;
            element = character.element;
            activeStrength = character.strength;
            activeResilience = character.resilience;
            activeIntelligence = character.intelligence;
            activeMind = character.mind;
            activeAgility = character.agility;
            activeCritChance = character.critChance;
            activeCritDamage = character.critDamage;          
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (character != null)
        {
            name = character.name;
            maxHp = character.maxHp;
            currentHp = character.maxHp;
            maxSp = character.maxSp;
            currentSp = character.maxSp;
            strength = character.strength;
            resilience = character.resilience;
            intelligence = character.intelligence;
            mind = character.mind;
            agility = character.agility;
            critChance = character.critChance;
            critDamage = character.critDamage;
            skills = character.skills;
            damageType = character.damageType;
            element = character.element;
            activeStrength = character.strength;
            activeResilience = character.resilience;
            activeIntelligence = character.intelligence;
            activeMind = character.mind;
            activeAgility = character.agility;
            activeCritChance = character.critChance;
            activeCritDamage = character.critDamage;          
        }

        CombatManager.Instance.TargetChanged += CheckTargeting;
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
            entity.TakeDamage(this.strength, damageType, element, 1f);
            await Task.Delay(500);
            entity.GetUntargeted();
            EndTurn(); // temp
        }
        else
        {
            entity.TakeDamage(this.strength, damageType, element, 1f);
            EndTurn(); // temp
        }
        
    }

    //element not relevant for now
    public void TakeDamage(int damageStat, Character.DamageType damageType, Character.Element element, float powerModifier)
    {
        int defendingStat;
        if (damageType == Character.DamageType.Magical)
        {
            defendingStat = activeMind;
        }
        else
        {
            defendingStat = activeResilience;
        }

        float defending = 1f; //no reduction
        if(state == State.Defending)
        {
            defending = 0.5f; //halve damage
        }
        //damagetype/element modifiers to be added, as well as things like area modifiers
        int damageTaken = Mathf.Clamp(Convert.ToInt32(((damageStat * powerModifier) - (defendingStat / 2)) * defending), 1, int.MaxValue);

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

    public void UseSkill(Skill skillUsed, TurnOrderEntity target/*, TurnOrderEntity? actingEntity*/)
    {
        statusbar.UpdateStatusbar(0, DmgPosition);
        switch(skillUsed.skilltype)
        {
            case Skill.Skilltype.PDamage:
                target.TakeDamage(skillUsed.value * strength, skillUsed.damageType, skillUsed.element, skillUsed.powerModifier);
            break;
            case Skill.Skilltype.MDamage:
                target.TakeDamage(skillUsed.value * intelligence, skillUsed.damageType, skillUsed.element, skillUsed.powerModifier);
            break;
            case Skill.Skilltype.Defense:
                GetShield(skillUsed.value * resilience);
            break;
            case Skill.Skilltype.Healing:
                GetHealed(skillUsed.value * mind); 
            break;
        }
        EndTurn();
    }

    public void UseItem(TurnOrderEntity targetEntity/*, Item item*/) //hard coded as a healthpotion rn
    {
        GetHealed(10); //item.value or so
    }
}

public enum State {
    Idle,
    Busy,
    Defending
}
