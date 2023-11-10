using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletaveis : MonoBehaviour
{

    public int itemValue;
    private AudioSource sound;
    
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.rotation.x, 30 * Time.deltaTime, transform.rotation.z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sound.Play();
            GameController.instance.UpdateScore(itemValue);
            Destroy(gameObject, 0.3f);
        }
        
    }
}
