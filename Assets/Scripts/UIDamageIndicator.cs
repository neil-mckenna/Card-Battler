using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDamageIndicator : MonoBehaviour
{
    public TMP_Text damageText;
    public float moveSpeed = 10f;
    public float lifeTime = 5f;

    private RectTransform myRect;

    // Start is called before the first frame update
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        Destroy(gameObject, lifeTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        myRect.anchoredPosition += new Vector2(0f, -moveSpeed * Time.deltaTime * 100);
        
    }
}
