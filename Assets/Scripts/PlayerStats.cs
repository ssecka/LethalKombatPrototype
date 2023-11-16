using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStats : MonoBehaviour
{

    private const float DAMAGE_BLOCK_COEFFICIENT = 0.4f;
    
    private Animator _animator;

    private HitFreezeSystem _hitFreezeSystem;
    [SerializeField] private SoundEffects _soundEffects;
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private Rigidbody _rb;
    private Movement _mov;
    public GameObject blockEffect;

    public HealthBarScript HealthBarScript;
    [SerializeField] private int MaxHealth = 1000;
    [SerializeField] private int CurrentHealth;
    
    [SerializeField] private int _team;

    public int GetTeam()
    {
        return _team;
    }

    public void SetTeam(int num)
    {
        _team = num;
        HealthBarScript.SetMaxHealth(MaxHealth, _team);
    }
    
    public bool IsBlocking { get; set; } = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Get the Animator component from your character.
        _animator = GetComponent<Animator>();
        _hitFreezeSystem ??= GameObject.Find("PlayerEnvironmentSystem")?.GetComponent<HitFreezeSystem>();
        HealthBarScript ??= new();
        HealthBarScript.SetMaxHealth(MaxHealth,_team);
        _rb = GetComponent<Rigidbody>();
        _mov = GetComponent<Movement>();

        CurrentHealth = MaxHealth;
    }
    
    // Update is called once per frame
    void Update()
    {
    }
    
    
    public void TakeDamage(int dmgAmount, Animator animator, ref EAttackType attackType, float xCoordPushbackOnBlock)
    {
        //Convert to int, leftshift 1, then back to enum
        attackType = (EAttackType)((int)attackType << 1);
        
        if (IsBlocking)
        {
            var blocked = dmgAmount * DAMAGE_BLOCK_COEFFICIENT;
            dmgAmount = (int)blocked;
            attackType = EAttackType.Block;
            PushBack(xCoordPushbackOnBlock); 
            Instantiate(blockEffect, _mov._lefthandAttackPoint.position, Quaternion.identity);
            GeneralFunctions.PlaySoundByEnum(EAttackType.Block, ref _soundEffects);
        }
        

        CurrentHealth -= dmgAmount;
        HealthBarScript.SetHealth(CurrentHealth, _team);

        _hitFreezeSystem.Freeze();

        if (CurrentHealth <= 0)
        {
            //throw new NotImplementedException("TODO: Implement Game End");
            GeneralFunctions.PrintDebugStatement("Im Deadge");
            animator.SetTrigger(Die);
        }
        // Play Hit Animation
        // TODO: INTERRUPT ANIMATION
        animator.SetTrigger(Hit);
        

        GeneralFunctions.PrintDebugStatement("New Life: " + CurrentHealth);

    }

    public void PushBack(float xCoordPushForce)
    {
        if (_mov.facingRight)
        {
            _rb.AddForce(xCoordPushForce,0,0, ForceMode.Impulse);
        }
        else
        {
           _rb.AddForce(-xCoordPushForce,0,0, ForceMode.Impulse); 
        }
        
    }
}
