using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI maxHealthText;
    public GameObject playerObject;
    public float health;
    public float maxHealth;
    [SerializeField] private Slider slider;


    private void Awake()
    {
        SetNumberText(slider.value);
        LoadValues();
    }
    private void FixedUpdate()
    {
        if (playerObject != null)
        {
        health = playerObject.GetComponent<IDamageable>().CurrentHP;
        maxHealth = playerObject.GetComponent<IDamageable>().MaxHP;
        SetHealth(health, maxHealth);
        }
    }
    public void SetNumberText(float value)
    {
        numberText.text = value.ToString();
    }
    public void SaveVolume()
    {
            float volumeValue = slider.value / 100;
            Debug.Log("Volume: " + volumeValue);
            PlayerPrefs.SetFloat("VolumeValue", volumeValue);
            LoadValues();
    }
    public void LoadValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("VolumeValue");
        Debug.Log("LOADED Volume: " + volumeValue);
        slider.value = volumeValue * 100;
        AudioListener.volume = volumeValue;
    }

    public void SetHealth(float value1, float value2)
    {
        healthText.text = value1.ToString();
        maxHealthText.text = value2.ToString();
    }
}
