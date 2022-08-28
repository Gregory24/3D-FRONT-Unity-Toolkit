using System.IO;
using System.Text;
using UnityEngine;

public class Instance : MonoBehaviour
{
    [Header("File Path")]
    public string obj_path;
    public string obj_name;
    [Header("Json Info")]
    public string instance_jid;
    public string category;
    public Vector3 init_scale;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    [Header("BBox")]
    public Vector3 floor_center;
    public Vector3[] init_bbox = new Vector3[8];
    public Vector3[] bbox = new Vector3[8];

    public void LoadObjMesh()
    {
        ObjectLoader loader = gameObject.AddComponent<ObjectLoader>();
        loader.Load(obj_path + '\\', obj_name);
        UpdateInitScale();
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        InitialBbox();
    }

    public void UpdateInitScale()
    {
        transform.position = new Vector3(0, 0, 0);
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        Vector3[] mesh_vertices = mesh.vertices;
        Vector3 max_vec = new Vector3(-99999, -99999, -99999);
        for (int i = 0; i < mesh_vertices.Length; i++)
        {
            if (mesh_vertices[i].x > max_vec.x) max_vec.x = mesh_vertices[i].x;
            if (mesh_vertices[i].y > max_vec.y) max_vec.y = mesh_vertices[i].y;
            if (mesh_vertices[i].z > max_vec.z) max_vec.z = mesh_vertices[i].z;
        }
        init_scale = max_vec;
    }

    public void InitialBbox()
    {
        init_bbox[0] = new Vector3(init_scale.x, 0, init_scale.z);
        init_bbox[1] = new Vector3(-init_scale.x, 0, init_scale.z);
        init_bbox[2] = new Vector3(-init_scale.x, 0, -init_scale.z);
        init_bbox[3] = new Vector3(init_scale.x, 0, -init_scale.z);
        init_bbox[4] = new Vector3(init_scale.x, init_scale.y, init_scale.z);
        init_bbox[5] = new Vector3(-init_scale.x, init_scale.y, init_scale.z);
        init_bbox[6] = new Vector3(-init_scale.x, init_scale.y, -init_scale.z);
        init_bbox[7] = new Vector3(init_scale.x, init_scale.y, -init_scale.z);
    }

    public void UpdateBbox()
    {
        Vector3[] inter_bbox = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            inter_bbox[i] = new Vector3(
                init_bbox[i].x * scale.x,
                init_bbox[i].y * scale.y,
                init_bbox[i].z * scale.z
            );
 
            inter_bbox[i] = rotation * inter_bbox[i];

            inter_bbox[i] = inter_bbox[i] + position - floor_center;
        }
        bbox = inter_bbox;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        UpdateBbox();
        DrawBbox(bbox);
    }

    public void DrawBbox(Vector3[] box_corners)
    {
        Gizmos.DrawLine(box_corners[0], box_corners[1]);
        Gizmos.DrawLine(box_corners[1], box_corners[2]);
        Gizmos.DrawLine(box_corners[2], box_corners[3]);
        Gizmos.DrawLine(box_corners[3], box_corners[0]);

        Gizmos.DrawLine(box_corners[4], box_corners[5]);
        Gizmos.DrawLine(box_corners[5], box_corners[6]);
        Gizmos.DrawLine(box_corners[6], box_corners[7]);
        Gizmos.DrawLine(box_corners[7], box_corners[4]);

        Gizmos.DrawLine(box_corners[0], box_corners[4]);
        Gizmos.DrawLine(box_corners[1], box_corners[5]);
        Gizmos.DrawLine(box_corners[2], box_corners[6]);
        Gizmos.DrawLine(box_corners[3], box_corners[7]);
    }
}
