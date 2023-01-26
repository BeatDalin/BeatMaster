using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
    public class Hammer : MonoBehaviour
    {
        private Vector3 _startPos, _endPos;
        private RhythmGameController _gameController;

        private float _timer;

        private void Start()
        {
            _startPos = GameObject.Find("Start").transform.position;
            _endPos = GameObject.Find("Hawhe").transform.position;
            GetComponent<Animator>().SetTrigger("Attack");
            StartCoroutine("CoHammerMove");
        }

        private static Vector3 CreateParabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        private IEnumerator CoHammerMove()
        {
            _timer = 0;
            while (transform.position.y >= _startPos.y||transform.position.x<_endPos.x)
            {
                _timer += Time.deltaTime;
                Vector3 tempPos = CreateParabola(_startPos, _endPos, 5, _timer);
                transform.position = tempPos;
                yield return new WaitForEndOfFrame();
            }
            Destroy(this.gameObject);
        }
    }
}