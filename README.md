# CN

## Unity UI 低版本兼容工具 (高版本转2018.3)

由于Unity在2018版本开发的中间时期开始采用了新的UI组件，导致高版本的Unity在降级时会出现UI丢失等问题。因此我编写了这个脚本。<br>

### 如何使用：

1.把场景里的所有物体都拖成预制体，随便保存在资产文件夹里某个位置。<br>
2.点击Unity顶部的"Tools"→"Compatibility Low UI"来等待替换完成。<br>
3.退出Unity编辑器，如果要求保存场景则点击保存，然后重新打开。<br>
4.随便点击任何UI或者EventSystem，确保所有的UI都已经替换成了脚本。<br>
（如果发现有没有替换成功的，可以使用记事本打开预制体文件，找到需要替换的GUID，添加到ProcessUnityVersion里然后重新执行）<br>
5.把场景里的所有预制体转化为实例。<br>
6.用旧版本Unity打开项目，尝试转换项目，转换完成后检查UI。<br><br>

2024/11/4 更新说明：
本项目原本是用于PSVita平台的项目移植使用的，因此后期本项目将不单单仅提供替换UI功能，还会增加别的内容，用来辅助开发者们更方便的移植游戏。<br>
1. 新增multiple的修复功能（有时解包出来的项目，multiple材质会出现问题，一些子sprite可能会出现剪切错位的情况，但sprite editor里又是正常的，因此此脚本可以用来修复这个问题。）<br>


# EN

## Unity UI Compatibility Tool for Lower Versions (Convert from High Version to 2018.3)

Due to the introduction of new UI components during the development of Unity 2018, high versions of Unity may encounter issues such as UI loss when downgrading. Therefore, I have created this script. <br>

### How to Use:

1. Drag all objects in the scene into prefabs and save them in any location within the Assets folder.<br>
2. Click on "Tools" → "Compatibility Low UI" in the top menu of Unity and wait for the replacement to complete.<br>
3. Exit the Unity editor; if prompted to save the scene, click "Save," then reopen Unity.<br>
4. Click on any UI element or the EventSystem to ensure all UIs have been replaced with the script. <br>
   (If you notice any that have not been replaced, you can open the prefab file with a text editor, find the GUID that needs replacement, add it to ProcessUnityVersion, and rerun the script.)<br>
5. Convert all prefabs in the scene to instances.<br>
6. Open the project in an older version of Unity, attempt to convert the project, and check the UI after conversion.<br><br>

### Update Notes (2024/11/4):

Originally, this project was designed for porting to the PSVita platform. Moving forward, it will not only offer UI replacement features but also include additional content to assist developers in more easily porting games.<br>

1. Added a fix for multiple materials (sometimes, when unpacking a project, multiple materials can cause issues, resulting in some child sprites being misaligned, even though they appear correct in the sprite editor. This script can be used to fix that issue.)<br>