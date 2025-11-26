using UnityEngine;

public class DoNotDestroyThis : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(gameObject);
}