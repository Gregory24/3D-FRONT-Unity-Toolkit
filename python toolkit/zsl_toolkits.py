from scene_parser import Scene, read_json
from utils import vedo_transform_mesh
from config import Config
import numpy as np
import time
import vedo
import json
import os


class ThreedFrontUnityToolbox:
    ''' Use it for pre-process 3d-front json files
    '''
    def __init__(self):
        # Dir settings
        self.threed_front_folder = Config.FRONT_PATH
        self.threed_future_folder = Config.FUTURE_PATH
        self.threed_texture_folder = Config.TEXTURE_PATH
        self.output_unity_json_path = Config.OUTPUT_JSON
        self.output_unity_floor_path = Config.OUTPUT_FLOOR
        self.invalid_house = Config.INVALID_HOUSE

        self.json_filenames = self.load_json_files()

    def load_json_files(self):
        json_filenames = os.listdir(self.threed_front_folder)
        for house in self.invalid_house:
            json_filenames.remove(house + '.json')
        return json_filenames

    def dataset_statistics(self):
        ''' output txt, includes:
                - room types
                - object categories
        '''
        room_types = {}
        object_categories = {}
        for i in range(len(self.json_filenames)):
            filename = self.json_filenames[i]
            scene = read_json(os.path.join(self.threed_front_folder, filename), self.threed_future_folder)
            rooms = scene.house.rooms
            for instanceid in rooms.keys():
                if len(rooms[instanceid].furniture) == 0:
                    continue
                if rooms[instanceid].type not in room_types.keys():
                    room_types[rooms[instanceid].type] = 1
                else:
                    room_types[rooms[instanceid].type] += 1

                for instance in rooms[instanceid].furniture:
                    if instance.info.category not in object_categories.keys():
                        object_categories[instance.info.category] = 1
                    else:
                        object_categories[instance.info.category] += 1
            print(i+1, filename)
        with open('stat_room_types.txt', 'w') as f:
            for type in room_types.keys():
                f.write(f'{type}: {room_types[type]}\n')
        f.close()
        with open('stat_object_categories.txt', 'w') as f:
            for type in object_categories.keys():
                f.write(f'{type}: {object_categories[type]}\n')
        f.close()

    def preprocess_data(self):
        for i in range(len(self.json_filenames)):
            time_start = time.time()
            filename = self.json_filenames[i]
            scene = read_json(os.path.join(self.threed_front_folder, filename), self.threed_future_folder)
            self.generate_json_floor(scene)
            time_end = time.time()
            print(f'{i+1} {filename} done in {time_end - time_start}!')

    def generate_json_floor(self, scene: Scene):
        house_uid = scene.house.uid
        rooms = scene.house.rooms
        for instanceid in rooms.keys():
            json_content = {}
            room_type = rooms[instanceid].type
            furniture = rooms[instanceid].furniture
            if len(furniture) == 0 or (room_type not in Config.SUPP_ROOM_TYPE):
                continue
            obj_paths, obj_names, tex_names, uid, jid = [], [], [], [], []
            categories, init_scales, poses, rots, scales = [], [], [], [], []
            for instance in furniture:
                json_content = {}
                obj_path = os.path.join(Config.FUTURE_PATH, instance.info.jid)
                obj_name = 'raw_model.obj'
                tex_name = 'texture.png'

                # calculate init_scale
                # mesh = vedo.Mesh(os.path.join(obj_path, obj_name))
                # max_vec = np.max(mesh.points(), 0)
                # mesh = trimesh.load_mesh(os.path.join(obj_path, obj_name), file_type='obj')
                # max_vec = np.max(mesh.vertices, 0)
                # init_scale = [float(max_vec[0]*2), float(max_vec[1]), float(max_vec[2]*2)]
                if len(instance.info.bbox) == 1:
                    init_scale = instance.info.bbox[0]
                elif len(instance.info.bbox) == 3:
                    init_scale = instance.info.bbox
                else:
                    print('Warning: invalid bbox infomation')

                category = instance.info.category
                pos, rot, scale = instance.pos, instance.rot, instance.scale
                if category in Config.SUPP_OBJ_CATEGORY:
                    obj_paths.append(obj_path); obj_names.append(obj_name); tex_names.append(tex_name);  # noqa
                    uid.append(instance.info.uid); jid.append(instance.info.jid)  # noqa
                    categories.append(category); init_scales.append(init_scale); poses.append(pos); rots.append(rot); scales.append(scale);  # noqa
            if len(uid) != 0:
                json_content['house_uid'] = house_uid
                json_content['room_uid'] = instanceid
                json_content['room_type'] = room_type
                json_content['floor_obj'] = os.path.join(self.output_unity_floor_path, f'{instanceid}_{house_uid}.obj')
                json_content['obj_paths'] = obj_paths
                json_content['obj_names'] = obj_names
                json_content['tex_names'] = tex_names
                json_content['instance_uid'] = uid
                json_content['instance_jid'] = jid
                json_content['categories'] = categories
                json_content['init_scales'] = init_scales
                json_content['poses'] = poses
                json_content['rots'] = rots
                json_content['scales'] = scales
                json_content['n_instance'] = len(uid)

                room = rooms[instanceid]
                room.output_floor_meshes(self.output_unity_floor_path, f'{instanceid}_{house_uid}', 'floor')
                json_file = os.path.join(self.output_unity_json_path, f'{instanceid}_{house_uid}.json')
                with open(json_file, 'w') as f:
                    json.dump(json_content, f)
                f.close()


class UnityRoom:
    ''' json content:
            'house_uid', 'room_uid', 'room_type', 'floor_obj'
            'obj_paths', 'obj_names', 'tex_names', 'instance_uid', 'instance_jid', 'categories'
            'init_scales', 'poses', 'rots', 'scales'
            'n_instance'
    '''
    def __init__(self, json_file):
        self.json_file = json_file
        self.parse_json()

    def parse_json(self):
        with open(self.json_file, 'r') as f:
            room = json.load(f)
        f.close()
        self.house_uid = room['house_uid']
        self.room_uid = room['room_uid']
        self.room_type = room['room_type']
        self.floor_obj = room['floor_obj']
        self.obj_paths = room['obj_paths']
        self.obj_names = room['obj_names']
        self.tex_names = room['tex_names']
        self.instance_uid = room['instance_uid']
        self.instance_jid = room['instance_jid']
        self.categories = room['categories']
        self.init_scales = room['init_scales']
        self.poses = room['poses']
        self.rots = room['rots']
        self.scales = room['scales']
        self.n_instance = room['n_instance']

    def render_room(self, with_bbox=False):
        # bbox初始状态：[0, init_scale[1]/2, 0] [0, 0, 1] init_scale
        # 到world的转换：instance.pos, instance.rot, instance.scale
        vedo_meshes = []
        init_scales = []
        vedo_meshes.append(vedo.Mesh(self.floor_obj))
        for i in range(self.n_instance):
            obj_file = os.path.join(self.obj_paths[i], self.obj_names[i])
            mesh = vedo.Mesh(obj_file)
            max_vec = np.max(mesh.points(), 0)
            init_scales.append(np.array([max_vec[0] * 2, max_vec[1], max_vec[2] * 2]))  # update init scale, necessary
            mesh = vedo_transform_mesh(mesh, self.poses[i], self.rots[i], self.scales[i])
            vedo_meshes.append(mesh)

        if with_bbox:
            box_meshes = []
            box_meshes.append(vedo.Mesh(self.floor_obj))
            for i in range(self.n_instance):
                cube = vedo.Cube(pos=[0, 0, 0], alpha=0.5)
                pts = cube.points()
                pts = pts * init_scales[i]
                pts = pts + np.array([0, init_scales[i][1] / 2, 0])
                cube.points(pts)
                cube = vedo_transform_mesh(cube, self.poses[i],  self.rots[i], self.scales[i])
                box_meshes.append(cube)
            vedo.show(box_meshes, axes=2, at=0, N=2)
            vedo.show(vedo_meshes, axes=2, at=1, interactive=1)
        else:
            vedo.show(vedo_meshes, axes=2, interactive=1)


if __name__ == '__main__':
    # pre-process data
    # toolkit = ThreedFrontUnityToolbox()
    # toolkit.preprocess_data()

    # render room by vedo
    json_file = r'F:\Dataset_3D-FRONT\Unity-json\Bedroom-42_7cbc4050-ff49-4985-b91b-a1786ae6dae3.json'
    unity_json_toolkit = UnityRoom(json_file)
    unity_json_toolkit.render_room(with_bbox=True)
