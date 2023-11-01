using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    private const string MAIN_CAMERA_SYSTEM = "Main Camera";
    public bool connectOnAwake;
    [HideInInspector] public NetworkRunner networkRunner;

    [SerializeField] private NetworkObject playerPrefabP1;
    [SerializeField] private NetworkObject playerPrefabP2;


    private void Awake()
    {
        if (connectOnAwake)
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
            SessionName = "TestRoom",
            PlayerCount = 2,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    /*public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
        NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
        runner.SetPlayerObject(runner.LocalPlayer, playerObject);
    }*/
    /*public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");

        // Increment the player count when a new player connects
        playerCount++;

        // Spawn the player at a different position based on their order of connection
        if (playerCount == 1)
        {
            Vector3 spawnPosition = new Vector3(4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP1, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
        }
        else
        {
            Vector3 spawnPosition = new Vector3(-4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP2, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
        }
    }*/
    
    
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");

        // Increment the player count when a new player connects
        
        // Spawn the player at a different position based on their order of connection
        Vector3 spawnPosition = (runner.SessionInfo.PlayerCount == 1) ? new Vector3(4.5f, 0.5f, 0) : new Vector3(-4.5f, 0.5f, 0);
        var rotation = (runner.SessionInfo.PlayerCount == 1) ? playerPrefabP1.transform.rotation = Quaternion.Euler(0f,-90f,0f): playerPrefabP1.transform.rotation = Quaternion.Euler(0f,90f,0f);
        NetworkObject playerObject = runner.Spawn(playerPrefabP1, spawnPosition, rotation);
        runner.SetPlayerObject(runner.LocalPlayer, playerObject);

        /*if (runner.SessionInfo.PlayerCount == 1)
        {
            Vector3 spawnPosition = new Vector3(4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP1, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
        }
        else
        {
            Vector3 spawnPosition = new Vector3(-4.5f, 0.5f, 0);
            NetworkObject playerObject = runner.Spawn(playerPrefabP2, spawnPosition);
            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
        }*/
      
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined");
        
        GameObject.Find(MAIN_CAMERA_SYSTEM).transform.GetComponent<FighterCamera>().SpawnInPlayer2();

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
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