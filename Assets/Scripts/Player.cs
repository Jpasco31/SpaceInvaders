using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float baseSpeed = 5.5f;
    public Projectile bulletPrefab;
    private bool _bulletActive;
    private bool _bulletPwrUp;
    private float _bulletPwrUpDuration = 3.0f; // Adjust the duration as needed

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += Vector3.left * (this.baseSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += Vector3.right * (this.baseSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!_bulletActive && !_bulletPwrUp)
        {
            Projectile projectile = Instantiate(this.bulletPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += BulletDestroyed;
            _bulletActive = true;
        }
        else if (_bulletPwrUp)
        {
            Projectile projectile = Instantiate(this.bulletPrefab, this.transform.position, Quaternion.identity);
        }
    }

    private void BulletDestroyed()
    {
        _bulletActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader") ||
            other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("BulletPowerUp"))
        {
            _bulletPwrUp = true;
            StartCoroutine(BulletPowerUpTimer());
        }
    }

    private IEnumerator BulletPowerUpTimer()
    {
        yield return new WaitForSeconds(_bulletPwrUpDuration);
        _bulletPwrUp = false;
    }
}

