using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraMoves : MonoBehaviour
{
    public GameObject virtualCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            virtualCamera.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            RigidbodyType2D type = other.transform.parent.GetComponent<Rigidbody2D>().bodyType;
            if (type != RigidbodyType2D.Static)
            {
                virtualCamera.SetActive(false);
            }
        }
    }
}
