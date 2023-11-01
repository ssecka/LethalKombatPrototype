using System;
using UnityEngine;
using UnityEngine.UI;


public class HealthBarScript : MonoBehaviour
{

    private GameObject _healthBarObject;
    public Slider healthSlider;
    public Image fill;
    private Gradient _gradient;

    private void Awake()
    {
    }

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
        
        healthSlider ??= _healthBarObject.GetComponent<Slider>();
        fill ??= _healthBarObject.transform.GetChild(0).GetComponent<Image>();

        GeneralFunctions.PrintDebugStatement(healthSlider);

        healthSlider.maxValue = health;
        healthSlider.value = health;

        fill.color = _gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;

        fill.color = _gradient.Evaluate(healthSlider.normalizedValue);
    }
}
