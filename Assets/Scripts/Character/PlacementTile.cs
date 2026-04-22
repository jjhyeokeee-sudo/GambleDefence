using UnityEngine;
using UnityEngine.EventSystems;

// URP 2D: OnMouseDown 대신 Physics2DRaycaster + 포인터 인터페이스 사용
public class PlacementTile : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsOccupied { get; private set; }
    public PlacedCharacter Occupant { get; private set; }

    private SpriteRenderer sr;

    static readonly Color ColNormal   = new Color(0.35f, 0.80f, 0.35f, 0.55f);
    static readonly Color ColHover    = new Color(0.60f, 1.00f, 0.60f, 0.75f);
    static readonly Color ColOccupied = new Color(0.55f, 0.55f, 0.55f, 0.45f);
    static readonly Color ColNoGold   = new Color(0.90f, 0.40f, 0.40f, 0.65f);

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Refresh();
    }

    public void OnPointerEnter(PointerEventData _)
    {
        if (IsOccupied) return;
        var pm = PlacementManager.Instance;
        bool canAfford = pm != null && pm.CanAffordSelected();
        sr.color = canAfford ? ColHover : ColNoGold;
    }

    public void OnPointerExit(PointerEventData _) { Refresh(); }

    public void OnPointerClick(PointerEventData _)
    {
        if (!IsOccupied)
            PlacementManager.Instance?.TryPlace(this);
    }

    public void SetOccupied(PlacedCharacter c)
    {
        IsOccupied = true;
        Occupant = c;
        Refresh();
    }

    public void Vacate()
    {
        IsOccupied = false;
        Occupant = null;
        Refresh();
    }

    void Refresh()
    {
        if (sr == null) return;
        sr.color = IsOccupied ? ColOccupied : ColNormal;
    }
}
