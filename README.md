# 3D-FRONT-Unity-Toolkit
A python toolkit to prepare unity3D-compatible rooms.

# Description
Parse the raw 3D-FRONT jsons into room-based simplified jsons.The Unity3D engine can load, edit, and save the room settings in runtime. The parsed data can be used in 3D scene synthesis algorithm development.

Our code is tested on Win10, python=3.8, Unity3D=2021.2.16f1c1.

# Preparation
- 3D-FRONT dataset:
  - Download the 3D-FRONT raw dataset from the official webset: https://tianchi.aliyun.com/specials/promotion/alibaba-3d-scene-dataset. We need to extract the zips for "3D-FRONT", "3D-FRONT-texture", and "3D-FUTURE-model" folders.
- Python dependency:
  - numpy
  - trimesh
  - vedo
  - json
- Unity3D Engine:
  - Unity3D 2021.2.16f1c1
  - Runtime OBJ Importer

# Comming Soom:
- Preprocess jsons files.
- Load rooms in Unity3D engine.

# Reference
- The methods in "scene_parser.py" and "utils.py" are basically from the repo "3D-FRONT-FUTURE/3D-FRONT-ToolBox".


