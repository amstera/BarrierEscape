using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject PlayerExplode;
    public float Speed = 5;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
    }

    public void Destroy()
    {
        GameObject explode = Instantiate(PlayerExplode, transform.position, Quaternion.identity);
        Destroy(explode, 2f);
		GetComponent<MeshRenderer>().enabled = false;
		GameManager.Instance.RestartLevel();
    }

    public void MoveLeft()
    {
        if (transform.position.x >= -2.3f)
        {
            transform.position += Vector3.left * Speed * Time.deltaTime;
        }
    }

    public void MoveRight()
    {
        if (transform.position.x <= 2.3f)
        {
            transform.position += Vector3.right * Speed * Time.deltaTime;
        }
    }
}
