using UnityEngine;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    private Text winnerText;
    private GameObject _background;
    private HPHandler _hpHandler;
    
    public void Setup(string playerName, HPHandler hpHandler)
    {
        _background = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        _background.SetActive(true);
        winnerText = _background.transform.GetChild(1).GetComponent<Text>();
        winnerText.text = playerName + " has won!";
    }

    public void PlayAgainButton()
    {
        var clientPlayer = GameObject.Find("Host").GetComponent<NetworkCharacterControllerPrototypeCustom>();
        var cHp = clientPlayer.GetComponentInChildren<HPHandler>();
        cHp.Round = 0;
        clientPlayer._isResetRequested = true;
        var hostPlayer = GameObject.Find("Client").GetComponent<NetworkCharacterControllerPrototypeCustom>();
        var hHp = hostPlayer.GetComponentInChildren<HPHandler>();
        hHp.Round = 0;
        hostPlayer._isResetRequested = true;
        
        _background = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        _background.SetActive(false);
        var healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        var Fill = healthBarObject.transform.GetChild(2).GetComponent<Image>();
        Fill.color = Color.white;
        Fill = healthBarObject.transform.GetChild(3).GetComponent<Image>();
        Fill.color = Color.white;
        
        healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        Fill = healthBarObject.transform.GetChild(2).GetComponent<Image>();
        Fill.color = Color.white;
        Fill = healthBarObject.transform.GetChild(3).GetComponent<Image>();
        Fill.color = Color.white;
    }
}
