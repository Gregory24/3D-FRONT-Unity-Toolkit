# 3D-FRONT Python Toolkit

# Documentation:
- **python toolkit**:
  - **config.py**: Important parameters of the dataset.
    - FRONT_PATH: the absolute address of the "3D-FRONT" folder.
    - FUTURE_PATH: the absolute address of the "3D-FUTURE-model" folder.
    - TEXTURE_PATH: the absolute address of the "3D-FRONT-texture" folder.
    - OUTPUT_JSON: the output folder for the simplified json, which will be used in Unity3D.
    - OUTPUT_FLOOR: the output folder for the floor obj files, which will be used in Unity3D.
    - INVALID_HOUSE: the list of invalid json, will be ignored in pre-process.
    - ROOM_TYPE: all room types.
    - OBJ_CATEGORY: all object categories.
    - SUPP_ROOM_TYPE: supported room types.
    - SUPP_OBJ_CATEGORY: supported object categories.
  
  - **scene_parser.py**: parse the raw json
  - **utils.py**: math and mesh object manipulation operation.
  - **zsl_toolkits**:
    - ThreedFrontUnityToolbox:
      - dataset_statistics(): output all room types and all object categories.
      - preprocess_data(): output the simplified json and floor obj files.
    - UnityRoom: check the simplified json
      - render_room(): render 3D room from the pre-processed json by vedo.
