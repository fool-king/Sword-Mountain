//道具效果管理器
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public static ItemEffectManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public bool ApplyItemEffect(string itemID, ItemData itemData = null)
    {
        // 优化：优先使用传入的itemData
        if (itemData == null) {
            // 注意：需要实现ItemDatabase或者从其他地方获取itemData
            Debug.LogWarning("ItemDatabase not implemented. Need to provide itemData.");
            return false;
        }
        
        if (!string.IsNullOrEmpty(itemData.effectScriptName))
        {
            System.Type effectType = System.Type.GetType(itemData.effectScriptName);
            if (effectType != null)
            {
                Component effectComponent = gameObject.GetComponent(effectType);
                if (effectComponent == null)
                {
                    effectComponent = gameObject.AddComponent(effectType);
                }
                
                IItemEffect itemEffect = effectComponent as IItemEffect;
                if (itemEffect != null)
                {
                    return itemEffect.ApplyEffect();
                }
            }
        }
        return false;
    }
}