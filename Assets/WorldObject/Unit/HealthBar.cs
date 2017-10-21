using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Unit unit;
    public Image image;

    private RectTransform healthBarRectTransform;

	// Use this for initialization
	void Start () {
        image = GetComponentInChildren<Image>();
        healthBarRectTransform = transform.parent.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 screenPos = Camera.main.WorldToViewportPoint(unit.GetHealthBarPos());
        if (IsOnScreen(screenPos)) {
            image.enabled = true;
            Vector2 newScreenPosition = new Vector2(
                screenPos.x * healthBarRectTransform.sizeDelta.x - healthBarRectTransform.sizeDelta.x * 0.5f,
                screenPos.y * healthBarRectTransform.sizeDelta.y - healthBarRectTransform.sizeDelta.y * 0.5f);
            image.rectTransform.anchoredPosition = newScreenPosition;
        } else {
            image.enabled = false;
        }
	}

    public void OnHealthChange() {
        image.fillAmount = unit.currentHealth / unit.maxHealth;
    }

    private bool IsOnScreen(Vector2 screenPos) {
        if(screenPos.x >= 0 && screenPos.y <= 1) {
            return true;
        }
        return false;
    }
}
