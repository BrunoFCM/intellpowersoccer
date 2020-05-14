using UnityEngine;


namespace UniHumanoid
{
    public class BoneGizmoDrawer : MonoBehaviour
    {
        const float size = 0.03f;
        readonly Vector3 SIZE = new Vector3(size, size, size);

        [SerializeField]
        public bool Draw = true;

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Draw && transform.parent != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(transform.localPosition, SIZE);
                Gizmos.DrawLine(transform.parent.position, transform.localPosition);

                UnityEditor.Handles.Label(transform.localPosition, name);
            }
#endif
        }
    }
}
