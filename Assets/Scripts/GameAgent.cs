using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GameAgent : Agent
{
    public static GameAgent Instance;
	public GameObject Player;

	public override void Initialize()
	{
        if (Instance == null)
        {
            Instance = this;
        }
	}

    public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(Player.transform.position.x);
        float radius = Player.GetComponent<SphereCollider>().radius / 2;
        Physics.SphereCast(Player.transform.position, radius, Vector3.forward, out RaycastHit hitInfoForward);
        if (hitInfoForward.collider != null && hitInfoForward.collider.GetComponent<Barrier>() != null)
        {
            sensor.AddObservation(hitInfoForward.distance);
            sensor.AddObservation(hitInfoForward.collider.transform.position.x);
            sensor.AddObservation(hitInfoForward.rigidbody.velocity.magnitude);
        }
        Physics.SphereCast(Player.transform.position, radius, Vector3.right, out RaycastHit hitInfoLeft);
        if (hitInfoLeft.collider != null && hitInfoLeft.collider.GetComponent<Barrier>() != null)
        {
            sensor.AddObservation(hitInfoLeft.distance <= 1f);
        }
        else
        {
            sensor.AddObservation(false);
        }
        Physics.SphereCast(Player.transform.position, radius, Vector3.left, out RaycastHit hitInfoRight);
        if (hitInfoRight.collider != null &&  hitInfoRight.collider.GetComponent<Barrier>() != null)
        {
            sensor.AddObservation(hitInfoRight.distance <= 1f);
        }
        else
        {
            sensor.AddObservation(false);
        }
    }

	public override void OnActionReceived(float[] vectorAction)
	{
        if (vectorAction[0] == 1)
		{
			Player.GetComponent<Player>().MoveLeft();
		}
		else if (vectorAction[0] == 2)
		{
			Player.GetComponent<Player>().MoveRight();
		}

        Physics.SphereCast(Player.transform.position, Player.GetComponent<SphereCollider>().radius/2, Vector3.forward, out RaycastHit hitInfo);
        if (hitInfo.collider.GetComponent<Barrier>() != null && GameManager.Instance.IsMoving)
        {
            if (hitInfo.distance <= 1.15)
            {
                GameManager.Instance.IsMoving = false;
                Player.GetComponent<Player>().Destroy();
            }
            else if (hitInfo.distance <= 1.5f)
            {
                AddReward(-0.01f);
            }
            else if (hitInfo.distance >= 2.5f)
            {
                AddReward(0.01f);
            }
        }

        if (Player.transform.position.x < -2 || Player.transform.position.x > 2)
        {
            AddReward(-1f);
        }
	}

	public override void OnEpisodeBegin()
	{
    }
}
