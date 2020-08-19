using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButtonUI : MonoBehaviour
{
    [SerializeField]
    private GameObject extensiveControls = null;
    [SerializeField]
    private Toggle soundButton = null;

    void Start()
    {
        extensiveControls.SetActive(soundButton.isOn);
    }

    public void togglePress()
    {
        extensiveControls.SetActive(soundButton.isOn);
    }

    public void unfoldedPress()
    {
        soundButton.isOn = !soundButton.isOn;
        extensiveControls.SetActive(soundButton.isOn);
    }
}
