using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public abstract class Tile : MonoBehaviour {
    protected const int spriteHeight = 1;
    protected const int spriteWidth = 1;
    protected int xPos;
    protected int yPos;
    public Sprite[] spriteArr;

    // Use this for initialization
    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = GetRandomSprite();
    }

    public void SetLoc(int xPos, int yPos) {
        this.xPos = xPos;
        this.yPos = yPos;
        transform.position = new Vector2(xPos * spriteWidth, yPos * spriteHeight);
    }

    // Returns a random sprite from the sprite array
    protected Sprite GetRandomSprite() {
        return spriteArr[Random.Range(0, spriteArr.Length - 1)];
    }
}
