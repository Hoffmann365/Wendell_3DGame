using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coletaveis : MonoBehaviour
{

    public int itemValue;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
            GameController.instance.UpdateScore(itemValue);
            Destroy(gameObject);
        }
        
    }
}
