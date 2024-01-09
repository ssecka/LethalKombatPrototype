using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class FireballHandler : NetworkBehaviour
{
    private const float SPEED = 4.75f;

    private List<LagCompensatedHit> _hits = new();

    private PlayerRef _firedByPlayer;
    private string _firedByPlayerName;
    private NetworkObject _firedByNetworkObject;
    public Transform attackPoint;

    private NetworkCharacterControllerPrototypeCustom _owner;

    private TickTimer _maxLiveTimer = TickTimer.None;
    
    //other objects

    private NetworkObject _networkObject;

    public void Fire(PlayerRef firedByPlayerRef, NetworkObject firedByNetworkObject, string firedByPlayerName, NetworkCharacterControllerPrototypeCustom owner)
    {
        this._firedByPlayer = firedByPlayerRef;
        this._firedByPlayerName = firedByPlayerName;
        this._firedByNetworkObject = firedByNetworkObject;

        this._owner = owner;
        
        _networkObject = GetComponent<NetworkObject>();

        _maxLiveTimer = TickTimer.CreateFromSeconds(Runner, 7);

    }

    public override void FixedUpdateNetwork()
    {
        
        transform.position += transform.forward * Runner.DeltaTime * SPEED;

        if (Object.HasStateAuthority)
        {
            if (_maxLiveTimer.Expired(Runner))
            {
                Runner.Despawn(_networkObject);

                return;
            }
            
            
            int hitCount =
                Runner.LagCompensation.OverlapSphere(
                    this.transform.position,
                    0.25f,
                    _firedByPlayer,
                    _hits, options: HitOptions.IncludePhysX);

            bool isValidHit = hitCount > 0;

            for (int i = 0; i < hitCount; i++)
            {
                if (_hits[i].Hitbox != null)
                {
                    if (_hits[i].Hitbox.Root.GetBehaviour<NetworkObject>() == _firedByNetworkObject)
                    {
                        isValidHit = false;
                    }

                    if (_hits[i].Hitbox.Root.GetComponentInChildren<Wall>() || _hits[i].Hitbox.Root.GetComponent<Wall>())
                    {
                        // we hit the wall...
                        Runner.Despawn(_networkObject);
                        return;
                    }
                }
            }

            if (isValidHit)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    var transformRoot = _hits[i].GameObject.transform.root;
                    HPHandler hpHandler = transformRoot
                        .GetComponentInChildren<HPHandler>();
                    
                    
                    if (hpHandler == null) continue;

                    var otherPlayer = transformRoot.GetComponentInChildren<NetworkCharacterControllerPrototypeCustom>();
                    
                    if(otherPlayer == null || _owner == otherPlayer) continue;
                    
                    
                    var otherPlayerIsBlocking = otherPlayer.BlockState == 1;
                    
                    hpHandler.OnHitTaken(100,otherPlayerIsBlocking);

                }
                
                Runner.Despawn(_networkObject);
            }
            
        }
    }
}
