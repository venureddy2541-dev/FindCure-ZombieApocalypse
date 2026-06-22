using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SensitivityManager : MonoBehaviour
{
    [SerializeField] SensitivitySB sensitivitySB;
    public Dictionary<SensyType,float> sensitivity = new Dictionary<SensyType,float>();
    public static SensitivityManager sensitivityManager;
    [SerializeField] Slider adsSlider;
    [SerializeField] TMP_Text adsText;
    [SerializeField] Slider pistolSlider;
    [SerializeField] TMP_Text pistolText;
    [SerializeField] Slider autoSlider;
    [SerializeField] TMP_Text autoText;
    [SerializeField] Slider shotGunSlider;
    [SerializeField] TMP_Text shotGunText;
    [SerializeField] Slider sniperSlider;
    [SerializeField] TMP_Text sniperText;

    public delegate void Ads(float value);
    public event Ads UpdateAdsSensy;

    void Awake()
    {
        if(sensitivityManager != null && sensitivityManager != this)
        {
            Destroy(this.gameObject);
            return;
        }    

        sensitivityManager = this;
        DontDestroyOnLoad(this);
        
        SetSliderMinMaxValues();
        UpdateSliderValues();
    } 

    void SetSliderMinMaxValues()
    {
        adsSlider.minValue = sensitivitySB.min;
        adsSlider.maxValue = sensitivitySB.max;
        pistolSlider.minValue = sensitivitySB.min;
        pistolSlider.maxValue = sensitivitySB.max;
        autoSlider.minValue = sensitivitySB.min;
        autoSlider.maxValue = sensitivitySB.max;
        shotGunSlider.minValue = sensitivitySB.min;
        shotGunSlider.maxValue = sensitivitySB.max;
        sniperSlider.minValue = sensitivitySB.min; 
        sniperSlider.maxValue = sensitivitySB.max; 
    }

    void UpdateSliderValues()
    {
        adsSlider.value = PlayerPrefs.GetFloat("ADSSensy",1f);
        pistolSlider.value = PlayerPrefs.GetFloat("PistolSensy",1f);
        autoSlider.value = PlayerPrefs.GetFloat("AutoSensy",1f);
        shotGunSlider.value = PlayerPrefs.GetFloat("ShotGunSensy",1f);
        sniperSlider.value = PlayerPrefs.GetFloat("SniperSensy",1f);

        sensitivity[SensyType.ADS] = adsSlider.value;
        adsText.text = ConvertValue(adsSlider.value);
        sensitivity[SensyType.PISTOLGUN] = pistolSlider.value;
        pistolText.text = ConvertValue(pistolSlider.value);
        sensitivity[SensyType.AUTOGUN] = autoSlider.value;
        autoText.text = ConvertValue(autoSlider.value);
        sensitivity[SensyType.SHOTGUN] = shotGunSlider.value;
        shotGunText.text = ConvertValue(shotGunSlider.value);
        sensitivity[SensyType.SNIPER] = sniperSlider.value;
        sniperText.text = ConvertValue(sniperSlider.value);
    }

    string ConvertValue(float value)
    {
        return Mathf.RoundToInt((value/sensitivitySB.max)*100).ToString();
    }

    public void ADS(float value)
    {
        sensitivity[SensyType.ADS] = value;
        adsText.text = ConvertValue(value);
        PlayerPrefs.SetFloat("ADSSensy",value);
        UpdateAdsSensy?.Invoke(value);
    }

    public void Pistol(float value)
    {
        sensitivity[SensyType.PISTOLGUN] = value;
        pistolText.text = ConvertValue(value);
        PlayerPrefs.SetFloat("PistolSensy",value);
    }

    public void Auto(float value)
    {
        sensitivity[SensyType.AUTOGUN] = value;
        autoText.text = ConvertValue(value);
        PlayerPrefs.SetFloat("AutoSensy",value);
    }

    public void ShotGun(float value)
    {
        sensitivity[SensyType.SHOTGUN] = value;
        shotGunText.text = ConvertValue(value);
        PlayerPrefs.SetFloat("ShotGunSensy",value);
    }

    public void Sniper(float value)
    {
        sensitivity[SensyType.SNIPER] = value;
        sniperText.text = ConvertValue(value);
        PlayerPrefs.SetFloat("SniperSensy",value);
    }
}
