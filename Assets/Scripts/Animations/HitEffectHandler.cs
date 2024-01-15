using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HitEffectHandler : NetworkBehaviour
{
    private PlayerRef _firedByPlayer;
    private string _firedByPlayerName;
    private NetworkObject _firedByNetworkObject;

    private bool _shootRight = false;
    
    private NetworkCharacterControllerPrototypeCustom _owner;

    private TickTimer _maxLiveTimer = TickTimer.None;
    
    //other objects

    private NetworkObject _networkObject;
    
    public void SpawnIn(PlayerRef firedByPlayerRef, NetworkObject firedByNetworkObject, string firedByPlayerName, NetworkCharacterControllerPrototypeCustom owner)
    {
        this._firedByPlayer = firedByPlayerRef;
        this._firedByPlayerName = firedByPlayerName;
        this._firedByNetworkObject = firedByNetworkObject;

        this._owner = owner;
        
        _networkObject = GetComponent<NetworkObject>();

        _maxLiveTimer = TickTimer.CreateFromSeconds(Runner, 1);

        var rotation = Quaternion.identity;

        this.transform.rotation = rotation;

    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        
        if (_maxLiveTimer.Expired(Runner))
        {
            Runner.Despawn(_networkObject);
        }
    }
}
