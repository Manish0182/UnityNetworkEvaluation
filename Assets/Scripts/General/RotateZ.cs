using UnityEngine;

public class RotateZ : MonoBehaviour
{
    private Vector3 _angle;
    [SerializeField] private float speed = 100;

    private void Start() => _angle = transform.eulerAngles;

    private void Update()
    {
        _angle.z -= Time.deltaTime * speed;
        transform.eulerAngles = _angle;
    }
}