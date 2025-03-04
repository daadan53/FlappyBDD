//using Unity.Mathematics;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private float _maxTime = 1.5f;
    [SerializeField] private float _heightRange = 0.45f;
    [SerializeField] private GameObject _pipe;
    bool canStart;
    
    private float _timer;

    void Awake()
    {
        GameManager.OnEnableToStart += EnableToStart;
    }

    void Start()
    {
        canStart = false;
        if(canStart)
        {
            SpawnPipe();
        }
    }

    void Update()
    {
        if(canStart)
        {
            if(_timer > _maxTime)
            {
                SpawnPipe();
                _timer = 0;
            }

            _timer += Time.deltaTime;
        }
    }

    private void SpawnPipe()
    {
        Vector3 spawnPos = transform.position + new Vector3(0, Random.Range(-_heightRange, _heightRange));
        GameObject pipe = Instantiate(_pipe, spawnPos, Quaternion.identity);

        Destroy(pipe, 10f);
    }

    void EnableToStart(bool _canStart)
    {
        canStart = _canStart;
    }
}
