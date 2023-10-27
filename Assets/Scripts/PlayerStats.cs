using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    private const float DAMAGE_BLOCK_COEFFICIENT = 0.1f;
    
    private Animator _animator;
    
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Hit = Animator.StringToHash("Hit");
    
    public HealthBarScript HealthBarScript;
    [SerializeField] private int maxhealth = 1000;
    [SerializeField] private int currenthealth;
    
    [SerializeField] public int _player;

    public bool IsBlocking { get; set; } = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Get the Animator component from your character.
        _animator = GetComponent<Animator>();
        
        HealthBarScript ??= new();
        currenthealth = maxhealth;
        HealthBarScript.SetMaxHealth(maxhealth, _player);
     
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //Testing if HealthBars are getting updated(works)
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(20);
        }*/
    }
    
    
    public void TakeDamage(int dmgAmount, Animator animator)
    {
        if (IsBlocking)
        {
            var blocked = dmgAmount + DAMAGE_BLOCK_COEFFICIENT;
            dmgAmount = (int)blocked;
        };
        
        currenthealth -= dmgAmount;
        
        if (currenthealth <= 0)
        {
            //throw new NotImplementedException("TODO: Implement Game End");
            GeneralFunctions.PrintDebugStatement("Im Deadge");
            animator.SetTrigger(Die);
        }
        // Play Hit Animation
        // TODO: INTERRUPT ANIMATION
        animator.SetTrigger(Hit);
        
        HealthBarScript.SetHealth(currenthealth);

        GeneralFunctions.PrintDebugStatement("New Life: " + currenthealth);

    }
}
