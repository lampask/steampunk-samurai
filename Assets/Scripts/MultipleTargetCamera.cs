using System.Linq;
using Management;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public Vector3 offset;
    public float smoothTime = .5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;

    private Vector3 _velocity;
    private Camera _cam;


    private void Start()
    {
        _cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Move();
        Zoom();
    }

    private void Zoom()
    {
        var newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, newZoom, Time.smoothDeltaTime);
    }

    private void Move()
    {
        var centerPoint = GetCenterPoint();

        var newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _velocity, smoothTime);
    }

    float GetGreatestDistance()
    {
        var players = LocalGameManager.instance.players.Select(p => p.transform).ToList();
        var bounds = new Bounds(players[0].position, Vector3.zero);
        for (var i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return bounds.size.x;
    }

    Vector3 GetCenterPoint()
    {
        var players = LocalGameManager.instance.players.Select(p => p.transform).ToList();
        if (players.Count == 1)
        {
            return players[0].position;
        }

        var bounds = new Bounds(players[0].position, Vector3.zero);
        for (var i = 0; i < players.Count; i++) 
        {
            bounds.Encapsulate(players[i].position);
        }

        return bounds.center;
    }

}
