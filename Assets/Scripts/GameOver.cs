
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class GameOver : MonoBehaviour
{
    private Text winnerText;
    private GameObject _background;
    public void Setup(string playerName)
    {
        _background = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        _background.SetActive(true);
        winnerText = _background.transform.GetChild(1).GetComponent<Text>();
        winnerText.text = playerName + " has won!";
    }
}
