using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public Knight parent;

    public List<Collider2D> detectedColliders = new List<Collider2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
        parent.StartCoroutine(parent.KnightAggro());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        parent.IsAggro = true;
        
    }
}
