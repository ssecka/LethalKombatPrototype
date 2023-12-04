using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    private const string MAIN_CAMERA_SYSTEM = "Main Camera";

    private CharacterInputHandler _characterInputHandler;
    
    public bool ConnectOnAwake;
    [HideInInspector] public NetworkRunner networkRunner;
    [SerializeField] private NetworkObject playerPrefabP1;
    [SerializeField] private NetworkObject playerPrefabP2;
    [SerializeField] private FighterCamera FighterCamera;

    private GameObject[] _players;

    public void PlaySound(EAttackType type, ref SoundEffects soundEffects) =>
        GeneralFunctions.PlaySoundByEnum(type, ref soundEffects);
    
    private void Awake()
    {
        Movement._fusionConnection = this;
        _players ??= new GameObject[2];
        FighterCamera ??= GameObject.Find(MAIN_CAMERA_SYSTEM).transform.GetComponent<FighterCamera>();
        if (ConnectOnAwake)
        {
            ConnectToRunner();
        }

        Debug.Log("Awakaned");
    }
    

    public async void ConnectToRunner()
    {
        Debug.Log("ConnectToRunner");
        networkRunner ??= gameObject.AddComponent<NetworkRunner>();

        await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "Showcase",
            PlayerCount = 2,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");

        // Increment the player count when a new player connects
        
        // Spawn the player at a different position based on their order of connection
        if (runner.SessionInfo.PlayerCount == 1)
        {
            Vector3 spawnPosition = new Vector3(4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP1, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
            _players[0] = playerObject.gameObject;

            _players[0].GetComponent<PlayerStats>().SetTeam(1);
            _players[0].transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (runner.SessionInfo.PlayerCount == 2)
        {
            Vector3 spawnPosition = new Vector3(-4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP2, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
            _players[1] = playerObject.gameObject;
            
            _players[1].GetComponent<PlayerStats>().SetTeam(2);
            _players[1].transform.rotation = Quaternion.Euler(0f, 90f, 0f); 

            FighterCamera ??= new();
            
            FighterCamera.SpawnInPlayer2(_players[0].transform,_players[1].transform);
            
            
            //now we need to give a ref to the other players...
            
        }
        
        
      
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined");

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (
        {
            
        }
    }
    
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}