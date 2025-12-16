using System;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [SerializeField] private HapticsController hapticsController;
    [SerializeField] private AudioSource yipeeSource;
    [SerializeField] private AudioClip yipeeSound;
    [SerializeField] private AudioClip catSound;
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        yipeeSource.PlayOneShot(catSound);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (other.contacts[0].normal.y < 0.0f)
            {
                hapticsController.SetRumble(0, 1, 5.0f);
            }
            else if (other.contacts[0].normal.y > 0.0f)
            {
                hapticsController.SetRumble(0, 0, 5.0f);
            }
            else if (other.contacts[0].normal.x < 0.0f)
            {
                hapticsController.SetRumble(1, 1, 5.0f);
            }
            else if (other.contacts[0].normal.x > 0.0f)
            {
                hapticsController.SetRumble(1, 0, 5.0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        yipeeSource.PlayOneShot(yipeeSound);
        canvas.SetActive(true);
        playerController.enabled = false;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        hapticsController.StopAllRumble();
    }
}
