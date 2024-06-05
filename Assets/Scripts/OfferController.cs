using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class OfferController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI offerName;
    [SerializeField] private TextMeshProUGUI offerDescription;
    [SerializeField] private Image offerSprite;
    [SerializeField] private Button offerButton;
    private PerkSO perk;

    public void Setup(PerkSO perk, UnityAction callback)
    {
        offerName.text = perk.perkName;
        offerDescription.text = perk.perkDescription;
        //TODO Change sprite
        offerButton.onClick.RemoveAllListeners();
        offerButton.onClick.AddListener(callback);
    }
}
