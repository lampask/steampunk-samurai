using Behaviours;
using UnityEngine;

namespace Gameplay
{
    public class SpriteDissolver : MonoBehaviour
    {
        private Material _pMat;
        public bool isDissolving;
        private float _dissolveAmount = -0.1f;
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        private PlayerBehaviour _parent;

        private void Start()
        {
            _pMat = GetComponent<SpriteRenderer>().material;
            _parent = GetComponentInParent<PlayerBehaviour>();
        }

        private void Update()
        {
            if (isDissolving)
            {
                _dissolveAmount = Mathf.Clamp(_dissolveAmount + Time.deltaTime, -0.1f, 1.2f);
                _pMat.SetFloat(Dissolve, _dissolveAmount);
                if (_dissolveAmount >= 1.2f)
                {
                    _dissolveAmount = -0.1f;
                    _pMat.SetFloat(Dissolve, _dissolveAmount);
                    isDissolving = false;
                    _parent.finishedDissolving.Invoke();
                }
            }
        }
    }
}