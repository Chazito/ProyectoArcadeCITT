using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelUpUIController : MonoBehaviour
{

    [SerializeField] private Transform offersContent;
    [SerializeField] private OfferController offerPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button banishButton;
    [SerializeField] private Button rerollButton;
    private List<OfferController> offerList;

    private GameDirector gameDirector;
    private PerkSO selectedPerk;

    public void Setup(List<PerkSO> perks)
    {
        gameDirector = GameDirector.instance;
        if (offerList == null)
        {
            offerList = new List<OfferController>();
        }
        selectedPerk = null;
        banishButton.interactable = false;
        rerollButton.interactable = false;
        confirmButton.interactable = false;
        for (int i = 0; i < perks.Count; i++)
        {
            if (perks.Count > offerList.Count)
            {
                offerList.Add(Instantiate(offerPrefab, offersContent));
            }
            int index = i;
            offerList[i].Setup(perks[i], delegate
            {
                selectedPerk = perks[index];
                confirmButton.interactable = true;
            });
            offerList[i].gameObject.SetActive(true);
        }
    }

    public void OnConfirm()
    {
        if (selectedPerk != null)
        {
            gameDirector.AddPerk(selectedPerk);
            selectedPerk = null;
            foreach (OfferController offer in offerList)
            {
                offer.gameObject.SetActive(false);
            }
            gameDirector.ResumeGame();
        }
    }
}
