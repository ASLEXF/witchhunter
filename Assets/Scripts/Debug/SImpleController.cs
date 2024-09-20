using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] ItemsUI itemsUI;
    [SerializeField] Backpack Backpack;

    float speed = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector3(horizontalInput * speed, verticalInput * speed, 0);

        //if (Input.GetButtonDown("Tab"))
        //{
        //    Backpack.Instance.ShowItemsUI();
        //}
    }
}
