using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    private GameObject _healthBarObject;
    [FormerlySerializedAs("gameOverScript")] public GameOverHandler gameOverHandlerScript;

    public Slider HealthSlider;
    public Image Fill;
    private Gradient _gradient;

    private const int ROUND_LIMIT = 10;

    public void SetMaxHealth(int health, string playerName)
    {
        _gradient ??= new();

        var colors = new GradientColorKey[3];
        colors[0] = new(Color.red, 0.0f);
        colors[1] = new(Color.yellow, 0.5f);
        colors[2] = new(Color.green, 1.0f);

        var alphas = new GradientAlphaKey[3];
        alphas[0] = new(1.0f, 0);
        alphas[1] = new(1.0f, 0.5f);
        alphas[2] = new(1.0f, 1f);

        _gradient.SetKeys(colors, alphas);

        if (playerName == "Host")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        }
        else if (playerName == "Client")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        }

        HealthSlider = _healthBarObject.GetComponent<Slider>();
        Fill = _healthBarObject.transform.GetChild(0).GetComponent<Image>();

        if (HealthSlider == null || Fill == null)
        {
            // Handle the case where either HealthSlider or Fill is still null
            Debug.LogError("HealthSlider or Fill is not found.");
            return;
        }

        GeneralFunctions.PrintDebugStatement(HealthSlider);

        HealthSlider.maxValue = health;
        HealthSlider.value = health;

        Fill.color = _gradient.Evaluate(1f);
    }


    public void SetHealth(int health, string playerName)
    {
        
        if (playerName == "Host")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        }
        else if (playerName == "Client")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        }
        
        HealthSlider = _healthBarObject.GetComponent<Slider>();
        Fill = _healthBarObject.transform.GetChild(0).GetComponent<Image>();
        
        HealthSlider.value = health;
        
        Fill.color = _gradient.Evaluate(HealthSlider.normalizedValue);
    }
    
    public void SetRound(int round, string playerName)
    {
        //Host lost so we paint "Client" rounds as yellow
        
        if (playerName == "Host")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;

            if (round == ROUND_LIMIT)
            {
                Fill = _healthBarObject.transform.GetChild(2).GetComponent<Image>();
                Fill.color = Color.yellow;
            }
            else
            {
                Fill = _healthBarObject.transform.GetChild(3).GetComponent<Image>();
                Fill.color = Color.yellow;
             
                gameOverHandlerScript.Setup("Client");

            }    
        }
        //Client lost so we paint "Host" rounds as yellow
        else if (playerName == "Client")
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

            if (round == ROUND_LIMIT)
            {
                Fill = _healthBarObject.transform.GetChild(2).GetComponent<Image>();
                Fill.color = Color.yellow;
            }
            else
            {
                Fill = _healthBarObject.transform.GetChild(3).GetComponent<Image>();
                Fill.color = Color.yellow;
                
                gameOverHandlerScript.Setup("Host");
            }     
        }
    }

    public void ResetHealthBarHost(int health)
    {
        _gradient ??= new();

        var colors = new GradientColorKey[3];
        colors[0] = new(Color.red, 0.0f);
        colors[1] = new(Color.yellow, 0.5f);
        colors[2] = new(Color.green, 1.0f);

        var alphas = new GradientAlphaKey[3];
        alphas[0] = new(1.0f, 0);
        alphas[1] = new(1.0f, 0.5f);
        alphas[2] = new(1.0f, 1f);

        _gradient.SetKeys(colors, alphas);
        
        _healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        
        HealthSlider = _healthBarObject.GetComponent<Slider>();
        Fill = _healthBarObject.transform.GetChild(0).GetComponent<Image>();

        if (HealthSlider == null || Fill == null)
        {
            // Handle the case where either HealthSlider or Fill is still null
            Debug.LogError("HealthSlider or Fill is not found.");
            return;
        }

        GeneralFunctions.PrintDebugStatement(HealthSlider);

        HealthSlider.maxValue = health;
        HealthSlider.value = health;

        Fill.color = _gradient.Evaluate(1f);
    }
    
    public void ResetHealthBarClient(int health)
    {
        _gradient ??= new();

        var colors = new GradientColorKey[3];
        colors[0] = new(Color.red, 0.0f);
        colors[1] = new(Color.yellow, 0.5f);
        colors[2] = new(Color.green, 1.0f);

        var alphas = new GradientAlphaKey[3];
        alphas[0] = new(1.0f, 0);
        alphas[1] = new(1.0f, 0.5f);
        alphas[2] = new(1.0f, 1f);

        _gradient.SetKeys(colors, alphas);
        
        _healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        
        HealthSlider = _healthBarObject.GetComponent<Slider>();
        Fill = _healthBarObject.transform.GetChild(0).GetComponent<Image>();

        if (HealthSlider == null || Fill == null)
        {
            // Handle the case where either HealthSlider or Fill is still null
            Debug.LogError("HealthSlider or Fill is not found.");
            return;
        }

        GeneralFunctions.PrintDebugStatement(HealthSlider);

        HealthSlider.maxValue = health;
        HealthSlider.value = health;

        Fill.color = _gradient.Evaluate(1f);
    }
    
}
