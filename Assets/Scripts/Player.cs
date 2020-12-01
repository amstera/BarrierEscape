using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject PlayerExplode;
    public float Speed = 5;

    void Update()
    {
        if (!GameManager.Instance.IsMoving)
        {
            return;
        }

        bool moveLeft = false, moveRight = false;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            moveLeft = touch.position.x <= Screen.width / 2;
            moveRight = touch.position.x > Screen.width / 2;
        }
        if (Input.GetKey(KeyCode.A) || moveLeft)
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D) || moveRight)
        {
            MoveRight();
        }
    }

    public void Destroy()
    {
        GameObject explode = Instantiate(PlayerExplode, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = explode.GetComponent<ParticleSystem>().main;
        main.startColor = GetComponent<MeshRenderer>().material.color;
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
