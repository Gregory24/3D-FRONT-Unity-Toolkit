using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Json info")]
    public string house_uid;
    public string room_uid;
    public string room_type;
    public string floor_obj;
    public List<string> obj_paths;
    public List<string> obj_names;
    public List<string> instance_uid;
    public List<string> instance_jid;
    public List<string> categories;
    public List<Vector3> init_scales;
    public List<Vector3> poses;
    public List<Quaternion> rots;
    public List<Vector3> scales;

    public List<Instance> instances = new List<Instance>();

    GameObject floor;
    public Vector3 floor_center;
    public Vector3[] floor_corner = new Vector3[4];

    public void SynchronizeRoomInfo(RoomData room_content)
    {
        house_uid = room_content.house_uid;
        room_uid = room_content.room_uid;
        room_type = room_content.room_type;
        floor_obj = room_content.floor_obj;
        obj_paths = room_content.obj_paths;
        obj_names = room_content.obj_names;
        instance_uid = room_content.instance_uid;
        instance_jid = room_content.instance_jid;
        categories = room_content.categories;
        init_scales = List2Vector3(room_content.init_scales);
        poses = List2Vector3(room_content.poses);
        rots = List2Quan(room_content.rots);
        scales = List2Vector3(room_content.scales);

        for (int i = 0; i < obj_paths.Count; i++)
        {
            GameObject inst_go = new GameObject(instance_uid[i] + categories[i]);
            inst_go.transform.SetParent(transform);
            Instance inst = inst_go.AddComponent<Instance>();
            inst.obj_path = obj_paths[i];
            inst.obj_name = obj_names[i];
            inst.category = categories[i];
            inst.instance_jid = instance_jid[i];
            inst.init_scale = init_scales[i];
            inst.position = poses[i];
            inst.rotation = rots[i];
            inst.scale = scales[i];
            instances.Add(inst);
        }
    }

    public void InitializeRoom()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            instances[i].LoadObjMesh();
        }
        LoadFloorMesh();
        CalculateCenter();
        CenterInstances();
    }

    public void CalculateCenter()
    {
        Mesh floor_mesh = floor.GetComponentInChildren<MeshFilter>().mesh;
        long vertex_count = floor_mesh.vertexCount;
        float[] x_value = new float[vertex_count];
        float[] y_value = new float[vertex_count];
        float[] z_value = new float[vertex_count];
        for (int i = 0; i < vertex_count; i++)
        {
            x_value[i] = floor_mesh.vertices[i].x;
            y_value[i] = floor_mesh.vertices[i].y;
            z_value[i] = floor_mesh.vertices[i].z;
        }
        Vector3 max = new Vector3(
            Mathf.Max(x_value),
            Mathf.Max(y_value),
            Mathf.Max(z_value)
        );
        Vector3 min = new Vector3(
            Mathf.Min(x_value),
            Mathf.Min(y_value),
            Mathf.Min(z_value)
        );
        floor_center = (max + min) * 0.5f;

        floor_corner[0] = new Vector3(max.x, floor_center.y, max.z) - floor_center;
        floor_corner[1] = new Vector3(min.x, floor_center.y, max.z) - floor_center;
        floor_corner[2] = new Vector3(min.x, floor_center.y, min.z) - floor_center;
        floor_corner[3] = new Vector3(max.x, floor_center.y, min.z) - floor_center;
    }

    public void CenterInstances()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            instances[i].floor_center = floor_center;
            instances[i].transform.position -= floor_center;
        }
        floor.transform.position -= floor_center;
    }

    public void LoadFloorMesh()
    {
        floor = new OBJLoader().Load(floor_obj);
        floor.transform.SetParent(transform);
        floor.transform.localScale = new Vector3(1, 1, 1);
    }

    List<Vector3> List2Vector3(List<List<float>> content)
    {
        List<Vector3> res = new List<Vector3>();
        for (int i = 0; i < content.Count; i++)
        {
            Vector3 vector = new Vector3(0, 0, 0);
            for (int j = 0; j < content[i].Count; j++)
            {
                vector[j] = content[i][j];
            }
            res.Add(vector);
        }
        return res;
    }

    List<Quaternion> List2Quan(List<List<float>> content)
    {
        List<Quaternion> res = new List<Quaternion>();
        for (int i = 0; i < content.Count; i++)
        {
            Quaternion quan = new Quaternion(0, 0, 0, 0);
            for (int j = 0; j < content[i].Count; j++)
            {
                quan[j] = content[i][j];
            }
            res.Add(quan);
        }
        return res;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(floor_corner[0], floor_corner[1]);
        Gizmos.DrawLine(floor_corner[1], floor_corner[2]);
        Gizmos.DrawLine(floor_corner[2], floor_corner[3]);
        Gizmos.DrawLine(floor_corner[3], floor_corner[0]);
    }
}
