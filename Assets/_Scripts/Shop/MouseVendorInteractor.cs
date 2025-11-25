using UnityEngine;

public class MouseVendorInteractor : MonoBehaviour
{
    [Header("Layer chỉ chứa vendor")]
    [SerializeField] LayerMask vendorLayer;

    [Header("Khoảng cách ray tối đa")]
    [SerializeField] float maxDistance = 100f;

    void Update()
    {
        // Nếu đang bị UI block thì thôi
        if (UIInputGuard.BlockInputNow()) return;

        // Chỉ xử lý khi bấm chuột phải 1 lần
        if (!Input.GetMouseButtonDown(1)) return;

        // Lấy vị trí chuột trên thế giới 2D
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Vì 2D nên dùng Raycast với direction = Vector2.zero, distance = 0
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, vendorLayer);

        if (hit.collider != null)
        {
            // Tìm component EquipmentVendor trên object hoặc parent
            EquipmentVendor vendor = hit.collider.GetComponentInParent<EquipmentVendor>();
            if (vendor != null)
            {
                vendor.TryOpenShop();
            }
        }
    }
}
