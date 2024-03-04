using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public ParticleSystem meteorEffect;
    public ParticleSystem meteorExplosion;
    public ParticleSystem dropPosEffect;

    public ParticleSystem CheckLifetime;

    public float dropSpeed;

    private Vector3 dropPos;
    private Vector3 dropDir;
    private float targetDIs;

    private bool isMeteorCollision;

    private float stayMeteor;

    public List<Collider> enemies;

    private float damage;

    public AudioSource spawnMeteor;
    public AudioSource explosion;

    private void Start()
    {
        targetDIs = 10f;
        isMeteorCollision = false;
        stayMeteor = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        stayMeteor -= Time.deltaTime;
        if (stayMeteor <= 0)
        {
            MoveMeteor();
        }

        if (targetDIs <= 0.5f && !isMeteorCollision)
        {
            isMeteorCollision = true;
            meteorEffect.Stop();
            dropPosEffect.Stop();
            meteorExplosion.Play();
            explosion.Play();
            CheckLifetime.Play();
            GetComponent<CapsuleCollider>().enabled = true;
        }

        if (!CheckLifetime.gameObject.activeSelf)
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }

        if (!meteorExplosion.gameObject.activeSelf)
        {
            Destroy(gameObject);
        }
    }

    public void SetMeor(Vector3 _dropPos)
    {
        spawnMeteor.Play();
        dropPos = _dropPos;
        dropDir = (meteorEffect.transform.position - _dropPos).normalized;
    }

    public void MoveMeteor()
    {
        if (isMeteorCollision)
        {
            return;
        }
        targetDIs = (meteorEffect.transform.position - dropPos).magnitude;
        meteorEffect.transform.Translate(dropDir * dropSpeed * Time.deltaTime, Space.World);
    }

    public void SetDagage(float _damage)
    {
        damage = _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemies.Contains(other))
            {
                enemies.Add(other);
                Vector3 damageDir = (other.transform.position - transform.position).normalized;
                damageDir.y = 0.5f;
                other.GetComponent<Enemy>().Runaway(damageDir);
                other.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }
}
