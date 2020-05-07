using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVisualizer : MonoBehaviour
{
    public int i;
    public float sphereSize;

    


    [SerializeField]MeshFilter m_MeshFilter;
    [SerializeField]Mesh m_Mesh;

    // Start is called before the first frame update
    void Start()
    {
        
        m_MeshFilter = gameObject.GetComponent<MeshFilter>();
        m_Mesh = m_MeshFilter.mesh;

        m_Mesh.GetTopology(0);
        //m_Mesh.

        Vector3[] vertices = m_MeshFilter.sharedMesh.vertices;
        Debug.Log("this mesh is made of  "  + m_Mesh.GetTopology(0));

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {

        if (!m_Mesh) { return; }

        if ( i < m_Mesh.vertices.Length )
        {
            Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

            Gizmos.DrawSphere(m.MultiplyPoint3x4(m_Mesh.vertices[m_Mesh.triangles[i]]), sphereSize);

        }
    }
}
