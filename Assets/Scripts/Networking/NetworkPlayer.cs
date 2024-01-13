using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public GameObject healthBarP1;
    public GameObject healthBarP2;
    public static NetworkPlayer Local { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        NetworkId desiredId;
        desiredId.Raw = 2;
        
        if (Object.Id == desiredId)
        {
            Debug.Log("Object ID: " + desiredId);
            Local = this;
            this.transform.name = "Host";
            
            // Set Health Bar P1 to "Host"
            healthBarP1.name = "Host Health Bar";
            // Set Health Bar P2 to "Client"
            healthBarP2.name = "Client Health Bar";

            Debug.Log("Spawned local player");
        }
        else
        {
            Debug.Log("Object ID: " + desiredId);
            this.transform.name = "Client";
            
            // Set Health Bar P1 to "Client"
            healthBarP1.name = "Client Health Bar";
            // Set Health Bar P2 to "Host"
            healthBarP2.name = "Host Health Bar";
            
            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
}