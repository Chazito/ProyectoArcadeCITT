using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
