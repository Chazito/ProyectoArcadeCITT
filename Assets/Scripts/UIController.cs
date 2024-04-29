using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private Color lowHealthColor;
    [SerializeField] private Color highHealthColor;
    private GameTime gameTime;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        gameTime = GameDirector.instance.gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerText != null) { timerText.text = gameTime.GetTimeString(); }
        if(playerController == null)
        {
            var playerObject = GameDirector.instance.GetPlayer();
            if (playerObject != null)
            {
                playerController = playerObject.GetComponent<PlayerController>();
            }
        }
        if(playerController != null)
        {
            float healthPercent = Mathf.Clamp01(playerController.CurrentHealth / playerController.MaxHealth);
            healthSlider.value = healthPercent;
            sliderFill.color = LerpColor(lowHealthColor, highHealthColor, healthPercent);
        }
    }

    public Color LerpColor(Color colorA, Color colorB, float value)
    {
        // Clamp value between 0 and 1
        value = Mathf.Clamp01(value);

        // Use Color.Lerp for linear interpolation
        return Color.Lerp(colorA, colorB, value);
    }
}
