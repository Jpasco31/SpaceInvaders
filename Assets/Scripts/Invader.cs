using UnityEngine;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1f;
    public virtual int score { get; set; } = 10;

    private SpriteRenderer spriteRenderer;
    private int animationFrame;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationSprites[0];
    }

    public void Start()
    {
        InvokeRepeating(nameof (AnimateSprite), this.animationTime, this.animationTime);
    }

    public void AnimateSprite()
    {
        if (this != null )
        {
            animationFrame++;

            // Loop back to the start if the animation frame exceeds the length
            if (animationFrame >= animationSprites.Length) {
                animationFrame = 0;
            }
            
            spriteRenderer.sprite = animationSprites[animationFrame];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            GameManager.Instance.OnInvaderKilled(this);
        } 
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Boundary"))
        {
            GameManager.Instance.OnBoundaryReached();
        }
    }
}