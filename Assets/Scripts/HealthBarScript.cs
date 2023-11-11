using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{

    private GameObject _healthBarObject;
    public Slider HealthSlider;
    public Image Fill;
    private Gradient _gradient;

    public void SetMaxHealth(int health, int playerNum)
    {

        _gradient ??= new();

        var colors = new GradientColorKey[3];
        colors[0] = new(Color.red, 0.0f);
        colors[1] = new(Color.yellow, 0.5f);
        colors[2] = new(Color.green, 1.0f);

        var alphas = new GradientAlphaKey[3];
        alphas[0] = new(1.0f,0);
        alphas[1] = new(1.0f,0.5f);
        alphas[2] = new(1.0f,1f);
         
        _gradient.SetKeys(colors,alphas);
        
        if (playerNum == 1)
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        }
        else if (playerNum == 2)
        {
            _healthBarObject = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        } 
        
        HealthSlider ??= _healthBarObject.GetComponent<Slider>();
        Fill ??= _healthBarObject.transform.GetChild(0).GetComponent<Image>();

        GeneralFunctions.PrintDebugStatement(HealthSlider);

        HealthSlider.maxValue = health;
        HealthSlider.value = health;

        Fill.color = _gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        HealthSlider.value = health;

        Fill.color = _gradient.Evaluate(HealthSlider.normalizedValue);
    }
}
