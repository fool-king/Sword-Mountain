// DynamicSortingOrder.cs
using UnityEngine;
using UnityEngine.Rendering;
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SortingGroup sortingGroup; // 如果用了Sorting Group
    public bool useSortingGroup = false;
    public float yOffset = 0f; // 如果需要微调基准点

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sortingGroup = GetComponent<SortingGroup>();
        
        // 自动检测是否使用Sorting Group
        if (sortingGroup != null)
        {
            useSortingGroup = true;
        }
    }

    void Update()
    {
        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        // 计算基于Y轴的排序值（Y值越小，排序越靠前）
        int sortingOrder = Mathf.RoundToInt(-(transform.position.y + yOffset) * 100);

        if (useSortingGroup && sortingGroup != null)
        {
            sortingGroup.sortingOrder = sortingOrder;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }

    // 在编辑模式下也可以预览效果
    void OnDrawGizmosSelected()
    {
        UpdateSortingOrder();
    }
}