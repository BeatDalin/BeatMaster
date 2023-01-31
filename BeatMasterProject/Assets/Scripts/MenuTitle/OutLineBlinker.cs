using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineBlinker : MonoBehaviour
{
    [SerializeField] private Material _mat;
    
    private float _origin;
    
    private bool _up;
    // Start is called before the first frame update
    void Start()
    {
        _origin = _mat.GetFloat("_Falloff");
    }

    // Update is called once per frame
    void Update()
    {
        _origin = Mathf.Clamp(_origin, 2f, 10f);

        if (_origin >= 10f)
        {
            _up = !_up;
        }

        if (_origin <= 2f)
        {
            _up = !_up;
        }

        if (_up)
        {
            _origin += 0.1f;
        }
        else
        {
            _origin -= 0.1f;
        }
        
        _mat.SetFloat("_Falloff", _origin);
    }
}
