using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperienceMagnet : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private Collider2D magnetCollider;

    private void Start()
    {
        //Set radius here
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ExperienceOrb orb = collision.GetComponent<ExperienceOrb>();
        if (orb)
        {
            orb.player = playerController;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ExperienceOrb orb = collision.GetComponent<ExperienceOrb>();
        if (orb)
        {
            orb.player = null;
        }
    }
}
