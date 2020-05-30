using System;
using UnityEngine;

namespace Gameplay
{
    public abstract class AbstractPowerup : MonoBehaviour
    {
        public abstract Vector2 position { get; protected set; }
        public abstract Sprite texture {get; protected set;}
        
        protected GameObject obj { get; private set; }

        public abstract void OnPickup(Player target);

        protected virtual void Awake()
        {
            obj = Instantiate(new GameObject(), position, Quaternion.identity);
            ((SpriteRenderer) obj.AddComponent(typeof(SpriteRenderer))).sprite = texture;
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player p))
            {
                OnPickup(p);
            }
        }
    }

}
