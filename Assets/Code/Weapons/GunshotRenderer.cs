using UnityEngine;

public class GunshotRenderer : MonoBehaviour
{
    [SerializeField] LineRenderer m_lineRenderer;
    public void ClearLine()
    {
        m_lineRenderer.positionCount = 0;
    }
    public void UpdateLinePositions(Vector3 destination)
    {
        m_lineRenderer.positionCount = 2;
        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, destination);
    }
}
