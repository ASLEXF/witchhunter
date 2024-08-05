using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private bool isBeingPushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found!");
        }
        else
        {
            rb.isKinematic = false;
        }
    }

    void FixedUpdate()
    {
        if (isBeingPushed)
        {
            Vector3 moveDirection = new Vector3(1f, 0f);
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = true; // 玩家开始推动NPC
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingPushed = false;
            rb.velocity = Vector3.zero;
        }
    }
}
