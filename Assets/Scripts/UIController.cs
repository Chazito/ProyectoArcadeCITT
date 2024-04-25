using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameTime gameTime;

    // Start is called before the first frame update
    void Start()
    {
        gameTime = GameDirector.instance.gameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(timerText != null) { timerText.text = gameTime.GetTimeString(); }
    }
}
