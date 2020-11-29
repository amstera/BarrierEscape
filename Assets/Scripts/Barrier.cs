using UnityEngine;

public class Barrier : MonoBehaviour
{
    public bool AddScore = true;

    void Update()
    {
        if (transform.position.z < Camera.main.transform.position.z - 0.5f)
        {
            if (AddScore)
            {
                GameManager.Instance.AddScore();
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && GameManager.Instance.IsMoving) 
        {
            GameManager.Instance.IsMoving = false;
            collision.collider.GetComponent<Player>().Destroy();
        }
    }
}
