using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class LoadRoomJson : MonoBehaviour
{
    public string unity_json_folder;
    public string unity_floor_folder;
    public string json_filename;
    public Room room;

    public void LoadRoom()
    {
        string json_file = unity_json_folder + '\\' + json_filename;
        string json_content = File.ReadAllText(json_file);
        RoomData room_content = JsonConvert.DeserializeObject<RoomData>(json_content);
        room.SynchronizeRoomInfo(room_content);
    }
}
