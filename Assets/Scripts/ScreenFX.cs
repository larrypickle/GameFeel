using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFX : MonoBehaviour
{
    public GameObject myVFX;
    // Start is called before the first frame update
   

    private void OnTriggerEnter2D(Collider2D collision)//makes the effect when ball hits ground
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit");
            gameObject.SetActive(false);
            
            GameObject spawnedObject = Instantiate(myVFX, transform.position, transform.rotation);
            Destroy(spawnedObject, 5f);
        }
    }
}
