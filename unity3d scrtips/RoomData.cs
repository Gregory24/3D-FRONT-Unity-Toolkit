using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public string house_uid { get; set; }
    public string room_uid { get; set; }
    public string room_type { get; set; }
    public string floor_obj { get; set; }
    public List<string> obj_paths { get; set; }
    public List<string> obj_names { get; set; }
    public List<string> tex_names { get; set; }
    public List<string> instance_uid { get; set; }
    public List<string> instance_jid { get; set; }
    public List<string> categories { get; set; }
    public List<List<float>> init_scales { get; set; }
    public List<List<float>> poses { get; set; }
    public List<List<float>> rots { get; set; }
    public List<List<float>> scales { get; set; }
}
