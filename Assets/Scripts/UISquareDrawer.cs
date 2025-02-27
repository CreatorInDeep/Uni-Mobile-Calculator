using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISquareDrawer : MonoBehaviour
{
    [SerializeField]
    private Image m_FrontImage;

    [SerializeField]
    private Image m_BackImage;

    [SerializeField]
    private float m_LineWidth = 5.0f;

    private float m_SquareSize = 10;

    private Canvas m_Canvas = null;

    public float SquareSize
    {
        get => m_SquareSize;
        set
        {
            m_SquareSize = value;

            if (m_Canvas == null)
                m_Canvas = GetComponentInParent<Canvas>();

            RectTransform rect = m_Canvas.GetComponent<RectTransform>();

            // Clamp square size within canvas bounds
            m_SquareSize = Mathf.Min(m_SquareSize, rect.sizeDelta.x - 100.0f, rect.sizeDelta.y - 100.0f);

            m_BackImage.rectTransform.sizeDelta = new Vector2(m_SquareSize, m_SquareSize);
            m_FrontImage.rectTransform.sizeDelta = new Vector2(m_SquareSize - m_LineWidth, m_SquareSize - m_LineWidth);
        }
    }
}
