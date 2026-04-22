using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private Gradient colorOverLife;

    private float timer;
    private Vector3 moveDirection;

    public void Initialize(int damage, bool isCritical = false)
    {
        Debug.Log(textMesh == null ? "textMesh is NULL" : "textMesh OK: " + textMesh.text);

        textMesh.text = damage.ToString();
        textMesh.sortingLayerID = SortingLayer.NameToID("UI");
        textMesh.sortingOrder = 100;  // Ensure it renders above other UI elements

        if (isCritical)
        {
            textMesh.color = Color.red;
            transform.localScale = Vector3.one * 1.3f;
        }
        else
        {
            textMesh.color = Color.white;
            transform.localScale = Vector3.one;
        }

        moveDirection = new Vector3(
            Random.Range(-0.3f, 0.3f),
            1f,
            0f
        ).normalized;

        timer = 0f;
        textMesh.ForceMeshUpdate();
        Debug.Log(transform.position);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        transform.position += moveDirection * floatSpeed * Time.deltaTime;

        float scale = scaleCurve != null ? scaleCurve.Evaluate(t) : 1f;
        transform.localScale = Vector3.one * scale;

        Color c = colorOverLife.Evaluate(t);
        textMesh.color = c;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}