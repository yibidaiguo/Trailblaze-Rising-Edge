using UnityEngine;
public class PlayerFloatInfo : MonoBehaviour
{
    [SerializeField] private TextMesh nameText;
    [SerializeField] private SpriteRenderer hpBarFillSpriteRenderer;
    public void UpdateName(string name)
    {
        nameText.text = name;
    }
    public void UpdateHp(float fillAmount)
    {
        hpBarFillSpriteRenderer.transform.localScale = new Vector3(fillAmount, 1, 1);
    }
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
