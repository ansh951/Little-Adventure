using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int value = 30;
    public enum PickUpType {
        heal, coin
    }
    
    public PickUpType type;
    public ParticleSystem CollectedVFX;

    public void OnTriggerEnter(Collider other)
    {
  

        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Character>().PickUpItem(this);

            if (CollectedVFX != null)
                Instantiate(CollectedVFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
