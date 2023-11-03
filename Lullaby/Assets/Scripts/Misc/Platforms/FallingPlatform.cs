using System.Collections;
using System.Collections.Generic;
using Lullaby;
using Lullaby.Entities;
using UnityEngine;

public class FallingPlatform : MonoBehaviour, IEntityContact
{
    public bool autoReset = true;
    public float fallDelay = 2f;
    public float resetDelay = 5f;
    public float fallGravity = 40f;

    [Header("Shake Setting")] 
    public bool shake = true;
    public float speed = 45f;
    public float height = 0.1f; // This is the height of the shake

    protected Collider collider;
    protected Vector3 initialPosition;

    protected Collider[] _overlaps = new Collider[32];
    
    /// <summary>
    /// Returns true if the fall routine was activated.
    /// </summary>
    public bool activated { get; protected set; }
    
    /// <summary>
    /// Returns true if this platform is falling.
    /// </summary>
    public bool falling { get; protected set; }

    /// <summary>
    /// Make the platform fall.
    /// </summary>
    public virtual void Fall()
    {
        falling = true;
        collider.isTrigger = true;
    }


    public virtual void Restart()
    {
        activated = falling = false;
        transform.position = initialPosition;
        collider.isTrigger = false;
        OffsetPlayer();
    }

    private void OffsetPlayer()
    {
        var center = collider.bounds.center;
        var extents = collider.bounds.extents;
        var maxY = collider.bounds.max.y;
        var overlaps = Physics.OverlapBoxNonAlloc(center, extents, this._overlaps);

        for (int i = 0; i < overlaps; i++)
        {
            if(!_overlaps[i].CompareTag(GameTags.Player)) continue;

            var distance = maxY - _overlaps[i].transform.position.y;
            var height = distance + this.height;
            var offset = transform.up * (distance + height * 0.5f); // We want to move the player half the height up

            _overlaps[i].transform.position += offset;
        }
    }

    public void OnEntityContact(Entity entity)
    {
        if (entity is Player && BoundsHelper.IsBellowPoint(collider, entity.stepPosition))
        {
            if (!activated)
            {
                activated = true;
                Debug.Log("Se detecta la colision");
                StartCoroutine(FallRoutine());
            }
        }
    }

    protected IEnumerator FallRoutine()
    {
        Debug.Log("Se inicia la corrutina de caida");
        var timer = fallDelay;

        while (timer >= 0)
        {
            if (shake && (timer <= fallDelay / 2f))
            {
                var shake = Mathf.Sin(Time.time * speed) * height;
                transform.position = initialPosition + transform.up * shake;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        Fall();

        if (autoReset)
        {
            yield return new WaitForSeconds(resetDelay);
            Restart();
        }
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        collider = GetComponent<Collider>();
        initialPosition = transform.position;
        tag = GameTags.Platform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (falling)
        {
            transform.position += fallGravity * Time.deltaTime * -transform.up;
        }
    }
}
