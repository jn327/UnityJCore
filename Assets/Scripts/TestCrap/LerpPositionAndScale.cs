using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpPositionAndScale : MonoBehaviour
{
    private Renderer _renderer;

    float scaleT = 0;
    float posT = 0;
    float colorT = 0;

    private enum MOVETYPE {COS, SAWTOOTH, PINGPONG };
    [SerializeField]
    MOVETYPE moveType = default;

    [SerializeField]
    private AnimationCurve _scaleEasing = default;
    [SerializeField]
    private AnimationCurve _positionEasing = default;
    [SerializeField]
    private AnimationCurve _colorEasing = default;

    [SerializeField]
    private Gradient _colorGradient = default;
    
    float scaleSpeed = 1;
    float scaleMinX = 1;
    float scaleMaxX = 1.5f;
    float scaleMinY = 1;
    float scaleMaxY = 0.75f;

    float colorSpeed = 1;

    float moveSpeed = 1;
    float posMin = 4;
    float posMax = -2;

    // Update is called once per frame
    void Update()
    {
        scaleT  += Time.deltaTime * scaleSpeed;
        posT    += Time.deltaTime * moveSpeed;
        colorT  += Time.deltaTime * colorSpeed;

        float scaleTN   = scaleT;
        float posTN     = posT;
        float colorTN   = colorT;
        if (moveType == MOVETYPE.COS)
        {
            scaleTN     = 0.5f+(Mathf.Cos(2* Mathf.PI * scaleTN) * 0.5f);
            posTN       = 0.5f+(Mathf.Cos(2* Mathf.PI * posTN) * 0.5f);
            colorTN     = 0.5f+(Mathf.Cos(2* Mathf.PI * colorTN) * 0.5f);
        }
        else if (moveType == MOVETYPE.SAWTOOTH)
        {
            scaleTN     = scaleTN % 1.0f;
            posTN       = posTN % 1.0f;   
            colorT      = colorTN % 1.0f;
        }
        else if (moveType == MOVETYPE.PINGPONG)
        {
            scaleTN     = Mathf.PingPong(scaleTN, 1.0f);
            posTN       = Mathf.PingPong(posTN, 1.0f);   
            colorTN     = Mathf.PingPong(colorTN, 1.0f);
        }

        if (_scaleEasing.keys.Length > 0)
        {
            scaleTN = _scaleEasing.Evaluate(scaleTN);
        }

        if (_positionEasing.keys.Length > 0)
        {
            posTN = _positionEasing.Evaluate(posTN);
        }

         if (_colorEasing.keys.Length > 0)
        {
            colorTN = _colorEasing.Evaluate(colorTN);
        }

        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
        _renderer.material.color = _colorGradient.Evaluate(colorTN);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(posMin, posMax, posTN);
        transform.position = pos;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Lerp(scaleMinX, scaleMaxX, scaleTN);
        scale.y = Mathf.Lerp(scaleMinY, scaleMaxY, scaleTN);
        transform.localScale = scale;
    }

    void OnDestroy() 
    { 
        DestroyImmediate(_renderer.material); 
    }
}
