
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameOverHandler : NetworkBehaviour
{
    private Text winnerText;
    private GameObject _background;
    
    [Networked(OnChanged = nameof(ResetFlagChanged))]
    public bool ResetFlag { get; set; }
    
    public void Setup(string playerName)
    {
        _background = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        _background.SetActive(true);
        winnerText = _background.transform.GetChild(1).GetComponent<Text>();
        winnerText.text = playerName + " has won!";
        ResetFlag = true;
    }

    public void PlayAgainButton()
    {
        //debug
        print("play again pressed");
        //SceneManager.LoadScene("SampleScene");
        var host = GameObject.Find("Host")?.GetComponentInChildren<NetworkCharacterControllerPrototypeCustom>();
        var client = GameObject.Find("Client")?.GetComponentInChildren<NetworkCharacterControllerPrototypeCustom>();
        
        
        // very very dirty way of handleing this
        if (host != null && client != null)
        {
            var p = host.GetType().GetField("_isResetRequested",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            p.SetValue(host,true);
            p.SetValue(client,true);

            ResetFlag = false;
        }
    }

    public static void ResetFlagChanged(Changed<GameOverHandler> changed)
    {
        if (changed.Behaviour.ResetFlag) return;
        if (changed.Behaviour._background == null)
        {
            changed.Behaviour._background = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        }
        changed.Behaviour._background.SetActive(false);
        
    }
}
