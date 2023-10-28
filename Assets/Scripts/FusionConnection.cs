using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    private int playerCount = 0;

    
    
    private const string MAIN_CAMERA_SYSTEM = "Main Camera";
    public bool connectOnAwake;
    [HideInInspector] public NetworkRunner networkRunner;

    [SerializeField] private NetworkObject playerPrefab;

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
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");

        // Increment the player count when a new player connects
        playerCount++;

        // Spawn the player at a different position based on their order of connection
        Vector3 spawnPosition = (playerCount == 1) ? new Vector3(4.5f, 0.5f, 0) : new Vector3(-4.5f, 0.5f, 0);
        NetworkObject playerObject = runner.Spawn(playerPrefab, spawnPosition);
        runner.SetPlayerObject(runner.LocalPlayer, playerObject);
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